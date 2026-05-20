using System.ComponentModel.DataAnnotations;

namespace Merchant.Service.Models.DTOs;

public class ApplyMerchantRequest
{
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Logo { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    [MaxLength(100)] public string? BusinessHours { get; set; }
    public decimal MinDeliveryAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public long? CampusId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
}

public class AuditMerchantRequest
{
    [Required] public int Status { get; set; }
    public string? Remark { get; set; }
}

public class UpdateMerchantRequest
{
    [MaxLength(100)] public string? Name { get; set; }
    [MaxLength(500)] public string? Logo { get; set; }
    [MaxLength(500)] public string? CoverImage { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    [MaxLength(100)] public string? BusinessHours { get; set; }
    public decimal? MinDeliveryAmount { get; set; }
    public decimal? DeliveryFee { get; set; }
    public long? CampusId { get; set; }
}

public class ProductCreateRequest
{
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    public long? CategoryId { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(500)] public string? Image { get; set; }
    public string? Images { get; set; }
    [Required] public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Stock { get; set; }
    public string? Unit { get; set; }
    public int IsRecommend { get; set; }
}

public class ProductUpdateRequest
{
    [MaxLength(100)] public string? Name { get; set; }
    public long? CategoryId { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(500)] public string? Image { get; set; }
    public string? Images { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? Stock { get; set; }
    public string? Unit { get; set; }
    public int? IsRecommend { get; set; }
}

public class ProductStatusRequest
{
    [Required] public int Status { get; set; }
}