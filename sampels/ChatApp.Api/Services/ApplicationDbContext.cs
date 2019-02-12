using ChatApp.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api
{
    public class ApplicationDbContext :IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseChatCore();
            builder.UseOpenIddict();
            base.OnModelCreating(builder);
        }

    }
}