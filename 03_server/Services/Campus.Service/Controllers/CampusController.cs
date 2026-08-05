using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Campus.Service.Data;
using Campus.Service.Models.DTOs;
using Campus.Service.Models.Entities;
using Campus.Common;

namespace Campus.Service.Controllers;

[ApiController]
[Route("api/v1/campus")]
public class CampusController : ControllerBase
{
    private readonly CampusDbContext _db;

    public CampusController(CampusDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] long? schoolId)
    {
        var query = _db.Campuses.Where(c => c.Status == 1).AsQueryable();
        if (schoolId.HasValue) query = query.Where(c => c.SchoolId == schoolId.Value);
        var list = await query.OrderBy(c => c.SortOrder).ToListAsync();
        return Ok(ApiResponse<List<CampusEntity>>.Success(list));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCampusRequest request)
    {
        var campus = new CampusEntity
        {
            SchoolId = request.SchoolId, Name = request.Name, Address = request.Address,
            Longitude = request.Longitude, Latitude = request.Latitude,
            DeliveryRadius = request.DeliveryRadius
        };
        _db.Campuses.Add(campus);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<CampusEntity>.Success(campus, "创建成功"));
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCampusRequest request)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权管理校区"));
        var campus = await _db.Campuses.FindAsync(id);
        if (campus == null) return Ok(ApiResponse.Error(404, "校区不存在"));
        if (request.Name != null) campus.Name = request.Name;
        if (request.Address != null) campus.Address = request.Address;
        if (request.Longitude.HasValue) campus.Longitude = request.Longitude;
        if (request.Latitude.HasValue) campus.Latitude = request.Latitude;
        if (request.DeliveryRadius.HasValue) campus.DeliveryRadius = request.DeliveryRadius.Value;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<CampusEntity>.Success(campus, "更新成功"));
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权管理校区"));
        var campus = await _db.Campuses.FindAsync(id);
        if (campus == null) return Ok(ApiResponse.Error(404, "校区不存在"));
        campus.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }
}