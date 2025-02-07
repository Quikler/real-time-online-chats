import { useAuth } from "@src/contexts/AuthContext";
import { MessageChat } from "./{chatId}.types";
import { twMerge } from "tailwind-merge";

type MessageContentProps = React.HTMLAttributes<HTMLDivElement> & {
  messageChat: MessageChat;
};

const MessageContent = ({ messageChat, className, ...rest }: MessageContentProps) => {
  const { user } = useAuth();

  const isUserMessageOwner = messageChat.user.id === user?.id;

  return (
    <div
      className={`flex items-center gap-2 ${isUserMessageOwner ? "justify-end" : "justify-start"}`}
    >
      <div
        {...rest}
        className={twMerge(
          `flex flex-col gap-1 ${isUserMessageOwner ? "items-end" : "items-start"}`,
          className
        )}
      >
        <p
          className={`p-3 text-lg rounded-lg break-all ${
            isUserMessageOwner ? "bg-slate-700 text-white" : "bg-slate-500 text-white"
          }`}
        >
          {messageChat.content}
        </p>
      </div>
    </div>
  );
};

export default MessageContent;
