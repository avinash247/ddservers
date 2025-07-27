
import React, { useState, useEffect } from "react";
import { fetchServers, getBackendServiceUrl } from "./config/servers";
import {
  Card,
  CardContent,
  Typography,
  Button,
  CircularProgress,
  Box,
  Grid,
  Divider
} from "@mui/material";
import HealthAndSafetyIcon from '@mui/icons-material/HealthAndSafety';
import Dashboard from "./ServerDashboard/Dashboard";
import { BrowserRouter as Router, Routes, Route, useNavigate, useParams, Link } from "react-router-dom";

function ServerCard({ server }) {
  const navigate = useNavigate();
  const [metrics, setMetrics] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    async function fetchMetrics() {
      setLoading(true);
      setError("");
      try {
        const backendUrl = await getBackendServiceUrl();
        const res = await fetch(`${backendUrl}/api/metrics/aggregate?serverAddresses=${encodeURIComponent(server.name)}`);
        const data = await res.json();
        setMetrics(data[0]);
      } catch {
        setError("Failed to fetch metrics");
      }
      setLoading(false);
    }
    fetchMetrics();
  }, [server.name]);

  return (
    <Card sx={{ mb: 3, borderRadius: 2, boxShadow: 2 }}>
      <CardContent>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={8}>
            <Typography variant="h6">{server.name}</Typography>
            <Typography variant="body2" color="text.secondary">Address: {server.address}</Typography>
            {metrics && <Typography variant="body2">Uptime: {metrics.Uptime} seconds</Typography>}
          </Grid>
          <Grid item xs={2}>
            <Box display="flex" alignItems="center">
              <HealthAndSafetyIcon color={metrics?.HealthCheck === "Healthy" ? "success" : "error"} />
              <Typography sx={{ ml: 1 }} fontWeight="bold">
                {metrics ? metrics.HealthCheck : "-"}
              </Typography>
            </Box>
          </Grid>
          <Grid item xs={2}>
            <Button variant="contained" onClick={() => navigate(`/dashboard/${encodeURIComponent(server.name)}`)}>View Details</Button>
          </Grid>
        </Grid>
        <Divider sx={{ my: 2 }} />
        <Typography variant="subtitle1">Server Metrics</Typography>
        {loading && <CircularProgress size={24} sx={{ mt: 1 }} />}
        {error && <Typography color="error">{error}</Typography>}
        {metrics && (
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={4}>Total CPU: {metrics.TotalCpu}</Grid>
            <Grid item xs={4}>Used CPU: {metrics.UsedCpu}</Grid>
            <Grid item xs={4}>Total RAM: {metrics.TotalRam} MB</Grid>
            <Grid item xs={4}>Used RAM: {metrics.UsedRam} MB</Grid>
            <Grid item xs={4}>Total Disk: {metrics.TotalDisk} GB</Grid>
            <Grid item xs={4}>Used Disk: {metrics.UsedDisk} GB</Grid>
            <Grid item xs={12}>Timestamp: {metrics.Timestamp}</Grid>
          </Grid>
        )}
      </CardContent>
    </Card>
  );
}

function HomePage() {
  const [servers, setServers] = useState([]);
  const [loadingServers, setLoadingServers] = useState(true);
  const [errorServers, setErrorServers] = useState("");

  useEffect(() => {
    fetchServers()
      .then(setServers)
      .catch(() => setErrorServers("Failed to load server configuration"))
      .finally(() => setLoadingServers(false));
  }, []);

  return (
    <div style={{ padding: 32 }}>
      <Typography variant="h4" gutterBottom>Server Dashboard</Typography>
      {loadingServers && <p>Loading server configuration...</p>}
      {errorServers && <p style={{ color: "red" }}>{errorServers}</p>}
      <div>
        {servers.map((server) => (
          <ServerCard key={server.address} server={server} />
        ))}
      </div>
    </div>
  );
}

function DashboardPage() {
  const { serverName } = useParams();
  const [server, setServer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchServers()
      .then((servers) => {
        const found = servers.find(s => s.name === serverName);
        if (found) setServer(found);
        else setError("Server not found");
      })
      .finally(() => setLoading(false));
  }, [serverName]);

  if (loading) return <p>Loading server details...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;
  if (!server) return <p>Server not found</p>;

  return (
    <div style={{ padding: 32 }}>
      <Link to="/">&#8592; Back to Home</Link>
      <Dashboard server={server} onClose={() => {}} />
    </div>
  );
}

export default function AppRouter() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/dashboard/:serverName" element={<DashboardPage />} />
      </Routes>
    </Router>
  );
}
