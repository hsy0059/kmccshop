using System.ComponentModel.DataAnnotations;
namespace Coupon.Service.Models.DTOs;

public class CreateCouponRequest
{
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string? Description { get; set; }
    [Required] public int Type { get; set; }
    [Required] public decimal DiscountValue { get; set; }
    public decimal MinAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int TotalCount { get; set; }
    public int PerUserLimit { get; set; } = 1;
    public long? MerchantId { get; set; }
    [Required] public DateTime StartTime { get; set; }
    [Required] public DateTime EndTime { get; set; }
    public int? ValidDays { get; set; }
}
public class UpdateCouponRequest
{
    [MaxLength(100)] public string? Name { get; set; }
    [MaxLength(255)] public string? Description { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int? TotalCount { get; set; }
    public int? PerUserLimit { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Status { get; set; }
}