using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coupon.Service.Models.Entities;

/// <summary>
/// 轻量级商家信息，仅用于跨服务归属校验（共享数据库）。
/// 映射到 merchant 表，只读取 id 和 user_id。
/// </summary>
[Table("merchant")]
public class MerchantInfo
{
    [Key]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }
}
