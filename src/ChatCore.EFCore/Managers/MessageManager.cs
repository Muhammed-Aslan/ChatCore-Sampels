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
    public class MessageManager :ManagerBase, IMessageManager<Message,string>
    {
        public MessageManager(IServiceProvider provider) : base(provider)
        { }


        /// <summary>
        /// Send message from server to appear in chat window between the messages;
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message">Must has a chat if the chatId parameter is null and must has MessageType </param>
        /// <returns></returns>
        public async Task<Message> SendServerMessageAsync(Message message)
        {
            if (!"345server".Contains(message.MessageType.ToString().ToLower()))
            {
                return null;
            }
            return await Send(message,false);
        }

        /// <summary>
        /// Sent message to a specific chat or user
        /// And returns true if sent successfully else return false
        /// </summary>
        /// <param name="chatId">chat that message will sent to this parameter is preferred to be entered it's giving "The best performance"</param>
        /// <param name="toUserId">message will sent to</param>
        /// <param name="messageContent">message content max length is 255 character</param>
        /// <returns></returns>
        public async Task<Message> SendMessageAsync(Message message)
        {
            if ("345server".Contains(message.MessageType.ToString().ToLower()))
                return null;
            return await Send(message);
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string chatId,int skip=0,int take=10)
        {
            
            var query = await Messages
                        .Where(m => m.ChatId == chatId)
                        .Include(m=>m.Attachments).Include(m=>m.FromUser)
                        .OrderByDescending(m => m.Date)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
            return query;
        }
        
        private async Task<Message> Send(Message message,bool isNoramlMessage = true)
        {
            await Db.Entry(CurrentUser).Collection(u => u.UsersChats).Query().Include(uc => uc.Chat).LoadAsync();
            var chat = CurrentUser.UsersChats.SingleOrDefault(uc => uc.ChatId == message.ChatId)?.Chat;
            if (chat == null)
                return null;
            message.Chat = chat;
            // if chatRoom don't save to databse
            if (chat.ChatType == ChatType.ChatRoom && isNoramlMessage)
                return message;
            await Messages.AddAsync(message);
            if (message.Attachments != null)
            {
                await Attachments.AddRangeAsync(message.Attachments);
            }
            await Db.SaveChangesAsync();
            return message;
        }
    }
}