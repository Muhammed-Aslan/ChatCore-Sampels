using ChatCore.Abstractions;
using ChatCore.Enums;
using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore.EFCore.Managers
{
    public class AccountManager : ManagerBase, IAccountManager<User, string>
    {

        public AccountManager(IServiceProvider provider) : base(provider)
        {
        }


        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await Users.SingleOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByAccountIdAsync(string userId)
        {
            return await Users.FirstOrDefaultAsync(u => u.AccountId == userId);
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
        {
            IQueryable<User> usersQuery = Users
                .AsNoTracking()
                .OrderBy(u => u.UserName);

            if (page > 0)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                usersQuery = usersQuery.Take(pageSize);

            return await usersQuery.ToListAsync();
        }
      
        /// <summary>
        /// Creates new chat user and throws an exception if there is any user using the given accountId
        /// </summary>
        /// <param name="user">The user to create </param>
        /// <param name="accoutId"></param>
        /// <exception cref="ArgumentException">Throws when the accountId is already has chat user registered </exception>
        public async Task<User> CreateUserAsync(User user, string accoutId)
        {
            NotNull(user, nameof(user));
            accoutId = accoutId ?? user.AccountId;
            NotNull(accoutId, nameof(accoutId));

            var existUser = await Users.SingleOrDefaultAsync(u => u.AccountId == accoutId);
            if (existUser != null)
                throw new ArgumentException($"Error Occurred while creating new Chat User {Environment.NewLine}" +
                                            $"there are exist user using the given AccountId !");
            user.AccountId = accoutId;
            user.Id = Guid.NewGuid().ToString();
            var entry = await Users.AddAsync(user);
            entry.State = EntityState.Added;
            await Db.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            NotNull(user, nameof(user));
            var oldUser = await Users.SingleOrDefaultAsync(u => u.Id == user.Id);
            if (!string.IsNullOrWhiteSpace(user.UserName) && user.UserName != oldUser.UserName)
            {
                var _user = await Users.FirstOrDefaultAsync(u => u.UserName == user.UserName && u.Id != user.Id);
                if (_user != null)
                    return null;
            }

            oldUser.Email = user.Email ?? oldUser.Email;
            oldUser.StatusMessage = user.StatusMessage ?? oldUser.StatusMessage;
            oldUser.Image = user.Image ?? oldUser.Image;

            Users.Attach(oldUser).State = EntityState.Modified;
            await Db.SaveChangesAsync();
            return oldUser;
        }

        public async Task<bool> DeleteUserAsync(string accountId)
        {
            var user = await Users.SingleOrDefaultAsync(u => u.AccountId == accountId);

            if (user != null)
                return await DeleteUserAsync(user);

            return false;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var query = new StringBuilder()
                .AppendLine($"DELETE FROM [Friends]")
                .AppendLine($"WHERE [Friends].[FromUserId] = '{user.Id}' OR [Friends].[ToUserId] = '{user.Id}' ")

                .AppendLine($"DELETE FROM [FriendRequest]")
                .AppendLine($"WHERE [FriendRequest].[FromUserId] = '{user.Id}' OR [FriendRequest].[ToUserId] = '{user.Id}' ")
            
                .AppendLine($"DELETE FROM [UsersChats]")
                .AppendLine($"WHERE [UsersChats].[UserId] = '{user.Id}' ")

                .AppendLine($"DELETE FROM [Chat]")
                .AppendLine($"WHERE [Chat].[CreatedByUserId] = '{user.Id}' AND [Chat].[ChatType] = 0 ");

            var transaction = await Db.Database.BeginTransactionAsync();
            
            await Db.Database.ExecuteSqlCommandAsync(new RawSqlString(query.ToString()));
            Users.Remove(user).State = EntityState.Deleted;
            await Db.SaveChangesAsync();
            transaction.Commit();
            return true;
        }

        public async Task<IEnumerable<User>> GetMatchUsersAsync(int page, int pageSize, string userName)
        {
            IQueryable<User> usersQuery = Users
                .Where(u => u.Id != CurrentUser.Id)
                .OrderBy(u => u.UserName);

            if (!string.IsNullOrWhiteSpace(userName))
                usersQuery = usersQuery.Where(u => u.UserName.Contains(userName));

            if (page > 0 && pageSize > 0)
                usersQuery = usersQuery.Skip((page - 1) * pageSize).Take(pageSize);

            return await usersQuery.ToListAsync();
        }

        public async Task<bool> ChangeUserStatusAsync(Status status)
        {
            CurrentUser.Status = status;
            Db.Entry(CurrentUser).State = EntityState.Modified;
            await Db.SaveChangesAsync();
            return true;
        }

        public async Task ChangeUserStatusAsync(User user)
        {
            CurrentUser.Status = user.Status;
            Db.Entry(CurrentUser).State = EntityState.Modified;
            await Db.SaveChangesAsync();
        }

        private void NotNull<T>(T obj ,string objName,string errorMessage= "cannot be null or Empty")
        {
            if (obj == null) throw new ArgumentNullException($"{objName} {errorMessage}");
        }
    }
}
