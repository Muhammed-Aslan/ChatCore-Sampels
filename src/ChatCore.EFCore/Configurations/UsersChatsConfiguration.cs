using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{

    class UsersChatsConfiguration : IEntityTypeConfiguration<UsersChats>
    {
        public void Configure(EntityTypeBuilder<UsersChats> builder)
        {
            builder.HasKey(uc=>uc.Id);
            builder.HasOne(uc => uc.Chat).WithMany(c => c.UsersChats).HasForeignKey(uc => uc.ChatId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(uc => uc.User).WithMany(c => c.UsersChats).HasForeignKey(uc => uc.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
