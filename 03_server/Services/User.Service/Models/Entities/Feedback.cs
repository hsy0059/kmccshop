using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Service.Models.Entities;

[Table("feedback")]
public class Feedback
{
    [Key]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    public int Type { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public string? Images { get; set; }

    [MaxLength(100)]
    [Column("contact_info")]
    public string? ContactInfo { get; set; }

    public int Status { get; set; } = 1;

    [MaxLength(500)]
    [Column("reply_content")]
    public string? ReplyContent { get; set; }

    [Column("replier_id")]
    public long? ReplierId { get; set; }

    [Column("replied_at")]
    public DateTime? RepliedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
