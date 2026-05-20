using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.Service.Data;
using Delivery.Service.Models.Entities;
using Campus.Common;

namespace Delivery.Service.Controllers;

[ApiController]
[Route("api/v1/delivery/order")]
[Authorize]
public class DeliveryController : ControllerBase
{
    private readonly DeliveryDbContext _db;

    public DeliveryController(DeliveryDbContext db) { _db = db; }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] PageModel page)
    {
        var orders = await _db.Set<Dictionary<string, object>>().ToListAsync();
        return Ok(ApiResponse<object>.Success(new { list = new List<object>(), total = 0 }));
    }

    [HttpPost("grab/{id:long}")]
    public async Task<IActionResult> Grab(long id)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var rider = await _db.Riders.FirstOrDefaultAsync(r => r.UserId == userId.Value && r.AuditStatus == RiderAuditStatus.Approved);
        if (rider == null) return Ok(ApiResponse.Error(403, "您不是认证骑手"));
        return Ok(ApiResponse<object>.Success(new { riderId = rider.Id }, "抢单成功"));
    }

    [HttpPost("start/{id:long}")]
    public async Task<IActionResult> StartDelivery(long id)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var rider = await _db.Riders.FirstOrDefaultAsync(r => r.UserId == userId.Value);
        if (rider != null) { rider.Status = RiderStatus.Delivering; await _db.SaveChangesAsync(); }
        return Ok(ApiResponse.Success("开始配送"));
    }

    [HttpPost("complete/{id:long}")]
    public async Task<IActionResult> CompleteDelivery(long id)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var rider = await _db.Riders.FirstOrDefaultAsync(r => r.UserId == userId.Value);
        if (rider != null) { rider.OrderCount++; rider.Status = RiderStatus.Online; await _db.SaveChangesAsync(); }
        return Ok(ApiResponse.Success("配送完成"));
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        return Ok(ApiResponse<object>.Success(new { }));
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] PageModel page)
    {
        return Ok(ApiResponse<object>.Success(new { list = new List<object>(), total = 0 }));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}