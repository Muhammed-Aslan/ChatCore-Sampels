using ChatCore.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatCore.Models
{
    public class UsersChats
    {
        public UsersChats()
        {
            Id = Guid.NewGuid().ToString();
            IsRead = IsKicked = IsPanned = false;
            JoinDate = DateTime.Now;
            UnReadMessagesCount = 0;
        }
        [Key]
        public string Id { get; set; }

        public bool IsRead { get; set; }

        public bool IsPanned { get; set; }
        public bool IsKicked { get; set; }

        public UserChatRools Rools { get; set; }
        

        public DateTime KickDate { get; set; }
        public DateTime PannedDate { get; set; }

        public DateTime JoinDate { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public int UnReadMessagesCount { get; set; }

    }
}
