using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatCore.EFCore.Configurations
{
    class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.HasKey(fr => fr.Id);

            builder.HasOne(fr => fr.FromUser).WithMany(u=>u.FriendRequestsFrom);
            builder.HasOne(fr => fr.ToUser).WithMany(u=>u.FriendRequestsTo);
            
        }
    }

    
}
