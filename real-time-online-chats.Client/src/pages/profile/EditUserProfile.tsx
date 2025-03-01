import { useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import FriendPreview from "./FriendPreview";
import { UserService } from "@src/services/api/UserService";
import { EditUserProfileType } from "./profile.types";
import EditProfileUserCard from "./EditProfileUserCard";
import { useUserProfile } from "./UserProfileContext";
import ErrorScreen from "@src/components/ui/ErrorScreen";

const EditUserProfile = () => {
  const { refreshUser, friends, ...userProfile } = useUserProfile();
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();

  const [editUserProfile, setEditUserProfile] = useState<EditUserProfileType>(userProfile);

  const isChanged = useMemo(() => {
    return JSON.stringify(userProfile) !== JSON.stringify(editUserProfile);
  }, [userProfile, editUserProfile]);

  if (!userId) return <ErrorScreen />;

  const handleEditFormSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      await UserService.updateUserProfile(userId, editUserProfile);
      await refreshUser();
      console.log("Updated profile successfully");
    } catch (e: any) {
      console.error("Error updating profile:", e.message);
    }

    navigate(-1);
  };

  const handleInputChange = (field: keyof EditUserProfileType, value: any) => {
    setEditUserProfile((prev) => ({ ...prev, [field]: value }));
  };

  return (
    <>
      <section>
        <form
          onSubmit={handleEditFormSubmit}
          className="relative flex flex-col gap-8 m-16 max-w-4xl mx-auto"
        >
          <div className="flex gap-8 lg:flex-row flex-col">
            <EditProfileUserCard
              isSubmitButtonEnabled={isChanged}
              onAvatarChange={(value) => handleInputChange("avatar", value)}
              className="w-full flex-grow"
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
                      value={editUserProfile.aboutMe}
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
