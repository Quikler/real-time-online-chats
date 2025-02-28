import CreateChatForm, { CreateChatFormData } from "@src/pages/chats/CreateChatForm";
import { ChatService } from "@src/services/api/ChatService";
import { useState } from "react";
import { toast } from "react-toastify";
import UserProfileCard from "./ProfileUserCard";
import UserProfileAbout from "./UserProfileAbout";
import UserProfileFriends from "./UserProfileFriends";

const UserProfile = () => {
  const [isChatFormOpen, setIsChatFormOpen] = useState(false);

  const handleCreateChatFormSubmit = async (
    _e: React.FormEvent<HTMLFormElement>,
    data: CreateChatFormData
  ) => {
    await ChatService.createChat(data)
      .then((data) => toast.success(`Chat '${data?.title}' created successfully`))
      .catch((e) => console.log(e));
  };

  return (
    <>
      <CreateChatForm
        isChatFormOpen={isChatFormOpen}
        setIsChatFormOpen={setIsChatFormOpen}
        onSubmit={handleCreateChatFormSubmit}
      />

      <section>
        <div className="relative flex flex-col gap-8 m-16 max-w-4xl mx-auto">
          <div className="flex gap-8 lg:flex-row flex-col">
            <UserProfileCard className="w-full flex-grow" />
            <UserProfileAbout />
          </div>

          <div className="bg-slate-700 rounded-2xl shadow-lg p-8">
            <p className="font-semibold text-2xl text-white text-center mb-8">Friends</p>
            <UserProfileFriends />
          </div>
        </div>
      </section>
    </>
  );
};

export default UserProfile;
