import MessageContent from "./MessageContent";
import { ChatMessage } from "./{chatId}.types";
import MessageActions from "./MessageActions";
import UserAvatarLink from "./UserAvatarLink";
import { memo } from "react";

type MessageProps = {
  message: ChatMessage;
  isCurrentUser: boolean;
  isCurrentUserPrevious: boolean;
  showUserInfo: boolean;
};

const Message = ({ message, isCurrentUserPrevious, isCurrentUser, showUserInfo }: MessageProps) => {
  console.count("Message render");
  const avatarSize = "64px";

  return (
    <li className={`flex gap-3 ${isCurrentUserPrevious ? "pt-2" : "pt-8"} ${isCurrentUser ? "justify-end" : "justify-start"}`}
    >
      {showUserInfo ? (
        <>
          <UserAvatarLink
            avatarUrl={message.user.avatarUrl}
            width={avatarSize}
            height={avatarSize}
            className={`rounded-full object-cover ${isCurrentUser ? "order-2" : ""}`}
            userId={message.user.id}
          />

          <div className="flex flex-col gap-3 max-w-[80%]">
            <p
              className={`text-xl text-white text-opacity-80 flex gap-4 ${isCurrentUser ? "font-medium self-end" : ""
                }`}
            >
              {message.user.email}
            </p>
            <div
              className={`flex gap-2 items-center ${isCurrentUser ? "justify-end" : "justify-start"
                }`}
            >
              {isCurrentUser && <MessageActions messageId={message.id} />}
              <MessageContent messageChat={message} />
            </div>
          </div>
        </>
      ) : (
        <>
          <div
            style={{ width: avatarSize }}
            className={`rounded-full object-cover ${isCurrentUser ? "order-2" : ""}`}
          />

          <div className="flex items-center gap-2 max-w-[80%]">
            {isCurrentUser && <MessageActions messageId={message.id} />}
            <MessageContent messageChat={message} />
          </div>
        </>
      )}
    </li>
  );
};

export default memo(Message);
