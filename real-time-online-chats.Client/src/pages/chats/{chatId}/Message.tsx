import MessageContent from "./MessageContent";
import { MessageChat } from "./{chatId}.types";
import MessageActions from "./MessageActions";
import { MessageService } from "@src/services/api/MessageService";

type MessageProps = {
  chatId: string;
  messageChat: MessageChat;
  isCurrentUser: boolean;
  showUserInfo: boolean;
  onDelete?: (messageId: string) => void;
  onEdit?: (messageId: string) => void;
};

const Message = ({
  chatId,
  onDelete,
  onEdit,
  messageChat,
  isCurrentUser,
  showUserInfo,
}: MessageProps) => {
  const avatarSize = "64px";

  const handleMessageDelete = (messageId: string) => {
    MessageService.deleteMessage(messageId, chatId)
      .then((data) => console.log("Message: " + data + " deleted"))
      .catch((e) => console.error("Error deleting message:", e.message));

    onDelete?.(messageId);
  };

  return (
    <>
      {showUserInfo ? (
        <>
          <img
            style={{ width: avatarSize, height: avatarSize }}
            src="/images/test-profile.jpg"
            className={`rounded-full object-cover ${isCurrentUser ? "order-2" : ""}`}
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
            <div
              className={`flex gap-2 items-center ${
                isCurrentUser ? "justify-end" : "justify-start"
              }`}
            >
              {isCurrentUser && (
                <MessageActions
                  messageId={messageChat.id}
                  onEdit={onEdit}
                  onDelete={handleMessageDelete}
                />
              )}
              <MessageContent messageChat={messageChat} />
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
            {isCurrentUser && (
              <MessageActions
                messageId={messageChat.id}
                onEdit={onEdit}
                onDelete={handleMessageDelete}
              />
            )}
            <MessageContent messageChat={messageChat} />
          </div>
        </>
      )}
    </>
  );
};

export default Message;
