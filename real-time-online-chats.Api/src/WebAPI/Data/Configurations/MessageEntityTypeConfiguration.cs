using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Data.Configurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder.HasKey(m => m.Id);

        builder
            .HasOne(m => m.User)
            .WithMany(u => u.Messages);

        builder
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages);
    }
}