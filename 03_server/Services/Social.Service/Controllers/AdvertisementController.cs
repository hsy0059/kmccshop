using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Service.Data;
using Social.Service.Models.Entities;
using Campus.Common;

namespace Social.Service.Controllers;

[ApiController]
[Route("api/v1/social/advertisement")]
public class AdvertisementController : ControllerBase
{
    private readonly SocialDbContext _db;
    public AdvertisementController(SocialDbContext db) { _db = db; }

    [HttpGet("position/{position}")]
    public async Task<IActionResult> GetByPosition(string position)
    {
        var now = DateTime.Now;
        var ads = await _db.Advertisements
            .Where(a => a.Position == position && a.Status == 1 &&
                        (a.StartTime == null || a.StartTime <= now) &&
                        (a.EndTime == null || a.EndTime >= now))
            .OrderBy(a => a.SortOrder).ToListAsync();
        return Ok(ApiResponse<List<Advertisement>>.Success(ads));
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetList([FromQuery] PageModel page)
    {
        var query = _db.Advertisements.AsQueryable();
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(a => a.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Advertisement>>.Success(PageResult<Advertisement>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Advertisement ad)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权管理广告"));
        ad.Status = 1;
        _db.Advertisements.Add(ad);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Advertisement>.Success(ad, "创建成功"));
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] Advertisement ad)
    {
        var existing = await _db.Advertisements.FindAsync(id);
        if (existing == null) return Ok(ApiResponse.Error(404, "广告不存在"));
        existing.Title = ad.Title;
        existing.Image = ad.Image;
        existing.LinkUrl = ad.LinkUrl;
        existing.Position = ad.Position;
        existing.SortOrder = ad.SortOrder;
        existing.StartTime = ad.StartTime;
        existing.EndTime = ad.EndTime;
        existing.Status = ad.Status;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Advertisement>.Success(existing, "更新成功"));
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权管理广告"));
        var ad = await _db.Advertisements.FindAsync(id);
        if (ad == null) return Ok(ApiResponse.Error(404, "广告不存在"));
        ad.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }
}