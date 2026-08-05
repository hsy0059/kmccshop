using System.ComponentModel.DataAnnotations;

namespace Merchant.Service.Models.DTOs;

public class ApplyMerchantRequest
{
    // 基础信息
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Required][MaxLength(20)] public string Phone { get; set; } = string.Empty;
    [Required][MaxLength(50)] public string ContactName { get; set; } = string.Empty;
    [MaxLength(6)] public string? SmsCode { get; set; }

    // 企业/资质信息
    [MaxLength(100)] public string? EnterpriseName { get; set; }
    [MaxLength(50)] public string? CreditCode { get; set; }
    [MaxLength(50)] public string? LegalPerson { get; set; }
    [Required][MaxLength(100)] public string BusinessCategory { get; set; } = string.Empty;
    [MaxLength(1000)] public string? BusinessScope { get; set; }
    [Required][MaxLength(500)] public string LicenseImage { get; set; } = string.Empty;
    [Required][MaxLength(500)] public string IdCardFront { get; set; } = string.Empty;
    [Required][MaxLength(500)] public string IdCardBack { get; set; } = string.Empty;

    // 店铺设置
    [MaxLength(500)] public string? Logo { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    [MaxLength(100)] public string? BusinessHours { get; set; }
    public decimal MinDeliveryAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public long? CampusId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    // 协议
    [Required] public bool AgreedTerms { get; set; }
    public int SubmitStep { get; set; }
}

public class SendApplySmsCodeRequest
{
    [Required][MaxLength(20)] public string Phone { get; set; } = string.Empty;
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