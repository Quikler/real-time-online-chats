import { useAuth } from "@src/hooks/useAuth";
import { useChat } from "./ChatContext";
import Message from "./Message";

const ChatMessages = () => {
  const { user } = useAuth();
  const { messages } = useChat();

  return (
    <ul className="flex-grow flex flex-col p-6 overflow-y-auto pt-40 pb-32">
      {messages?.map((message, index) => {
        // If previous message was the current user's
        const isCurrentUserPrevious = index > 0 && message.user.id === messages[index - 1].user.id;

        const isCurrentUser = message.user.id === user?.id;

        const showUserInfo = !isCurrentUserPrevious;

        return (
          <li
            key={index}
            className={`flex gap-3 ${isCurrentUserPrevious ? "pt-2" : "pt-8"} ${
              isCurrentUser ? "justify-end" : "justify-start"
            }`}
          >
            <Message
              messageChat={message}
              isCurrentUser={isCurrentUser}
              showUserInfo={showUserInfo}
            />
          </li>
        );
      })}
    </ul>
  );
};

export default ChatMessages;
