import Button from "@src/components/ui/Button";
import { useChatInfo } from "./ChatInfoContext";
import ChatUsersMenu from "./ChatUsersMenu";
import ButtonLink from "@src/components/ui/ButtonLink";
import { useState } from "react";

const ChatHeader = () => {
  console.count("ChatHeader render")

  const { chatInfo } = useChatInfo();

  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleModalOpen = () => setIsModalOpen(!isModalOpen);

  return (
    <div
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
      <ChatUsersMenu isModalOpen={isModalOpen} setIsModalOpen={setIsModalOpen} />
    </div>
  );
};

export default ChatHeader;
