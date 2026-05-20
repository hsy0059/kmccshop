using System.ComponentModel.DataAnnotations;

namespace Campus.Service.Models.DTOs;

public class CreateCampusRequest
{
    [Required] public long SchoolId { get; set; }
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int DeliveryRadius { get; set; } = 3000;
}

public class UpdateCampusRequest
{
    [MaxLength(100)] public string? Name { get; set; }
    [MaxLength(255)] public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int? DeliveryRadius { get; set; }
}

public class CreateDeliveryZoneRequest
{
    [Required] public long CampusId { get; set; }
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
    public decimal MinOrderAmount { get; set; }
    public int EstimatedTime { get; set; } = 30;
}

public class UpdateDeliveryZoneRequest
{
    [MaxLength(100)] public string? Name { get; set; }
    public decimal? DeliveryFee { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? EstimatedTime { get; set; }
}