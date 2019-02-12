using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ChatCore.ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ChatCore.Models;
using ChatCore.Enums;

namespace ChatCore.ChatApp.Controllers
{
    [Authorize]
    [CustomRoute("[controller]")]
    [ApiController]
    public class ChatController : CustomControllerBase
    {
        // GET: /ChatApp/Chat
        [HttpGet]
        public async Task<IEnumerable<ChatViewModel>> GetAllChatsAsync()//IEnumerable<ChatViewModel>
        {
            return Mapper.Map<IEnumerable<ChatViewModel>>(await ChatContext.ChatManager.GetAllChatsAsync());
        }

        // GET: /ChatApp/Chat/chatId
        [HttpGet]
        [Route("ChatId/{chatId}")]
        public async Task<ChatViewModel> GetChatByIdAsync([FromRoute]string chatId)
        {
            return Mapper.Map<ChatViewModel>(await ChatContext.ChatManager.GetChatByIdAsync(chatId));
        }

        [HttpGet]
        [Route("UserId/{userId}")]
        public async Task<ChatViewModel> GetChatWithAsync([FromRoute]string userId)
        {
            return Mapper.Map<ChatViewModel>(await ChatContext.ChatManager.GetChatWithAsync(userId));
        }
        
        // POST: /ChatApp/Chat
        [ProducesResponseType(statusCode: 200, type: typeof(ChatViewModel))]
        [ProducesResponseType(statusCode: 400, type: typeof(ModelStateDictionary))]
        [HttpPost]
        public async Task<IActionResult> CreateChatAsync([FromBody]ChatViewModel chat)
        {
            if (ModelState.IsValid)
            {
                var _chat = Mapper.Map<Chat>(chat);
                var createdChat = await ChatContext.ChatManager.CreateChatAsync(_chat);
                if (createdChat != null)
                    return Ok(Mapper.Map<ChatViewModel>(createdChat));
                if (chat.ChatType == ChatType.Personal)
                {
                    ModelState.AddModelError("InvaledChatType", "Chat User cannot create chat with type Personal");
                }
                
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

    }
}