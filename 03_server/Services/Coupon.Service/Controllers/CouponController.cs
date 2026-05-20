using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coupon.Service.Data;
using Coupon.Service.Models.DTOs;
using Coupon.Service.Models.Entities;
using Campus.Common;

namespace Coupon.Service.Controllers;

[ApiController]
[Route("api/v1/coupon")]
public class CouponController : ControllerBase
{
    private readonly CouponDbContext _db;
    public CouponController(CouponDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetAvailableList([FromQuery] PageModel page)
    {
        var now = DateTime.Now;
        var query = _db.Coupons.Where(c => c.Status == 1 && c.StartTime <= now && c.EndTime >= now && c.ReceivedCount < c.TotalCount);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Models.Entities.Coupon>>.Success(PageResult<Models.Entities.Coupon>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyCoupons([FromQuery] PageModel page)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var query = _db.UserCoupons.Where(uc => uc.UserId == userId.Value);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(uc => uc.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        var couponIds = list.Select(uc => uc.CouponId).Distinct();
        var coupons = await _db.Coupons.Where(c => couponIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id);
        var result = list.Select(uc => new
        {
            uc.Id, uc.Status, uc.ReceivedAt, uc.UsedAt, uc.ExpireAt,
            Coupon = coupons.GetValueOrDefault(uc.CouponId)
        }).ToList();
        return Ok(ApiResponse<object>.Success(new { list = result, total }));
    }

    [HttpPost("receive/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Receive(long id)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon == null || coupon.Status != 1) return Ok(ApiResponse.Error(404, "优惠券不存在"));
        if (coupon.ReceivedCount >= coupon.TotalCount) return Ok(ApiResponse.Error(400, "已领完"));
        var userCount = await _db.UserCoupons.CountAsync(uc => uc.UserId == userId.Value && uc.CouponId == id);
        if (userCount >= coupon.PerUserLimit) return Ok(ApiResponse.Error(400, "已达领取上限"));

        coupon.ReceivedCount++;
        var expireAt = coupon.ValidDays.HasValue ? DateTime.Now.AddDays(coupon.ValidDays.Value) : coupon.EndTime;
        var userCoupon = new UserCoupon
        {
            UserId = userId.Value, CouponId = id,
            ReceivedAt = DateTime.Now, ExpireAt = expireAt
        };
        _db.UserCoupons.Add(userCoupon);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<UserCoupon>.Success(userCoupon, "领取成功"));
    }

    [HttpGet("merchant/list")]
    [Authorize]
    public async Task<IActionResult> GetMerchantCouponList([FromQuery] PageModel page, [FromQuery] long? merchantId)
    {
        var query = _db.Coupons.AsQueryable();
        if (merchantId.HasValue) query = query.Where(c => c.MerchantId == merchantId.Value);
        if (!string.IsNullOrEmpty(page.Keyword)) query = query.Where(c => c.Name.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Models.Entities.Coupon>>.Success(PageResult<Models.Entities.Coupon>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("admin/list")]
    [Authorize]
    public async Task<IActionResult> AdminList([FromQuery] PageModel page)
    {
        var query = _db.Coupons.AsQueryable();
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(c => c.Name.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Models.Entities.Coupon>>.Success(PageResult<Models.Entities.Coupon>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("admin/create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
    {
        var coupon = new Models.Entities.Coupon
        {
            Name = request.Name, Description = request.Description, Type = request.Type,
            DiscountValue = request.DiscountValue, MinAmount = request.MinAmount,
            MaxDiscount = request.MaxDiscount, TotalCount = request.TotalCount,
            PerUserLimit = request.PerUserLimit, MerchantId = request.MerchantId,
            StartTime = request.StartTime, EndTime = request.EndTime, ValidDays = request.ValidDays
        };
        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Models.Entities.Coupon>.Success(coupon, "创建成功"));
    }

    [HttpPut("admin/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCouponRequest request)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon == null) return Ok(ApiResponse.Error(404, "优惠券不存在"));
        if (request.Name != null) coupon.Name = request.Name;
        if (request.Description != null) coupon.Description = request.Description;
        if (request.DiscountValue.HasValue) coupon.DiscountValue = request.DiscountValue.Value;
        if (request.MinAmount.HasValue) coupon.MinAmount = request.MinAmount.Value;
        if (request.MaxDiscount.HasValue) coupon.MaxDiscount = request.MaxDiscount;
        if (request.TotalCount.HasValue) coupon.TotalCount = request.TotalCount.Value;
        if (request.PerUserLimit.HasValue) coupon.PerUserLimit = request.PerUserLimit.Value;
        if (request.StartTime.HasValue) coupon.StartTime = request.StartTime.Value;
        if (request.EndTime.HasValue) coupon.EndTime = request.EndTime.Value;
        if (request.Status.HasValue) coupon.Status = request.Status.Value;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Models.Entities.Coupon>.Success(coupon, "更新成功"));
    }

    [HttpDelete("admin/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon == null) return Ok(ApiResponse.Error(404, "优惠券不存在"));
        coupon.Status = 0;
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