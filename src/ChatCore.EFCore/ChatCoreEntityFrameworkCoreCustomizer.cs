using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ChatCore.EFCore
{
    public class ChatCoreEntityFrameworkCoreCustomizer: RelationalModelCustomizer
    {
        public ChatCoreEntityFrameworkCoreCustomizer(ModelCustomizerDependencies dependencies)
            : base(dependencies) { }

        public override void Customize(ModelBuilder modelBuilder, DbContext context)
        {

            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            modelBuilder.UseChatCore();

            base.Customize(modelBuilder, context);
        }
    }
}
