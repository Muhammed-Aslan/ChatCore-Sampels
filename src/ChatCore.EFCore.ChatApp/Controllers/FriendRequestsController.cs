using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ChatCore.ChatApp.ViewModels;
using ChatCore.EFCore.ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
namespace ChatCore.ChatApp.Controllers
{
    [Authorize]
    [CustomRoute("[controller]")]
    [ApiController]
    public class FriendRequestsController : CustomControllerBase
    {

        // GET: ChatApp/FriendRequsts
        [HttpGet]
        [Route("All")]
        public async Task<IEnumerable<FriendRequestViewModel>> GetFriendRequsts()
        {
            return Mapper.Map<IEnumerable<FriendRequestViewModel>>(await ChatContext.ContactManager.GetAllFriendRequstsAsync());
        }

        [HttpGet]
        [Route("From")]
        public async Task<IEnumerable<FriendRequestViewModel>> GetFriendRequstsFrom()
        {
            return Mapper.Map<IEnumerable<FriendRequestViewModel>>(await ChatContext.ContactManager.GetFriendRequstsFromAsync());
        }

        [HttpGet]
        [Route("To")]
        public async Task<IEnumerable<FriendRequestViewModel>> GetFriendRequstsTo()
        {
            return Mapper.Map<IEnumerable<FriendRequestViewModel>>(await ChatContext.ContactManager.GetFriendRequstsToAsync());
        }

        [HttpPost]
        [Route("Send/{toUserId}")]
        public async Task<IActionResult> Send([FromRoute]string toUserId)
        {
            var request = await ChatContext.ContactManager.SendAddFriendRequestAsync(toUserId);
            if (request != null)
            {
                var requestDto = Mapper.Map<FriendRequestViewModel>(request);

                var connections = new List<string>(ConnectionMap.GetConnections(request.ToUserId));

                _ = HubContext.Clients.Clients(connections).SendAsync(SignalRClientMethod.ReceiveFriendRequest, requestDto);
                return Ok(requestDto);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("Accept/{requestId}")]
        public async Task<IActionResult> Accept([FromRoute]string requestId)
        {
            var contact = await ChatContext.ContactManager.AcceptAddFriendRequestAsync(requestId);
            if (contact != null)
            {
                var contactDto = Mapper.Map<ContactViewModel>(contact);
                var currUserDto = Mapper.Map<ContactViewModel>(ChatContext.CurrentUser);

                var contactConnections = new List<string>(ConnectionMap.GetConnections(contact.Id));
                _ = HubContext.Clients.Clients(contactConnections).SendAsync(SignalRClientMethod.ReceiveFriendRequestResponse, requestId, currUserDto);

                var currConnections = new List<string>(ConnectionMap.GetConnections(ChatContext.CurrentUser.Id));
                _ = HubContext.Clients.Clients(currConnections).SendAsync(SignalRClientMethod.ReceiveFriendRequestResponse, requestId, contactDto);

                var chat = await ChatContext.ChatManager.GetChatWithAsync(contact.Id);
                foreach (var connection in contactConnections.Concat(currConnections))
                {
                    _= HubContext.Groups.AddToGroupAsync(connection,chat.Id);
                }

                return Ok(new {requestId, contact = contactDto });
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("Remove/{requestId}")]
        public async Task<IActionResult> Remove([FromRoute] string requestId)
        {
            var request = await ChatContext.ContactManager.RemoveFriendRequestByIdAsync(requestId);
            if(request != null)
            {
                var userId = request.ToUserId == ChatContext.CurrentUser.Id ? request.FromUserId : request.ToUserId;
                var connections = new List<string>(ConnectionMap.GetConnections(userId));
                _ = HubContext.Clients.Clients(connections).SendAsync(SignalRClientMethod.ReceiveFriendRequestResponse, requestId, null);
                return Ok(true);
            }
            return BadRequest();
        }
    }
}