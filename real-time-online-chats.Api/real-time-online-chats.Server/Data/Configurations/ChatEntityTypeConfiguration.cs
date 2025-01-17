using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Data.Configurations;

public class ChatEntityTypeConfiguration : IEntityTypeConfiguration<ChatEntity>
{
    public void Configure(EntityTypeBuilder<ChatEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .HasOne(c => c.Owner)
            .WithMany(u => u.OwnedChats);

        builder
            .HasMany(c => c.Members)
            .WithMany(u => u.MemberChats);

        builder
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat);
    }
}