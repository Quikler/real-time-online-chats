import Button from "@src/components/ui/Button";
import ButtonLink from "@src/components/ui/ButtonLink";
import Modal from "@src/components/ui/Modal";
import React, { useState } from "react";
import { ChatInfo, UserChat } from "./{chatId}.types";
import { useAuth } from "@src/contexts/AuthContext";

type ChatHeaderProps = {
  users?: UserChat[];
  chatInfo?: ChatInfo;
  onChatLeave: React.MouseEventHandler<HTMLButtonElement>;
  onChatDelete: React.MouseEventHandler<HTMLButtonElement>;
};

const ChatHeader = ({ users, chatInfo, onChatLeave, onChatDelete }: ChatHeaderProps) => {
  const { user } = useAuth();

  const [isModalOpen, setIsModalOpen] = useState(false);
  const handleModalOpen = () => setIsModalOpen(!isModalOpen);

  return (
    <div className="bg-slate-700 fixed w-full right-0 left-0 flex items-center justify-between p-6">
      <ButtonLink
        to="/chats"
        className="text-3xl text-white hover:text-slate-300 transition-colors duration-300"
      >
        ←
      </ButtonLink>
      <div className="text-white flex flex-col flex-grow ml-4">
        <p className="text-3xl font-semibold">{chatInfo?.title}</p>
        <p className="text-lg text-opacity-80">3/52 members online</p>
      </div>
      <div className="w-auto">
        <Button
          className="text-3xl text-white hover:text-slate-300 transition-colors duration-300"
          onClick={handleModalOpen}
        >
          ⋮
        </Button>
        <Modal
          className="flex flex-col gap-4 p-6 bg-slate-700 shadow-lg"
          title={chatInfo?.title}
          isModalOpen={isModalOpen}
          setIsModalOpen={setIsModalOpen}
        >
          <ul className="text-white cursor-default">
            {users?.map((value, index) => (
              <li
                key={index}
                className={`flex items-center gap-3 p-3 transition-colors duration-300 ${
                  chatInfo?.ownerId === value.id
                    ? "bg-slate-400 hover:bg-slate-300"
                    : "bg-slate-600 hover:bg-slate-500"
                }`}
              >
                <img
                  src="/images/test-profile.jpg"
                  className="w-12 h-12 rounded-full object-cover"
                />
                <div className="overflow-x-auto text-wrap break-words">{value.email}</div>
              </li>
            ))}
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
    </div>
  );
};

export default ChatHeader;
