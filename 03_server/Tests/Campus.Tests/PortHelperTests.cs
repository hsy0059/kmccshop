using System.Net;
using System.Net.Sockets;
using Campus.Common;
using Microsoft.Extensions.Logging;

namespace Campus.Tests;

public class PortHelperTests : IDisposable
{
    private readonly string _originalEnv;
    private const string EnvKey = "ASPNETCORE_ENVIRONMENT";

    public PortHelperTests()
    {
        _originalEnv = Environment.GetEnvironmentVariable(EnvKey) ?? string.Empty;
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(EnvKey, _originalEnv);
    }

    [Fact]
    public void FreePortIfNeeded_NonDevelopmentEnvironment_DoesNotLogPort()
    {
        // Arrange
        Environment.SetEnvironmentVariable(EnvKey, "Production");
        var logger = new TestLogger<int>();
        var port = GetFreePort();

        // Act
        PortHelper.FreePortIfNeeded(port, logger);

        // Assert
        Assert.DoesNotContain(logger.Logs, l => l.Contains($"当前服务分配端口: {port}"));
    }

    [Fact]
    public void FreePortIfNeeded_DevelopmentAndPortFree_LogsPortAndAvailability()
    {
        // Arrange
        Environment.SetEnvironmentVariable(EnvKey, "Development");
        var logger = new TestLogger<int>();
        var port = GetFreePort();

        // Act
        PortHelper.FreePortIfNeeded(port, logger);

        // Assert
        Assert.Contains(logger.Logs, l => l.Contains($"当前服务分配端口: {port}"));
        Assert.Contains(logger.Logs, l => l.Contains($"端口 {port} 未被占用，可直接使用"));
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("development")]
    [InlineData("")]
    public void FreePortIfNeeded_DevelopmentLikeEnvironment_LogsPort(string envValue)
    {
        // Arrange
        Environment.SetEnvironmentVariable(EnvKey, envValue);
        var logger = new TestLogger<int>();
        var port = GetFreePort();

        // Act
        PortHelper.FreePortIfNeeded(port, logger);

        // Assert
        Assert.Contains(logger.Logs, l => l.Contains($"当前服务分配端口: {port}"));
    }

    [Fact]
    public void FreePortIfNeeded_DevelopmentAndPortOccupiedByCurrentProcess_DoesNotKillSelf()
    {
        // Arrange
        Environment.SetEnvironmentVariable(EnvKey, "Development");
        var logger = new TestLogger<int>();
        var port = GetFreePort();

        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        try
        {
            // Act
            PortHelper.FreePortIfNeeded(port, logger);

            // Assert: current process should not be killed; listener should still accept connections.
            Assert.Contains(logger.Logs, l => l.Contains($"当前服务分配端口: {port}"));
            using var client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            Assert.True(client.Connected);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public void FreePortIfNeeded_DevelopmentAndPortOccupiedOnAllInterfaces_DoesNotKillSelf()
    {
        // Arrange
        Environment.SetEnvironmentVariable(EnvKey, "Development");
        var logger = new TestLogger<int>();
        var port = GetFreePort();

        using var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        try
        {
            // Act
            PortHelper.FreePortIfNeeded(port, logger);

            // Assert: 0.0.0.0 listener should also be detected as current process and not killed.
            Assert.Contains(logger.Logs, l => l.Contains($"当前服务分配端口: {port}"));
            using var client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            Assert.True(client.Connected);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static int GetFreePort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)socket.LocalEndPoint!).Port;
    }

    [Fact]
    public void FreePortIfNeeded_WhenOperationThrows_LogsError()
    {
        // Arrange: 通过 logger 在 Information 级别抛异常，触发外层 catch 的 LogError 分支。
        Environment.SetEnvironmentVariable(EnvKey, "Development");
        var logger = new ThrowingLogger<int>();
        var port = GetFreePort();

        // Act
        PortHelper.FreePortIfNeeded(port, logger);

        // Assert
        Assert.Contains(logger.Logs, l => l.Contains($"自动释放端口 {port} 失败"));
    }

    private class TestLogger<T> : ILogger<T>
    {
        public List<string> Logs { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Logs.Add(formatter(state, exception));
        }
    }

    private class ThrowingLogger<T> : ILogger<T>
    {
        public List<string> Logs { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Information)
                throw new InvalidOperationException("模拟端口释放过程异常");

            Logs.Add(formatter(state, exception));
        }
    }
}
