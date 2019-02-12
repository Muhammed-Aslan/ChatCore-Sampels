using ChatCore.EFCore;
using ChatCore.ChatApp.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ChatCore.ChatApp.Services;

namespace ChatCore.ChatApp.Controllers
{
    public abstract class CustomControllerBase : ControllerBase
    {
        public CustomControllerBase()
        {
        }
        private ChatCoreContext _chatContext;
        private IHubContext<ChatCoreHub> _hubContext;
        protected ChatCoreContext ChatContext => _chatContext ??  (_chatContext = HttpContext.RequestServices.GetRequiredService<ChatCoreContext>());
        protected IHubContext<ChatCoreHub> HubContext => _hubContext ?? (_hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ChatCoreHub>>());
        private ConnectionMap _connectionMap;
        protected ConnectionMap ConnectionMap => _connectionMap ?? (_connectionMap = HttpContext.RequestServices.GetRequiredService<ConnectionMap>());


        //protected async Task SendTo<T>(T obj)
        //{
        //    HubContext.Clients.GroupExcept("",ConnectionMap.ge)
        //    return null;
        //}
    }
}
