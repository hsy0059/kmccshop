using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Campus.Service.Models.Entities;

[Table("school")]
public class School
{
    [Key] public long Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Column("short_name")] [MaxLength(50)] public string? ShortName { get; set; }
    [MaxLength(50)] public string? Province { get; set; }
    [MaxLength(50)] public string? City { get; set; }
    [MaxLength(50)] public string? District { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    [MaxLength(500)] public string? Logo { get; set; }
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("campus")]
public class CampusEntity
{
    [Key] public long Id { get; set; }
    [Column("school_id")] public long SchoolId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    [Column("delivery_radius")] public int DeliveryRadius { get; set; } = 3000;
    public int Status { get; set; } = 1;
    [Column("sort_order")] public int SortOrder { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("delivery_zone")]
public class DeliveryZone
{
    [Key] public long Id { get; set; }
    [Column("campus_id")] public long CampusId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Column("delivery_fee")] public decimal DeliveryFee { get; set; }
    [Column("min_order_amount")] public decimal MinOrderAmount { get; set; }
    [Column("estimated_time")] public int EstimatedTime { get; set; } = 30;
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
