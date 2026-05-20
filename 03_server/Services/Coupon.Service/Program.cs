using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Coupon.Service.Data;
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
builder.Services.AddDbContext<CouponDbContext>(o => o.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));
builder.Services.AddCampusRedis(builder.Configuration.GetConnectionString("Redis") ?? Campus.Common.Constants.RedisConnectionString);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://0.0.0.0:53225");
app.Run();