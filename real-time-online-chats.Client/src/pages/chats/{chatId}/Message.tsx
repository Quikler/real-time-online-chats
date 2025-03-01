import MessageContent from "./MessageContent";
import { MessageChat } from "./{chatId}.types";
import MessageActions from "./MessageActions";
import { MessageService } from "@src/services/api/MessageService";
import UserAvatarLink from "./UserAvatarLink";
import { useChat } from "./ChatContext";
import { useMessages } from "./MessagesContext";

type MessageProps = {
  messageChat: MessageChat;
  isCurrentUser: boolean;
  showUserInfo: boolean;
};

const Message = ({ messageChat, isCurrentUser, showUserInfo }: MessageProps) => {
  const avatarSize = "64px";

  const { setEditableMessage, setMessage } = useMessages();
  const { messages, chatInfo } = useChat();

  const handleMessageDelete = (messageId: string) => {
    MessageService.deleteMessage(messageId, chatInfo.id)
      .then((data) => console.log("Message: " + data + " deleted"))
      .catch((e) => console.error("Error deleting message:", e.message));

    setEditableMessage(null);
    setMessage("");
  };

  const onMessageEdit = (messageId: string) => {
    const message = messages.find((m) => m.id === messageId);
    console.log("Message:", message);
    if (!message) return;

    setEditableMessage(message);
    setMessage(message.content);
  };

  return (
    <>
      {showUserInfo ? (
        <>
          <UserAvatarLink
            avatarUrl={messageChat.user.avatarUrl}
            width={avatarSize}
            height={avatarSize}
            className={`rounded-full object-cover ${isCurrentUser ? "order-2" : ""}`}
            userId={messageChat.user.id}
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
                  onEdit={onMessageEdit}
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
                onEdit={onMessageEdit}
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
