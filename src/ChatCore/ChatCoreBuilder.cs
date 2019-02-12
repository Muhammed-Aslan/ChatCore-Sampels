using ChatApp.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ChatCoreBuilder
    {
        public IServiceCollection Services { get; }
        public ChatCoreBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public ChatCoreBuilder UseIdentityProvider<TIdentityProvider>()
        {
            return UseIdentityProvider(typeof(TIdentityProvider));
        }
        public ChatCoreBuilder UseIdentityProvider (Type IdentityProvider)
        {
            if (!typeof(IIdentityProvider<string>).IsAssignableFrom(IdentityProvider))
            {
                var errorMsg = new StringBuilder()
                    .AppendLine($"The {nameof(IdentityProvider)} type is invalid.")
                    .AppendLine($"The {nameof(IdentityProvider)} type Must implement ChatCore.Abstractions.IIdentityProvider interface .");
                throw new ArgumentException(errorMsg.ToString());
            }

            Services.AddOptions();
            Services.TryAddTransient(typeof(IIdentityProvider<string>), IdentityProvider);

            return this;
        }
    }
}