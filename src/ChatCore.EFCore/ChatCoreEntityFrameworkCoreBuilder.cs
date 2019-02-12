using System;
using ChatCore.EFCore.Managers;
using ChatCore.Abstractions;
using ChatCore.EFCore;
using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ChatCoreEntityFrameworkCoreBuilder
    {
        public ChatCoreEntityFrameworkCoreBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }


        public IServiceCollection Services { get; }


        public ChatCoreEntityFrameworkCoreBuilder Configure(Action<ChatCoreEntityFrameworkCoreOptions> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Services.Configure(configuration);

            Services.TryAddScoped<IAccountManager<User, string>, AccountManager>();
            Services.TryAddScoped<IChatManager<Chat,string>, ChatManager>();
            Services.TryAddScoped<IContactManager<FriendRequest,User, string>, ContactManager>();
            Services.TryAddScoped<IMessageManager<Message, string>, MessageManager>();
            Services.TryAddScoped(typeof(ChatCoreContext));


            return this;
        }


        public ChatCoreEntityFrameworkCoreBuilder UseDbContext<TContext>()
            where TContext : DbContext
            => UseDbContext(typeof(TContext));


        public ChatCoreEntityFrameworkCoreBuilder UseDbContext(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(DbContext).IsAssignableFrom(type))
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            return Configure(options => options.DbContextType = type);
        }

    }
}
