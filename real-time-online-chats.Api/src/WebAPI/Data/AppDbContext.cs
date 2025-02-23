using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data.Configurations;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Data;

public class AppDbContext
    : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    public virtual DbSet<ChatEntity> Chats { get; set; } = null!;
    public virtual DbSet<MessageEntity> Messages { get; set; } = null!;
    public virtual DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public AppDbContext() { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        builder.ApplyConfiguration(new ChatEntityTypeConfiguration());
        builder.ApplyConfiguration(new MessageEntityTypeConfiguration());
        builder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());

        base.OnModelCreating(builder);
    }
}