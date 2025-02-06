import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import Environment from "@src/environment/environment";
import { useState, useEffect } from "react";

const useMessageHubConnection = (chatId?: string) => {
  const [connection, setConnection] = useState<HubConnection>();

  // Initialize the connection to messageHub
  useEffect(
    () => {
      const newConnection = new HubConnectionBuilder()
        .withUrl(`${Environment.rootApiUrl}/messageHub`, {
          //accessTokenFactory: () => token,
        })
        .withStatefulReconnect()
        .withAutomaticReconnect()
        .build();

      setConnection(newConnection);

      return () => {
        newConnection
          .stop()
          .catch((e) => console.error("[SignalR]: Error stopping connection:", e.message));
      };
    },
    [
      /*token*/
    ]
  );

  useEffect(() => {
    if (!connection) return;

    const startConnection = async () => {
      try {
        await connection.start();
        console.log("[SignalR]: Connection started");

        if (chatId) {
          await connection.invoke("JoinChatGroup", chatId);
          console.log(`${chatId} - Joined chat group`);
        }
      } catch (e: any) {
        console.error("[SignalR]: Error starting connection:", e.message);
      }
    };

    startConnection();

    connection.onreconnected(async () => {
      console.log("[SignalR]: Connection reestablished");
      if (chatId) {
        await connection.invoke("JoinChatGroup", chatId);
        console.log(`${chatId} - Rejoined chat group`);
      }
    });

    return () => {
      connection
        .stop()
        .catch((e) => console.error("[SignalR]: Error stopping connection", e.message));
    };
  }, [connection, chatId]);

  return connection;
};

export default useMessageHubConnection;