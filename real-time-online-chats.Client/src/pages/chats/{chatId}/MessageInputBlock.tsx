import EditableMessage from "./EditableMessage";
import MessageInput from "./MessageInput";
import { useMessage } from "./MessageContext";

const MessageInputBlock = () => {
  const { editableMessage } = useMessage();

  return (
    <div className="fixed flex flex-col gap-2 bottom-0 right-0 left-0 w-full bg-slate-700 p-4">
      {editableMessage && <EditableMessage />}
      <MessageInput />
    </div>
  );
};

export default MessageInputBlock;
