import { useNavigate, useParams } from "react-router-dom";
import { ChatService } from "@src/services/api/ChatService";
import ChatHeader from "./ChatHeader";
import { MessagesContextProvider } from "./MessagesContext";
import ChatMessages from "./ChatMessages";
import CountOfNewMessages from "./CountOfNewMessages";
import MessageInputBlock from "./MessageInputBlock";
import { ChatContextProvider } from "./ChatContext";

const Chat = () => {
  console.count("Chat render");
  const { chatId } = useParams<{ chatId: string }>();
  const navigate = useNavigate();

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
      <ChatContextProvider>
        <ChatHeader onChatDelete={handleChatDelete} onChatLeave={handleChatLeave} />

        <CountOfNewMessages />

        <MessagesContextProvider>
          <ChatMessages />
          <MessageInputBlock />
        </MessagesContextProvider>
      </ChatContextProvider>
    </div>
  );
};

export default Chat;
