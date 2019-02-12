using System;
using ChatCore.EFCore;
using ChatCore.EFCore.Configurations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class ChatCoreEntityFrameworkCoreHelpers
    {
            /// <summary>
            /// Registers the ChatCore entity sets in the Entity Framework Core  context
            /// </summary>
            /// <param name="builder">The builder used to configure the Entity Framework context.</param>
            /// <returns>The Entity Framework context builder.</returns>
            public static DbContextOptionsBuilder UseChatCore( this DbContextOptionsBuilder builder)
            {
                if (builder == null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }
                var context = builder.Options.ContextType;

                return builder.ReplaceService<IModelCustomizer, ChatCoreEntityFrameworkCoreCustomizer>();
            }

            /// <summary>
            /// Registers the ChatCore entity sets in the Entity Framework Core context 
            /// </summary>
            /// <param name="builder">The builder used to configure the Entity Framework context.</param>
            /// <returns>The Entity Framework context builder.</returns>
            public static ModelBuilder UseChatCore(this ModelBuilder builder)
            {
                if (builder == null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }

            return builder
                .ApplyConfiguration(new AttachmentConfiguration())
                .ApplyConfiguration(new ChatConfiguration())
                .ApplyConfiguration(new UserConfiguration())
                .ApplyConfiguration(new FriendRequestConfiguration())
                .ApplyConfiguration(new FriendsConfiguration())
                .ApplyConfiguration(new MessageConfiguration())
                .ApplyConfiguration(new UsersChatsConfiguration());
        }
    }
}
