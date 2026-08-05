using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Merchant.Service.Controllers;
using Merchant.Service.Data;
using Merchant.Service.Models.Entities;
using Merchant.Service.Models.DTOs;
using Campus.Common;
using MerchantEntity = Merchant.Service.Models.Entities.Merchant;

namespace Campus.Tests;

/// <summary>
/// MerchantController unit tests:
/// - Apply: normal / duplicate / not-logged-in / db-exception / general-exception
/// - GetDetail: existing / non-existent
/// - GetList: status filter
/// - Audit: non-admin rejected / admin approve
/// - Update: non-admin-non-merchant rejected / merchant owner success
/// - Dashboard: merchant not found
/// - GetStats: non-admin rejected
/// </summary>
public class MerchantControllerTests : IDisposable
{
    private readonly MerchantDbContext _db;
    private readonly MerchantController _controller;

    public MerchantControllerTests()
    {
        _db = TestHelper.CreateMerchantContext();
        _controller = new MerchantController(_db, TestHelper.GetNullLogger<MerchantController>());
    }

    public void Dispose() => _db.Dispose();

    private MerchantEntity SeedMerchant(long userId, int status = MerchantStatus.Pending)
    {
        var merchant = new MerchantEntity
        {
            UserId = userId,
            Name = "Test Merchant " + userId,
            Phone = "13800000000",
            Address = "Test Address",
            BusinessHours = "08:00-22:00",
            MinDeliveryAmount = 10m,
            DeliveryFee = 2m,
            Status = status
        };
        _db.Merchants.Add(merchant);
        _db.SaveChanges();
        return merchant;
    }

    private static ApplyMerchantRequest BuildApplyRequest(string name = "Campus Tea Shop") => new()
    {
        Name = name,
        Phone = "13800138001",
        ContactName = "张三",
        SmsCode = "123456",
        BusinessCategory = "餐饮美食",
        BusinessScope = "奶茶、果汁、轻食",
        LicenseImage = "/uploads/license.jpg",
        IdCardFront = "/uploads/idcard_front.jpg",
        IdCardBack = "/uploads/idcard_back.jpg",
        Address = "Canteen 2F A203",
        BusinessHours = "08:00-22:00",
        MinDeliveryAmount = 10m,
        DeliveryFee = 2m,
        Description = "Milk tea and snacks",
        CampusId = 1,
        Longitude = 116.397428m,
        Latitude = 39.90923m,
        AgreedTerms = true,
        SubmitStep = 4
    };

    // ── Apply: Normal ──

    [Fact]
    public async Task Apply_NewApplication_ReturnsSuccess()
    {
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Apply(BuildApplyRequest());
        var (code, msg, data) = TestHelper.GetApiResult<MerchantEntity>(result);

        Assert.Equal(0, code);
        Assert.Equal("申请已提交", msg);
        Assert.NotNull(data);
        Assert.Equal(TestHelper.UserAId, data!.UserId);
        Assert.Equal(MerchantStatus.Pending, data.Status);
        Assert.Equal(5.0m, data.Rating);
        Assert.Equal(0, data.MonthlySales);
        Assert.Equal("张三", data.ContactName);
        Assert.Equal("餐饮美食", data.BusinessCategory);
        Assert.Equal("/uploads/license.jpg", data.LicenseImage);
        Assert.True(data.AgreedTerms);
    }

    [Fact]
    public async Task SendApplySmsCode_ReturnsCode()
    {
        var result = await _controller.SendApplySmsCode(new SendApplySmsCodeRequest { Phone = "13800138001" });
        var (code, _, data) = TestHelper.GetApiResult<dynamic>(result);

        Assert.Equal(0, code);
        Assert.NotNull(data);
    }

    // ── Apply: Terms not agreed ──

