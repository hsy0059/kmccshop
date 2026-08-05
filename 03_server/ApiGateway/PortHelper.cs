using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace ApiGateway;

public static class PortHelper
{
    /// <summary>
    /// 在开发环境下，如果目标端口已被占用，自动终止占用该端口的旧进程。
    /// </summary>
    public static void FreePortIfNeeded(int port, ILogger logger)
    {
        if (!IsDevelopment()) return;

        try
        {
            var listener = IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .FirstOrDefault(e => e.Port == port);

            if (listener == null) return;

            var pid = FindPidByNetstat(port);
            if (pid == 0)
            {
                logger.LogWarning("端口 {Port} 被占用，但无法获取占用进程 PID", port);
                return;
            }

            var currentId = Environment.ProcessId;
            if (pid == currentId) return;

            var process = Process.GetProcessById(pid);
            logger.LogInformation("开发环境端口 {Port} 被进程 {ProcessName}({Pid}) 占用，正在自动释放...", port, process.ProcessName, pid);
            process.Kill();
            process.WaitForExit(TimeSpan.FromSeconds(5));
            logger.LogInformation("进程 {Pid} 已终止，端口 {Port} 已释放", pid, port);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "自动释放端口 {Port} 失败", port);
        }
    }

    private static bool IsDevelopment()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.IsNullOrEmpty(env) || env.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    private static int FindPidByNetstat(int port)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = $"-ano",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            if (proc == null) return 0;
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            // 匹配 TCP  0.0.0.0:53517 ... LISTENING  12345
            var regex = new Regex($@"^\s*TCP\s+\S+:{port}\s+\S+\s+(\w+)\s+(\d+)\s*$", RegexOptions.Multiline);
            var match = regex.Match(output);
            if (match.Success && int.TryParse(match.Groups[2].Value, out var pid))
                return pid;
        }
        catch { }
        return 0;
    }
}
