import React, { createContext, useEffect, useMemo, useReducer, useState } from "react";
import { useNavigate, useParams } from "react-router";
import { handleError, useCustomHook } from "@src/utils/helpers";
import ErrorScreen from "@src/components/ui/ErrorScreen";
import { ChatMessagesService } from "@src/services/api/ChatMessagesService";
import { ChatMessage, ChatUser } from "./{chatId}.types";
import { HubConnection } from "@microsoft/signalr";
import { useAuth } from "@src/hooks/useAuth";
import { toast } from "react-toastify";
import useMessageHubConnection from "./hooks/useMessageHubConnection";
import { createMessageChatFromUserChat } from "./{chatId}.helpers";
import { useChatInfo } from "./ChatInfoContext";
import { useChatUsers } from "./ChatUsersContext";

type MessagesAction =
  | { type: "SET_MESSAGES"; payload: ChatMessage[] }
  | { type: "UPDATE_MESSAGES"; payload: ChatMessage }
  | { type: "ADD_MESSAGE"; payload: ChatMessage }
  | { type: "REMOVE_MESSAGE"; payload: string };

// Reducer Function
const chatMessagesReducer = (state: ChatMessage[], action: MessagesAction): ChatMessage[] => {
  switch (action.type) {
    case "SET_MESSAGES":
      return action.payload;
    case "UPDATE_MESSAGES":
      return state.map((m) =>
        m.id === action.payload.id ? { ...m, content: action.payload.content } : m
      );
    case "ADD_MESSAGE":
      return [...state, action.payload];
    case "REMOVE_MESSAGE":
      return state.filter((msg) => msg.id !== action.payload);
    default:
      return state;
  }
};

type ChatMessagesContextType = {
  chatMessages: ChatMessage[];
  countOfNewMessages: number;
  setCountOfNewMessages: React.Dispatch<React.SetStateAction<number>>;
};

type Props = { children: React.ReactNode };

const ChatMessagesContext = createContext({} as ChatMessagesContextType);

