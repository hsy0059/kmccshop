using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Campus.Service.Data;
using Campus.Service.Models.DTOs;
using Campus.Service.Models.Entities;
using Campus.Common;

namespace Campus.Service.Controllers;

[ApiController]
[Route("api/v1/campus/delivery-fee")]
public class DeliveryFeeController : ControllerBase
{
    private readonly CampusDbContext _db;

    public DeliveryFeeController(CampusDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] long? campusId)
    {
        var query = _db.DeliveryZones.AsQueryable();
        if (campusId.HasValue) query = query.Where(z => z.CampusId == campusId.Value);
        var list = await query.OrderBy(z => z.CreatedAt).ToListAsync();
        return Ok(ApiResponse<List<DeliveryZone>>.Success(list));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryZoneRequest request)
    {
        var zone = new DeliveryZone
        {
            CampusId = request.CampusId, Name = request.Name,
            DeliveryFee = request.DeliveryFee, MinOrderAmount = request.MinOrderAmount,
            EstimatedTime = request.EstimatedTime
        };
        _db.DeliveryZones.Add(zone);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<DeliveryZone>.Success(zone, "创建成功"));
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDeliveryZoneRequest request)
    {
        var zone = await _db.DeliveryZones.FindAsync(id);
        if (zone == null) return Ok(ApiResponse.Error(404, "不存在"));
        if (request.Name != null) zone.Name = request.Name;
        if (request.DeliveryFee.HasValue) zone.DeliveryFee = request.DeliveryFee.Value;
        if (request.MinOrderAmount.HasValue) zone.MinOrderAmount = request.MinOrderAmount.Value;
        if (request.EstimatedTime.HasValue) zone.EstimatedTime = request.EstimatedTime.Value;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<DeliveryZone>.Success(zone, "更新成功"));
    }
}