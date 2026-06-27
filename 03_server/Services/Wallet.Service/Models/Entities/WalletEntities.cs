using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wallet.Service.Models.Entities;

[Table("user_wallet")]
public class UserWallet
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    public decimal Balance { get; set; }
    [Column("frozen_balance")] public decimal FrozenBalance { get; set; }
    [Column("total_income")] public decimal TotalIncome { get; set; }
    [Column("total_expense")] public decimal TotalExpense { get; set; }
    [Column("pay_password")] [MaxLength(255)] public string? PayPassword { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("wallet_log")]
public class WalletLog
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    [Column("balance_before")] public decimal? BalanceBefore { get; set; }
    [Column("balance_after")] public decimal? BalanceAfter { get; set; }
    [Column("related_id")] public long? RelatedId { get; set; }
    [Column("related_type")] [MaxLength(50)] public string? RelatedType { get; set; }
    [MaxLength(255)] public string? Description { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("withdraw")]
public class Withdraw
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    [Column("actual_amount")] public decimal ActualAmount { get; set; }
    [Column("account_type")] [MaxLength(50)] public string? AccountType { get; set; }
    [Column("account_info")] [MaxLength(255)] public string? AccountInfo { get; set; }
    public int Status { get; set; } = 1;
    [Column("reject_reason")] [MaxLength(255)] public string? RejectReason { get; set; }
    [Column("auditor_id")] public long? AuditorId { get; set; }
    [Column("audited_at")]
    public DateTime? AuditedAt { get; set; }
    [MaxLength(255)] public string? Remark { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}