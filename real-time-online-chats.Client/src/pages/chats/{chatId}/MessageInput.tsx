import { isNullOrWhitespace } from "@src/utils/helpers";
import { useMessage } from "./MessageContext";
import { ChatMessagesService } from "@src/services/api/ChatMessagesService";
import { useChatInfo } from "./ChatInfoContext";
import { memo } from "react";

const MessageInput = () => {
  console.count("MessageInput render");

  const { chatInfo } = useChatInfo();
  const { message, setMessage, editableMessage, setEditableMessage } = useMessage();

  const handleMessageSend = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (isNullOrWhitespace(message)) return;
    setMessage("");

    if (editableMessage?.id) {
      ChatMessagesService.updateMessage(chatInfo.id, editableMessage?.id, { content: message })
        .then(() => console.log("Updated message successfully."))
        .catch((e) => console.error("Error updating message:", e.message));

      setEditableMessage(null);

      return;
    }

    ChatMessagesService.createMessage(chatInfo.id, { content: message })
      .then(() => {
        window.scrollTo(0, document.body.scrollHeight);
      })
      .catch((e) => console.error("Error creating message:", e.message));
  };

  return (
    <form className="flex items-center gap-4" onSubmit={handleMessageSend}>
      <input
        name="message"
        autoComplete="off"
        value={message}
        onChange={(e) => setMessage(e.target.value)}
        className="flex-grow p-3 bg-slate-600 text-white placeholder-white focus:outline-none"
        placeholder="Send a message..."
      />
      <button
        type="submit"
        className="p-3 px-4 bg-slate-600 text-white rounded-lg hover:bg-slate-500 transition-colors duration-300"
      >
        ➢
      </button>
    </form>
  );
};

export default memo(MessageInput);
