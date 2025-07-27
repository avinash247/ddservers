# Distributed Server Metrics & Control (.NET 9)

This solution provides a distributed system for monitoring server metrics and controlling services using gRPC and REST APIs. It targets .NET 9 and includes two main projects:

## Projects

### 1. DdServerGrpc
- **Type:** gRPC server (.NET 9)
- **Purpose:** Exposes gRPC endpoints for:
  - Streaming server metrics (CPU, RAM, Disk, Uptime, Health)
  - Controlling system services (start/stop/status) on Windows and Linux
- **Key Endpoints:**
  - `MetricsServiceImpl`: Streams real-time metrics
  - `ServiceControlManagerImpl`: Start/stop/status for system services

### 2. DdServerConsumer
- **Type:** ASP.NET Core Web API (.NET 9)
- **Purpose:** Consumes gRPC endpoints from multiple servers, aggregates metrics, and exposes RESTful APIs for clients.
- **Key Endpoints:**
  - `/api/metrics/aggregate`: Aggregates metrics from multiple gRPC servers
  - `/api/service/start|stop|status`: Controls services on remote servers via gRPC

## Features

- Real-time server metrics streaming via gRPC
- Cross-platform service control (Windows: `sc.exe`, Linux: `systemctl`)
- REST API for easy integration and aggregation
- OpenAPI (Swagger) support for REST endpoints

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 (recommended)
- (Optional) Docker for containerized deployment

### Build & Run

1. **Clone the repository**
2. **Restore dependencies**