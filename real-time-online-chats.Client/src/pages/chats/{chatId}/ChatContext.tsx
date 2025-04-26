import { ChatService, ChatLevel } from "@src/services/api/ChatService";
import { createContext, useContext, useEffect, useMemo, useReducer, useState } from "react";
import { ChatInfo, MessageChat, UserChat } from "./{chatId}.types";
import { useNavigate, useParams } from "react-router-dom";
import ErrorScreen from "@src/components/ui/ErrorScreen";
import { LoaderScreen } from "@src/components/ui/Loader";
import { HubConnection } from "@microsoft/signalr";
import { createMessageChatFromUserChat } from "./{chatId}.helpers";
import { useAuth } from "@src/hooks/useAuth";
import useMessageHubConnection from "./hooks/useMessageHubConnection";
import { toast } from "react-toastify";

type ChatContextType = {
  chatInfo: ChatInfo;
  messages: MessageChat[];
  users: UserChat[];
  countOfNewMessages: number;
  setCountOfNewMessages: React.Dispatch<React.SetStateAction<number>>;
};

type MessagesAction =
  | { type: "SET_MESSAGES"; payload: MessageChat[] }
  | { type: "UPDATE_MESSAGES"; payload: MessageChat }
  | { type: "ADD_MESSAGE"; payload: MessageChat }
  | { type: "REMOVE_MESSAGE"; payload: string };

// Reducer Function
const messagesReducer = (state: MessageChat[], action: MessagesAction): MessageChat[] => {
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
  const [users, setUsers] = useState<UserChat[]>([]);

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
    if (!chatId) return;

    console.log("fetch chat");

    const abortChatDetailed = new AbortController();
    const abortJoinChat = new AbortController();

    const joinAndFetchChat = async () => {
      try {
        await ChatService.addMemberMe(chatId, { signal: abortJoinChat.signal });
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
          messagesDispatch({ type: "SET_MESSAGES", payload: data.messages });
          setUsers(data.users);
          setLoading(false);
        }
      } catch (e: any) {
        setError(true);
        console.error("Error joining or fetching the chat:", e.message);
      }
    };

    joinAndFetchChat();

    return () => {
      abortChatDetailed.abort();
      abortJoinChat.abort();
    };
  }, [chatId]);

  useEffect(() => {
    if (!connection || !chatInfo) return;

    const registerSignalREventHandlers = (connection: HubConnection) => {
      connection.off("SendMessage");
      connection.on("SendMessage", (message: MessageChat) => {
        messagesDispatch({ type: "ADD_MESSAGE", payload: message });
        if (message.user.id !== user?.id) {
          setCountOfNewMessages((prev) => prev + 1);
        }
      });

      connection.off("DeleteChat");
      connection.on("DeleteChat", () => {
        toast("Chat has been deleted");
        navigate("/chats");
      });

      connection.off("JoinChat");
      connection.on("JoinChat", (joinedUser: UserChat) => {
        const message = createMessageChatFromUserChat(
          joinedUser,
          `<!-- User ${joinedUser.email} joined chat -->`
        );

        messagesDispatch({ type: "ADD_MESSAGE", payload: message });
        setUsers((prev) => [...prev, joinedUser]);
      });

      connection.off("LeaveChat");
      connection.on("LeaveChat", (leavedUser: UserChat) => {
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
      connection.on("UpdateMessage", (message: MessageChat) => {
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
  }, [connection, chatInfo.ownerId]);

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
