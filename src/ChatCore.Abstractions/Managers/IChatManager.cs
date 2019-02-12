using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatCore.Abstractions
{
    public interface IChatManager<TChat, Tkey> 
        where TChat : class
        where Tkey :IEquatable<Tkey>
    {

        /// <summary>
        /// Create New Chat Between the current user and another users
        /// Returns The created chat
        /// </summary>
        /// <param name="withUserId"></param>
        /// <returns>The Id of created chat</returns>
        Task<TChat> CreateChatAsync(TChat chat);

        Task<bool> RemoveChatById(Tkey chatId);
        /// <summary>
        /// Returns List of all chats belongs to the current user
        /// </summary>
        /// <returns>List of all chats</returns>
        Task<IEnumerable<TChat>> GetAllChatsAsync();
        
        Task<bool> ReadChatAsync(Tkey chatId);

        /// <summary>
        /// Gets chat with specific user if the chat if exist, else will return null
        /// </summary>
        /// <param name="withUserId"></param>
        /// <returns></returns>
        Task<TChat> GetChatWithAsync(Tkey withUserId);
        Task<IEnumerable<TChat>> GetAllChatWithAsync(Tkey withUserId);

        Task<TChat> GetChatByIdAsync(Tkey chatId);

        Task<bool> AddToChatGroupAsync(Tkey chatId, Tkey userId);
    }
}
