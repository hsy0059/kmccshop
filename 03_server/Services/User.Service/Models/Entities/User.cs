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

    [Column("password_hash")] [MaxLength(255)]
    public string? PasswordHash { get; set; }

    [MaxLength(50)]
    public string? Nickname { get; set; }

    [MaxLength(500)]
    public string? Avatar { get; set; }

    public int Gender { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [Column("wechat_openid")] [MaxLength(100)]
    public string? WechatOpenid { get; set; }

    [Column("wechat_unionid")] [MaxLength(100)]
    public string? WechatUnionid { get; set; }

    [Column("user_type")] public int UserType { get; set; } = 1;

    [Column("student_id")] [MaxLength(50)]
    public string? StudentId { get; set; }

    [Column("real_name")] [MaxLength(50)]
    public string? RealName { get; set; }

    [Column("school_id")] public long? SchoolId { get; set; }

    [Column("campus_id")] public long? CampusId { get; set; }

    public int Status { get; set; } = 1;

    [Column("last_login_at")] public DateTime? LastLoginAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
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

    [Column("user_id")] public long UserId { get; set; }
    [Column("role_id")] public long RoleId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("user_address")]
public class UserAddress
{
    [Key]
    public long Id { get; set; }

    [Column("user_id")] public long UserId { get; set; }

    [Column("contact_name")] [MaxLength(50)]
    public string ContactName { get; set; } = string.Empty;

    [Column("contact_phone")] [MaxLength(20)]
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

    [Column("is_default")] public int IsDefault { get; set; }

    [MaxLength(50)]
    public string? Tag { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
