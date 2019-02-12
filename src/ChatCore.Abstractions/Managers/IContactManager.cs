using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ChatCore.Abstractions
{
    public interface IContactManager<TFriendRequest,TUser,Tkey>
        where TFriendRequest :class
        where TUser :class
        where Tkey :IEquatable<Tkey>
    {
        Task<IEnumerable<TUser>> GetAllContactsAsync();
        Task<bool> RemoveContactByIdAsync(Tkey id);

        Task<IEnumerable<TFriendRequest>> GetAllFriendRequstsAsync();
        Task<IEnumerable<TFriendRequest>> GetFriendRequstsFromAsync();
        Task<IEnumerable<TFriendRequest>> GetFriendRequstsToAsync();
        Task<TUser> AcceptAddFriendRequestAsync(Tkey requestId);
        Task<TFriendRequest> SendAddFriendRequestAsync(Tkey toUserId);
        Task<TFriendRequest> RemoveFriendRequestByIdAsync(Tkey requestId);

    }
}
