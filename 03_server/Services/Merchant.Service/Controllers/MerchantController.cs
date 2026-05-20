using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Merchant.Service.Data;
using Merchant.Service.Models.DTOs;
using Merchant.Service.Models.Entities;
using Campus.Common;

namespace Merchant.Service.Controllers;

[ApiController]
[Route("api/v1/merchant")]
public class MerchantController : ControllerBase
{
    private readonly MerchantDbContext _db;

    public MerchantController(MerchantDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page, [FromQuery] int? status = null)
    {
        var query = _db.Merchants.AsQueryable();
        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(m => m.Name.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(m => m.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Models.Entities.Merchant>>.Success(PageResult<Models.Entities.Merchant>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("detail/{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var merchant = await _db.Merchants.FindAsync(id);
        if (merchant == null) return Ok(ApiResponse.Error(404, "商家不存在"));
        return Ok(ApiResponse<Models.Entities.Merchant>.Success(merchant));
    }

    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply([FromBody] ApplyMerchantRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);

        var exists = await _db.Merchants.AnyAsync(m => m.UserId == userId);
        if (exists) return Ok(ApiResponse.Error(400, "已提交过商家申请"));

        var merchant = new Models.Entities.Merchant
        {
            UserId = userId, Name = request.Name, Logo = request.Logo,
            Phone = request.Phone, Description = request.Description,
            Address = request.Address, BusinessHours = request.BusinessHours,
            MinDeliveryAmount = request.MinDeliveryAmount, DeliveryFee = request.DeliveryFee,
            CampusId = request.CampusId, Longitude = request.Longitude, Latitude = request.Latitude,
            Status = MerchantStatus.Pending
        };
        _db.Merchants.Add(merchant);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Models.Entities.Merchant>.Success(merchant, "申请已提交"));
    }

    [HttpPost("admin/audit/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Audit(long id, [FromBody] AuditMerchantRequest request)
    {
        var merchant = await _db.Merchants.FindAsync(id);
        if (merchant == null) return Ok(ApiResponse.Error(404, "商家不存在"));
        if (request.Status != MerchantStatus.Open && request.Status != MerchantStatus.Rest
            && request.Status != MerchantStatus.Disabled)
            return Ok(ApiResponse.Error(400, "无效的审核状态"));

        merchant.Status = request.Status;

        var statusMsg = request.Status switch
        {
            MerchantStatus.Open => "审核通过",
            MerchantStatus.Rest => "设为休息",
            MerchantStatus.Disabled => "已禁用",
            _ => "状态已更新"
        };
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success(statusMsg));
    }

    [HttpPut("admin/update/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateMerchantRequest request)
    {
        var merchant = await _db.Merchants.FindAsync(id);
        if (merchant == null) return Ok(ApiResponse.Error(404, "商家不存在"));
        if (request.Name != null) merchant.Name = request.Name;
        if (request.Logo != null) merchant.Logo = request.Logo;
        if (request.CoverImage != null) merchant.CoverImage = request.CoverImage;
        if (request.Phone != null) merchant.Phone = request.Phone;
        if (request.Description != null) merchant.Description = request.Description;
        if (request.Address != null) merchant.Address = request.Address;
        if (request.BusinessHours != null) merchant.BusinessHours = request.BusinessHours;
        if (request.MinDeliveryAmount.HasValue) merchant.MinDeliveryAmount = request.MinDeliveryAmount.Value;
        if (request.DeliveryFee.HasValue) merchant.DeliveryFee = request.DeliveryFee.Value;
        if (request.CampusId.HasValue) merchant.CampusId = request.CampusId.Value;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Models.Entities.Merchant>.Success(merchant, "更新成功"));
    }

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);
        var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
        if (merchant == null) return Ok(ApiResponse.Error(404, "未找到商家"));
        var productCount = await _db.Products.CountAsync(p => p.MerchantId == merchant.Id);
        return Ok(ApiResponse<object>.Success(new { merchant, productCount }));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalMerchants = await _db.Merchants.CountAsync(m => m.Status == MerchantStatus.Open);
        var pendingCount = await _db.Merchants.CountAsync(m => m.Status == MerchantStatus.Pending);
        return Ok(ApiResponse<object>.Success(new { totalMerchants, pendingCount }));
    }

    [HttpGet("my-stats")]
    [Authorize]
    public async Task<IActionResult> GetMyStats()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);
        var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
        if (merchant == null) return Ok(ApiResponse.Error(404, "未找到商家"));
        var productCount = await _db.Products.CountAsync(p => p.MerchantId == merchant.Id);
        return Ok(ApiResponse<object>.Success(new { productCount, rating = merchant.Rating, merchantId = merchant.Id, merchantName = merchant.Name }));
    }
}