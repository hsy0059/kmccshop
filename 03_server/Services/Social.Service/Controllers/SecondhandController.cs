using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Service.Data;
using Social.Service.Models.Entities;
using Campus.Common;

namespace Social.Service.Controllers;

[ApiController]
[Route("api/v1/social/secondhand")]
public class SecondhandController : ControllerBase
{
    private readonly SocialDbContext _db;
    public SecondhandController(SocialDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page)
    {
        var query = _db.SecondGoods.Where(g => g.Status == 1 && g.IsSold == 0).AsQueryable();
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(g => g.Title.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(g => g.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<SecondGoods>>.Success(PageResult<SecondGoods>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("detail/{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var goods = await _db.SecondGoods.FindAsync(id);
        if (goods == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        goods.ViewCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<SecondGoods>.Success(goods));
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] SecondGoods goods)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        goods.UserId = userId.Value;
        goods.Status = 1;
        _db.SecondGoods.Add(goods);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<SecondGoods>.Success(goods, "发布成功"));
    }

    [HttpDelete("delete/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        var goods = await _db.SecondGoods.FindAsync(id);
        if (goods == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        goods.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }

    [HttpPost("{id:long}/favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(long id)
    {
        var goods = await _db.SecondGoods.FindAsync(id);
        if (goods == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        goods.FavoriteCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("收藏成功"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}