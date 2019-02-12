using System;

namespace ChatCore.Models
{
    public class Friends
    {
        public Friends()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }

        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public string FromUserId { get; set; }
        public virtual User FromUser { get; set; }

        public string ToUserId { get; set; }
        public virtual User ToUser { get; set; }

    }
}
