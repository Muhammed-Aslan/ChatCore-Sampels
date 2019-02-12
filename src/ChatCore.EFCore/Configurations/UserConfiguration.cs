using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(uc=>uc.Id);
            builder.HasMany(u => u.Messages).WithOne(m => m.FromUser).HasForeignKey(m=>m.FromUserId);

            builder.HasMany(u => u.OwnedChats).WithOne(c => c.CreatedByUser).HasForeignKey(c=>c.CreatedByUserId);
            builder.HasMany(u => u.UsersChats).WithOne(cu => cu.User).HasForeignKey(uc => uc.UserId);

            builder.HasMany(u => u.FriendRequestsFrom).WithOne(fr => fr.FromUser).HasForeignKey(fr=>fr.FromUserId);

            builder.HasMany(u => u.FriendRequestsTo).WithOne(fr => fr.ToUser).HasForeignKey(fr=>fr.ToUserId);

            builder.HasMany(u => u.FriendsTo).WithOne(f => f.ToUser).HasForeignKey(f => f.ToUserId);

            builder.HasMany(u => u.FriendsFrom).WithOne(f => f.FromUser).HasForeignKey(fr=>fr.FromUserId);
            
        }
    }
}
