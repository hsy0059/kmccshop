using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.Service.Data;
using Delivery.Service.Models.DTOs;
using Delivery.Service.Models.Entities;
using Campus.Common;

namespace Delivery.Service.Controllers;

[ApiController]
[Route("api/v1/delivery/rider")]
public class RiderController : ControllerBase
{
    private readonly DeliveryDbContext _db;

    public RiderController(DeliveryDbContext db) { _db = db; }

    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply([FromBody] RiderApplyRequest request)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var exists = await _db.Riders.AnyAsync(r => r.UserId == userId.Value);
        if (exists) return Ok(ApiResponse.Error(400, "已提交过骑手申请"));
        var rider = new Rider
        {
            UserId = userId.Value, RealName = request.RealName, Phone = request.Phone,
            IdCard = request.IdCard, VehicleType = request.VehicleType,
            VehicleNumber = request.VehicleNumber, CampusId = request.CampusId,
            AuditStatus = RiderAuditStatus.Pending, Status = RiderStatus.Offline
        };
        _db.Riders.Add(rider);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Rider>.Success(rider, "申请已提交"));
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> GetInfo()
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var rider = await _db.Riders.FirstOrDefaultAsync(r => r.UserId == userId.Value);
        if (rider == null) return Ok(ApiResponse.Error(404, "未找到骑手信息"));
        return Ok(ApiResponse<Rider>.Success(rider));
    }

    [HttpPut("status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateRiderStatusRequest request)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var rider = await _db.Riders.FirstOrDefaultAsync(r => r.UserId == userId.Value);
        if (rider == null) return Ok(ApiResponse.Error(404, "未找到骑手信息"));
        rider.Status = request.Status;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("状态更新成功"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}