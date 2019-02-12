using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ChatCore.Abstractions
{
    public interface IAccountManager<TUser, Tkey>
        where TUser : class
        where Tkey :IEquatable<Tkey>
    {
        TUser CurrentUser { get; }
        Task<TUser> CreateUserAsync(TUser user, Tkey accoutId);
        Task<bool> DeleteUserAsync(TUser user);
        Task<bool> DeleteUserAsync(Tkey userId);
        Task<TUser> GetUserByEmailAsync(string email);
        Task<TUser> GetUserByIdAsync(Tkey userId);
        Task<TUser> GetUserByAccountIdAsync(Tkey userId);
        Task<TUser> GetUserByUserNameAsync(string userName);
        Task<IEnumerable<TUser>> GetUsersAsync(int page, int pageSize);
        Task<TUser> UpdateUserAsync(TUser user);
        Task ChangeUserStatusAsync(TUser user);
        Task<IEnumerable<TUser>> GetMatchUsersAsync(int page, int pageSize, string userName );

    }
}
