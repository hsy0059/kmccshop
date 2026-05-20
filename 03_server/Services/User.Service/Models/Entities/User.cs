using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Service.Models.Entities;

[Table("user")]
public class User
{
    [Key]
    public long Id { get; set; }

    [MaxLength(50)]
    public string? Username { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    [MaxLength(50)]
    public string? Nickname { get; set; }

    [MaxLength(500)]
    public string? Avatar { get; set; }

    public int Gender { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? WechatOpenid { get; set; }

    [MaxLength(100)]
    public string? WechatUnionid { get; set; }

    public int UserType { get; set; } = 1;

    [MaxLength(50)]
    public string? StudentId { get; set; }

    [MaxLength(50)]
    public string? RealName { get; set; }

    public long? SchoolId { get; set; }

    public long? CampusId { get; set; }

    public int Status { get; set; } = 1;

    public DateTime? LastLoginAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

[Table("role")]
public class Role
{
    [Key]
    public long Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("user_role")]
public class UserRole
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }
    public long RoleId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("user_address")]
public class UserAddress
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [MaxLength(50)]
    public string ContactName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Province { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? District { get; set; }

    [MaxLength(255)]
    public string Detail { get; set; } = string.Empty;

    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    public int IsDefault { get; set; }

    [MaxLength(50)]
    public string? Tag { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
