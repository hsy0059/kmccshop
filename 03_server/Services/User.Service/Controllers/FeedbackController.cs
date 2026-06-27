using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.Service.Data;
using User.Service.Models.DTOs;
using Campus.Common;

namespace User.Service.Controllers;

[ApiController]
[Route("api/v1/user/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly UserDbContext _db;

    public FeedbackController(UserDbContext db)
    {
        _db = db;
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetFeedbackList([FromQuery] PageModel page)
    {
        var query = _db.Set<Feedback>().AsQueryable();

        var total = await query.CountAsync();
        var list = await query.OrderByDescending(f => f.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToListAsync();

        return Ok(ApiResponse<PageResult<Feedback>>.Success(
            PageResult<Feedback>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("submit")]
    [Authorize]
    public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var feedback = new Feedback
        {
            UserId = userId.Value,
            Type = request.Type,
            Title = request.Title,
            Content = request.Content,
            Images = request.Images,
            ContactInfo = request.ContactInfo
        };

        _db.Set<Feedback>().Add(feedback);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<Feedback>.Success(feedback, "提交成功"));
    }

    [HttpPut("{id:long}/reply")]
    [Authorize]
    public async Task<IActionResult> ReplyFeedback(long id, [FromBody] FeedbackReplyRequest request)
    {
        var feedback = await _db.Set<Feedback>().FindAsync(id);
        if (feedback == null) return Ok(ApiResponse.Error(404, "反馈不存在"));

        feedback.ReplyContent = request.ReplyContent;
        feedback.ReplierId = GetUserId();
        feedback.RepliedAt = DateTime.Now;
        feedback.Status = FeedbackStatus.Replied;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("回复成功"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}

[System.ComponentModel.DataAnnotations.Schema.Table("feedback")]
public class Feedback
{
    [System.ComponentModel.DataAnnotations.Key]
    public long Id { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("user_id")]
    public long UserId { get; set; }
    public int Type { get; set; }

    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public string? Images { get; set; }

    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public string? ContactInfo { get; set; }

    public int Status { get; set; } = 1;

    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string? ReplyContent { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.Column("replier_id")]
    public long? ReplierId { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("replied_at")]
    public DateTime? RepliedAt { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [System.ComponentModel.DataAnnotations.Schema.Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
