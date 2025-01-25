import { useState } from "react";
import CreateChatForm, {
  CreateChatFormData,
} from "../pages/chats/create-chat-form";
import { ChatService } from "../../services/api/chat-service";
import { toast } from "react-toastify";

const UserProfile = () => {
  const [isChatFormOpen, setIsChatFormOpen] = useState(false);

  const handleChatFormSubmit = (
    e: React.FormEvent<HTMLFormElement>,
    data: CreateChatFormData
  ) => {
    ChatService.createChat(data).subscribe({
      next: (response) => {
        toast.success("Created chat successfully");
      },
      error: (error) => toast.error(error),
    });
  };

  return (
    <>
      <CreateChatForm
        isChatFormOpen={isChatFormOpen}
        setIsChatFormOpen={setIsChatFormOpen}
        onSubmit={handleChatFormSubmit}
      />

      <section className="relative pt-36 pb-24">
        <div className="w-full bg-darkBlue-200 absolute top-0 left-0 z-0 h-60 object-cover" />
        <div className="w-full max-w-7xl mx-auto px-6 md:px-8">
          <div className="flex items-center justify-center relative z-10 mb-2.5">
            <img
              src="/images/test-profile.jpg"
              alt="user-avatar-image"
              className="border-4 border-solid w-56 h-56 border-white rounded-full object-cover"
            />
          </div>
          <div className="flex flex-col sm:flex-row max-sm:gap-5 items-center justify-between mb-5">
            <div className="flex items-center gap-4 mx-auto">
              <button
                onClick={() => setIsChatFormOpen(!isChatFormOpen)}
                className="rounded-lg font-medium hover:bg-lightGreen-200 bg-lightGreen-100 py-3 px-4 text-white"
              >
                Create chat
              </button>

              <button className="rounded-lg font-medium hover:bg-purple-200 bg-purple-100 py-3 px-4 text-white">
                Edit Profile
              </button>
            </div>
          </div>
          <h3 className="text-center font-bold text-3xl leading-10 text-gray-900 mb-3">
            Dante Sparda
          </h3>
          <p
            className="font-normal text-base leading-7 text-gray-500 text-justify mb-8"
            style={{ textAlignLast: "center" }}
          >
            A social media influencer and singer i vopshe pizdaty chuvak
          </p>
          <div className="flex flex-col items-center justify-center gap-5">
            <p className="text-3xl leading-10 text-gray-900 ">Owner in chats</p>
          </div>
        </div>
      </section>
    </>
  );
};

export default UserProfile;
