using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ChatCore.Enums;
using ChatCore.EFCore;
using ChatCore.ChatApp.Services;
using ChatCore.EFCore.ChatApp.Services;

namespace ChatCore.ChatApp.Hubs
{
    public class ChatCoreHub : Hub
    {
        private ChatCoreContext ChatContext { get; }
        private ConnectionMap ConnectionMap { get; }

        public ChatCoreHub(ConnectionMap connectionMap, ChatCoreContext chatContext)
        {
            ConnectionMap = connectionMap;
            ChatContext = chatContext;
        }



        public async Task StatusChanged(Status status)
        {
            var user = ChatContext.CurrentUser;
            user.Status = status;

            await ChatContext.AccountManager.ChangeUserStatusAsync(user);

            foreach (var chat in (await ChatContext.ChatManager.GetAllChatsAsync()))
            {
                if (chat.ChatType == ChatType.Personal)
                    _= Clients.GroupExcept(chat.Id,Context.ConnectionId).SendAsync(SignalRClientMethod.ReceiveStatusChanged, user.Id, status);
            }

        }



        public override async Task OnConnectedAsync()
        {
            ConnectionMap.Add(ChatContext.CurrentUser.Id, Context.ConnectionId);
            var chats = await ChatContext.ChatManager.GetAllChatsAsync();

            var user = ChatContext.CurrentUser;
            user.Status = Status.Online;

            await ChatContext.AccountManager.ChangeUserStatusAsync(user);
            //Create tunnel to be enable for receive chat messages or other friends actions
            foreach (var chat in chats)
            {
                //Convert the status to be Online
                if (chat.ChatType == ChatType.Personal)
                    await Clients.Group(chat.Id).SendAsync(SignalRClientMethod.ReceiveStatusChanged, ChatContext.CurrentUser.Id, Status.Online);

                _= Groups.AddToGroupAsync(Context.ConnectionId, chat.Id);
            }
            
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionMap.Remove(ChatContext.CurrentUser.Id, Context.ConnectionId);
            await StatusChanged(Status.Offline);
            await base.OnDisconnectedAsync(exception);
        }
    }
}