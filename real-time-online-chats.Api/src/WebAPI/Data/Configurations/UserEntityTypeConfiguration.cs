using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Data.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder
            .HasMany(u => u.MemberChats)
            .WithMany(c => c.Members);

        builder
            .HasMany(u => u.Messages)
            .WithOne(m => m.User);
    }
}