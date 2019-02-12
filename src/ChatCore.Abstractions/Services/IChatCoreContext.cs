using System;

namespace ChatCore.Abstractions
{
    public interface IChatCoreContext<TFriendRequest, TChat, TChatApplicationUser, TMessage, Tkey>
        where TFriendRequest : class
        where TChat : class
        where TChatApplicationUser : class
        where TMessage : class
        where Tkey : IEquatable<Tkey>
    {
        IAccountManager<TChatApplicationUser, Tkey> AccountManager { get; }
        IChatManager<TChat, Tkey> ChatManager { get; }
        IContactManager<TFriendRequest, TChatApplicationUser, Tkey> ContactManager { get; }
        IMessageManager<TMessage, Tkey> MessageManager { get; }
    }
}