using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social.Service.Models.Entities;

[Table("post")]
public class Post
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    [Column("category_id")] public long? CategoryId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Images { get; set; }
    [Column("view_count")] public int ViewCount { get; set; }
    [Column("like_count")] public int LikeCount { get; set; }
    [Column("comment_count")] public int CommentCount { get; set; }
    [Column("favorite_count")] public int FavoriteCount { get; set; }
    [Column("is_top")] public int IsTop { get; set; }
    [Column("is_essence")] public int IsEssence { get; set; }
    [Column("is_locked")] public int IsLocked { get; set; }
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
    [Column("post_id")] public long PostId { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("post_comment")]
public class PostComment
{
    [Key] public long Id { get; set; }
    [Column("post_id")] public long PostId { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    [Column("parent_id")] public long? ParentId { get; set; }
    [Column("reply_to_user_id")] public long? ReplyToUserId { get; set; }
    [MaxLength(500)] public string Content { get; set; } = string.Empty;
    [Column("like_count")] public int LikeCount { get; set; }
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

[Table("second_goods")]
public class SecondGoods
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    public string? Images { get; set; }
    public decimal Price { get; set; }
    [Column("original_price")] public decimal? OriginalPrice { get; set; }
    [MaxLength(50)] public string? Category { get; set; }
    [Column("condition_desc")] [MaxLength(50)] public string? ConditionDesc { get; set; }
    [Column("view_count")] public int ViewCount { get; set; }
    [Column("favorite_count")] public int FavoriteCount { get; set; }
    [Column("contact_info")] [MaxLength(100)] public string? ContactInfo { get; set; }
    [Column("campus_id")] public long? CampusId { get; set; }
    public int Status { get; set; } = 1;
    [Column("is_sold")] public int IsSold { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("lost_found")]
public class LostFound
{
    [Key] public long Id { get; set; }
    [Column("user_id")] public long UserId { get; set; }
    public int Type { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    public string? Images { get; set; }
    [MaxLength(100)] public string? Location { get; set; }
    [Column("contact_info")] [MaxLength(100)] public string? ContactInfo { get; set; }
    [MaxLength(50)] public string? Category { get; set; }
    [Column("campus_id")] public long? CampusId { get; set; }
    public int Status { get; set; } = 1;
    [Column("view_count")] public int ViewCount { get; set; }
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
    [Column("link_url")] [MaxLength(500)] public string? LinkUrl { get; set; }
    [MaxLength(50)] public string Position { get; set; } = string.Empty;
    [Column("sort_order")] public int SortOrder { get; set; }
    [Column("start_time")] public DateTime? StartTime { get; set; }
    [Column("end_time")] public DateTime? EndTime { get; set; }
    public int Status { get; set; } = 1;
    [Column("click_count")] public int ClickCount { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}