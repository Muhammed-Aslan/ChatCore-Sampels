using ChatCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatCore.Models
{
    public class Message:Message<Chat,User,Attachment,string>
    {
        public Message()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Now;
            MessageType = MessageType.TextMessage;
        }
    }
    public class Message<TChat,TUser, TAttachment, TKey>
        where TChat:class
        where TUser:class
        where TAttachment : class
        where TKey:IEquatable<TKey>
    {

        public TKey Id { get; set; }

        public MessageType MessageType { get; set; }

        public DateTime Date { get; set; }

        public string Content { get; set; }

        public TKey ChatId { get; set; }

        public virtual TChat Chat { get; set; }
        
        public virtual ICollection<TAttachment> Attachments { get; set; }

        public TKey FromUserId { get; set; }
        public virtual TUser FromUser { get; set; }

    }
}