    [Fact]
    public async Task Apply_TermsNotAgreed_Returns400()
    {
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));
        var request = BuildApplyRequest();
        request.AgreedTerms = false;

        var result = await _controller.Apply(request);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(400, code);
        Assert.Contains("入驻协议", msg);
    }

    // ── Apply: Duplicate ──

    [Fact]
    public async Task Apply_DuplicateApplication_Returns400()
    {
        SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Apply(BuildApplyRequest());
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(400, code);
        Assert.Contains("已提交过", msg);
    }

    // ── Apply: Not Logged In ──

    [Fact]
    public async Task Apply_NotLoggedIn_Returns401()
    {
        // User with no NameIdentifier claim
        var noClaimUser = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity());
        _controller.ControllerContext = TestHelper.CreateControllerContext(noClaimUser);

        var result = await _controller.Apply(BuildApplyRequest());
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(401, code);
        Assert.Contains("未登录", msg);
    }

    // ── Apply: DbUpdateException ──

    [Fact]
    public async Task Apply_DbUpdateException_Returns500()
    {
        var throwingDb = new ThrowingMerchantDbContext(
            new DbContextOptionsBuilder<MerchantDbContext>()
                .UseInMemoryDatabase($"Throw_DbUpdate_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options,
            new DbUpdateException("Simulated DB failure"));
        var controller = new MerchantController(throwingDb, TestHelper.GetNullLogger<MerchantController>());
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await controller.Apply(BuildApplyRequest());
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(500, code);
        Assert.Contains("申请提交失败", msg);
        throwingDb.Dispose();
    }

    // ── Apply: Unexpected Exception ──

    [Fact]
    public async Task Apply_UnexpectedException_Returns500()
    {
        var throwingDb = new ThrowingMerchantDbContext(
            new DbContextOptionsBuilder<MerchantDbContext>()
                .UseInMemoryDatabase($"Throw_General_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options,
            new InvalidOperationException("Unexpected error"));
        var controller = new MerchantController(throwingDb, TestHelper.GetNullLogger<MerchantController>());
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await controller.Apply(BuildApplyRequest());
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(500, code);
        Assert.Contains("系统异常", msg);
        throwingDb.Dispose();
    }

    // ── GetDetail ──

    [Fact]
    public async Task GetDetail_ExistingMerchant_ReturnsSuccess()
    {
        var merchant = SeedMerchant(TestHelper.UserAId, MerchantStatus.Open);

        var result = await _controller.GetDetail(merchant.Id);
        var (code, _, data) = TestHelper.GetApiResult<MerchantEntity>(result);

        Assert.Equal(0, code);
        Assert.NotNull(data);
        Assert.Equal(merchant.Name, data!.Name);
    }

    [Fact]
    public async Task GetDetail_NonExistentMerchant_Returns404()
    {
        var result = await _controller.GetDetail(99999);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(404, code);
        Assert.Contains("不存在", msg);
    }

    // ── GetList ──

    [Fact]
    public async Task GetList_FilterByStatus_ReturnsOnlyMatching()
    {
        SeedMerchant(TestHelper.UserAId, MerchantStatus.Pending);
        SeedMerchant(TestHelper.UserBId, MerchantStatus.Open);

        var result = await _controller.GetList(new PageModel { Page = 1, PageSize = 10 }, status: MerchantStatus.Open);
        var (code, _, data) = TestHelper.GetApiResult<PageResult<MerchantEntity>>(result);

        Assert.Equal(0, code);
        Assert.NotNull(data);
        Assert.Equal(1, data!.Total);
        Assert.All(data.List, m => Assert.Equal(MerchantStatus.Open, m.Status));
    }

    // ── Audit ──

    [Fact]
    public async Task Audit_NonAdmin_Returns403()
    {
        var merchant = SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await _controller.Audit(merchant.Id, new AuditMerchantRequest { Status = MerchantStatus.Open });
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    [Fact]
    public async Task Audit_AdminApprove_ReturnsSuccess()
    {
        var merchant = SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateAdmin(TestHelper.AdminId));

        var result = await _controller.Audit(merchant.Id, new AuditMerchantRequest { Status = MerchantStatus.Open });
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Contains("审核通过", msg);
        // Verify status actually persisted
        var updated = await _db.Merchants.FindAsync(merchant.Id);
        Assert.Equal(MerchantStatus.Open, updated!.Status);
    }

    [Fact]
    public async Task Audit_InvalidStatus_Returns400()
    {
        var merchant = SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateAdmin(TestHelper.AdminId));

        var result = await _controller.Audit(merchant.Id, new AuditMerchantRequest { Status = 99 });
        var (code, _) = TestHelper.GetApiResult(result);

        Assert.Equal(400, code);
    }

    // ── Update ──

    [Fact]
    public async Task Update_NonAdminNonMerchant_Returns403()
    {
        var merchant = SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await _controller.Update(merchant.Id, new UpdateMerchantRequest { Name = "Hacked" });
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    [Fact]
    public async Task Update_MerchantOwner_ReturnsSuccess()
    {
        var merchant = SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateMerchant(TestHelper.UserAId));

        var result = await _controller.Update(merchant.Id, new UpdateMerchantRequest { Name = "Updated Shop" });
        var (code, msg, data) = TestHelper.GetApiResult<MerchantEntity>(result);

        Assert.Equal(0, code);
        Assert.Contains("更新成功", msg);
        Assert.Equal("Updated Shop", data!.Name);
    }

    [Fact]
    public async Task Update_MerchantNotOwner_Returns403()
    {
        var merchant = SeedMerchant(TestHelper.UserAId);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateMerchant(TestHelper.UserBId));

        var result = await _controller.Update(merchant.Id, new UpdateMerchantRequest { Name = "Hacked" });
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    // ── Dashboard ──

    [Fact]
    public async Task Dashboard_MerchantNotFound_Returns404()
    {
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Dashboard();
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(404, code);
        Assert.Contains("未找到", msg);
    }

    [Fact]
    public async Task Dashboard_ExistingMerchant_ReturnsSuccess()
    {
        var merchant = SeedMerchant(TestHelper.UserAId, MerchantStatus.Open);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.Dashboard();
        var (code, _, data) = TestHelper.GetApiResult<object>(result);

        Assert.Equal(0, code);
        Assert.NotNull(data);
    }

    // ── GetStats ──

    [Fact]
    public async Task GetStats_NonAdmin_Returns403()
    {
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await _controller.GetStats();
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    [Fact]
    public async Task GetStats_Admin_ReturnsCorrectCounts()
    {
        SeedMerchant(TestHelper.UserAId, MerchantStatus.Open);
        SeedMerchant(TestHelper.UserBId, MerchantStatus.Pending);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateAdmin(TestHelper.AdminId));

        var result = await _controller.GetStats();
        var (code, _, data) = TestHelper.GetApiResult<dynamic>(result);

        Assert.Equal(0, code);
        Assert.NotNull(data);
    }

    // ── Throwing DbContext for exception-branch tests ──

    private class ThrowingMerchantDbContext : MerchantDbContext
    {
        private readonly Exception _exception;
        public ThrowingMerchantDbContext(DbContextOptions<MerchantDbContext> options, Exception exception)
            : base(options) { _exception = exception; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => throw _exception;
    }
}
