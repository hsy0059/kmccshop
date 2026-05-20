using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Service.Data;
using Social.Service.Models.Entities;
using Campus.Common;

namespace Social.Service.Controllers;

[ApiController]
[Route("api/v1/social/lostandfound")]
public class LostFoundController : ControllerBase
{
    private readonly SocialDbContext _db;
    public LostFoundController(SocialDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page, [FromQuery] int? type, [FromQuery] string? category)
    {
        var query = _db.LostFounds.Where(l => l.Status == 1).AsQueryable();
        if (type.HasValue) query = query.Where(l => l.Type == type.Value);
        if (!string.IsNullOrEmpty(category)) query = query.Where(l => l.Category == category);
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(l => l.Title.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(l => l.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<LostFound>>.Success(PageResult<LostFound>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("detail/{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var lf = await _db.LostFounds.FindAsync(id);
        if (lf == null) return Ok(ApiResponse.Error(404, "不存在"));
        lf.ViewCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<LostFound>.Success(lf));
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] LostFound lf)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        lf.UserId = userId.Value;
        lf.Status = 1;
        _db.LostFounds.Add(lf);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<LostFound>.Success(lf, "发布成功"));
    }

    [HttpDelete("delete/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        var lf = await _db.LostFounds.FindAsync(id);
        if (lf == null) return Ok(ApiResponse.Error(404, "不存在"));
        lf.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}