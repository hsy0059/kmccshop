using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order.Service.Models.Entities;

[Table("delivery_order")]
public class DeliveryOrder
{
    [Key] public long Id { get; set; }
    [Column("order_no")] [MaxLength(32)] public string OrderNo { get; set; } = string.Empty;
    [Column("user_id")] public long UserId { get; set; }
    [Column("merchant_id")] public long MerchantId { get; set; }
    [Column("rider_id")] public long? RiderId { get; set; }
    [Column("address_id")] public long? AddressId { get; set; }
    [Column("total_amount")] public decimal TotalAmount { get; set; }
    [Column("delivery_fee")] public decimal DeliveryFee { get; set; }
    [Column("discount_amount")] public decimal DiscountAmount { get; set; }
    [Column("actual_amount")] public decimal ActualAmount { get; set; }
    [Column("payment_method")] public int? PaymentMethod { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(255)] public string? Remark { get; set; }
    [Column("delivery_time")] public DateTime? DeliveryTime { get; set; }
    [Column("cancel_reason")] [MaxLength(255)] public string? CancelReason { get; set; }
    [Column("refund_status")] public int RefundStatus { get; set; }
    [Column("refund_amount")] public decimal? RefundAmount { get; set; }
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
    [Column("order_id")] public long OrderId { get; set; }
    [Column("product_id")] public long ProductId { get; set; }
    [Column("product_name")] [MaxLength(100)] public string ProductName { get; set; } = string.Empty;
    [Column("product_image")] [MaxLength(500)] public string? ProductImage { get; set; }
    [Column("spec_name")] [MaxLength(100)] public string? SpecName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    [Column("total_price")] public decimal TotalPrice { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("errand_order")]
public class ErrandOrder
{
    [Key] public long Id { get; set; }
    [Column("order_no")] [MaxLength(32)] public string OrderNo { get; set; } = string.Empty;
    [Column("user_id")] public long UserId { get; set; }
    [Column("rider_id")] public long? RiderId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [Column("pickup_address")] [MaxLength(255)] public string PickupAddress { get; set; } = string.Empty;
    [Column("delivery_address")] [MaxLength(255)] public string DeliveryAddress { get; set; } = string.Empty;
    [Column("tip_amount")] public decimal TipAmount { get; set; }
    public int Status { get; set; } = 1;
    [Column("contact_name")] [MaxLength(50)] public string? ContactName { get; set; }
    [Column("contact_phone")] [MaxLength(20)] public string? ContactPhone { get; set; }
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
    [Column("order_id")] public long OrderId { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    [Column("target_id")] public long TargetId { get; set; }
    [Column("target_type")] public int TargetType { get; set; }
    public int Rating { get; set; }
    [MaxLength(500)] public string? Content { get; set; }
    public string? Images { get; set; }
    [Column("reply_content")] [MaxLength(500)] public string? ReplyContent { get; set; }
    [Column("replied_at")]
    public DateTime? RepliedAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
