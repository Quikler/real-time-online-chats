import MessageContent from "./MessageContent";
import { MessageChat } from "./{chatId}.types";

type MessageProps = {
  messageChat: MessageChat;
  isCurrentUser: boolean;
  showUserInfo: boolean;
};

const Message = ({ messageChat, isCurrentUser, showUserInfo }: MessageProps) => {
  return (
    <>
      {showUserInfo ? (
        <>
          <img
            src="/images/test-profile.jpg"
            className={`w-10 h-10 rounded-full object-cover ${isCurrentUser ? "order-2" : ""}`}
            alt="Profile"
          />
          <div className="flex flex-col gap-3 max-w-[80%]">
            <p
              className={`text-xl text-white text-opacity-80 flex gap-4 ${
                isCurrentUser ? "font-medium self-end" : ""
              }`}
            >
              {messageChat.user.email}
            </p>
            <MessageContent messageChat={messageChat} />
          </div>
        </>
      ) : (
        <>
          <div
            className={`w-10 h-10 rounded-full object-cover ${isCurrentUser ? "order-2" : ""}`}
          />
          <div className="flex flex-col gap-3 max-w-[80%]">
            <MessageContent messageChat={messageChat} />
          </div>
        </>
      )}
    </>
  );
};

export default Message;
