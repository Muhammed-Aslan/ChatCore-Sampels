using System;
using Microsoft.EntityFrameworkCore;

namespace ChatCore.EFCore
{
    public class ChatCoreEntityFrameworkCoreOptions
    {
        /// <summary>
        /// Gets or sets the concrete type of the <see cref="DbContext"/> used by the
        /// ChatCore Entity Framework Core .
        /// </summary>
        public Type DbContextType { get; set; }
    }
}
