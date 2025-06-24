import React, { createContext, useMemo, useState } from "react";
import { ChatMessage } from "./{chatId}.types";
import { useCustomHook } from "@src/utils/helpers";

type MessageContextType = {
  message: string;
  setMessage: React.Dispatch<React.SetStateAction<string>>;
  editableMessage: ChatMessage | null | undefined;
  setEditableMessage: React.Dispatch<React.SetStateAction<ChatMessage | null | undefined>>;
};

type MessageProviderProps = { children: React.ReactNode };

const MessageContext = createContext({} as MessageContextType);

export const MessageContextProvider = ({ children }: MessageProviderProps) => {
  console.count("MessageContextProvider render")
  const [message, setMessage] = useState<string>("");
  const [editableMessage, setEditableMessage] = useState<ChatMessage | null | undefined>();

  const value = useMemo(
    () => ({
      message,
      setMessage,
      editableMessage,
      setEditableMessage,
    }),
    [message, editableMessage]
  );

  return <MessageContext.Provider value={value}>{children}</MessageContext.Provider>;
};

export const useMessage = () => useCustomHook(MessageContext, useMessage.name);