export const ChatMessagesContextProvider = ({ children }: Props) => {
  console.count("ChatMessagesContextProvider render");
  const { chatId } = useParams<{ chatId: string }>();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  const [chatMessages, chatMessagesDispatch] = useReducer(chatMessagesReducer, []);

  useEffect(() => {
    if (!chatId) return;
    
    const abort = new AbortController();
    const fetchChatMessages = async () => {
      try {
        const messages = await ChatMessagesService.getAllMessages(chatId, { signal: abort.signal });
        chatMessagesDispatch({ type: "SET_MESSAGES", payload: messages });
        setLoading(false)
      } catch (e: any) {
        handleError(e);
        setError(true);
      }
    };

    fetchChatMessages();

    return () => {
      abort.abort();
    }
  }, [chatId]);


  const navigate = useNavigate();

  const { user } = useAuth();
  const connection = useMessageHubConnection(chatId);

  const [countOfNewMessages, setCountOfNewMessages] = useState(0);

  useEffect(() => {
    const scrollHandler = () => {
      const scrollPosition = window.scrollY + window.innerHeight;
      const bottomPosition = document.documentElement.scrollHeight;

      if (scrollPosition >= bottomPosition - 40) {
        setCountOfNewMessages(0);
      }
    };

    window.addEventListener("scroll", scrollHandler);

    return () => window.removeEventListener("scroll", scrollHandler);
  }, []);

  const { chatInfo, setChatInfo } = useChatInfo();
  const { chatUsers, setChatUsers } = useChatUsers();

  useEffect(() => {
    if (!connection || !chatInfo || !chatUsers || !chatMessages) return;

    const registerSignalREventHandlers = (connection: HubConnection) => {
      connection.off("SendMessage");
      connection.on("SendMessage", (message: ChatMessage) => {
        chatMessagesDispatch({ type: "ADD_MESSAGE", payload: message });
        if (message.user.id !== user?.id) {
          setCountOfNewMessages((prev) => prev + 1);
        }
      });

      connection.off("DeleteChat");
      connection.on("DeleteChat", () => {
        toast.success("Chat deleted successfully");
        navigate("/chats");
      });

      connection.off("JoinChat");
      connection.on("JoinChat", (joinedUser: ChatUser) => {
        const message = createMessageChatFromUserChat(
          joinedUser,
          `<!-- User ${joinedUser.email} joined chat -->`
        );

        chatMessagesDispatch({ type: "ADD_MESSAGE", payload: message });
        setChatUsers((prev) => [...prev, joinedUser]);
      });

      connection.off("LeaveChat");
      connection.on("LeaveChat", (leavedUser: ChatUser) => {
        console.log("Current owner:", chatInfo.ownerId);
        console.log("Left the chat:", leavedUser.id);

        const newUsers = chatUsers.filter((u) => u.id !== leavedUser.id);
        const newOwner = newUsers[0];

        console.log("New owner:", newOwner);

        const message = createMessageChatFromUserChat(
          leavedUser,
          leavedUser.id === chatInfo.ownerId
            ? `<!-- User ${leavedUser.email} left the chat and new owner is ${newOwner.email} -->`
            : `<!-- User ${leavedUser.email} left the chat -->`
        );

        chatMessagesDispatch({ type: "ADD_MESSAGE", payload: message });

        setChatInfo((prevChatInfo) =>
          prevChatInfo ? { ...prevChatInfo, ownerId: newOwner.id } : prevChatInfo
        );

        setChatUsers(newUsers);
      });

      connection.off("KickMember");
      connection.on("KickMember", (kickedMemberId: string) => {
        const newUsers = chatUsers.filter((u) => u.id !== kickedMemberId);
        const kickedMember = chatUsers.find((u) => u.id === kickedMemberId);
        const owner = chatUsers.find((u) => u.id === chatInfo.ownerId);

        console.log('newUsers', newUsers);
        console.log('kickedMember', kickedMember);
        console.log('owner', owner);

        if (!kickedMember || !owner) return;

        if (user?.id === kickedMemberId) {
          connection.stop();
          navigate('/chats');
          toast.info('You have been kicked from chat')
          return;
        }

        const message = createMessageChatFromUserChat(
          kickedMember,
          `<!-- User ${kickedMember.email} has been kicked from chat by owner ${owner.email} -->`
        );

        chatMessagesDispatch({ type: "ADD_MESSAGE", payload: message });
        setChatUsers(newUsers);
      });

      connection.off("DeleteMessage");
      connection.on("DeleteMessage", (messageId: string) => {
        chatMessagesDispatch({ type: "REMOVE_MESSAGE", payload: messageId });
      });

      connection.off("UpdateMessage");
      connection.on("UpdateMessage", (message: ChatMessage) => {
        chatMessagesDispatch({ type: "UPDATE_MESSAGES", payload: message });
      });

      connection.off("UpdateOwner");
      connection.on("UpdateOwner", (oldOwnerId: string, newOwnerId: string) => {
        const oldOwner = chatUsers.find((u) => u.id === oldOwnerId);
        const newOwner = chatUsers.find((u) => u.id === newOwnerId);

        if (!oldOwner || !newOwner) return;

        setChatInfo({ ...chatInfo, ownerId: newOwner.id });

        const message = createMessageChatFromUserChat(
          oldOwner,
          `<!-- ${oldOwner.email} granted ${newOwner.email} to owner -->`
        );

        chatMessagesDispatch({ type: "ADD_MESSAGE", payload: message });
      });
    };

    registerSignalREventHandlers(connection);
  }, [connection, chatUsers, chatMessages, chatInfo]);

  if (error) <ErrorScreen />

  const value = useMemo(() => ({
    chatMessages,
    countOfNewMessages,
    setCountOfNewMessages
  }), [chatMessages, countOfNewMessages]);

  return <ChatMessagesContext.Provider value={value}>{loading ? null : children}</ChatMessagesContext.Provider>
}

export const useChatMessages = () => useCustomHook(ChatMessagesContext, useChatMessages.name);
