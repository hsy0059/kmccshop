using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Campus.Infrastructure;

public static class RedisExtensions
{
    public static IServiceCollection AddCampusRedis(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = ConfigurationOptions.Parse(connectionString);
            config.AbortOnConnectFail = false;
            config.ConnectTimeout = 5000;
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddScoped<RedisService>();
        return services;
    }
}

public class RedisService
{
    private readonly IConnectionMultiplexer _redis;
    private IDatabase Db => _redis.GetDatabase();

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        await Db.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await Db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await Db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await Db.KeyExistsAsync(key);
    }

    public async Task SetHashAsync(string key, string field, string value)
    {
        await Db.HashSetAsync(key, field, value);
    }

    public async Task<string?> GetHashAsync(string key, string field)
    {
        var value = await Db.HashGetAsync(key, field);
        return value.HasValue ? value.ToString() : null;
    }
}
