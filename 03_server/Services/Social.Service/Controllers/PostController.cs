using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Service.Data;
using Social.Service.Models.Entities;
using Campus.Common;

namespace Social.Service.Controllers;

[ApiController]
[Route("api/v1/social/post")]
public class PostController : ControllerBase
{
    private readonly SocialDbContext _db;

    public PostController(SocialDbContext db) { _db = db; }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page, [FromQuery] long? categoryId)
    {
        var query = _db.Posts.Where(p => p.Status == 1).AsQueryable();
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(p => p.Title.Contains(page.Keyword));
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(p => p.IsTop).ThenByDescending(p => p.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Post>>.Success(PageResult<Post>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("detail/{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return Ok(ApiResponse.Error(404, "帖子不存在"));
        post.ViewCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Post>.Success(post));
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Post post)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        post.UserId = userId.Value;
        post.Status = 1;
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Post>.Success(post, "发布成功"));
    }

    [HttpPost("{id:long}/like")]
    [Authorize]
    public async Task<IActionResult> ToggleLike(long id)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        var existing = await _db.PostLikes.FirstOrDefaultAsync(l => l.PostId == id && l.UserId == userId.Value);
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return Ok(ApiResponse.Error(404, "帖子不存在"));
        if (existing != null)
        {
            _db.PostLikes.Remove(existing);
            post.LikeCount = Math.Max(0, post.LikeCount - 1);
        }
        else
        {
            _db.PostLikes.Add(new PostLike { PostId = id, UserId = userId.Value });
            post.LikeCount++;
        }
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success(existing != null ? "已取消点赞" : "点赞成功"));
    }

    [HttpPost("{id:long}/favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(long id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return Ok(ApiResponse.Error(404, "帖子不存在"));
        post.FavoriteCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("收藏成功"));
    }

    [HttpDelete("delete/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return Ok(ApiResponse.Error(404, "帖子不存在"));
        post.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }

    [HttpGet("{id:long}/comments")]
    public async Task<IActionResult> GetComments(long id, [FromQuery] PageModel page)
    {
        var query = _db.PostComments.Where(c => c.PostId == id && c.Status == 1);
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<PostComment>>.Success(PageResult<PostComment>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpPost("{id:long}/comment")]
    [Authorize]
    public async Task<IActionResult> AddComment(long id, [FromBody] PostComment comment)
    {
        var userId = GetUserId(); if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));
        comment.PostId = id;
        comment.UserId = userId.Value;
        _db.PostComments.Add(comment);
        var post = await _db.Posts.FindAsync(id);
        if (post != null) post.CommentCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<PostComment>.Success(comment, "评论成功"));
    }

    [HttpPost("comment/{id:long}/like")]
    [Authorize]
    public async Task<IActionResult> LikeComment(long id)
    {
        var comment = await _db.PostComments.FindAsync(id);
        if (comment == null) return Ok(ApiResponse.Error(404, "评论不存在"));
        comment.LikeCount++;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("点赞成功"));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}