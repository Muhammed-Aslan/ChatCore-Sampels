using ChatCore.Models;
using ChatCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using ChatApp.Abstractions;

namespace ChatCore.EFCore
{

    public class ChatCoreContext : ChatCoreContext<FriendRequest, Chat, User, Message, string>
    {
        public ChatCoreContext(IContactManager<FriendRequest, User, string> contactManager,
                        IChatManager<Chat, string> chatManager,
                        IMessageManager<Message, string> messageManager,
                        IAccountManager<User, string> accountManager,
                        IIdentityProvider<string> currenUserProvider
                        ):base(contactManager,chatManager,messageManager,accountManager,currenUserProvider){}
    }

    public class ChatCoreContext<TFriendRequest, TChat, TUser, TMessage, TKey> :
        IChatCoreContext<TFriendRequest, TChat, TUser, TMessage, TKey>
        where TFriendRequest :FriendRequest
        where TChat :Chat
        where TUser:User
        where TMessage:Message
        where TKey :IEquatable<TKey>
        
    {
        public IAccountManager<TUser, TKey> AccountManager { get; }

        public IChatManager<TChat, TKey> ChatManager { get; }

        public IContactManager<TFriendRequest, TUser, TKey> ContactManager { get; }

        public IMessageManager<TMessage, TKey> MessageManager { get; }
        
        private IIdentityProvider<TKey> _identityProvider;

        public TUser CurrentUser => AccountManager.CurrentUser;


        public ChatCoreContext( IContactManager<TFriendRequest, TUser, TKey> contactManager,
                                IChatManager<TChat, TKey> chatManager,
                                IMessageManager<TMessage, TKey> messageManager,
                                IAccountManager<TUser, TKey> accountManager,
                                IIdentityProvider<TKey> identityProvider)
        {
            ContactManager = contactManager;
            ChatManager = chatManager;
            MessageManager = messageManager;
            AccountManager = accountManager;
            _identityProvider = identityProvider;
        }
    }
}
