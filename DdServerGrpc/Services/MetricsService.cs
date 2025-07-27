namespace DdServerGrpc.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

public class MetricsServiceImpl : MetricsService.MetricsServiceBase
{
    private readonly DateTime _startTime = DateTime.UtcNow;

    public override async Task StreamMetrics(MetricsRequest request, IServerStreamWriter<MetricsResponse> responseStream, ServerCallContext context)
    {
        int interval = request.IntervalSeconds > 0 ? request.IntervalSeconds : 1;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var totalCpu = GetTotalCpu();
            var usedCpu = GetCpuUsage();
            var totalRam = GetTotalRam();
            var usedRam = GetUsedRam();
            var totalDisk = GetTotalDisk();
            var usedDisk = GetUsedDisk();
            var uptime = (DateTime.UtcNow - _startTime).TotalSeconds;
            var health = GetHealthCheck();
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await responseStream.WriteAsync(new MetricsResponse
            {
                TotalCpu = totalCpu,
                UsedCpu = usedCpu,
                TotalRam = totalRam,
                UsedRam = usedRam,
                TotalDisk = totalDisk,
                UsedDisk = usedDisk,
                Uptime = uptime,
                HealthCheck = health,
                Timestamp = ts
            });
            await Task.Delay(interval * 1000);
        }
    }

    private double GetTotalCpu() => Environment.ProcessorCount;
    private double GetCpuUsage()
    {
        // Cross-platform CPU usage: average over 1 second
        var startCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
        var startTime = DateTime.UtcNow;
        System.Threading.Thread.Sleep(1000);
        var endCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
        var endTime = DateTime.UtcNow;
        var cpuUsedMs = (endCpuTime - startCpuTime).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;
        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;
        return Math.Round(cpuUsageTotal, 2);
    }
    private double GetTotalRam()
    {
        // Cross-platform total RAM
        return Math.Round(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0), 2); // MB
    }
    private double GetUsedRam()
    {
        // Cross-platform used RAM (approximate)
        return Math.Round(GC.GetTotalMemory(false) / (1024.0 * 1024.0), 2); // MB
    }
    private double GetTotalDisk()
    {
        double total = 0;
        foreach (var drive in System.IO.DriveInfo.GetDrives())
        {
            if (drive.IsReady)
                total += drive.TotalSize;
        }
        return Math.Round(total / (1024.0 * 1024.0 * 1024.0), 2); // GB
    }
    private double GetUsedDisk()
    {
        double used = 0;
        foreach (var drive in System.IO.DriveInfo.GetDrives())
        {
            if (drive.IsReady)
                used += (drive.TotalSize - drive.TotalFreeSpace);
        }
        return Math.Round(used / (1024.0 * 1024.0 * 1024.0), 2); // GB
    }
    private string GetHealthCheck()
    {
        var cpu = GetCpuUsage();
        var ram = GetUsedRam();
        var totalRam = GetTotalRam();
        var disk = GetUsedDisk();
        var totalDisk = GetTotalDisk();
        if (cpu > 90 || ram > totalRam * 0.9 || disk > totalDisk * 0.9)
            return "Unhealthy";
        return "Healthy";
    }
}

