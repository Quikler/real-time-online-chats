using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data.Configurations;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<ChatEntity> Chats { get; set; } = null!;
    public DbSet<MessageEntity> Messages { get; set; } = null!;
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        builder.ApplyConfiguration(new ChatEntityTypeConfiguration());
        builder.ApplyConfiguration(new MessageEntityTypeConfiguration());
        builder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());

        base.OnModelCreating(builder);
    }
}