using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order.Service.Models.Entities;

[Table("delivery_order")]
public class DeliveryOrder
{
    [Key] public long Id { get; set; }
    [MaxLength(32)] public string OrderNo { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long MerchantId { get; set; }
    public long? RiderId { get; set; }
    public long? AddressId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public int? PaymentMethod { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(255)] public string? Remark { get; set; }
    public DateTime? DeliveryTime { get; set; }
    [MaxLength(255)] public string? CancelReason { get; set; }
    public int RefundStatus { get; set; }
    public decimal? RefundAmount { get; set; }
    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("order_item")]
public class OrderItem
{
    [Key] public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    [MaxLength(100)] public string ProductName { get; set; } = string.Empty;
    [MaxLength(500)] public string? ProductImage { get; set; }
    [MaxLength(100)] public string? SpecName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("errand_order")]
public class ErrandOrder
{
    [Key] public long Id { get; set; }
    [MaxLength(32)] public string OrderNo { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long? RiderId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(255)] public string PickupAddress { get; set; } = string.Empty;
    [MaxLength(255)] public string DeliveryAddress { get; set; } = string.Empty;
    public decimal TipAmount { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(50)] public string? ContactName { get; set; }
    [MaxLength(20)] public string? ContactPhone { get; set; }
    [MaxLength(255)] public string? Remark { get; set; }
    [Column("picked_at")]
    public DateTime? PickedAt { get; set; }
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("order_comment")]
public class OrderComment
{
    [Key] public long Id { get; set; }
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public long TargetId { get; set; }
    public int TargetType { get; set; }
    public int Rating { get; set; }
    [MaxLength(500)] public string? Content { get; set; }
    public string? Images { get; set; }
    [MaxLength(500)] public string? ReplyContent { get; set; }
    [Column("replied_at")]
    public DateTime? RepliedAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
