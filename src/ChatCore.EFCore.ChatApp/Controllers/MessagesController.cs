using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using ChatCore.ChatApp.ViewModels;
using Microsoft.AspNetCore.SignalR;
using ChatCore.Models;
using System.Linq;
using ChatCore.EFCore.ChatApp.Services;

namespace ChatCore.ChatApp.Controllers
{
    [Authorize]
    [CustomRoute("[controller]")]
    [ApiController]
    public class MessagesController : CustomControllerBase
    {
        //SendMessage
        // POST: ChatApp/Messages
        [HttpPost]
        public async Task<IActionResult> SendMessageAsync([FromBody] MessageViewModel message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var msg = Mapper.Map<Message>(message);
            msg.Date = DateTime.Now;
            var sentMsg = await ChatContext.MessageManager.SendMessageAsync(msg);

            if (sentMsg == null)
            {
                ModelState.AddModelError("Invalid Message", "ChatId is not valid !");
                return BadRequest(ModelState);
            }
            var sentMsgDto = Mapper.Map<MessageViewModel>(sentMsg);

            if (ConnectionMap.GetConnections(ChatContext.CurrentUser.Id).Count() > 1)
            {
                _ = HubContext.Clients.Group(sentMsgDto.ChatId).SendAsync(SignalRClientMethod.ReceiveMessage, sentMsgDto);
                return Ok();
            }

            else
            {
                _ = HubContext.Clients.GroupExcept(sentMsgDto.ChatId, ConnectionMap.GetConnections(ChatContext.CurrentUser.Id).First()).SendAsync(SignalRClientMethod.ReceiveMessage, sentMsgDto);
                return Ok(sentMsgDto);
            }
        }

        // POST: ChatApp/Messages/{chatId}/{skip}/{take}

        [HttpGet("{chatId}/{skip}/{take}")]
        public async Task<IEnumerable<MessageViewModel>> GetMessagesAsync([FromRoute]string chatId,int skip,int take=10)
        {
            var msgs = await ChatContext.MessageManager.GetMessagesAsync(chatId, skip, take);
            return Mapper.Map<IEnumerable<MessageViewModel>>(msgs);
        }

    }
}