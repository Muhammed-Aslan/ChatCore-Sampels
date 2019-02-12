using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{
    class FriendsConfiguration : IEntityTypeConfiguration<Friends>
    {
        public void Configure(EntityTypeBuilder<Friends> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.FromUser).WithMany(u=>u.FriendsFrom).HasForeignKey(f => f.FromUserId);
            builder.HasOne(f => f.ToUser).WithMany(u=>u.FriendsTo).HasForeignKey(f => f.ToUserId);
            builder.HasOne(f => f.Chat);
        }
    }

    
}
