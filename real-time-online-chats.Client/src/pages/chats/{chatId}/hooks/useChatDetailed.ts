import { ChatService } from "@src/services/api/ChatService";
import { useState, useEffect } from "react";
import { ChatInfo, MessageChat, UserChat } from "../{chatId}.types";

const useChatDetailed = (chatId?: string) => {
  const [chatInfo, setChatInfo] = useState<ChatInfo>();
  const [messages, setMessages] = useState<MessageChat[]>([]);
  const [users, setUsers] = useState<UserChat[]>([]);

  useEffect(() => {
    if (!chatId) return;

    const abortController = new AbortController();

    ChatService.getChatDetailed(chatId, { signal: abortController.signal })
      .then((data) => {
        if (data) {
          setChatInfo({
            id: data.id,
            title: data.title,
            ownerId: data.ownerId,
            creationTime: data.creationTime,
          });
          setMessages(data.messages);
          setUsers(data.users);
        }
      })
      .catch((e) => console.error("Error fetching chat data:", e.message));

    return () => abortController.abort();
  }, [chatId]);

  return { chatInfo, messages, users, setMessages, setUsers, setChatInfo };
};

export default useChatDetailed;
