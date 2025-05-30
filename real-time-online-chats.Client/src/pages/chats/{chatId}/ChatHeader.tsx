import Button from "@src/components/ui/Button";
import ButtonLink from "@src/components/ui/ButtonLink";
import Modal from "@src/components/ui/Modal";
import React, { useEffect, useState } from "react";
import { useAuth } from "@src/hooks/useAuth";
import UserAvatar from "./UserAvatar";
import { Link } from "react-router-dom";
import UserContextMenu from "./UserContextMenu";
import OwnerContextMenu from "./OwnerContextMenu";
import { useChat } from "./ChatContext";

type ChatHeaderProps = {
  onChatLeave?: React.MouseEventHandler<HTMLButtonElement>;
  onChatDelete?: React.MouseEventHandler<HTMLButtonElement>;
};

const ChatHeader = ({ onChatLeave, onChatDelete }: ChatHeaderProps) => {
  console.count("ChatHeader render")
  const { user } = useAuth();

  const { users, chatInfo } = useChat();

  const [isModalOpen, setIsModalOpen] = useState(false);

  const [menuPosition, setMenuPosition] = useState<{ x: number; y: number }>({ x: 0, y: 0 });
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  const handleModalOpen = () => setIsModalOpen(!isModalOpen);

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

  useEffect(() => {
    const handleClose = () => {
      setSelectedUserId(null);
      setMenuPosition({ x: 0, y: 0 });
    };

    if (selectedUserId) {
      document.addEventListener("click", handleClose);
      document.addEventListener("scroll", handleClose);
      window.addEventListener("resize", handleClose);
    }

    return () => {
      document.removeEventListener("click", handleClose);
      document.removeEventListener("scroll", handleClose);
      window.removeEventListener("resize", handleClose);
    };
  }, [selectedUserId]);

  return (
    <div
      // onClick={() => setIsUserContextOpen(null)}
      className="bg-slate-700 fixed w-full right-0 left-0 flex items-center justify-between p-6"
    >
      <ButtonLink
        to="/chats"
        className="text-3xl text-white hover:text-slate-300 transition-colors duration-300"
      >
        ←
      </ButtonLink>
      <div className="text-white flex flex-col overflow-hidden flex-grow mx-4">
        <p className="text-3xl font-semibold overflow-hidden text-ellipsis text-nowrap">
          {chatInfo?.title}
        </p>
        <p className="text-lg text-opacity-80">3/52 members online</p>
      </div>
      <Button
        className="text-3xl w-auto text-white hover:text-slate-300 transition-colors duration-300"
        onClick={handleModalOpen}
      >
        ⋮
      </Button>

      <Modal
        isHeaderEllipsis={true}
        className="flex flex-col gap-4 p-6 shadow-lg"
        title={chatInfo?.title}
        isModalOpen={isModalOpen}
        setIsModalOpen={setIsModalOpen}
      >
        <ul className="text-white">
          {users?.map((userChat, index) => {
            let contextMenu: JSX.Element;

            if (userChat.id === user?.id) {
              contextMenu = (
                <UserContextMenu
                  isVisible={selectedUserId === userChat.id}
                  userId={userChat.id}
                  position={menuPosition}
                />
              );
            } else if (user?.id === chatInfo?.ownerId) {
              contextMenu = (
                <OwnerContextMenu
                  isVisible={selectedUserId === userChat.id}
                  position={menuPosition}
                  userId={userChat.id}
                />
              );
            } else {
              contextMenu = (
                <UserContextMenu
                  isVisible={selectedUserId === userChat.id}
                  userId={userChat.id}
                  position={menuPosition}
                />
              );
            }

            return (
              <li onContextMenu={(e) => handleUserContextMenu(e, userChat.id)} key={index}>
                <Link
                  to={`/profile/${userChat.id}`}
                  className={`flex items-center gap-3 p-3 transition-colors duration-300 ${
                    chatInfo?.ownerId === userChat.id
                      ? "bg-gradient-to-b from-slate-700 hover:from-slate-600 to-slate-900"
                      : "bg-slate-700 hover:bg-slate-800"
                  }`}
                >
                  <UserAvatar avatarUrl={userChat.avatarUrl} width="48px" height="48px" />
                  <div className="overflow-x-auto text-wrap break-words">{userChat.email}</div>
                </Link>
                {contextMenu}
              </li>
            );
          })}
        </ul>
        <div className="flex gap-2">
          <Button variant="primary" onClick={onChatLeave}>
            Leave
          </Button>
          {user?.id === chatInfo?.ownerId && (
            <Button variant="danger" onClick={onChatDelete}>
              Delete chat
            </Button>
          )}
        </div>
      </Modal>
    </div>
  );
};

export default ChatHeader;
