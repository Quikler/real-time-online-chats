import { Garbage, Edit } from "@src/components/svg/SVGCommon";
import Button from "@src/components/ui/Button";
import Modal from "@src/components/ui/Modal";
import { useState } from "react";

type MessageActionsProps = {
  messageId: string;
  onDelete?: (messageId: string) => void;
  onEdit?: (messageId: string) => void;
};

const MessageActions = ({ messageId, onDelete, onEdit }: MessageActionsProps) => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleYesClick = () => {
    setIsModalOpen(false);
    onDelete?.(messageId);
  };

  return (
    <div className="flex gap-2">
      <button onClick={() => setIsModalOpen(true)}>
        <Garbage cursor="pointer" />
      </button>
      <button onClick={() => onEdit?.(messageId)}>
        <Edit cursor="pointer" />
      </button>

      <Modal
        title="Are you sure you want to delete this message?"
        isModalOpen={isModalOpen}
        setIsModalOpen={setIsModalOpen}
      >
        <div className="flex gap-2">
          <Button onClick={handleYesClick} variant="danger">
            Yes
          </Button>
          <Button onClick={() => setIsModalOpen(false)}>No</Button>
        </div>
      </Modal>
    </div>
  );
};

export default MessageActions;
