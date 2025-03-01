import React, { createContext, useContext, useMemo, useState } from "react";
import { MessageChat } from "./{chatId}.types";

type MessagesContextType = {
  message: string;
  setMessage: React.Dispatch<React.SetStateAction<string>>;
  editableMessage: MessageChat | null | undefined;
  setEditableMessage: React.Dispatch<React.SetStateAction<MessageChat | null | undefined>>;
};

type MessagesProviderProps = { children: React.ReactNode };

const MessagesContext = createContext({} as MessagesContextType);

export const MessageContextProvider = ({ children }: MessagesProviderProps) => {
  const [message, setMessage] = useState<string>("");
  const [editableMessage, setEditableMessage] = useState<MessageChat | null | undefined>();

  const value = useMemo(
    () => ({
      message,
      setMessage,
      editableMessage,
      setEditableMessage,
    }),
    [message, editableMessage,]
  );

  return <MessagesContext.Provider value={value}>{children}</MessagesContext.Provider>;
};

export const useMessages = () => {
  const useMessageContext = useContext(MessagesContext);

  if (!useMessageContext) {
    throw new Error("useMessagesContext must be used within a MessagesContextProvider");
  }

  return useMessageContext;
};
