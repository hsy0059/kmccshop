using System.Security.Claims;

namespace Campus.Common;

/// <summary>
/// 从 JWT Claims 中读取当前用户信息的扩展方法
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>获取当前登录用户 Id，未登录或 Token 异常返回 null</summary>
    public static long? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }

    /// <summary>获取当前用户类型（1学生 2商家 3骑手 4管理员），未识别返回 0</summary>
    public static int GetUserType(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("user_type")?.Value;
        return int.TryParse(claim, out var type) ? type : 0;
    }

    /// <summary>是否为管理员</summary>
    public static bool IsAdmin(this ClaimsPrincipal user) => user.GetUserType() == UserType.Admin;

    /// <summary>是否为商家</summary>
    public static bool IsMerchant(this ClaimsPrincipal user) => user.GetUserType() == UserType.Merchant;

    /// <summary>是否为骑手</summary>
    public static bool IsRider(this ClaimsPrincipal user) => user.GetUserType() == UserType.Rider;
}
