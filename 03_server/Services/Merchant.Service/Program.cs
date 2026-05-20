using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Merchant.Service.Data;
using Campus.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? Campus.Common.Constants.JwtSecretKey;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? Campus.Common.Constants.JwtIssuer,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? Campus.Common.Constants.JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=campus_platform;User Id=root;Password=root123;";
builder.Services.AddDbContext<MerchantDbContext>(o => o.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));
builder.Services.AddCampusRedis(builder.Configuration.GetConnectionString("Redis") ?? Campus.Common.Constants.RedisConnectionString);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// 从配置或环境读取可监听的 URL，支持以分号分隔多个值，避免硬编码端口冲突
var urls = builder.Configuration["ASPNETCORE_URLS"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrWhiteSpace(urls))
{
    foreach (var u in urls.Split(';', StringSplitOptions.RemoveEmptyEntries))
    {
        app.Urls.Add(u.Trim());
    }
}

// 让 Kestrel 使用默认行为或由宿主/launchSettings 控制
app.Run();
