using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Merchant.Service.Data;
using Merchant.Service.Models.DTOs;
using Merchant.Service.Models.Entities;
using Campus.Common;

namespace Merchant.Service.Controllers;

[ApiController]
[Route("api/v1/product")]
public class ProductController : ControllerBase
{
    private readonly MerchantDbContext _db;

    public ProductController(MerchantDbContext db) { _db = db; }

    [HttpGet("list/{merchantId:long}")]
    public async Task<IActionResult> GetList(long merchantId, [FromQuery] PageModel page)
    {
        var query = _db.Products.Where(p => p.MerchantId == merchantId && p.Status == 1);
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(p => p.Name.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(p => p.SortOrder).ThenByDescending(p => p.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Product>>.Success(PageResult<Product>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("detail/{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        return Ok(ApiResponse<Product>.Success(product));
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);
        var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.UserId == userId && m.Status == MerchantStatus.Open);
        if (merchant == null) return Ok(ApiResponse.Error(403, "商家不存在或未审核通过"));

        var product = new Product
        {
            MerchantId = merchant.Id, CategoryId = request.CategoryId,
            Name = request.Name, Description = request.Description,
            Image = request.Image, Images = request.Images,
            Price = request.Price, DiscountPrice = request.DiscountPrice,
            Stock = request.Stock, Unit = request.Unit ?? "份",
            IsRecommend = request.IsRecommend
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Product>.Success(product, "创建成功"));
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] ProductUpdateRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        if (request.Name != null) product.Name = request.Name;
        if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId;
        if (request.Description != null) product.Description = request.Description;
        if (request.Image != null) product.Image = request.Image;
        if (request.Images != null) product.Images = request.Images;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.DiscountPrice.HasValue) product.DiscountPrice = request.DiscountPrice;
        if (request.Stock.HasValue) product.Stock = request.Stock.Value;
        if (request.Unit != null) product.Unit = request.Unit;
        if (request.IsRecommend.HasValue) product.IsRecommend = request.IsRecommend.Value;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Product>.Success(product, "更新成功"));
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        product.Status = 0;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }

    [HttpPut("{id:long}/status")]
    [Authorize]
    public async Task<IActionResult> ToggleStatus(long id, [FromBody] ProductStatusRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return Ok(ApiResponse.Error(404, "商品不存在"));
        product.Status = request.Status;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success("状态更新成功"));
    }
}