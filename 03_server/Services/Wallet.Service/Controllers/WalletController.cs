using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Service.Data;
using Wallet.Service.Models.DTOs;
using Wallet.Service.Models.Entities;
using Campus.Common;

namespace Wallet.Service.Controllers;

[ApiController]
[Route("api/v1/wallet")]
public class WalletController : ControllerBase
{
    private readonly WalletDbContext _db;
    public WalletController(WalletDbContext db) { _db = db; }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> GetInfo()
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var wallet = await _db.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value);
        if (wallet == null)
        {
            wallet = new UserWallet { UserId = userId.Value, Balance = 0 };
            _db.UserWallets.Add(wallet);
            await _db.SaveChangesAsync();
        }
        return Ok(ApiResponse<UserWallet>.Success(wallet));
    }

    [HttpGet("logs")]
    [Authorize]
    public async Task<IActionResult> GetLogs([FromQuery] PageModel page)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var query = _db.WalletLogs.Where(l => l.UserId == userId.Value);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(l => l.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<WalletLog>>.Success(PageResult<WalletLog>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("withdraw")]
    [Authorize]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var wallet = await _db.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value);
        if (wallet == null || wallet.Balance < request.Amount)
            return Ok(ApiResponse.Error(400, "余额不足"));
        wallet.Balance -= request.Amount;
        wallet.FrozenBalance += request.Amount;

        var withdraw = new Withdraw
        {
            UserId = userId.Value, Amount = request.Amount, Fee = 0,
            ActualAmount = request.Amount, AccountType = request.AccountType,
            AccountInfo = request.AccountInfo, Status = WithdrawStatus.Pending
        };
        _db.Withdraws.Add(withdraw);

        _db.WalletLogs.Add(new WalletLog
        {
            UserId = userId.Value, Type = WalletLogType.Withdraw, Amount = -request.Amount,
            BalanceBefore = wallet.Balance + request.Amount, BalanceAfter = wallet.Balance,
            RelatedId = withdraw.Id, RelatedType = "withdraw", Description = "发起提现"
        });

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Withdraw>.Success(withdraw, "提现申请已提交"));
    }

    [HttpGet("withdraws")]
    [Authorize]
    public async Task<IActionResult> GetWithdraws([FromQuery] PageModel page)
    {
        var query = _db.Withdraws.AsQueryable();
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(w => w.AccountInfo!.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(w => w.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Withdraw>>.Success(PageResult<Withdraw>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPut("withdraw/{id:long}/audit")]
    [Authorize]
    public async Task<IActionResult> AuditWithdraw(long id, [FromBody] WithdrawAuditRequest request)
    {
        var withdraw = await _db.Withdraws.FindAsync(id);
        if (withdraw == null) return Ok(ApiResponse.Error(404, "提现记录不存在"));
        var auditorId = GetUserId();
        withdraw.Status = request.Status;
        withdraw.AuditorId = auditorId;
        withdraw.AuditedAt = DateTime.Now;
        if (request.Status == WithdrawStatus.Rejected)
            withdraw.RejectReason = request.RejectReason;

        if (request.Status == WithdrawStatus.Rejected)
        {
            var wallet = await _db.UserWallets.FirstOrDefaultAsync(w => w.UserId == withdraw.UserId);
            if (wallet != null)
            {
                wallet.Balance += withdraw.Amount;
                wallet.FrozenBalance -= withdraw.Amount;
            }
        }

        if (request.Status == WithdrawStatus.Approved)
        {
            var wallet = await _db.UserWallets.FirstOrDefaultAsync(w => w.UserId == withdraw.UserId);
            if (wallet != null) wallet.FrozenBalance -= withdraw.Amount;
        }

        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("审核完成"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}