using Microsoft.AspNetCore.Mvc;
using Social.Service.Controllers;
using Social.Service.Data;
using Social.Service.Models.Entities;
using Campus.Common;

namespace Campus.Tests;

/// <summary>
/// Social controller permission tests: Secondhand/LostFound delete ownership checks,
/// PostComment Status initialization.
/// </summary>
public class SocialControllerTests : IDisposable
{
    private readonly SocialDbContext _db;

    public SocialControllerTests()
    {
        _db = TestHelper.CreateSocialContext();
    }

    public void Dispose() => _db.Dispose();

    // ── Secondhand Delete Tests ──

    [Fact]
    public async Task DeleteSecondhand_OwnGoods_ReturnsSuccess()
    {
        var goods = new SecondGoods
        {
            UserId = TestHelper.UserAId, Title = "test item", Price = 10m, Status = 1
        };
        _db.SecondGoods.Add(goods);
        await _db.SaveChangesAsync();

        var controller = new SecondhandController(_db);
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await controller.Delete(goods.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("删除成功", msg);
    }

    [Fact]
    public async Task DeleteSecondhand_OtherUserGoods_Returns403()
    {
        var goods = new SecondGoods
        {
            UserId = TestHelper.UserAId, Title = "test item", Price = 10m, Status = 1
        };
        _db.SecondGoods.Add(goods);
        await _db.SaveChangesAsync();

        var controller = new SecondhandController(_db);
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await controller.Delete(goods.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    // ── LostFound Delete Tests ──

    [Fact]
    public async Task DeleteLostFound_OwnItem_ReturnsSuccess()
    {
        var lf = new LostFound
        {
            UserId = TestHelper.UserAId, Type = 1, Title = "test", Description = "desc", Status = 1
        };
        _db.LostFounds.Add(lf);
        await _db.SaveChangesAsync();

        var controller = new LostFoundController(_db);
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var result = await controller.Delete(lf.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(0, code);
        Assert.Equal("删除成功", msg);
    }

    [Fact]
    public async Task DeleteLostFound_OtherUserItem_Returns403()
    {
        var lf = new LostFound
        {
            UserId = TestHelper.UserAId, Type = 1, Title = "test", Description = "desc", Status = 1
        };
        _db.LostFounds.Add(lf);
        await _db.SaveChangesAsync();

        var controller = new LostFoundController(_db);
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserBId));

        var result = await controller.Delete(lf.Id);
        var (code, msg) = TestHelper.GetApiResult(result);

        Assert.Equal(403, code);
        Assert.Contains("无权", msg);
    }

    // ── PostComment Status Initialization Test ──

    [Fact]
    public async Task AddComment_SetsStatusTo1()
    {
        var post = new Post
        {
            UserId = TestHelper.UserAId, Title = "test post", Content = "content",
            Status = 1, CategoryId = 1
        };
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        var controller = new PostController(_db, TestHelper.GetNullLogger<PostController>());
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        var comment = new PostComment { Content = "test comment" };
        var result = await controller.AddComment(post.Id, comment);
        var (code, _, data) = TestHelper.GetApiResult<PostComment>(result);

        Assert.Equal(0, code);
        Assert.Equal(1, data!.Status);
    }

    [Fact]
    public async Task AddComment_WithStatus0_OverridesTo1()
    {
        var post = new Post
        {
            UserId = TestHelper.UserAId, Title = "test post2", Content = "content",
            Status = 1, CategoryId = 1
        };
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        var controller = new PostController(_db, TestHelper.GetNullLogger<PostController>());
        controller.ControllerContext = TestHelper.CreateControllerContext(
            TestHelper.CreateStudent(TestHelper.UserAId));

        // Client tries to send Status = 0
        var comment = new PostComment { Content = "malicious", Status = 0 };
        var result = await controller.AddComment(post.Id, comment);
        var (code, _, data) = TestHelper.GetApiResult<PostComment>(result);

        Assert.Equal(0, code);
        Assert.Equal(1, data!.Status);  // Server should override to 1
    }
}
