using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.Service.Data;
using Order.Service.Models.DTOs;
using Order.Service.Models.Entities;
using Campus.Common;

namespace Order.Service.Controllers;

[ApiController]
[Route("api/v1/order")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _db;

    public OrderController(OrderDbContext db) { _db = db; }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetList([FromQuery] PageModel page, [FromQuery] int? status)
    {
        var query = _db.DeliveryOrders.AsQueryable();
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(o => o.OrderNo.Contains(page.Keyword));
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(o => o.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<DeliveryOrder>>.Success(PageResult<DeliveryOrder>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("my-orders")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders([FromQuery] PageModel page)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var query = _db.DeliveryOrders.Where(o => o.UserId == userId.Value);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(o => o.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<DeliveryOrder>>.Success(PageResult<DeliveryOrder>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var order = await _db.DeliveryOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "订单不存在"));
        var items = await _db.OrderItems.Where(i => i.OrderId == id).ToListAsync();
        return Ok(ApiResponse<object>.Success(new { order, items }));
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var totalAmount = request.Items.Sum(i => i.Price * i.Quantity);
        var deliveryFee = 2m;
        var orderNo = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        var order = new DeliveryOrder
        {
            OrderNo = orderNo, UserId = userId.Value, MerchantId = request.MerchantId,
            AddressId = request.AddressId, TotalAmount = totalAmount, DeliveryFee = deliveryFee,
            ActualAmount = totalAmount + deliveryFee, Remark = request.Remark, Status = OrderStatus.PendingPayment
        };
        _db.DeliveryOrders.Add(order);
        await _db.SaveChangesAsync();

        foreach (var item in request.Items)
        {
            _db.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id, ProductId = item.ProductId,
                ProductName = item.ProductName, ProductImage = item.ProductImage,
                SpecName = item.SpecName, Price = item.Price,
                Quantity = item.Quantity, TotalPrice = item.Price * item.Quantity
            });
        }
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<DeliveryOrder>.Success(order, "下单成功"));
    }

    [HttpPost("{id:long}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(long id)
    {
        var order = await _db.DeliveryOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "订单不存在"));
        if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.PendingAccept)
            return Ok(ApiResponse.Error(400, "当前状态不可取消"));
        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("订单已取消"));
    }

    [HttpPost("{id:long}/pay")]
    [Authorize]
    public async Task<IActionResult> Pay(long id)
    {
        var order = await _db.DeliveryOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "订单不存在"));
        if (order.Status != OrderStatus.PendingPayment)
            return Ok(ApiResponse.Error(400, "订单状态不正确"));
        order.Status = OrderStatus.PendingAccept;
        order.PaidAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("支付成功"));
    }

    [HttpPost("{id:long}/refund")]
    [Authorize]
    public async Task<IActionResult> Refund(long id)
    {
        var order = await _db.DeliveryOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "订单不存在"));
        order.RefundStatus = 1;
        order.RefundAmount = order.ActualAmount;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("退款申请已提交"));
    }

    [HttpGet("export")]
    [Authorize]
    public async Task<IActionResult> Export()
    {
        var orders = await _db.DeliveryOrders.OrderByDescending(o => o.CreatedAt).Take(1000).ToListAsync();
        return Ok(ApiResponse<List<DeliveryOrder>>.Success(orders));
    }

    [HttpGet("statistics")]
    [Authorize]
    public async Task<IActionResult> Statistics()
    {
        var totalOrders = await _db.DeliveryOrders.CountAsync();
        var totalAmount = await _db.DeliveryOrders.Where(o => o.Status == OrderStatus.Completed).SumAsync(o => o.ActualAmount);
        var todayOrders = await _db.DeliveryOrders.CountAsync(o => o.CreatedAt.Date == DateTime.Today);
        var todayRevenue = await _db.DeliveryOrders.Where(o => o.CreatedAt.Date == DateTime.Today && o.Status == OrderStatus.Completed).SumAsync(o => o.ActualAmount);
        return Ok(ApiResponse<object>.Success(new { totalOrders, totalAmount, todayOrders, todayRevenue }));
    }

    [HttpGet("merchant-stats")]
    [Authorize]
    public async Task<IActionResult> GetMerchantStats([FromQuery] long merchantId)
    {
        var today = DateTime.Today;
        var todayOrders = await _db.DeliveryOrders.CountAsync(o => o.MerchantId == merchantId && o.CreatedAt.Date == today);
        var todayRevenue = await _db.DeliveryOrders.Where(o => o.MerchantId == merchantId && o.CreatedAt.Date == today && o.Status == OrderStatus.Completed).SumAsync(o => o.ActualAmount);
        return Ok(ApiResponse<object>.Success(new { todayOrders, todayRevenue }));
    }

    [HttpPost("comment/submit")]
    [Authorize]
    public async Task<IActionResult> SubmitComment([FromBody] SubmitCommentRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var comment = new OrderComment
        {
            OrderId = request.OrderId, UserId = userId.Value,
            TargetId = request.TargetId, TargetType = request.TargetType,
            Rating = request.Rating, Content = request.Content, Images = request.Images
        };
        _db.OrderComments.Add(comment);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<OrderComment>.Success(comment, "评价成功"));
    }

    [HttpGet("merchant/list")]
    [Authorize]
    public async Task<IActionResult> GetMerchantOrderList([FromQuery] PageModel page, [FromQuery] int? status, [FromQuery] long? merchantId)
    {
        var query = _db.DeliveryOrders.AsQueryable();
        if (merchantId.HasValue) query = query.Where(o => o.MerchantId == merchantId.Value);
        if (status.HasValue) query = query.Where(o => o.Status == status.Value);
        if (!string.IsNullOrEmpty(page.Keyword)) query = query.Where(o => o.OrderNo.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(o => o.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<DeliveryOrder>>.Success(PageResult<DeliveryOrder>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("{id:long}/accept")]
    [Authorize]
    public async Task<IActionResult> Accept(long id)
    {
        var order = await _db.DeliveryOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "订单不存在"));
        if (order.Status != OrderStatus.PendingAccept) return Ok(ApiResponse.Error(400, "订单状态不正确"));
        order.Status = OrderStatus.Accepted;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("接单成功"));
    }

    [HttpPost("{id:long}/complete")]
    [Authorize]
    public async Task<IActionResult> Complete(long id)
    {
        var order = await _db.DeliveryOrders.FindAsync(id);
        if (order == null) return Ok(ApiResponse.Error(404, "订单不存在"));
        order.Status = OrderStatus.Completed;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("订单已完成"));
    }

    [HttpGet("comment/list")]
    [Authorize]
    public async Task<IActionResult> GetCommentList([FromQuery] PageModel page, [FromQuery] long? merchantId)
    {
        var query = _db.OrderComments.AsQueryable();
        if (merchantId.HasValue)
        {
            var orderIds = await _db.DeliveryOrders.Where(o => o.MerchantId == merchantId.Value).Select(o => o.Id).ToListAsync();
            query = query.Where(c => orderIds.Contains(c.OrderId));
        }
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<OrderComment>>.Success(PageResult<OrderComment>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("comment/{id:long}/reply")]
    [Authorize]
    public async Task<IActionResult> ReplyComment(long id, [FromBody] ReplyCommentRequest request)
    {
        var comment = await _db.OrderComments.FindAsync(id);
        if (comment == null) return Ok(ApiResponse.Error(404, "评论不存在"));
        comment.ReplyContent = request.ReplyContent;
        comment.RepliedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("回复成功"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}