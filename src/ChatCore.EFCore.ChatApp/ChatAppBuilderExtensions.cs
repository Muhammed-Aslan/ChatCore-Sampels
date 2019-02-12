using Microsoft.AspNetCore.Builder;
using ChatCore.ChatApp.Hubs;
using AutoMapper;
using ChatCore.ChatApp.ViewModels;
using ChatCore.ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ChatAppBuilderExtensions
    {
        public static ChatCoreEntityFrameworkCoreBuilder AddChatApp(this ChatCoreEntityFrameworkCoreBuilder builder)
        {
            builder.Services.AddSingleton(typeof(ConnectionMap));
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            
            builder.Services.AddSignalR();
            return builder;
        }
       
        /// <summary>
        /// Fix signalR connection Headers by taking the access_token from QueryString and adding it into Request Headers 
        /// as Authorization header this will prepare signalR connection to be able for authentication otherwise signalR connections will be UnAuthorized
        /// </summary>
        public static IApplicationBuilder FixChatAppSignalRHeaders(this IApplicationBuilder app)
        {

            app.Use(async (httpContext, next) =>
            {
                var accessToken = httpContext.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";
                }
                await next();
            });
            return app;
        }

        /// <summary>
        /// Adds ChatApp Middleware to http pipeline and maps it to the specified path
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseChatApp(this IApplicationBuilder app, string MapPath = "/ChatApp",string HubEndPoint="/ChatHub")
        {
            if (!string.IsNullOrWhiteSpace(MapPath))
            {
                RouteCustomizer.Prefix = MapPath;
                MapPath = MapPath.StartsWith("/") ? MapPath : "/" + MapPath;
            }
            else MapPath = "/ChatApp";
            if (!string.IsNullOrWhiteSpace(HubEndPoint))
            {
                HubEndPoint = HubEndPoint.StartsWith("/") ? HubEndPoint : "/" + HubEndPoint;
            }
            else HubEndPoint = "/ChatHub";
            app.MapWhen(context => context.Request.Path.StartsWithSegments(MapPath),
                _app =>
                {
                    _app.UseSignalR(h => h.MapHub<ChatCoreHub>(MapPath + HubEndPoint));
                    MvcApplicationBuilderExtensions.UseMvc(app);
                });

            return app;
        }

    }
}