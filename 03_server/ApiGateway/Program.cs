using Campus.Common;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var ocelotConfig = Environment.GetEnvironmentVariable("OCELOT_CONFIG") ?? "ocelot.json";
builder.Configuration.AddJsonFile(ocelotConfig, optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();
PortHelper.FreePortIfNeeded(53517, logger);
PortHelper.FreePortIfNeeded(53514, logger);

var app = builder.Build();

app.UseCors();
app.UseAuthorization();

app.Urls.Add("http://0.0.0.0:53517");
// 仅本地开发环境启用 HTTPS（容器内无开发证书）
if (!Environment.GetEnvironmentVariables().Contains("DOTNET_RUNNING_IN_CONTAINER"))
{
    app.Urls.Add("https://0.0.0.0:53514");
}

await app.UseOcelot();

app.Run();