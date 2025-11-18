import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5000/hubs/ticket';

let connection = null;

export const createSignalRConnection = () => {
  const token = localStorage.getItem('token');

  connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: () => token,
      skipNegotiation: false,
      transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

  return connection;
};

export const startConnection = async () => {
  if (!connection) {
    connection = createSignalRConnection();
  }

  if (connection.state === signalR.HubConnectionState.Disconnected) {
    try {
      await connection.start();
      console.log('SignalR Connected');
      return connection;
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      // Retry after 5 seconds
      setTimeout(startConnection, 5000);
      throw err;
    }
  }

  return connection;
};

export const stopConnection = async () => {
  if (connection && connection.state !== signalR.HubConnectionState.Disconnected) {
    try {
      await connection.stop();
      console.log('SignalR Disconnected');
    } catch (err) {
      console.error('SignalR Disconnection Error:', err);
    }
  }
};

export const getConnection = () => connection;

export default {
  createSignalRConnection,
  startConnection,
  stopConnection,
  getConnection
};
