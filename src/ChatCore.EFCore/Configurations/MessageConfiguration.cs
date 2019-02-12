using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{

    class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(uc => uc.Id);

            builder.HasOne(m => m.Chat).WithMany(c => c.Messages).HasForeignKey(m => m.ChatId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(m => m.FromUser).WithMany(u => u.Messages).HasForeignKey(m => m.FromUserId).OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(m => m.Attachments).WithOne(a => a.Message).HasForeignKey(a => a.MessageId);

            builder.Property(m => m.Content).HasMaxLength(255);
        }
    }
    
}
