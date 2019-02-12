using ChatCore.Enums;
using System;

namespace ChatCore.Models
{

    public class Attachment:Attachment<Message,string>
    {
        public Attachment()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
    public class Attachment<TMessage,TKey>
        where TMessage :class
        where TKey:IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public AttachmentType AttachmentType { get; set; }

        public string Uri { get; set; }
        
        public string MessageId { get; set; }
        public virtual TMessage Message { get; set; }
    }
}
