import { ChatService } from "@src/services/api/ChatService";
import { createContext, useContext, useEffect, useMemo, useReducer, useState } from "react";
import { ChatInfo, ChatMessage, ChatUser } from "./{chatId}.types";
import { useNavigate, useParams } from "react-router-dom";
import ErrorScreen from "@src/components/ui/ErrorScreen";
import { LoaderScreen } from "@src/components/ui/Loader";
import { HubConnection } from "@microsoft/signalr";
import { createMessageChatFromUserChat } from "./{chatId}.helpers";
import { useAuth } from "@src/hooks/useAuth";
import useMessageHubConnection from "./hooks/useMessageHubConnection";
import { toast } from "react-toastify";
import { ChatMessagesService } from "@src/services/api/ChatMessagesService";
import { ChatUsersService } from "@src/services/api/ChatUsersService";
import { handleError } from "@src/utils/helpers";

type ChatContextType = {
  chatInfo: ChatInfo;
  messages: ChatMessage[];
  users: ChatUser[];
  countOfNewMessages: number;
  setCountOfNewMessages: React.Dispatch<React.SetStateAction<number>>;
};

type MessagesAction =
  | { type: "SET_MESSAGES"; payload: ChatMessage[] }
  | { type: "UPDATE_MESSAGES"; payload: ChatMessage }
  | { type: "ADD_MESSAGE"; payload: ChatMessage }
  | { type: "REMOVE_MESSAGE"; payload: string };

// Reducer Function
const messagesReducer = (state: ChatMessage[], action: MessagesAction): ChatMessage[] => {
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

type ChatProviderProps = { children: React.ReactNode };

const ChatContext = createContext({} as ChatContextType);

export const ChatContextProvider = ({ children }: ChatProviderProps) => {
  console.count("ChatContextProvider render");
  const { chatId } = useParams<{ chatId: string }>();

  const navigate = useNavigate();

  const { user } = useAuth();
  const connection = useMessageHubConnection(chatId);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  const [chatInfo, setChatInfo] = useState<ChatInfo>({} as ChatInfo);
  const [messages, messagesDispatch] = useReducer(messagesReducer, []);
  const [users, setUsers] = useState<ChatUser[]>([]);

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

  useEffect(() => {
    if (!chatId || user?.id === undefined) return;

    const abort = new AbortController();
    const abortConfig = { signal: abort.signal }

    const loadChat = async () => {
        const joinChat = async () => {
          try {
            await ChatUsersService.addMe(chatId, abortConfig);
            console.log("Joined chat:", chatId);
          } catch (e: any) {
            if (e?.response?.data?.errors[0] == "User already in chat") {
              console.log('User already in chat');
            } else {
              handleError(e);
              setError(true);
            }
          }
        };

        const fetchChatInfo = async () => {
          try {
            const chatInfo = await ChatService.getChatInfo(chatId, abortConfig);
            setChatInfo(chatInfo);
          } catch (e: any) {
            handleError(e);
            setError(true);
          }
        };

        const fetchChatMessages = async () => {
          try {
            const messages = await ChatMessagesService.getAllMessages(chatId, abortConfig);
            messagesDispatch({ type: "SET_MESSAGES", payload: messages });
          } catch (e: any) {
            handleError(e);
            setError(true);
          }
        };

        const fetchChatUsers = async () => {
          try {
            const users = await ChatUsersService.getAllUsersByChatId(chatId, abortConfig);
            setUsers(users);
          } catch (e: any) {
            handleError(e);
            setError(true);
          }
        };

        await Promise.all([joinChat(), fetchChatInfo(), fetchChatMessages(), fetchChatUsers()]);

        if (!error) {
          setLoading(false);
        }
    }

    loadChat();

    return () => {
      abort.abort();
    };
  }, [chatId]);

  useEffect(() => {
    if (!connection || !chatInfo) return;

    const registerSignalREventHandlers = (connection: HubConnection) => {
      connection.off("SendMessage");
      connection.on("SendMessage", (message: ChatMessage) => {
        messagesDispatch({ type: "ADD_MESSAGE", payload: message });
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

        messagesDispatch({ type: "ADD_MESSAGE", payload: message });
        setUsers((prev) => [...prev, joinedUser]);
      });

      connection.off("LeaveChat");
      connection.on("LeaveChat", (leavedUser: ChatUser) => {
        console.log("Current owner:", chatInfo.ownerId);
        console.log("Left the chat:", leavedUser.id);

        const newUsers = users.filter((u) => u.id !== leavedUser.id);
        const newOwner = newUsers[0];

        console.log("New owner:", newOwner);

        const message = createMessageChatFromUserChat(
          leavedUser,
          leavedUser.id === chatInfo.ownerId
            ? `<!-- User ${leavedUser.email} left the chat and new owner is ${newOwner.email} -->`
            : `<!-- User ${leavedUser.email} left the chat -->`
        );

        messagesDispatch({ type: "ADD_MESSAGE", payload: message });

        setChatInfo((prevChatInfo) =>
          prevChatInfo ? { ...prevChatInfo, ownerId: newOwner.id } : prevChatInfo
        );

        setUsers(newUsers);
      });

      connection.off("KickMember");
      connection.on("KickMember", (kickedMemberId: string) => {
        const newUsers = users.filter((u) => u.id !== kickedMemberId);
        const kickedMember = users.find((u) => u.id === kickedMemberId);
        const owner = users.find((u) => u.id === chatInfo.ownerId);

        if (!kickedMember || !owner) return;

        if (user?.id === kickedMemberId) {
          connection.stop();
        }

        const message = createMessageChatFromUserChat(
          kickedMember,
          `<!-- User ${kickedMember.email} has been kicked from chat by owner ${owner.email} -->`
        );

        messagesDispatch({ type: "ADD_MESSAGE", payload: message });
        setUsers(newUsers);
      });

      connection.off("DeleteMessage");
      connection.on("DeleteMessage", (messageId: string) => {
        messagesDispatch({ type: "REMOVE_MESSAGE", payload: messageId });
      });

      connection.off("UpdateMessage");
      connection.on("UpdateMessage", (message: ChatMessage) => {
        messagesDispatch({ type: "UPDATE_MESSAGES", payload: message });
      });

      connection.off("UpdateOwner");
      connection.on("UpdateOwner", (oldOwnerId: string, newOwnerId: string) => {
        const oldOwner = users.find((u) => u.id === oldOwnerId);
        const newOwner = users.find((u) => u.id === newOwnerId);

        if (!oldOwner || !newOwner) return;

        setChatInfo({ ...chatInfo, ownerId: newOwner.id });

        const message = createMessageChatFromUserChat(
          oldOwner,
          `<!-- ${oldOwner.email} granted ${newOwner.email} to owner -->`
        );

        messagesDispatch({ type: "ADD_MESSAGE", payload: message });
      });
    };

    registerSignalREventHandlers(connection);
  }, [connection, chatInfo?.ownerId]);

  const value = useMemo(
    () => ({
      users,
      chatInfo,
      messages,
      countOfNewMessages,
      setCountOfNewMessages,
    }),
    [users, chatInfo, messages, countOfNewMessages]
  );

  if (loading) return <LoaderScreen />;
  if (error) return <ErrorScreen />;

  return <ChatContext.Provider value={value}>{children}</ChatContext.Provider>;
};

export const useChat = () => {
  const chatContext = useContext(ChatContext);

  if (!chatContext) {
    throw new Error("useChat must be used within a ChatContextProvider");
  }

  return chatContext;
};
