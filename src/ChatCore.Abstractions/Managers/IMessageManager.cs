using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatCore.Abstractions
{
    public interface IMessageManager<TMessage, Tkey>
        where TMessage:class
        where Tkey :IEquatable<Tkey>
    {
        Task<TMessage> SendMessageAsync(TMessage message);
        Task<TMessage> SendServerMessageAsync(TMessage message);
        Task<IEnumerable<TMessage>> GetMessagesAsync(Tkey chatId, int skip, int take);
    }
}
