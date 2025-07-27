import React, { useState, useEffect } from "react";
import { getBackendServiceUrl } from "../config/servers";
import {
  Typography,
  Divider,
  Grid,
  CircularProgress,
  Button,
  Paper
} from "@mui/material";

export default function Dashboard({ server, onClose }) {
  const [metrics, setMetrics] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!server) return;
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
  }, [server]);

  if (!server) return null;

  return (
    <Paper sx={{ margin: '40px auto', padding: 4, borderRadius: 2, maxWidth: 600, position: 'relative', top: '10vh' }}>
      <Typography variant="h5" gutterBottom>Dashboard for {server.name}</Typography>
      <Button variant="outlined" sx={{ position: 'absolute', top: 16, right: 16 }} onClick={onClose}>Close</Button>
      <Divider sx={{ my: 2 }} />
      {loading && <CircularProgress size={24} sx={{ mt: 1 }} />}
      {error && <Typography color="error">{error}</Typography>}
      {metrics && (
        <>
          <Typography variant="subtitle1">Server Metrics</Typography>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={6}>Total CPU: {metrics.TotalCpu}</Grid>
            <Grid item xs={6}>Used CPU: {metrics.UsedCpu}</Grid>
            <Grid item xs={6}>Total RAM: {metrics.TotalRam} MB</Grid>
            <Grid item xs={6}>Used RAM: {metrics.UsedRam} MB</Grid>
            <Grid item xs={6}>Total Disk: {metrics.TotalDisk} GB</Grid>
            <Grid item xs={6}>Used Disk: {metrics.UsedDisk} GB</Grid>
            <Grid item xs={12}>Timestamp: {metrics.Timestamp}</Grid>
          </Grid>
          <Divider sx={{ my: 2 }} />
          <Typography variant="subtitle1">Service Manager</Typography>
          <Typography color="text.secondary">Service management options coming soon...</Typography>
        </>
      )}
    </Paper>
  );
}
