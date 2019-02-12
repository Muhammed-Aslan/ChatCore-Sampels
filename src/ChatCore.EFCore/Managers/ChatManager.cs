using ChatCore.Abstractions;
using ChatCore.Enums;
using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCore.EFCore.Managers
{
    public class ChatManager : ManagerBase, IChatManager<Chat,string>
    {

        public ChatManager(IServiceProvider provider) : base(provider)
        { }


        /// <summary>
        /// Gets chat with specific user if the chat if exist, else will return null
        /// </summary>
        /// <param name="withUserId"></param>
        /// <returns></returns>
        public async Task<Chat> GetChatWithAsync(string withUserId)
        {
            var withUser = Users.Find(withUserId);
            if (withUser == null)
                return null;

            var chat = await UsersChats
                .Where(uc=>uc.UserId == withUserId).Include(uc=>uc.User)
                .Select(uc => uc.Chat).Include(c=>c.UsersChats)
                                    .SingleOrDefaultAsync(c =>
                                    c.ChatType == ChatType.Personal &&
                                    c.UsersChats.SingleOrDefault(u=>u.UserId==CurrentUser.Id)!=null);
            return chat;
        }

        /// <summary>
        /// Gets all chats with specific user if there are any chat, else will return null
        /// </summary>
        /// <param name="withUserId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Chat>> GetAllChatWithAsync(string withUserId)
        {
            var withUser = Users.Find(withUserId);
            if (withUser == null)
                return null;

            var chats = (await UsersChats.Where(u => u.UserId==withUserId )
                .Select(u=>u.Chat).ToListAsync())
                .Intersect(await GetAllChatsAsync());
            return chats;
        }


        /// <summary>
        /// Converts there isRead value to true
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageIds"></param>
        public async Task<bool> ReadChatAsync(string chatId)
        {
            var x = CurrentUser.UsersChats.SingleOrDefault(c => c.Chat.Id == chatId);
            if (x == null)
            {
                return false;
            }
            x.UnReadMessagesCount = 0;
            x.IsRead = true;
            UsersChats.Attach(x);
            Db.Entry(x).State = EntityState.Modified;
            await Db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Returns List of all chats belongs to the current user
        /// </summary>
        /// <returns>List of all chats</returns>
        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            var chats = await UsersChats
                .Where(uc => uc.User.Id == CurrentUser.Id)
                .Select(uc => uc.Chat)
                .OrderBy(c => c.ModifyDate).ToListAsync();
            return chats;
        }

        public async Task<Chat> GetChatByIdAsync(string chatId)
        {
            var chat = (await GetUsersChatsAsync(chatId)).Chat;
            if (chat == null)
                return null;
            if(chat.ChatType != ChatType.ChatRoom)
            {
                chat.UsersChats = await UsersChats.Include(uc => uc.User).Where(uc => uc.ChatId == chatId).ToListAsync();
                chat.Messages = await GetMessagesAsync(chatId);
            }
            return chat;
        }


        public async Task<bool> AddToChatGroupAsync(string chatId, string userId)
        {
            var uc = await GetUsersChatsAsync(chatId);
            var user = Users.Find(userId);
            if (uc != null && uc.Rools == UserChatRools.Admin && user!= null)
            { 
                UsersChats.Add(new UsersChats()
                {
                    ChatId = uc.ChatId,
                    Rools = UserChatRools.User,
                    UserId = userId
                }).State = EntityState.Added;

                await Db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create New Chat Between the current user and another users
        /// Returns The created chat
        /// </summary>
        /// <param name="withUserId"></param>
        /// <returns>The Id of created chat</returns>
        public async Task<Chat> CreateChatAsync(Chat chat)
        {
            if (chat.ChatType == ChatType.Personal)
                return null;
            if (chat.ChatType == ChatType.ChatRoom)
            {
#warning  this implementation is wrong "Should tell the user the reason behind the null value"
                if (await Chats.SingleOrDefaultAsync(c => c.Name == chat.Name) != null)
                    return null;
            }
            return await CreateChatAsync(chat.ChatType, chat.UsersChats?.Select(c => c.User).ToArray(), chat);
        }
        
        public async Task<bool> RemoveChatById(string chatId)
        {
            var chat = await Chats.FindAsync(chatId);
            if (chat == null || chat.ChatType == ChatType.Personal)
                return false;
            Chats.Remove(chat).State = EntityState.Deleted;
            await Db.SaveChangesAsync();
            return true;
        }

        private async Task<Chat> CreateChatAsync(ChatType type, User[] withUsers = null, Chat chat = null)
        {
            if (withUsers?.Length == 1 && type == ChatType.Personal)
            {
                var existChat = await GetChatWithAsync(withUsers.First().Id);
                if (existChat != null)
                    return existChat;
            }
            chat = new Chat()
            {
                Name = chat?.Name,
                CreatedByUser = CurrentUser,
                ChatType = chat?.ChatType ?? type
            };

            bool isCurrentUserAdded = false;
            if (withUsers != null)
                foreach (var user in withUsers)
                {
                    if (withUsers.Length == 1)
                    {
                        chat.UsersChats.Add(new UsersChats() { Chat = chat, User = user, Rools = UserChatRools.Admin });
                        break;
                    }
                    isCurrentUserAdded = (user.Id == CurrentUser.Id);
                    if (isCurrentUserAdded)
                        chat.UsersChats.Add(new UsersChats() { Chat = chat, User = user, Rools = UserChatRools.Admin });
                    chat.UsersChats.Add(new UsersChats() { Chat = chat, User = user, Rools = UserChatRools.User });
                }
            if (!isCurrentUserAdded)
                chat.UsersChats.Add(new UsersChats() { Chat = chat, User = CurrentUser, Rools = UserChatRools.Admin });

            var entityEntryChat = await Chats.AddAsync(chat);
            entityEntryChat.State = EntityState.Added;
            UsersChats.AddRange(chat.UsersChats);
            //if the problem not solved must separate the UsersChats collection and add it later after saveChages for adding the chat
            // be cause the UsersChats elements has a references to the chat
            await Db.SaveChangesAsync();
            return entityEntryChat.Entity;
        }

        private async Task<UsersChats> GetUsersChatsAsync(string chatId)
        {
            var Uc = await UsersChats
                .Include(uc => uc.Chat)
                .Include(uc => uc.User)
                .SingleOrDefaultAsync(uc => uc.Chat.Id == chatId && (uc.UserId == CurrentUser.Id || uc.Chat.ChatType == ChatType.ChatRoom));
            return Uc;
        }

        private async Task<List<Message>> GetMessagesAsync(string chatId, int skip=0, int take = 10)
        {

            var query = await Messages
                        .Where(m => m.ChatId == chatId)
                        .Include(m => m.Attachments).Include(m => m.FromUser)
                        .OrderByDescending(m => m.Date)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
            return query;
        }
    }
}
