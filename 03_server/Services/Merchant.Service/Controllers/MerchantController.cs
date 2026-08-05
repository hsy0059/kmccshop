using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Merchant.Service.Data;
using Merchant.Service.Models.DTOs;
using Merchant.Service.Models.Entities;
using Campus.Common;

namespace Merchant.Service.Controllers;

[ApiController]
[Route("api/v1/merchant")]
public class MerchantController : ControllerBase
{
    private readonly MerchantDbContext _db;
    private readonly ILogger<MerchantController> _logger;

    public MerchantController(MerchantDbContext db, ILogger<MerchantController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] PageModel page, [FromQuery] int? status = null)
    {
        var query = _db.Merchants.AsQueryable();
        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);
        if (!string.IsNullOrEmpty(page.Keyword))
            query = query.Where(m => m.Name.Contains(page.Keyword));
        var total = await query.CountAsync();
        var list = await query.OrderByDescending(m => m.CreatedAt)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
        return Ok(ApiResponse<PageResult<Models.Entities.Merchant>>.Success(PageResult<Models.Entities.Merchant>.Of(list, total, page.Page, page.PageSize)));
    }

    [HttpGet("detail/{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var merchant = await _db.Merchants.FindAsync(id);
        if (merchant == null) return Ok(ApiResponse.Error(404, "商家不存在"));
        return Ok(ApiResponse<Models.Entities.Merchant>.Success(merchant));
    }

    [HttpPost("apply/send-sms")]
    [AllowAnonymous]
    public async Task<IActionResult> SendApplySmsCode([FromBody] SendApplySmsCodeRequest request)
    {
        // 入口日志：手机号脱敏
        var maskedPhone = !string.IsNullOrEmpty(request.Phone) && request.Phone.Length >= 7
            ? request.Phone[..3] + "****" + request.Phone[^4..]
            : "***";
        _logger.LogInformation("SendApplySmsCode 入口: phone={MaskedPhone}", maskedPhone);

        // 短信发送预留接口：接入真实短信服务前，默认生成 6 位验证码并记录日志
        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        _logger.LogInformation("SendApplySmsCode 验证码已生成: phone={MaskedPhone}, code={Code}", maskedPhone, code);

        // TODO: 接入短信服务商（阿里云/腾讯云等），调用 SendSmsAsync(request.Phone, code)

        // 环境判断：生产环境不返回验证码，仅返回发送成功提示；开发环境返回验证码便于联调
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "(unset)";
        var isDev = env == "Development";
        _logger.LogInformation("SendApplySmsCode 环境判断: env={Env}, isDev={IsDev}, willReturnCode={WillReturnCode}",
            env, isDev, isDev);

        if (isDev)
        {
            _logger.LogWarning("SendApplySmsCode [开发环境] 响应中包含验证码: phone={MaskedPhone}, code={Code}", maskedPhone, code);
            return Ok(ApiResponse<object>.Success(new { code }, "验证码已发送"));
        }

        _logger.LogInformation("SendApplySmsCode [生产环境] 响应中不包含验证码: phone={MaskedPhone}", maskedPhone);
        return Ok(ApiResponse<object>.Success(new { }, "验证码已发送"));
    }

    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply([FromBody] ApplyMerchantRequest request)
    {
        // 入口日志：记录申请关键信息（脱敏电话）
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var maskedPhone = request.Phone != null && request.Phone.Length >= 7
            ? request.Phone[..3] + "****" + request.Phone[^4..]
            : "***";
        _logger.LogInformation(
            "Apply 入口: userId={UserId}, name={Name}, phone={MaskedPhone}, category={Category}, agreedTerms={AgreedTerms}",
            userIdClaim ?? "(null)", request.Name, maskedPhone, request.BusinessCategory, request.AgreedTerms);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("Apply 失败: 未登录 (userIdClaim 为空), name={Name}", request.Name);
            return Ok(ApiResponse.Error(401, "未登录"));
        }
        var userId = long.Parse(userIdClaim);

        // 协议必须勾选
        if (!request.AgreedTerms)
        {
            _logger.LogWarning("Apply 失败: 未同意入驻协议, userId={UserId}", userId);
            return Ok(ApiResponse.Error(400, "请先同意入驻协议"));
        }

        // 重复申请检查
        var exists = await _db.Merchants.AnyAsync(m => m.UserId == userId);
        _logger.LogInformation("Apply 重复检查: userId={UserId}, exists={Exists}", userId, exists);
        if (exists)
        {
            _logger.LogWarning("Apply 失败: 用户已提交过商家申请, userId={UserId}", userId);
            return Ok(ApiResponse.Error(400, "已提交过商家申请"));
        }

        // 构造实体并入库
        var merchant = new Models.Entities.Merchant
        {
            UserId = userId,
            Name = request.Name,
            Phone = request.Phone,
            ContactName = request.ContactName,
            SmsCode = request.SmsCode,
            EnterpriseName = request.EnterpriseName,
            CreditCode = request.CreditCode,
            LegalPerson = request.LegalPerson,
            BusinessCategory = request.BusinessCategory,
            BusinessScope = request.BusinessScope,
            LicenseImage = request.LicenseImage,
            IdCardFront = request.IdCardFront,
            IdCardBack = request.IdCardBack,
            Logo = request.Logo,
            Description = request.Description,
            Address = request.Address,
            BusinessHours = request.BusinessHours,
            MinDeliveryAmount = request.MinDeliveryAmount,
            DeliveryFee = request.DeliveryFee,
            CampusId = request.CampusId,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            AgreedTerms = request.AgreedTerms,
            SubmitStep = request.SubmitStep,
            Status = MerchantStatus.Pending
        };
        _db.Merchants.Add(merchant);
        try
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation(
                "Apply 成功: merchantId={MerchantId}, userId={UserId}, name={Name}, status={Status}",
                merchant.Id, userId, merchant.Name, merchant.Status);
            return Ok(ApiResponse<Models.Entities.Merchant>.Success(merchant, "申请已提交"));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Apply 数据库写入异常: userId={UserId}, name={Name}, innerMessage={InnerMessage}",
                userId, request.Name, ex.InnerException?.Message ?? "(none)");
            return Ok(ApiResponse.Error(500, "申请提交失败，请稍后重试"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply 未预期异常: userId={UserId}, name={Name}", userId, request.Name);
            return Ok(ApiResponse.Error(500, "系统异常，请稍后重试"));
        }
    }

    [HttpPost("admin/audit/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Audit(long id, [FromBody] AuditMerchantRequest request)
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权审核商家"));
        var merchant = await _db.Merchants.FindAsync(id);
        if (merchant == null) return Ok(ApiResponse.Error(404, "商家不存在"));
        if (request.Status != MerchantStatus.Open && request.Status != MerchantStatus.Rest
            && request.Status != MerchantStatus.Disabled)
            return Ok(ApiResponse.Error(400, "无效的审核状态"));

        merchant.Status = request.Status;

        var statusMsg = request.Status switch
        {
            MerchantStatus.Open => "审核通过",
            MerchantStatus.Rest => "设为休息",
            MerchantStatus.Disabled => "已禁用",
            _ => "状态已更新"
        };
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Success(statusMsg));
    }

    [HttpPut("admin/update/{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateMerchantRequest request)
    {
        if (!User.IsAdmin() && !User.IsMerchant()) return Ok(ApiResponse.Error(403, "无权修改商家信息"));
        var merchant = await _db.Merchants.FindAsync(id);
        if (merchant == null) return Ok(ApiResponse.Error(404, "商家不存在"));
        if (!User.IsAdmin())
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || long.Parse(userIdClaim) != merchant.UserId)
                return Ok(ApiResponse.Error(403, "无权修改他人商家信息"));
        }
        if (request.Name != null) merchant.Name = request.Name;
        if (request.Logo != null) merchant.Logo = request.Logo;
        if (request.CoverImage != null) merchant.CoverImage = request.CoverImage;
        if (request.Phone != null) merchant.Phone = request.Phone;
        if (request.Description != null) merchant.Description = request.Description;
        if (request.Address != null) merchant.Address = request.Address;
        if (request.BusinessHours != null) merchant.BusinessHours = request.BusinessHours;
        if (request.MinDeliveryAmount.HasValue) merchant.MinDeliveryAmount = request.MinDeliveryAmount.Value;
        if (request.DeliveryFee.HasValue) merchant.DeliveryFee = request.DeliveryFee.Value;
        if (request.CampusId.HasValue) merchant.CampusId = request.CampusId.Value;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<Models.Entities.Merchant>.Success(merchant, "更新成功"));
    }

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);
        var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
        if (merchant == null) return Ok(ApiResponse.Error(404, "未找到商家"));
        var productCount = await _db.Products.CountAsync(p => p.MerchantId == merchant.Id);
        return Ok(ApiResponse<object>.Success(new { merchant, productCount }));
    }

    [HttpGet("stats")]
    [Authorize]
    public async Task<IActionResult> GetStats()
    {
        if (!User.IsAdmin()) return Ok(ApiResponse.Error(403, "无权查看统计数据"));
        var totalMerchants = await _db.Merchants.CountAsync(m => m.Status == MerchantStatus.Open);
        var pendingCount = await _db.Merchants.CountAsync(m => m.Status == MerchantStatus.Pending);
        return Ok(ApiResponse<object>.Success(new { totalMerchants, pendingCount }));
    }

    [HttpGet("my-stats")]
    [Authorize]
    public async Task<IActionResult> GetMyStats()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Ok(ApiResponse.Error(401, "未登录"));
        var userId = long.Parse(userIdClaim);
        var merchant = await _db.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
        if (merchant == null) return Ok(ApiResponse.Error(404, "未找到商家"));
        var productCount = await _db.Products.CountAsync(p => p.MerchantId == merchant.Id);
        return Ok(ApiResponse<object>.Success(new { productCount, rating = merchant.Rating, merchantId = merchant.Id, merchantName = merchant.Name }));
    }
}