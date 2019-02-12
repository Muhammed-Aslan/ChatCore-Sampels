using System;
using ChatCore.Models;
using ChatApp.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChatCore.EFCore.Managers
{
    public abstract class ManagerBase
    {
        protected DbContext Db { get; }
        protected DbSet<Chat> Chats { get; }
        protected DbSet<Message> Messages { get; }
        protected DbSet<Attachment> Attachments { get; }
        protected DbSet<UsersChats> UsersChats { get; }
        protected DbSet<User> Users { get; }
        protected DbSet<FriendRequest> FriendRequests { get; }
        protected DbSet<Friends> Friends { get; }
        private string _currentUserApplicationId;
        private User _currentUser;
        public User CurrentUser => _currentUser ?? 
           (!string.IsNullOrWhiteSpace(_currentUserApplicationId)?
            (_currentUser = Users.SingleOrDefaultAsync(u => u.AccountId == _currentUserApplicationId).Result):
            throw new Exception("IdentityProvider provides invalid user id"));
            


        public ManagerBase(IServiceProvider provider)
        {
            var options = ((IOptions<ChatCoreEntityFrameworkCoreOptions>)provider.GetService(typeof(IOptions<ChatCoreEntityFrameworkCoreOptions>))).Value;
            var currenUserProvider = (IIdentityProvider<string>)provider.GetService(typeof(IIdentityProvider<string>));
            Db = (DbContext)provider.GetService(options.DbContextType);

            Chats = Db.Set<Chat>();
            Users = Db.Set<User>();
            Messages = Db.Set<Message>();
            Attachments = Db.Set<Attachment>();
            UsersChats = Db.Set<UsersChats>();
            FriendRequests = Db.Set<FriendRequest>();
            Friends = Db.Set<Friends>();
        
            _currentUserApplicationId = currenUserProvider?.CurrentUserId;
        }
    }
}
