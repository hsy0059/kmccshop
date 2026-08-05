using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Campus.Common;

namespace User.Service.Controllers;

[ApiController]
[Route("api/v1/file")]
public class FileController : ControllerBase
{
    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Ok(ApiResponse.Error(400, "请选择文件"));

        if (file.Length > Constants.MaxFileSize)
            return Ok(ApiResponse.Error(400, "文件大小不能超过10MB"));

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        var filePath = Path.Combine(uploadDir, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/{fileName}";
        return Ok(ApiResponse<object>.Success(new { url, fileName }, "上传成功"));
    }
}
