using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.Service.Data;
using Delivery.Service.Models.DTOs;
using Delivery.Service.Models.Entities;
using Campus.Common;

namespace Delivery.Service.Controllers;

[ApiController]
[Route("api/v1/delivery/rider-admin")]
[Authorize]
public class RiderAdminController : ControllerBase
{
    private readonly DeliveryDbContext _db;

    public RiderAdminController(DeliveryDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权管理骑手"));
        var query = _db.Riders.AsQueryable();
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(r => r.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Rider>>.Success(PageResult<Rider>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("approve/{id:long}")]
    public async Task<IActionResult> Approve(long id, [FromBody] RiderApproveRequest request)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权审核骑手"));
        var rider = await _db.Riders.FindAsync(id);
        if (rider == null) return Ok(ApiResponse.Error(404, "骑手不存在"));
        rider.AuditStatus = request.AuditStatus;
        if (request.AuditStatus == RiderAuditStatus.Approved) rider.Status = RiderStatus.Offline;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("审核完成"));
    }
}