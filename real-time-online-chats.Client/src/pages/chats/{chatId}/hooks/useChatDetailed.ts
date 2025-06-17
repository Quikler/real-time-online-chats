import { ChatLevel, ChatService } from "@src/services/api/ChatService";
import { useState, useEffect } from "react";
import { ChatInfo, MessageChat, UserChat } from "../{chatId}.types";
import { ChatUsersService } from "@src/services/api/ChatUsersService";

const useChatDetailed = (chatId?: string) => {
  const [chatInfo, setChatInfo] = useState<ChatInfo>();
  const [messages, setMessages] = useState<MessageChat[]>([]);
  const [users, setUsers] = useState<UserChat[]>([]);

  useEffect(() => {
    if (!chatId) return;

    const abortChatDetailed = new AbortController();
    const abortJoinChat = new AbortController();

    const joinAndFetchChat = async () => {
      try {
        await ChatUsersService.addMe(chatId, { signal: abortJoinChat.signal });
        console.log("Joined chat:", chatId);

        const data = await ChatService.getChat(chatId, ChatLevel.Detail, {
          signal: abortChatDetailed.signal,
        });

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
      } catch (e: any) {
        console.error("Error joining or fetching the chat:", e.message);
      }
    };

    joinAndFetchChat();

    return () => {
      abortChatDetailed.abort();
      abortJoinChat.abort();
    };
  }, [chatId]);

  return { chatInfo, messages, users, setMessages, setUsers, setChatInfo };
};

export default useChatDetailed;
