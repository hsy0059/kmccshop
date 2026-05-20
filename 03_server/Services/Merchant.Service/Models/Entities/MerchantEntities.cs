using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merchant.Service.Models.Entities;

[Table("merchant")]
public class Merchant
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Logo { get; set; }
    [MaxLength(500)] public string? CoverImage { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    [MaxLength(100)] public string? BusinessHours { get; set; }
    public decimal MinDeliveryAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Rating { get; set; } = 5.0m;
    public int MonthlySales { get; set; }
    public int Status { get; set; }
    public long? CampusId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("category")]
public class Category
{
    [Key] public long Id { get; set; }
    public long MerchantId { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Icon { get; set; }
    public int SortOrder { get; set; }
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
    public long MerchantId { get; set; }
    public long? CategoryId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(500)] public string? Image { get; set; }
    public string? Images { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Stock { get; set; }
    public int Sales { get; set; }
    [MaxLength(20)] public string Unit { get; set; } = "份";
    public int Status { get; set; } = 1;
    public int IsRecommend { get; set; }
    public int SortOrder { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("product_spec")]
public class ProductSpec
{
    [Key] public long Id { get; set; }
    public long ProductId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
