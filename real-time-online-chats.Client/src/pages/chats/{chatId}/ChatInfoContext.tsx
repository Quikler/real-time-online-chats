import React, { createContext, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import { handleError, useCustomHook } from "@src/utils/helpers";
import { ChatService } from "@src/services/api/ChatService";
import ErrorScreen from "@src/components/ui/ErrorScreen";
import { ChatInfo } from "./{chatId}.types";

type ChatInfoContextType = {
  chatInfo: ChatInfo;
  setChatInfo: React.Dispatch<React.SetStateAction<ChatInfo>>;
};

type Props = { children: React.ReactNode };

const ChatInfoContext = createContext({} as ChatInfoContextType);

export const ChatInfoContextProvider = ({ children }: Props) => {
  console.count("ChatInfoContextProvider render");
  const { chatId } = useParams<{ chatId: string }>();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  const [chatInfo, setChatInfo] = useState<ChatInfo>({} as ChatInfo);

  useEffect(() => {
    if (!chatId) return;
    
    const abort = new AbortController();
    const fetchChatInfo = async () => {
      try {
        const chatInfo = await ChatService.getChatInfo(chatId, { signal: abort.signal });
        setChatInfo(chatInfo);
        setLoading(false)
      } catch (e: any) {
        handleError(e);
        setError(true);
      }
    };

    fetchChatInfo();

    return () => {
      abort.abort();
    }
  }, [chatId]);

  if (error) <ErrorScreen />

  const value = useMemo(() => ({
    chatInfo,
    setChatInfo
  }), [chatInfo]);

  return <ChatInfoContext.Provider value={value}>{loading ? null : children}</ChatInfoContext.Provider>
}

export const useChatInfo = () => useCustomHook(ChatInfoContext, useChatInfo.name);
