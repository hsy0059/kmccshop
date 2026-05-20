using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.Service.Data;
using User.Service.Models.DTOs;
using User.Service.Models.Entities;
using User.Service.Services;
using Campus.Common.Security;
using Campus.Infrastructure;

namespace User.Service.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly UserDbContext _db;
    private readonly IJwtService _jwt;
    private readonly RedisService _redis;

    public AuthController(UserDbContext db, IJwtService jwt, RedisService redis)
    {
        _db = db;
        _jwt = jwt;
        _redis = redis;
    }

    [HttpPost("password-login")]
    public async Task<IActionResult> PasswordLogin([FromBody] PasswordLoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == request.Phone);
        if (user == null)
            return Ok(ApiResponse.Error(400, "手机号未注册"));

        if (user.Status == 0)
            return Ok(ApiResponse.Error(403, "账号已被禁用"));

        if (string.IsNullOrEmpty(user.PasswordHash) || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Ok(ApiResponse.Error(400, "密码错误"));

        user.LastLoginAt = DateTime.Now;
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user.Id, user.Phone ?? "", user.UserType);
        return Ok(ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            UserInfo = MapUserInfo(user)
        }, "登录成功"));
    }

    [HttpPost("send-code")]
    public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var key = $"sms:code:{request.Phone}";
        await _redis.SetAsync(key, code, TimeSpan.FromMinutes(5));
        return Ok(ApiResponse<object>.Success(new { code }, "验证码发送成功"));
    }

    [HttpPost("login-by-code")]
    public async Task<IActionResult> LoginByCode([FromBody] CodeLoginRequest request)
    {
        var key = $"sms:code:{request.Phone}";
        var storedCode = await _redis.GetAsync(key);
        if (storedCode == null || storedCode != request.Code)
            return Ok(ApiResponse.Error(400, "验证码错误或已过期"));

        await _redis.DeleteAsync(key);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == request.Phone);
        if (user == null)
        {
            user = new Models.Entities.User
            {
                Phone = request.Phone,
                Nickname = "用户" + request.Phone[^4..],
                UserType = Campus.Common.UserType.Student
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        if (user.Status == 0)
            return Ok(ApiResponse.Error(403, "账号已被禁用"));

        user.LastLoginAt = DateTime.Now;
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user.Id, user.Phone ?? "", user.UserType);
        return Ok(ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            UserInfo = MapUserInfo(user)
        }, "登录成功"));
    }

    [HttpPost("wechat-login")]
    public async Task<IActionResult> WechatLogin([FromBody] WechatLoginRequest request)
    {
        return Ok(ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = _jwt.GenerateToken(1, "", Campus.Common.UserType.Student),
            UserInfo = new UserInfo { Id = 1, Nickname = request.Nickname ?? "微信用户", UserType = 1 }
        }, "微信登录成功"));
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            return Ok(ApiResponse.Error(401, "未登录"));

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return Ok(ApiResponse.Error(404, "用户不存在"));

        return Ok(ApiResponse<UserInfo>.Success(MapUserInfo(user)));
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(ApiResponse.Success("已登出"));
    }

    private static UserInfo MapUserInfo(Models.Entities.User user)
    {
        return new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Phone = user.Phone,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Gender = user.Gender,
            Email = user.Email,
            UserType = user.UserType,
            StudentId = user.StudentId,
            RealName = user.RealName,
            SchoolId = user.SchoolId,
            CampusId = user.CampusId,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        };
    }
}
