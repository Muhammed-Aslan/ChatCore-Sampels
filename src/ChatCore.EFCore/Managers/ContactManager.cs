using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatCore.Abstractions;
using ChatCore.Models;
using System;
using ChatCore.Enums;

namespace ChatCore.EFCore.Managers
{
    public class ContactManager :ManagerBase, IContactManager<FriendRequest,User,string>
    {
        public ContactManager(IServiceProvider provider) : base(provider)
        { }
        
        /// <summary>
        /// Returns List of Friend requests or Null
        /// </summary>
        public async Task<IEnumerable<FriendRequest>> GetFriendRequstsToAsync()
        {
            if (CurrentUser.HasFriendRequest)
            {
                await Db.Entry(CurrentUser).Collection(u => u.FriendRequestsTo).Query().Include(r=>r.FromUser).OrderBy(x=>x.Date).LoadAsync();
                return CurrentUser.FriendRequestsTo.ToList();
            }
            return Enumerable.Empty<FriendRequest>();
        }

        public async Task<IEnumerable<FriendRequest>> GetAllFriendRequstsAsync()
        {
            return await FriendRequests.Where(r => r.FromUserId == CurrentUser.Id || r.ToUserId == CurrentUser.Id)
                .Include(r=>r.FromUser)
                .Include(r=>r.ToUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequstsFromAsync()
        {
            await Db.Entry(CurrentUser).Collection(u => u.FriendRequestsFrom).Query().Include(r => r.ToUser).OrderBy(x => x.Date).LoadAsync();
            return CurrentUser.FriendRequestsTo.ToList();
        }

        /// <summary>
        /// Send a Add request to specific user.. Returns the request  
        /// </summary>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public async Task<FriendRequest> SendAddFriendRequestAsync(string toUserId)
        {
            if (CurrentUser.Id == toUserId)
            {
                return null;
            }
            var toUser = await Users.FindAsync(toUserId);

            if (toUser == null || (await GetAllContactsAsync()).Any(x => x.Id == toUserId))
                return null;

            await Db.Entry(CurrentUser).Collection(u => u.FriendRequestsFrom).LoadAsync();
            if (CurrentUser.FriendRequestsFrom.Any(r => r.ToUserId == toUserId) ||
                (await GetAllFriendRequstsAsync()).Any(f => f.FromUserId == toUser.Id))
                return null;

            FriendRequest request = new FriendRequest()
            {
                FromUser = CurrentUser,
                ToUser = toUser
            };
            (await FriendRequests.AddAsync(request)).State = EntityState.Added;

            toUser.HasFriendRequest = true;
            Db.Entry(toUser).State = EntityState.Modified;
            await Db.SaveChangesAsync();
            return request;
        }

        /// <summary>
        /// Accept a specific friend request 
        /// </summary>
        /// <param name="id">The specific friend Id</param>
        public async Task<User> AcceptAddFriendRequestAsync(string requestId)
        {
            
            var request = await RemoveFriendRequestByIdAsync(requestId);

            var user = request?.ToUserId == CurrentUser.Id ? request.FromUser:null;

            if (user == null)
                return null;

            var chat = new Chat()
            {
                CreatedByUser = CurrentUser,
                ChatType = ChatType.Personal
            };
            var uc = new List<UsersChats>()
            {
                new UsersChats {Chat = chat,User = CurrentUser,Rools= UserChatRools.Admin },
                new UsersChats {Chat = chat,User = user,Rools= UserChatRools.Admin }
            };
            chat.UsersChats = uc;
            (await Chats.AddAsync(chat)).State = EntityState.Added;

            await UsersChats.AddRangeAsync(uc);

            var friends = new Friends() { FromUser = user, ToUser = CurrentUser ,Chat =chat};
            (await Friends.AddAsync(friends)).State = EntityState.Added;

            await Db.SaveChangesAsync();

            return user;
        }
        
        /// <summary>
        /// Remove a specific friend request "From or To"
        /// </summary>
        /// <param name="id">The specific friend Id</param>
        public async Task<FriendRequest> RemoveFriendRequestByIdAsync(string requestId)
        {
            var friendRequests =
                await FriendRequests
                .Include(r=>r.FromUser)
                .Include(r=>r.ToUser)
                .Where(r => r.FromUser.Id == CurrentUser.Id || r.ToUser.Id == CurrentUser.Id).ToListAsync();

            var request = friendRequests?.SingleOrDefault(r => r.Id == requestId);

            if (request == null)
                return null;

            var user = (request.ToUserId == CurrentUser.Id) ? request.ToUser : request.FromUser;

            (FriendRequests.Remove(request)).State = EntityState.Deleted;
            friendRequests.Remove(request);
            user.HasFriendRequest = friendRequests.Any(r => r.ToUser.Id == user.Id);
            Db.Entry(user).State = EntityState.Modified;

            await Db.SaveChangesAsync();
            return request;
        }
        
        /// <summary>
        /// Delete the friend with his chat and all messages
        /// </summary>
        /// <param name="contactId">friend user id</param>
        /// <returns>true if deleted successfully else returns false</returns>
        public async Task<bool> RemoveContactByIdAsync(string contactId)
        {
            var delfriend = await Friends.Include(f=>f.Chat).SingleOrDefaultAsync(f => (f.ToUser.Id == CurrentUser.Id && f.FromUser.Id == contactId) ||
                                                         (f.ToUser.Id == contactId && f.FromUser.Id == CurrentUser.Id));
            if (delfriend == null)
                return false;

            (Friends.Remove(delfriend)).State = EntityState.Deleted;
            (Chats.Remove(delfriend.Chat)).State = EntityState.Deleted;
            await Db.SaveChangesAsync();
            return true;
        }
        
        /// <summary>
        /// Returns All friends
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllContactsAsync()
        {
            var friends = await Friends
                .Include(f => f.FromUser)
                .Include(f => f.ToUser)
                .Where(f => f.ToUser.Id == CurrentUser.Id || f.FromUser.Id == CurrentUser.Id).ToListAsync();

            var contacts = new List<User>();
            foreach (var f in friends)
            {
                if (f.ToUser.Id == CurrentUser.Id)
                    contacts.Add(f.FromUser);
                else
                    contacts.Add(f.ToUser);
            };
            contacts.OrderBy(u => u.UserName);
            return contacts;
        }
        
        
    }
}
