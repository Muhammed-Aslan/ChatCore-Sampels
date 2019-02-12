using System;

namespace ChatCore.Models
{
    public class FriendRequest
    {
        public FriendRequest()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Now;
        }
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string FromUserId { get; set; }
        public virtual User FromUser { get; set; }

        public string ToUserId { get; set; }
        public virtual User ToUser { get; set; }
    }

}
