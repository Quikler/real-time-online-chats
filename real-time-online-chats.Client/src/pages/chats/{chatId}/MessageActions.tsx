import { Garbage, Edit } from "@src/components/svg/SVGCommon";
import Button from "@src/components/ui/Button";
import Modal from "@src/components/ui/Modal";
import { ChatMessagesService } from "@src/services/api/ChatMessagesService";
import { useState } from "react";
import { useChat } from "./ChatContext";
import { useMessage } from "./MessageContext";

type MessageActionsProps = {
  messageId: string;
};

const MessageActions = ({ messageId }: MessageActionsProps) => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const { messages, chatInfo } = useChat();
  const { setEditableMessage, setMessage } = useMessage();

  const handleMessageDelete = () => {
    setIsModalOpen(false);
    ChatMessagesService.deleteMessage(chatInfo.id, messageId)
      .then((data) => console.log("Message: " + data + " deleted"))
      .catch((e) => console.error("Error deleting message:", e.message));

    setEditableMessage(null);
    setMessage("");
  };

  const handleMessageEdit = () => {
    const message = messages.find((m) => m.id === messageId);
    console.log("Message:", message);
    if (!message) return;

    setEditableMessage(message);
    setMessage(message.content);
  };

  return (
    <div className="flex gap-2">
      <button onClick={() => setIsModalOpen(true)}>
        <Garbage cursor="pointer" />
      </button>
      <button onClick={handleMessageEdit}>
        <Edit cursor="pointer" />
      </button>

      <Modal
        title="Are you sure you want to delete this message?"
        isModalOpen={isModalOpen}
        setIsModalOpen={setIsModalOpen}
      >
        <div className="flex gap-2">
          <Button onClick={handleMessageDelete} variant="danger">
            Yes
          </Button>
          <Button onClick={() => setIsModalOpen(false)}>No</Button>
        </div>
      </Modal>
    </div>
  );
};

export default MessageActions;
