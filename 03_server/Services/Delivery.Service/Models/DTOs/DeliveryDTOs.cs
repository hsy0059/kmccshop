using System.ComponentModel.DataAnnotations;

namespace Delivery.Service.Models.DTOs;

public class RiderApplyRequest
{
    [Required][MaxLength(50)] public string RealName { get; set; } = string.Empty;
    [Required][MaxLength(20)] public string Phone { get; set; } = string.Empty;
    [MaxLength(20)] public string? IdCard { get; set; }
    [MaxLength(50)] public string? VehicleType { get; set; }
    [MaxLength(50)] public string? VehicleNumber { get; set; }
    public long? CampusId { get; set; }
}

public class RiderApproveRequest
{
    [Required] public int AuditStatus { get; set; }
    public string? Remark { get; set; }
}

public class UpdateRiderStatusRequest
{
    [Required] public int Status { get; set; }
}