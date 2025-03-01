import { Close } from "@src/components/svg/SVGCommon";
import { useMessages } from "./MessagesContext";

const EditableMessage = () => {
  console.count("EditableMessage render");

  const { editableMessage, setEditableMessage, setMessage } = useMessages();

  const handleClick = () => {
    setEditableMessage(null);
    setMessage("");
  };

  return (
    <div className="flex items-center gap-2 text-white">
      <p>Edit message: {editableMessage?.content}</p>
      <button
        className="p-1 px-2 bg-slate-600 text-white rounded-lg hover:bg-slate-500 transition-colors duration-300"
        onClick={handleClick}
      >
        <Close width="12" />
      </button>
    </div>
  );
};

export default EditableMessage;
