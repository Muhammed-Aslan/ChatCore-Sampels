using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{

    class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(uc=>uc.Id);
            builder.HasOne(a => a.Message).WithMany(m => m.Attachments).HasForeignKey(a => a.MessageId).OnDelete(DeleteBehavior.Cascade);
        }
    }


}
