import { useNavigate, useParams } from "react-router-dom";
import { ChatService } from "@src/services/api/ChatService";
import ChatHeader from "./ChatHeader";
import { MessageContextProvider } from "./MessageContext";
import ChatMessages from "./ChatMessages";
import CountOfNewMessages from "./CountOfNewMessages";
import MessageInputBlock from "./MessageInputBlock";
import { ChatUsersService } from "@src/services/api/ChatUsersService";
import { ChatUsersContextProvider } from "./ChatUsersContext";
import { ChatInfoContextProvider } from "./ChatInfoContext";
import { ChatMessagesContextProvider } from "./ChatMessagesContext";

const Chat = () => {
  console.count("Chat render");
  const { chatId } = useParams<{ chatId: string }>();
  const navigate = useNavigate();

  const handleChatLeave = () => {
    if (chatId) {
      ChatUsersService.deleteMemberMe(chatId)
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
      <ChatUsersContextProvider>
        <ChatInfoContextProvider>
          <ChatHeader onChatDelete={handleChatDelete} onChatLeave={handleChatLeave} />

          <ChatMessagesContextProvider>
            <CountOfNewMessages />

            <MessageContextProvider>
              <ChatMessages />
              <MessageInputBlock />
            </MessageContextProvider>
          </ChatMessagesContextProvider>
        </ChatInfoContextProvider>
      </ChatUsersContextProvider>
    </div>
  );
};

export default Chat;
