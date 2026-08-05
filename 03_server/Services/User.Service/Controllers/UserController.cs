using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.Service.Data;
using User.Service.Models.DTOs;
using User.Service.Models.Entities;
using Campus.Common;

namespace User.Service.Controllers;

[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly UserDbContext _db;

    public UserController(UserDbContext db)
    {
        _db = db;
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return Ok(ApiResponse.Error(404, "用户不存在"));

        return Ok(ApiResponse<UserInfo>.Success(MapUserInfo(user)));
    }

    [HttpPut("info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return Ok(ApiResponse.Error(404, "用户不存在"));

        if (request.Nickname != null) user.Nickname = request.Nickname;
        if (request.Gender.HasValue) user.Gender = request.Gender.Value;
        if (request.Email != null) user.Email = request.Email;
        if (request.StudentId != null) user.StudentId = request.StudentId;
        if (request.RealName != null) user.RealName = request.RealName;
        if (request.CampusId.HasValue) user.CampusId = request.CampusId.Value;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<UserInfo>.Success(MapUserInfo(user), "更新成功"));
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetUserList([FromQuery] PageModel page)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权查看用户列表"));
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrEmpty(page.Keyword))
        {
            query = query.Where(u => u.Phone!.Contains(page.Keyword) ||
                                     u.Nickname!.Contains(page.Keyword) ||
                                     u.RealName!.Contains(page.Keyword));
        }

        var total = await query.CountAsync();
        var list = await query.OrderByDescending(u => u.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync();

        var data = PageResult<UserInfo>.Of(
            list.Select(MapUserInfo).ToList(),
            total, page.Page, page.PageSize);

        return Ok(ApiResponse<PageResult<UserInfo>>.Success(data));
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] AdminUpdateUserRequest request)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权修改用户信息"));
        var user = await _db.Users.FindAsync(id);
        if (user == null) return Ok(ApiResponse.Error(404, "用户不存在"));

        if (request.Nickname != null) user.Nickname = request.Nickname;
        if (request.UserType.HasValue) user.UserType = request.UserType.Value;
        if (request.Status.HasValue) user.Status = request.Status.Value;
        if (request.RealName != null) user.RealName = request.RealName;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<UserInfo>.Success(MapUserInfo(user), "更新成功"));
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(long id)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权删除用户"));
        var user = await _db.Users.FindAsync(id);
        if (user == null) return Ok(ApiResponse.Error(404, "用户不存在"));

        user.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }

    [HttpPost("avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Ok(ApiResponse.Error(400, "请选择文件"));

        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Constants.UserAvatarDirectory);

        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        var filePath = Path.Combine(uploadDir, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/{Constants.UserAvatarDirectory}/{fileName}";

        var user = await _db.Users.FindAsync(userId.Value);
        if (user != null)
        {
            user.Avatar = url;
            await _db.SaveChangesAsync();
        }

        return Ok(ApiResponse<object>.Success(new { url }));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
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

    [HttpGet("stats")]
    [Authorize]
    public async Task<IActionResult> GetStats()
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权查看统计数据"));
        var totalUsers = await _db.Users.CountAsync();
        var todayNew = await _db.Users.CountAsync(u => u.CreatedAt.Date == DateTime.Today);
        return Ok(ApiResponse<object>.Success(new { totalUsers, todayNew }));
    }
}  
