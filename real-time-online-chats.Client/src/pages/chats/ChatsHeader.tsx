import { Users } from "@src/components/svg/SVGCommon";
import Button from "@src/components/ui/Button";
import { useState } from "react";
import CreateChatForm, { CreateChatFormData } from "./CreateChatForm";
import { ChatService } from "@src/services/api/ChatService";
import { useNavigate } from "react-router-dom";

export default function ChatsHeaderSection() {
  const [isChatFormOpen, setIsChatFormOpen] = useState(false);

  const navigate = useNavigate();

  const handleCreateChatFormSubmit = async (
    _e: React.FormEvent<HTMLFormElement>,
    data: CreateChatFormData
  ) => {
    await ChatService.createChat(data)
      .then((data) => {
        if (data) {
          navigate(`/chats/${data.id}`, {
            state: {
              chatId: data.id,
            },
          });
        }
      })
      .catch((e) => console.log(e));
  };

  return (
    <section className="w-full lg:h-screen lg:my-0 my-24 flex justify-center items-center m-auto max-w-screen-xl">
      <div className="flex flex-col gap-6 px-[10%] text-gray-300 w-full">
        <div className="flex items-center gap-6">
          <Users width={96} height={96} />
          <div className="text-6xl">
            <p>CHATS</p>
            <p>SECTION</p>
          </div>
        </div>

        <p className="text-2xl font-light text-justify">
          Hello in chats section! If you don't have an account don't worry you can chat with anyone
          anywhere without registration needed! Simply find the chat or create one, but be sure
          after the browser close your chats and data will be gone!
        </p>

        <div className="flex gap-2">
          <Button onClick={() => setIsChatFormOpen(true)}>Create chat</Button>
          <Button variant="secondary">Find chat</Button>
        </div>
      </div>

      <CreateChatForm
        isChatFormOpen={isChatFormOpen}
        setIsChatFormOpen={setIsChatFormOpen}
        onSubmit={handleCreateChatFormSubmit}
      />
    </section>
  );
}
