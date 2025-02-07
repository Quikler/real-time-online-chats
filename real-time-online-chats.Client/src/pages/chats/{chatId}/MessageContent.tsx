import { useAuth } from "@src/contexts/AuthContext";
import { MessageChat } from "./{chatId}.types";
import { twMerge } from "tailwind-merge";

type MessageContentProps = React.HTMLAttributes<HTMLDivElement> & {
  messageChat: MessageChat;
  
};

const MessageContent = ({ messageChat, className, ...rest }: MessageContentProps) => {
  const { user } = useAuth();

  return (
    <div
      {...rest}
      className={twMerge(
        `flex flex-col gap-1 ${messageChat.user.id === user?.id ? "items-end" : "items-start"}`,
        className
      )}
    >
      <p
        className={`p-3 text-lg rounded-lg break-all ${
          messageChat.user.id === user?.id ? "bg-slate-700 text-white" : "bg-slate-500 text-white"
        }`}
      >
        {messageChat.content}
      </p>
    </div>
  );
};

export default MessageContent;
