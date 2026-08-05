using System.Security.Claims;
using Campus.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Campus.Tests;

/// <summary>
/// Test helpers for creating mock users and in-memory DbContexts.
/// </summary>
public static class TestHelper
{
    // ── User IDs ──
    public const long UserAId = 1001;  // Student
    public const long UserBId = 1002;  // Student
    public const long MerchantId = 2001;
    public const long AdminId = 3001;

    // ── Create ClaimsPrincipal for different user types ──

    public static ClaimsPrincipal CreateStudent(long userId) =>
        CreatePrincipal(userId, UserType.Student);

    public static ClaimsPrincipal CreateMerchant(long userId) =>
        CreatePrincipal(userId, UserType.Merchant);

    public static ClaimsPrincipal CreateAdmin(long userId) =>
        CreatePrincipal(userId, UserType.Admin);

    private static ClaimsPrincipal CreatePrincipal(long userId, int userType)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("user_type", userType.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    // ── Create ControllerContext with a specific user ──

    public static ControllerContext CreateControllerContext(ClaimsPrincipal user)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    // ── Create in-memory DbContexts ──

    public static Order.Service.Data.OrderDbContext CreateOrderContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<Order.Service.Data.OrderDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? $"Order_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new Order.Service.Data.OrderDbContext(options);
    }

    public static Social.Service.Data.SocialDbContext CreateSocialContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<Social.Service.Data.SocialDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? $"Social_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new Social.Service.Data.SocialDbContext(options);
    }

    public static Wallet.Service.Data.WalletDbContext CreateWalletContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<Wallet.Service.Data.WalletDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? $"Wallet_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new Wallet.Service.Data.WalletDbContext(options);
    }

    public static Merchant.Service.Data.MerchantDbContext CreateMerchantContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<Merchant.Service.Data.MerchantDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? $"Merchant_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new Merchant.Service.Data.MerchantDbContext(options);
    }

    // ── Extract ApiResponse from IActionResult ──

    public static (int code, string message) GetApiResult(IActionResult result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value!;
        var codeProp = value.GetType().GetProperty("Code")!;
        var msgProp = value.GetType().GetProperty("Message")!;
        return ((int)codeProp.GetValue(value)!, (string)msgProp.GetValue(value)!);
    }

    public static (int code, string message, T? data) GetApiResult<T>(IActionResult result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = okResult.Value;
        // Handle both ApiResponse and ApiResponse<T>
        var codeProp = apiResponse!.GetType().GetProperty("Code")!;
        var msgProp = apiResponse.GetType().GetProperty("Message")!;
        var dataProp = apiResponse.GetType().GetProperty("Data");
        return (
            (int)codeProp.GetValue(apiResponse)!,
            (string)msgProp.GetValue(apiResponse)!,
            dataProp == null ? default : (T?)dataProp.GetValue(apiResponse)
        );
    }

    // ── Null logger for controllers that require ILogger ──

    public static ILogger<T> GetNullLogger<T>() => NullLogger<T>.Instance;
}
