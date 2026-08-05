using Microsoft.AspNetCore.Mvc;
using Order.Service.Controllers;
using Order.Service.Data;
using Order.Service.Models.Entities;
using Order.Service.Models.DTOs;
using Campus.Common;

namespace Campus.Tests;

/// <summary>
/// Order controller permission tests: Cancel, Pay, Refund, SubmitComment ownership checks.
/// </summary>
public class OrderControllerTests : IDisposable
{
    private readonly OrderDbContext _db;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _db = TestHelper.CreateOrderContext();
        _controller = new OrderController(_db);
    }

    public void Dispose() => _db.Dispose();

    private DeliveryOrder SeedOrder(long userId, int status = OrderStatus.PendingPayment, int refundStatus = 0)
    {
        var order = new DeliveryOrder
        {
            OrderNo = $"TEST{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}",
            UserId = userId, MerchantId = 1, TotalAmount = 10m, DeliveryFee = 2m,
            ActualAmount = 12m, Status = status, RefundStatus = refundStatus
        };
        _db.DeliveryOrders.Add(order);
        _db.SaveChanges();
        return order;
    }

    // ── Cancel Tests ──

    [Fact]
    public async Task Cancel_OwnOrder_ReturnsSuccess()
    {
        var order = SeedOrder(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Cancel(order.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("订单已取消", msg);
    }

    [Fact]
    public async Task Cancel_OtherUserOrder_Returns403()
    {
        var order = SeedOrder(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await _controller.Cancel(order.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    [Fact]
    public async Task Cancel_AlreadyCancelled_Returns400()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.Cancelled);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Cancel(order.Id);
        var (code, _) = TestHelper.GetApiResult(result);

        Assert.Equal(400, code);
    }

    // ── Pay Tests ──

    [Fact]
    public async Task Pay_OwnOrder_ReturnsSuccess()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.PendingPayment);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Pay(order.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("支付成功", msg);
    }

    [Fact]
    public async Task Pay_OtherUserOrder_Returns403()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.PendingPayment);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await _controller.Pay(order.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    // ── Refund Tests ──

    [Fact]
    public async Task Refund_OwnOrder_ReturnsSuccess()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.PendingAccept);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Refund(order.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("退款申请已提交", msg);
    }

    [Fact]
    public async Task Refund_OtherUserOrder_Returns403()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.PendingAccept);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await _controller.Refund(order.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    [Fact]
    public async Task Refund_AlreadyRefunded_Returns400()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.PendingAccept, refundStatus: 1);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Refund(order.Id);
        var (code, _) = TestHelper.GetApiResult(result);

        Assert.Equal(400, code);
    }

    // ── SubmitComment Tests ──

    [Fact]
    public async Task SubmitComment_OwnOrder_ReturnsSuccess()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.Completed);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var request = new SubmitCommentRequest
        {
            OrderId = order.Id, TargetType = 1, TargetId = 1, Rating = 5, Content = "good"
        };
        var result = await _controller.SubmitComment(request);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("评价成功", msg);
    }

    [Fact]
    public async Task SubmitComment_OtherUserOrder_Returns403()
    {
        var order = SeedOrder(TestHelper.UserAId, OrderStatus.Completed);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var request = new SubmitCommentRequest
        {
            OrderId = order.Id, TargetType = 1, TargetId = 1, Rating = 5, Content = "bad"
        };
        var result = await _controller.SubmitComment(request);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    // ── Merchant Access Tests ──

    [Fact]
    public async Task GetMerchantOrderList_Student_Returns403()
    {
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.GetMerchantOrderList(
            new PageModel { Page = 1, PageSize = 10 }, null, null);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("仅商家", msg);
    }

    [Fact]
    public async Task GetMerchantStats_Student_Returns403()
    {
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.GetMerchantStats(merchantId: 1);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("仅商家", msg);
    }

    [Fact]
    public async Task GetMerchantOrderList_Merchant_ReturnsSuccess()
    {
        SeedOrder(TestHelper.UserAId, OrderStatus.Completed);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateMerchant(TestHelper.MerchantId));

        var result = await _controller.GetMerchantOrderList(
            new PageModel { Page = 1, PageSize = 10 }, null, null);
        var (code, _) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
    }
}
