// Loads server configuration from public/servers.json at runtime
export async function fetchConfig() {
  const res = await fetch("/servers.json");
  return await res.json();
}

export async function fetchServers() {
  const config = await fetchConfig();
  return config.SAVED_SERVERS || [];
}

export async function getBackendServiceUrl() {
  const config = await fetchConfig();
  return config.BACKEND_SERVICE;
}

