# Server Dashboard

This React application provides a dashboard for monitoring and controlling gRPC server instances. It communicates with backend APIs (ServiceController, MetricsController) to display metrics and manage services.

## Features
- List all configured servers on the homepage
- View real-time metrics for each server
- Start, stop, restart, and check status of services

## Getting Started
1. Install dependencies:
   ```bash
   npm install
   ```
2. Start the development server:
   ```bash
   npm run dev
   ```

## Configuration
- Configure the list of servers in `src/config/servers.js`.
- Backend API endpoints should match those provided by your .NET backend.
