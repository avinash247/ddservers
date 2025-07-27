namespace DdServerGrpc.Services;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class ServiceControlManagerImpl : ServiceControlManager.ServiceControlManagerBase
{
   public override Task<ServiceResponse> RestartService(ServiceRequest request, ServerCallContext context)
    {
        bool success = false;
        string message = "";
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Stop then start using sc.exe
                var stopPsi = new ProcessStartInfo("sc.exe", $"stop {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var stopProc = Process.Start(stopPsi);
                stopProc?.WaitForExit();
                var startPsi = new ProcessStartInfo("sc.exe", $"start {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var startProc = Process.Start(startPsi);
                startProc?.WaitForExit();
                success = (stopProc?.ExitCode == 0) && (startProc?.ExitCode == 0);
                message = $"Stopped: {stopProc?.StandardOutput.ReadToEnd()}\nStarted: {startProc?.StandardOutput.ReadToEnd()}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Stop then start using systemctl
                var stopPsi = new ProcessStartInfo("systemctl", $"stop {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var stopProc = Process.Start(stopPsi);
                stopProc?.WaitForExit();
                var startPsi = new ProcessStartInfo("systemctl", $"start {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var startProc = Process.Start(startPsi);
                startProc?.WaitForExit();
                success = (stopProc?.ExitCode == 0) && (startProc?.ExitCode == 0);
                message = $"Stopped: {stopProc?.StandardOutput.ReadToEnd()}\nStarted: {startProc?.StandardOutput.ReadToEnd()}";
            }
            else
            {
                message = "Unsupported OS";
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        return Task.FromResult(new ServiceResponse { Success = success, Message = message });
    }

    public override Task<ServiceResponse> StartService(ServiceRequest request, ServerCallContext context)
    {
        bool success = false;
        string message = "";
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var psi = new ProcessStartInfo("sc.exe", $"start {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                success = proc.ExitCode == 0;
                message = proc.StandardOutput.ReadToEnd();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var psi = new ProcessStartInfo("systemctl", $"start {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                success = proc.ExitCode == 0;
                message = proc.StandardOutput.ReadToEnd();
            }
            else
            {
                message = "Unsupported OS";
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        return Task.FromResult(new ServiceResponse { Success = success, Message = message });
    }

    public override Task<ServiceResponse> StopService(ServiceRequest request, ServerCallContext context)
    {
        bool success = false;
        string message = "";
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var psi = new ProcessStartInfo("sc.exe", $"stop {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                success = proc.ExitCode == 0;
                message = proc.StandardOutput.ReadToEnd();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var psi = new ProcessStartInfo("systemctl", $"stop {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                success = proc.ExitCode == 0;
                message = proc.StandardOutput.ReadToEnd();
            }
            else
            {
                message = "Unsupported OS";
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        return Task.FromResult(new ServiceResponse { Success = success, Message = message });
    }

    public override Task<ServiceStatusResponse> GetServiceStatus(ServiceRequest request, ServerCallContext context)
    {
        string status = "unknown";
        string message = "";
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var psi = new ProcessStartInfo("sc.exe", $"query {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                message = proc.StandardOutput.ReadToEnd();
                if (message.Contains("RUNNING")) status = "running";
                else if (message.Contains("STOPPED")) status = "stopped";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var psi = new ProcessStartInfo("systemctl", $"is-active {request.ServiceName}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                var output = proc.StandardOutput.ReadToEnd().Trim();
                status = output == "active" ? "running" : "stopped";
                message = output;
            }
            else
            {
                message = "Unsupported OS";
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        return Task.FromResult(new ServiceStatusResponse { ServiceName = request.ServiceName, Status = status, Message = message });
    }
}
