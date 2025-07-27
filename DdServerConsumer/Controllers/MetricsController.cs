using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using DdServerGrpc;
using Grpc.Core;

namespace DdServerConsumer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    [HttpGet("aggregate")]
    public async Task<IActionResult> AggregateMetrics([FromQuery] string[] serverAddresses)
    {
        var results = new List<object>();
        foreach (var address in serverAddresses)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new MetricsService.MetricsServiceClient(channel);
            var call = client.StreamMetrics(new MetricsRequest { IntervalSeconds = 1 });
            if (await call.ResponseStream.MoveNext())
            {
                var metrics = call.ResponseStream.Current;
                results.Add(new
                {
                    Address = address,
                    metrics.TotalCpu,
                    metrics.UsedCpu,
                    metrics.TotalRam,
                    metrics.UsedRam,
                    metrics.TotalDisk,
                    metrics.UsedDisk,
                    metrics.Uptime,
                    metrics.HealthCheck,
                    metrics.Timestamp
                });
            }
        }
        return Ok(results);
    }
}
