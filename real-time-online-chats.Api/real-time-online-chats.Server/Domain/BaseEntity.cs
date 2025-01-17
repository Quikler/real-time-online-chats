using System.ComponentModel.DataAnnotations;

namespace real_time_online_chats.Server.Domain;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}