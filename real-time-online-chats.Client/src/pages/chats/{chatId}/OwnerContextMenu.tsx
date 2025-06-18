import { ChatService } from "@src/services/api/ChatService";
import { Link } from "react-router-dom";
import { useChat } from "./ChatContext";
import { ChatUsersService } from "@src/services/api/ChatUsersService";

type OwnerContextMenuProps = React.HTMLAttributes<HTMLDivElement> & {
  isVisible: boolean;
  position: { x: number; y: number };
  userId: string;
};

const OwnerContextMenu = ({ isVisible, position, userId, ...rest }: OwnerContextMenuProps) => {
  const { chatInfo } = useChat();

  if (!isVisible) return null;

  const handleKick = async () => {
    try {
      await ChatUsersService.deleteMember(chatInfo.id, userId);
    } catch (e: any) {
      console.error("Cannot kick user from chat:", e.message);
    }
  };

  const handleUpdateOwner = async () => {
    try {
      await ChatService.updateOwner(chatInfo.id, userId);
    } catch (e: any) {
      console.error("Cannot update chat owner:", e.message);
    }
  };

  return (
    <div
      {...rest}
      style={{
        top: position.y,
        left: position.x,
      }}
      className="absolute z-[999] bg-slate-700 rounded-lg shadow-lg overflow-hidden border border-slate-600"
    >
      <ul className="divide-y divide-slate-600">
        <li>
          <Link to={`/profile/${userId}`}>
            <button className="w-full px-6 py-3 text-white hover:bg-slate-600 transition-colors duration-200 text-left">
              Visit profile
            </button>
          </Link>
        </li>
        <li>
          <button
            onClick={handleKick}
            className="w-full px-6 py-3 text-white hover:bg-slate-600 transition-colors duration-200 text-left"
          >
            Kick user
          </button>
        </li>
        <li>
          <button
            onClick={handleUpdateOwner}
            className="w-full px-6 py-3 text-white hover:bg-slate-600 transition-colors duration-200 text-left"
          >
            Make owner
          </button>
        </li>
        <li>
          <button className="w-full px-6 py-3 text-white hover:bg-slate-600 transition-colors duration-200 text-left">
            Send message
          </button>
        </li>
        <li>
          <button className="w-full px-6 py-3 text-red-500 hover:bg-red-500 hover:text-white transition-colors duration-200 text-left">
            Block user
          </button>
        </li>
      </ul>
    </div>
  );
};

export default OwnerContextMenu;
