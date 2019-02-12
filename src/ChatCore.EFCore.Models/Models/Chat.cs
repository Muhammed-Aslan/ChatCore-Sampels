using ChatCore.Enums;
using System;
using System.Collections.Generic;

namespace ChatCore.Models
{
    public class Chat
    {
        public Chat()
        {
            Id = Guid.NewGuid().ToString();
            ModifyDate = DateTime.Now;
            CreateDate = DateTime.Now;
        }
        public string Id { get; set; }

        public ChatType ChatType { get; set; }
        
        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; }

        public DateTime ModifyDate { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public string CreatedByUserId { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual ICollection<UsersChats> UsersChats { get; set; }
    }

}
