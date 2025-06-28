import ChatHeader from "./ChatHeader";
import { MessageContextProvider } from "./MessageContext";
import ChatMessages from "./ChatMessages";
import { CountOfNewMessagesWrapper } from "./CountOfNewMessages";
import MessageInputBlock from "./MessageInputBlock";
import { ChatUsersContextProvider } from "./ChatUsersContext";
import { ChatInfoContextProvider } from "./ChatInfoContext";
import { ChatMessagesContextProvider } from "./ChatMessagesContext";

const Chat = () => {
  console.count("Chat render");
  return (
    <div
      className="flex flex-col bg-gradient-to-br from-slate-700 to-slate-900 lg:px-16 flex-grow"
      style={{ minHeight: "720px" }}
    >
      <ChatUsersContextProvider>
        <ChatInfoContextProvider>
          <ChatHeader />

          <ChatMessagesContextProvider>
            <CountOfNewMessagesWrapper />

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
