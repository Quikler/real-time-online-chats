import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import FriendPreview from "./FriendPreview";
import { UserService } from "@src/services/api/UserService";
import { EditUserProfileType, UserFriendType, UserProfileType } from "./profile.types";
import EditProfileUserCard from "./EditProfileUserCard";

const EditUserProfile = () => {
  const { userId } = useParams<{ userId: string }>();

  if (!userId) return;

  const [userProfile, setUserProfile] = useState<UserProfileType>({
    id: "",
    email: "",
    firstName: "",
    lastName: "",
    aboutMe: "",
    activityStatus: "",
    casualStatus: "",
    moodStatus: "",
    workStatus: "",
    gamingStatus: "",
    avatarUrl: "",
  });

  const [editUserProfile, setEditUserProfile] = useState<EditUserProfileType>({
    aboutMe: "",
    activityStatus: "",
    casualStatus: "",
    moodStatus: "",
    workStatus: "",
    gamingStatus: "",
  });

  const [friends, setFriends] = useState<UserFriendType[]>();

  useEffect(() => {
    if (!userId) return;

    const abortController = new AbortController();

    UserService.getUserProfile(userId, { signal: abortController.signal })
      .then((data) => {
        if (data) {
          setUserProfile(data);
          setFriends(data.friends);
        }
      })
      .catch((e) => console.error("Error fetching profile:", e.message));

    return () => abortController.abort();
  }, [userId]);

  const handleEditFormSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    console.log("Edit data:", editUserProfile);

    UserService.updateUserProfile(userId, editUserProfile)
      .then(() => {
        console.log("Updated profile successfully");
      })
      .catch((e) => console.error("Error updating progile:", e.message));
  };

  const handleInputChange = (field: keyof UserProfileType, value: string) => {
    setEditUserProfile((prev) => ({ ...prev, [field]: value }));
  };

  if (!userProfile) return;

  return (
    <>
      <section>
        <form
          onSubmit={handleEditFormSubmit}
          className="relative flex flex-col gap-8 m-16 max-w-4xl mx-auto"
        >
          <div className="flex gap-8 lg:flex-row flex-col">
            <EditProfileUserCard
              onAvatarChange={(file) => {
                setEditUserProfile({ ...editUserProfile, avatar: file });
              }}
              className="w-full flex-grow"
              firstName={userProfile?.firstName}
              lastName={userProfile?.lastName}
              email={userProfile?.email}
              activityStatus={userProfile?.activityStatus}
              casualStatus={userProfile?.casualStatus}
              moodStatus={userProfile?.moodStatus}
              workStatus={userProfile?.workStatus}
              gamingStatus={userProfile?.gamingStatus}
              avatarUrl={userProfile?.avatarUrl}
              socialLinks={{ github: "test", facebook: "" }}
              onActivityStatusChange={(value) => handleInputChange("activityStatus", value)}
              onCasualStatusChange={(value) => handleInputChange("casualStatus", value)}
              onMoodStatusChange={(value) => handleInputChange("moodStatus", value)}
              onWorkStatusChange={(value) => handleInputChange("workStatus", value)}
              onGamingStatusChange={(value) => handleInputChange("gamingStatus", value)}
            />

            <div className="flex w-full flex-grow flex-col gap-6 p-8 bg-slate-700 rounded-2xl shadow-lg">
              <section className="relative flex flex-col gap-12 m-auto">
                <h2 className="text-4xl font-bold text-white text-center">About Me</h2>
                <div className="flex gap-8 lg:flex-row flex-col">
                  <div className="w-full flex flex-col gap-6 ">
                    <textarea
                      name="aboutMe"
                      onChange={(e) => handleInputChange("aboutMe", e.target.value)}
                      placeholder="Write about yourself..."
                      className="rounded-2xl shadow-lg focus:outline-none min-h-96 p-8 text-white bg-slate-600 leading-relaxed text-opacity-90"
                    />
                  </div>
                </div>
              </section>
            </div>
          </div>

          <div className="bg-slate-700 rounded-2xl shadow-lg p-8">
            <p className="font-semibold text-2~xl text-white text-center mb-8">Friends</p>
            <ul className="flex flex-col gap-6">
              {friends?.map((value, index) => (
                <li key={index}>
                  <FriendPreview
                    avatarUrl={value.avatarUrl}
                    firstName={value.firstName}
                    lastName={value.lastName}
                    email={value.email}
                    id={value.id}
                  />
                </li>
              ))}
            </ul>
          </div>
        </form>
      </section>
    </>
  );
};

export default EditUserProfile;
