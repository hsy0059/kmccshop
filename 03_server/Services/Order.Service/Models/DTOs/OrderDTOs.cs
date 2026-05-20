using System.ComponentModel.DataAnnotations;

namespace Order.Service.Models.DTOs;

public class OrderItemRequest
{
    [Required] public long ProductId { get; set; }
    [Required][MaxLength(100)] public string ProductName { get; set; } = string.Empty;
    [MaxLength(500)] public string? ProductImage { get; set; }
    [MaxLength(100)] public string? SpecName { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public int Quantity { get; set; } = 1;
}

public class CreateOrderRequest
{
    [Required] public long MerchantId { get; set; }
    public long? AddressId { get; set; }
    [MaxLength(255)] public string? Remark { get; set; }
    [Required] public List<OrderItemRequest> Items { get; set; } = new();
    public long? CouponId { get; set; }
}

public class CreateErrandRequest
{
    [Required][MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [Required][MaxLength(255)] public string PickupAddress { get; set; } = string.Empty;
    [Required][MaxLength(255)] public string DeliveryAddress { get; set; } = string.Empty;
    [Required] public decimal TipAmount { get; set; }
    [MaxLength(50)] public string? ContactName { get; set; }
    [MaxLength(20)] public string? ContactPhone { get; set; }
    [MaxLength(255)] public string? Remark { get; set; }
}

public class SubmitCommentRequest
{
    [Required] public long OrderId { get; set; }
    [Required] public long TargetId { get; set; }
    [Required] public int TargetType { get; set; }
    [Required][Range(1, 5)] public int Rating { get; set; }
    [MaxLength(500)] public string? Content { get; set; }
    public string? Images { get; set; }
}

public class ReplyCommentRequest
{
    [Required][MaxLength(500)] public string ReplyContent { get; set; } = string.Empty;
}