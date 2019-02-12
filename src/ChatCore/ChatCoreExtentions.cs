using ChatApp.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ChatCoreExtentions
    {
        public static ChatCoreBuilder AddChatCore(this IServiceCollection services, Type IdentityProvider)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new ChatCoreBuilder(services).UseIdentityProvider(IdentityProvider); 
        }

        public static ChatCoreBuilder AddChatCore<TIdentityProvider>(this IServiceCollection services)where TIdentityProvider:IIdentityProvider<string>
        {
            return AddChatCore(services, typeof(TIdentityProvider));
        }

        public static IServiceCollection AddChatCore<TIdentityProvider>(this IServiceCollection services,Action<ChatCoreBuilder> configuration)where TIdentityProvider :IIdentityProvider<string>
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration(AddChatCore(services, typeof(TIdentityProvider)));

            return services;
        }

        public static IServiceCollection AddChatCore(this IServiceCollection services ,Action<ChatCoreBuilder> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            configuration(new ChatCoreBuilder(services));
            return services;
        }
    }
}
