using ChatCore.Enums;
using System;
using System.Collections.Generic;

namespace ChatCore.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string AccountId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        /// <summary>
        /// ChatApplicationUser Current status - for example online - offline
        /// </summary>
        public Status Status { get; set; }
        public string StatusMessage { get;  set; }
        public string Image { get;  set; }


        public Gender Gender { get; set; }
        public bool HasFriendRequest { get;  set; }

        /// <summary>
        /// Chats that created by this ChatApplicationUser
        /// </summary>
        public ICollection<Chat> OwnedChats { get; set; }

        /// <summary>
        /// Represents the relation between the chats and ChatApplicationUsers belong to them
        /// </summary>
        public ICollection<UsersChats> UsersChats { get; set; }

        /// <summary>
        /// Messages sent by "From" this ChatApplicationUser
        /// </summary>
        public ICollection<Message> Messages { get;  set; }
       
        /// <summary>
        /// Requests Sent by this ChatApplicationUser to an other ChatApplicationUsers
        /// </summary>
        public ICollection<FriendRequest> FriendRequestsFrom { get; set; }

        /// <summary>
        /// Requests sent to this ChatApplicationUser from an other ChatApplicationUsers
        /// </summary>
        public ICollection<FriendRequest> FriendRequestsTo { get; set; }
        
        public ICollection<Friends> FriendsFrom { get; set; }
        public ICollection<Friends> FriendsTo { get; set; }
    }
}
