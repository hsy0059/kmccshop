using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Service.Models.Entities;

[Table("rider")]
public class Rider
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(50)] public string RealName { get; set; } = string.Empty;
    [MaxLength(20)] public string Phone { get; set; } = string.Empty;
    [MaxLength(20)] public string? IdCard { get; set; }
    public decimal Balance { get; set; }
    public decimal Rating { get; set; } = 5.0m;
    public int OrderCount { get; set; }
    public int Status { get; set; }
    public int AuditStatus { get; set; }
    [MaxLength(50)] public string? VehicleType { get; set; }
    [MaxLength(50)] public string? VehicleNumber { get; set; }
    public long? CampusId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
