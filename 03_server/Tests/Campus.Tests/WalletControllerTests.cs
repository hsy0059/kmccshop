using Microsoft.AspNetCore.Mvc;
using Wallet.Service.Controllers;
using Wallet.Service.Data;
using Wallet.Service.Models.Entities;
using Wallet.Service.Models.DTOs;
using Campus.Common;

namespace Campus.Tests;

/// <summary>
/// Wallet controller tests: Withdraw balance check, transaction consistency.
/// </summary>
public class WalletControllerTests : IDisposable
{
    private readonly WalletDbContext _db;
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
        _db = TestHelper.CreateWalletContext();
        _controller = new WalletController(_db);
        _controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));
    }

    public void Dispose() => _db.Dispose();

    private void SeedWallet(long userId, decimal balance, decimal frozen = 0)
    {
        _db.UserWallets.Add(new UserWallet
        {
            UserId = userId, Balance = balance, FrozenBalance = frozen
        });
        _db.SaveChanges();
    }

    // ── Withdraw Tests ──

    [Fact]
    public async Task Withdraw_SufficientBalance_ReturnsSuccess()
    {
        SeedWallet(TestHelper.UserAId, 100m);
        var request = new WithdrawRequest { Amount = 30m, AccountType = "alipay", AccountInfo = "test@example.com" };

        var result = await _controller.Withdraw(request);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("提现申请已提交", msg);
    }

    [Fact]
    public async Task Withdraw_InsufficientBalance_Returns400()
    {
        SeedWallet(TestHelper.UserAId, 10m);
        var request = new WithdrawRequest { Amount = 50m, AccountType = "alipay", AccountInfo = "test@example.com" };

        var result = await _controller.Withdraw(request);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(400, code);
        Assert.Contains("余额不足", msg);
    }

    [Fact]
    public async Task Withdraw_UpdatesBalanceAndFrozenBalance()
    {
        SeedWallet(TestHelper.UserAId, 100m);
        var request = new WithdrawRequest { Amount = 30m, AccountType = "alipay", AccountInfo = "test@example.com" };

        await _controller.Withdraw(request);

        var wallet = _db.UserWallets.First(w => w.UserId == TestHelper.UserAId);
        Assert.Equal(70m, wallet.Balance);       // 100 - 30 = 70
        Assert.Equal(30m, wallet.FrozenBalance);  // 0 + 30 = 30
    }

    [Fact]
    public async Task Withdraw_CreatesWalletLog()
    {
        SeedWallet(TestHelper.UserAId, 100m);
        var request = new WithdrawRequest { Amount = 30m, AccountType = "alipay", AccountInfo = "test@example.com" };

        await _controller.Withdraw(request);

        var logs = _db.WalletLogs.Where(l => l.UserId == TestHelper.UserAId).ToList();
        Assert.Single(logs);
        Assert.Equal(WalletLogType.Withdraw, logs[0].Type);
        Assert.Equal(-30m, logs[0].Amount);
        Assert.Equal(70m, logs[0].BalanceAfter);  // 100 - 30 = 70
    }

    [Fact]
    public async Task Withdraw_CreatesWithdrawRecord()
    {
        SeedWallet(TestHelper.UserAId, 100m);
        var request = new WithdrawRequest { Amount = 30m, AccountType = "alipay", AccountInfo = "test@example.com" };

        await _controller.Withdraw(request);

        var withdraws = _db.Withdraws.Where(w => w.UserId == TestHelper.UserAId).ToList();
        Assert.Single(withdraws);
        Assert.Equal(30m, withdraws[0].Amount);
        Assert.Equal(WithdrawStatus.Pending, withdraws[0].Status);
    }

    // ── GetInfo Tests ──

    [Fact]
    public async Task GetInfo_ExistingWallet_ReturnsBalance()
    {
        SeedWallet(TestHelper.UserAId, 50m);

        var result = await _controller.GetInfo();
        var (code, _, data) = TestHelper.GetApiResult<UserWallet>(result);

        Assert.Equal(0, code);
        Assert.Equal(50m, data!.Balance);
    }

    [Fact]
    public async Task GetInfo_NoWallet_CreatesNewWithZeroBalance()
    {
        var result = await _controller.GetInfo();
        var (code, _, data) = TestHelper.GetApiResult<UserWallet>(result);

        Assert.Equal(0, code);
        Assert.Equal(0m, data!.Balance);
    }
}
