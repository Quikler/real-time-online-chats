import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useRef } from "react";
import { toast } from "react-toastify";
import { ChatService } from "@src/services/api/ChatService";
import ChatHeader from "./ChatHeader";
import { useChat } from "./ChatContext";
import MessageInput from "./MessageInput";
import { scrollToBottomOfBody } from "@src/utils/helpers";
import EditableMessage from "./EditableMessage";
import { MessageContextProvider } from "./MessagesContext";
import ChatMessages from "./ChatMessages";
import CountOfNewMessages from "./CountOfNewMessages";

const Chat = () => {
  const { chatId } = useParams<{ chatId: string }>();
  const navigate = useNavigate();

  const { setCountOfNewMessages, messages } = useChat();

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

  const hasScrolled = useRef(false);
  useEffect(() => {
    if (messages?.length > 0 && !hasScrolled.current) {
      scrollToBottomOfBody();
      hasScrolled.current = true;
    }
  }, [messages]);

  const handleChatLeave = () => {
    if (chatId) {
      ChatService.deleteMemberMe(chatId)
        .then(() => navigate("/chats"))
        .catch((e) => console.error("Error leaving chat:", e.message));
    }
  };

  const handleChatDelete = () => {
    if (chatId) {
      ChatService.deleteChat(chatId)
        .then(() => {
          toast.success("Chat deleted successfully");
          navigate("/chats");
        })
        .catch((e) => console.error("Error deleting chat:", e.message));
    }
  };

  return (
    <div
      className="flex flex-col bg-gradient-to-br from-slate-700 to-slate-900 lg:px-16 flex-grow"
      style={{ minHeight: "720px" }}
    >
      <ChatHeader onChatDelete={handleChatDelete} onChatLeave={handleChatLeave} />

      <CountOfNewMessages />

      <MessageContextProvider>
        <ChatMessages />
        <div className="fixed flex flex-col gap-2 bottom-0 right-0 left-0 w-full bg-slate-700 p-4">
          <EditableMessage />
          <MessageInput />
        </div>
      </MessageContextProvider>
    </div>
  );
};

export default Chat;
