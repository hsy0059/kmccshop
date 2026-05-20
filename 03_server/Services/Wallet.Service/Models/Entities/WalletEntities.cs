using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wallet.Service.Models.Entities;

[Table("user_wallet")]
public class UserWallet
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public decimal Balance { get; set; }
    public decimal FrozenBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    [MaxLength(255)] public string? PayPassword { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("wallet_log")]
public class WalletLog
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public decimal? BalanceBefore { get; set; }
    public decimal? BalanceAfter { get; set; }
    public long? RelatedId { get; set; }
    [MaxLength(50)] public string? RelatedType { get; set; }
    [MaxLength(255)] public string? Description { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("withdraw")]
public class Withdraw
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public decimal ActualAmount { get; set; }
    [MaxLength(50)] public string? AccountType { get; set; }
    [MaxLength(255)] public string? AccountInfo { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(255)] public string? RejectReason { get; set; }
    public long? AuditorId { get; set; }
    [Column("audited_at")]
    public DateTime? AuditedAt { get; set; }
    [MaxLength(255)] public string? Remark { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}