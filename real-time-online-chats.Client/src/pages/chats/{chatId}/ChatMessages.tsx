import { useAuth } from "@src/hooks/useAuth";
import Message from "./Message";
import { useChatMessages } from "./ChatMessagesContext";

const ChatMessages = () => {
  console.count("ChatMessages render");

  const { user } = useAuth();
  const { chatMessages } = useChatMessages();

  return (
    <>
      <ul className="flex-grow flex flex-col p-6 overflow-y-auto pt-40 pb-32">
        {chatMessages?.map((message, index) => {
          // If previous message was the current user's
          const isCurrentUserPrevious =
            index > 0 && message.user.id === chatMessages[index - 1].user.id;

          const isCurrentUser = message.user.id === user?.id;

          const showUserInfo = !isCurrentUserPrevious;

          return (
            <Message
              key={message.id}
              message={message}
              isCurrentUserPrevious={isCurrentUserPrevious}
              isCurrentUser={isCurrentUser}
              showUserInfo={showUserInfo}
            />
          );
        })}
      </ul>
    </>
  );
};

export default ChatMessages;
