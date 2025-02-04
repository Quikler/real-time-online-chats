import CreateChatForm, { CreateChatFormData } from "@src/pages/chats/CreateChatForm";
import { ChatService } from "@src/services/api/ChatService";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { useParams } from "react-router-dom";
import FriendPreview from "./FriendPreview";
import UserCard from "./UserCard";
import { UserService } from "@src/services/api/UserService";
import { UserProfileType } from "./types";

const UserProfile = () => {
  const [isChatFormOpen, setIsChatFormOpen] = useState(false);

  const { userId } = useParams<{ userId: string }>();

  const [userProfile, setUserProfile] = useState<UserProfileType>();

  useEffect(() => {
    if (!userId) return;

    const abortController = new AbortController();

    UserService.getUserProfile(userId, { signal: abortController.signal })
      .then((data) => {
        setUserProfile(data);
      })
      .catch((e) => console.error("Error fetching profile:", e.message));

    return () => abortController.abort();
  }, [userId]);

  const handleChatFormSubmit = async (
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
        onSubmit={handleChatFormSubmit}
      />

      <section className="relative flex flex-col gap-8 m-16 max-w-7xl mx-auto">
        <div className="flex gap-8 lg:flex-row flex-col">
          <UserCard
            className="w-full flex-grow"
            firstName={userProfile?.firstName}
            lastName={userProfile?.lastName}
            email={userProfile?.email}
            activityStatus={userProfile?.activityStatus}
            casualStatus={userProfile?.casualStatus}
            moodStatus={userProfile?.moodStatus}
            workStatus={userProfile?.workStatus}
            gamingStatus={userProfile?.gamingStatus}
            avatarUrl="/images/test-profile.jpg"
            socialLinks={{ github: "test", facebook: "" }}
          />

          <div className="flex w-full flex-grow flex-col gap-6 p-8 bg-slate-700 rounded-2xl shadow-lg">
            <section className="relative flex flex-col gap-12 m-auto">
              <h2 className="text-4xl font-bold text-white text-center">About Me</h2>
              <div className="flex gap-8 lg:flex-row flex-col">
                <div className="w-full flex flex-col gap-6 p-8 bg-slate-600 rounded-2xl shadow-lg">
                  <p className="text-white leading-relaxed text-opacity-90">
                    {userProfile?.aboutMe != null ? (
                      <>{userProfile.aboutMe}</>
                    ) : (
                      <>There is nothing yet...</>
                    )}
                  </p>
                </div>
              </div>
            </section>
          </div>
        </div>

        <div className="bg-slate-700 rounded-2xl shadow-lg p-8">
          <p className="font-semibold text-2xl text-white text-center mb-8">Friends</p>
          <ul className="flex flex-col gap-6">
            {userProfile?.friends.map((value, index) => (
              <li key={index}>
                <FriendPreview
                  firstName={value.firstName}
                  lastName={value.lastName}
                  email={value.email}
                  id={value.id}
                />
              </li>
            ))}
          </ul>
        </div>
      </section>
    </>
  );
};

export default UserProfile;
