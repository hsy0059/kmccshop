using System.ComponentModel.DataAnnotations;

namespace User.Service.Models.DTOs;

public class PasswordLoginRequest
{
    [Required(ErrorMessage = "手机号不能为空")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码至少6位")]
    public string Password { get; set; } = string.Empty;
}

public class WechatLoginRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string? Avatar { get; set; }
}

public class SendCodeRequest
{
    [Required]
    [RegularExpression(@"^1[3-9]\d{9}$")]
    public string Phone { get; set; } = string.Empty;
}

public class CodeLoginRequest
{
    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfo UserInfo { get; set; } = null!;
}

public class UserInfo
{
    public long Id { get; set; }
    public string? Username { get; set; }
    public string? Phone { get; set; }
    public string? Nickname { get; set; }
    public string? Avatar { get; set; }
    public int Gender { get; set; }
    public string? Email { get; set; }
    public int UserType { get; set; }
    public string? StudentId { get; set; }
    public string? RealName { get; set; }
    public long? SchoolId { get; set; }
    public long? CampusId { get; set; }
    public int Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateUserRequest
{
    [MaxLength(50)]
    public string? Nickname { get; set; }

    public int? Gender { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? StudentId { get; set; }

    [MaxLength(50)]
    public string? RealName { get; set; }

    public long? CampusId { get; set; }
}

public class AdminUpdateUserRequest
{
    public string? Nickname { get; set; }
    public int? UserType { get; set; }
    public int? Status { get; set; }
    public string? RealName { get; set; }
}

public class UpdateAddressRequest
{
    [Required] public string ContactName { get; set; } = string.Empty;
    [Required] public string ContactPhone { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    [Required] public string Detail { get; set; } = string.Empty;
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int IsDefault { get; set; }
    public string? Tag { get; set; }
}

public class FeedbackRequest
{
    [Required]
    [Range(1, 3)]
    public int Type { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public string? Images { get; set; }

    [MaxLength(100)]
    public string? ContactInfo { get; set; }
}

public class FeedbackReplyRequest
{
    [Required]
    [MaxLength(500)]
    public string ReplyContent { get; set; } = string.Empty;
}
