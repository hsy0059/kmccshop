using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.Service.Data;
using User.Service.Models.DTOs;
using User.Service.Models.Entities;
using Campus.Common;

namespace User.Service.Controllers;

[ApiController]
[Route("api/v1/address")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly UserDbContext _db;

    public AddressController(UserDbContext db)
    {
        _db = db;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAddressList()
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var list = await _db.UserAddresses
            .Where(a => a.UserId == userId.Value)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(ApiResponse<List<UserAddress>>.Success(list));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAddress([FromBody] UpdateAddressRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        if (request.IsDefault == 1)
        {
            var existingDefaults = await _db.UserAddresses
                .Where(a => a.UserId == userId.Value && a.IsDefault == 1)
                .ToListAsync();
            foreach (var addr in existingDefaults)
                addr.IsDefault = 0;
        }

        var address = new UserAddress
        {
            UserId = userId.Value,
            ContactName = request.ContactName,
            ContactPhone = request.ContactPhone,
            Province = request.Province,
            City = request.City,
            District = request.District,
            Detail = request.Detail,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            IsDefault = request.IsDefault,
            Tag = request.Tag
        };

        _db.UserAddresses.Add(address);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<UserAddress>.Success(address, "添加成功"));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAddress(long id, [FromBody] UpdateAddressRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var address = await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value);
        if (address == null) return Ok(ApiResponse.Error(404, "地址不存在"));

        if (request.IsDefault == 1)
        {
            var existingDefaults = await _db.UserAddresses
                .Where(a => a.UserId == userId.Value && a.IsDefault == 1 && a.Id != id)
                .ToListAsync();
            foreach (var addr in existingDefaults)
                addr.IsDefault = 0;
        }

        address.ContactName = request.ContactName;
        address.ContactPhone = request.ContactPhone;
        address.Province = request.Province;
        address.City = request.City;
        address.District = request.District;
        address.Detail = request.Detail;
        address.Longitude = request.Longitude;
        address.Latitude = request.Latitude;
        address.IsDefault = request.IsDefault;
        address.Tag = request.Tag;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<UserAddress>.Success(address, "更新成功"));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAddress(long id)
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var address = await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value);
        if (address == null) return Ok(ApiResponse.Error(404, "地址不存在"));

        _db.UserAddresses.Remove(address);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse.Success("删除成功"));
    }

    [HttpGet("default")]
    public async Task<IActionResult> GetDefaultAddress()
    {
        var userId = GetUserId();
        if (userId == null) return Ok(ApiResponse.Error(401, "未登录"));

        var address = await _db.UserAddresses
            .FirstOrDefaultAsync(a => a.UserId == userId.Value && a.IsDefault == 1);

        if (address == null)
            address = await _db.UserAddresses
                .FirstOrDefaultAsync(a => a.UserId == userId.Value);

        return Ok(ApiResponse<UserAddress?>.Success(address));
    }

    private long? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !long.TryParse(claim, out var id)) return null;
        return id;
    }
}
