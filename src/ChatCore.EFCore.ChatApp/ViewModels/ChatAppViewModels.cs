using ChatCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatCore.ChatApp.ViewModels
{
    public class ChatViewModel
    {
        public string Id { get; set; }

        public ChatType ChatType { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public int UnReadMessagesCoun { get; set; }

        public DateTime CreateDate { get; }

        public ContactViewModel CreatedByUser { get; set; }

        public DateTime ModifyDate { get; set; }

        public IEnumerable<MessageViewModel> Messages { get; set; }

        public IEnumerable<ContactViewModel> Users { get; set; }
    }

    public class MessageViewModel
    {
        public string Id { get; set; }

        public MessageType MessageType { get; set; }

        [StringLength(255)]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string ChatId { get; set; }

        [Required]
        public string FromUserId { get; set; }

        public IEnumerable<AttachmentViewModel> Attachments { get; set; }

    }

    public class UserViewModel : ContactViewModel
    {
        public string AccountId { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool HasFriendRequest { get; set; }

        public IEnumerable<ContactViewModel> Contacts { get; set; }
    }

    public class ContactViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Image { get; set; }

        public string StatusMessage { get; set; }

        public Status Status { get; set; }

        public Gender Gender { get; set; }
    }

    public class AttachmentViewModel
    {
        public string Id { get; set; }

        public AttachmentType AttachmentType { get; set; }

        public string Url { get; set; }
    }

    public class FriendRequestViewModel
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public ContactViewModel FromUser { get; set; }

        public ContactViewModel ToUser { get; set; }
    }
}
