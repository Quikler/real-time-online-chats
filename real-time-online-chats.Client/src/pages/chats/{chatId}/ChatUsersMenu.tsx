import Button from "@src/components/ui/Button";
import Modal from "@src/components/ui/Modal";
import { Link, useNavigate } from "react-router";
import OwnerContextMenu from "./OwnerContextMenu";
import UserAvatar from "./UserAvatar";
import UserContextMenu from "./UserContextMenu";
import { useChatInfo } from "./ChatInfoContext";
import { useChatUsers } from "./ChatUsersContext";
import { memo, useState } from "react";
import { useAuth } from "@src/hooks/useAuth";
import { ChatService } from "@src/services/api/ChatService";
import { ChatUsersService } from "@src/services/api/ChatUsersService";

type ChatUsersMenuProps = {
  isModalOpen: boolean;
  setIsModalOpen: React.Dispatch<React.SetStateAction<boolean>>;
}

const ChatUsersMenu = ({ isModalOpen, setIsModalOpen }: ChatUsersMenuProps) => {
  const { user } = useAuth();

  const { chatInfo } = useChatInfo();

  const navigate = useNavigate();

  const handleChatLeave = () => {
    if (chatInfo.id) {
      ChatUsersService.deleteMemberMe(chatInfo.id)
        .then(() => navigate("/chats"))
        .catch((e) => console.error("Error leaving chat:", e.message));
    }
  };

  const handleChatDelete = () => {
    if (chatInfo.id) {
      ChatService.deleteChat(chatInfo.id)
        .then(() => navigate('/chats'))
        .catch((e) => console.error("Error deleting chat:", e.message));
    }
  };

  return <Modal
    isHeaderEllipsis={true}
    className="flex flex-col gap-4 p-6 shadow-lg"
    title={chatInfo?.title}
    isModalOpen={isModalOpen}
    setIsModalOpen={setIsModalOpen}
  >
    <UsersListMenu />
    <div className="flex gap-2">
      <Button variant="primary" onClick={handleChatLeave}>
        Leave
      </Button>
      {user?.id === chatInfo?.ownerId && (
        <Button variant="danger" onClick={handleChatDelete}>
          Delete chat
        </Button>
      )}
    </div>
  </Modal>
}

export default memo(ChatUsersMenu);

const UsersListMenu = memo(function UsersListMenu() {
  const { user } = useAuth();

  const { chatInfo } = useChatInfo();
  const { chatUsers } = useChatUsers();

  const [menuPosition, setMenuPosition] = useState<{ x: number; y: number }>({ x: 0, y: 0 });
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  const handleUserContextMenu = (
    e: React.MouseEvent<HTMLLIElement, MouseEvent>,
    userId: string
  ) => {
    e.preventDefault();

    if (menuPosition.x !== 0 && menuPosition.y !== 0) {
      setSelectedUserId(null);
      setMenuPosition({ x: 0, y: 0 });
      return;
    }

    setMenuPosition({ x: e.clientX, y: e.clientY });
    setSelectedUserId(userId);
  };

  return <ul className="text-white">
    {chatUsers?.map((userChat, index) =>
      <li onContextMenu={(e) => handleUserContextMenu(e, userChat.id)} key={index}>
        <Link
          to={`/profile/${userChat.id}`}
          className={`flex items-center gap-3 p-3 transition-colors h-full w-full duration-300 ${chatInfo?.ownerId === userChat.id
            ? "bg-gradient-to-b from-slate-700 hover:from-slate-600 to-slate-900"
            : "bg-slate-700 hover:bg-slate-800"
            }`}
        >
          <UserAvatar avatarUrl={userChat.avatarUrl} width="48px" height="48px" />
          <div className="overflow-x-auto text-wrap break-words">{userChat.email}</div>
        </Link>
        {userChat.id === user?.id || user?.id !== chatInfo.ownerId ?
          <UserContextMenu
            isVisible={selectedUserId === userChat.id}
            userId={userChat.id}
            position={menuPosition}
          />
          :
          <OwnerContextMenu
            isVisible={selectedUserId === userChat.id}
            position={menuPosition}
            userId={userChat.id}
          />
        }
      </li>
    )}
  </ul>
});
