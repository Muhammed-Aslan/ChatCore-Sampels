using System;

namespace ChatApp.Abstractions
{

    public interface IIdentityProvider<TKey> where TKey :IEquatable<TKey>
    {
        TKey CurrentUserId { get; }
    }
}