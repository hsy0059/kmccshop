using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social.Service.Models.Entities;

[Table("post")]
public class Post
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public long? CategoryId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Images { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int FavoriteCount { get; set; }
    public int IsTop { get; set; }
    public int IsEssence { get; set; }
    public int IsLocked { get; set; }
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("post_like")]
public class PostLike
{
    [Key] public long Id { get; set; }
    public long PostId { get; set; }
    public long UserId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("post_comment")]
public class PostComment
{
    [Key] public long Id { get; set; }
    public long PostId { get; set; }
    public long UserId { get; set; }
    public long? ParentId { get; set; }
    public long? ReplyToUserId { get; set; }
    [MaxLength(500)] public string Content { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("second_goods")]
public class SecondGoods
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    public string? Images { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    [MaxLength(50)] public string? Category { get; set; }
    [MaxLength(50)] public string? ConditionDesc { get; set; }
    public int ViewCount { get; set; }
    public int FavoriteCount { get; set; }
    [MaxLength(100)] public string? ContactInfo { get; set; }
    public long? CampusId { get; set; }
    public int Status { get; set; } = 1;
    public int IsSold { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("lost_found")]
public class LostFound
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public int Type { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    public string? Images { get; set; }
    [MaxLength(100)] public string? Location { get; set; }
    [MaxLength(100)] public string? ContactInfo { get; set; }
    [MaxLength(50)] public string? Category { get; set; }
    public long? CampusId { get; set; }
    public int Status { get; set; } = 1;
    public int ViewCount { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("advertisement")]
public class Advertisement
{
    [Key] public long Id { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Image { get; set; } = string.Empty;
    [MaxLength(500)] public string? LinkUrl { get; set; }
    [MaxLength(50)] public string Position { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Status { get; set; } = 1;
    public int ClickCount { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}