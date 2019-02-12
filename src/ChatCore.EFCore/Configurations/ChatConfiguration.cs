using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{
    class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(uc => uc.Id);

            builder.HasOne(c => c.CreatedByUser).WithMany(u => u.OwnedChats).HasForeignKey(c => c.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(c => c.Messages).WithOne(m => m.Chat).HasForeignKey(m => m.ChatId);
            builder.HasMany(c => c.UsersChats).WithOne(uc => uc.Chat).HasForeignKey(uc => uc.ChatId);

            builder.Property(c => c.Name).HasMaxLength(50);
        }
    }

    
}
