using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merchant.Service.Models.Entities;

[Table("merchant")]
public class Merchant
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Logo { get; set; }
    [Column("cover_image")] [MaxLength(500)] public string? CoverImage { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    [Column("business_hours")] [MaxLength(100)] public string? BusinessHours { get; set; }
    [Column("min_delivery_amount")] public decimal MinDeliveryAmount { get; set; }
    [Column("delivery_fee")] public decimal DeliveryFee { get; set; }
    public decimal Rating { get; set; } = 5.0m;
    [Column("monthly_sales")] public int MonthlySales { get; set; }
    public int Status { get; set; }
    [Column("campus_id")] public long? CampusId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    // 入驻扩展信息
    [Column("contact_name")] [MaxLength(50)] public string? ContactName { get; set; }
    [Column("enterprise_name")] [MaxLength(100)] public string? EnterpriseName { get; set; }
    [Column("credit_code")] [MaxLength(50)] public string? CreditCode { get; set; }
    [Column("legal_person")] [MaxLength(50)] public string? LegalPerson { get; set; }
    [Column("business_category")] [MaxLength(100)] public string? BusinessCategory { get; set; }
    [Column("business_scope")] [MaxLength(1000)] public string? BusinessScope { get; set; }
    [Column("license_image")] [MaxLength(500)] public string? LicenseImage { get; set; }
    [Column("id_card_front")] [MaxLength(500)] public string? IdCardFront { get; set; }
    [Column("id_card_back")] [MaxLength(500)] public string? IdCardBack { get; set; }
    [Column("sms_code")] [MaxLength(10)] public string? SmsCode { get; set; }
    [Column("agreed_terms")] public bool AgreedTerms { get; set; }
    [Column("submit_step")] public int SubmitStep { get; set; }
    [Column("audit_remark")] [MaxLength(500)] public string? AuditRemark { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("category")]
public class Category
{
    [Key] public long Id { get; set; }
    [Column("merchant_id")] public long MerchantId { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Icon { get; set; }
    [Column("sort_order")] public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("product")]
public class Product
{
    [Key] public long Id { get; set; }
    [Column("merchant_id")] public long MerchantId { get; set; }
    [Column("category_id")] public long? CategoryId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(500)] public string? Image { get; set; }
    public string? Images { get; set; }
    public decimal Price { get; set; }
    [Column("discount_price")] public decimal? DiscountPrice { get; set; }
    public int Stock { get; set; }
    public int Sales { get; set; }
    [MaxLength(20)] public string Unit { get; set; } = "份";
    public int Status { get; set; } = 1;
    [Column("is_recommend")] public int IsRecommend { get; set; }
    [Column("sort_order")] public int SortOrder { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("product_spec")]
public class ProductSpec
{
    [Key] public long Id { get; set; }
    [Column("product_id")] public long ProductId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
