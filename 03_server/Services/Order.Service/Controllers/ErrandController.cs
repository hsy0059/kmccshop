using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Order.Service.Data;
using Order.Service.Models.DTOs;
using Order.Service.Models.Entities;
using Campus.Common;

namespace Order.Service.Controllers;

[ApiController]
[Route("api/v1/order/errand")]
[Authorize]
public class ErrandController : ControllerBase
{
    private readonly OrderDbContext _db;

    public ErrandController(OrderDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page)
    {
        var query = _db.ErrandOrders.AsQueryable();
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(o => o.Title.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(o => o.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<ErrandOrder>>.Success(PageResult<ErrandOrder>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var order = await _db.ErrandOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "跑腿订单不存在"));
        return Ok(ApiResponse<ErrandOrder>.Success(order));
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateErrandRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);

        var orderNo = "E" + DateTime.Now.ToString("yyyyMMddHHmmss") + RandomNumberGenerator.GetInt32(1000, 10000);
        var order = new ErrandOrder
        {
            OrderNo = orderNo, UserId = userId,
            Title = request.Title, Description = request.Description,
            PickupAddress = request.PickupAddress, DeliveryAddress = request.DeliveryAddress,
            TipAmount = request.TipAmount, ContactName = request.ContactName,
            ContactPhone = request.ContactPhone, Remark = request.Remark,
            Status = ErrandOrderStatus.Pending
        };
        _db.ErrandOrders.Add(order);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<ErrandOrder>.Success(order, "发布成功"));
    }
}