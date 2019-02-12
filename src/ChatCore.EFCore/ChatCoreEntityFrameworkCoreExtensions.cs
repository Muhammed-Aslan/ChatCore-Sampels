using System;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes extensions allowing to register the ChatCore Entity Framework Core services.
    /// </summary>
    public static class ChatCoreEntityFrameworkCoreExtensions
    {
        /// <summary>
        /// Registers the Entity Framework Core stores services in the DI container and
        /// configures ChatCore to use the Entity Framework Core entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by ChatCore to register new services.</param>
        public static ChatCoreEntityFrameworkCoreBuilder UseEntityFrameworkCore(this ChatCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            return new ChatCoreEntityFrameworkCoreBuilder(builder.Services);
        }

        /// <summary>
        /// Registers the Entity Framework Core Manager services in the DI container and
        /// configures ChatCore to use the Entity Framework Core entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by ChatCore to register new services.</param>
        /// <param name="configuration">The configuration delegate used to configure the Entity Framework Core services.</param>
        public static ChatCoreBuilder UseEntityFrameworkCore(
            this ChatCoreBuilder builder,
            Action<ChatCoreEntityFrameworkCoreBuilder> configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration(builder.UseEntityFrameworkCore());

            return builder;
        }


        public static ChatCoreEntityFrameworkCoreBuilder UseEntityFrameworkCore<TDbContext>(this ChatCoreBuilder builder)where TDbContext:DbContext
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return new ChatCoreEntityFrameworkCoreBuilder(builder.Services).UseDbContext<TDbContext>();
        }
    }
}