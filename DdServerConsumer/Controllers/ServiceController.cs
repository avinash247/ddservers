using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using DdServerGrpc; // Ensure this namespace matches your gRPC service definitions
namespace DdServerConsumer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceController : ControllerBase
{
    [HttpPost("start")]
    public async Task<IActionResult> StartService([FromQuery] string serverAddress, [FromQuery] string serviceName)
    {
        using var channel = GrpcChannel.ForAddress(serverAddress);
        var client = new ServiceControlManager.ServiceControlManagerClient(channel);
        var response = await client.StartServiceAsync(new ServiceRequest { ServiceName = serviceName });
        return Ok(response);
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopService([FromQuery] string serverAddress, [FromQuery] string serviceName)
    {
        using var channel = GrpcChannel.ForAddress(serverAddress);
        var client = new ServiceControlManager.ServiceControlManagerClient(channel);
        var response = await client.StopServiceAsync(new ServiceRequest { ServiceName = serviceName });
        return Ok(response);
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetServiceStatus([FromQuery] string serverAddress, [FromQuery] string serviceName)
    {
        using var channel = GrpcChannel.ForAddress(serverAddress);
        var client = new ServiceControlManager.ServiceControlManagerClient(channel);
        var response = await client.GetServiceStatusAsync(new ServiceRequest { ServiceName = serviceName });
        return Ok(response);
    }
}
