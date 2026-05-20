using System.ComponentModel.DataAnnotations;
namespace Wallet.Service.Models.DTOs;

public class WithdrawRequest
{
    [Required] public decimal Amount { get; set; }
    [MaxLength(50)] public string? AccountType { get; set; }
    [MaxLength(255)] public string? AccountInfo { get; set; }
}
public class WithdrawAuditRequest
{
    [Required] public int Status { get; set; }
    [MaxLength(255)] public string? RejectReason { get; set; }
}