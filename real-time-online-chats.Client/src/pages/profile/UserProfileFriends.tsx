import FriendPreview from "./FriendPreview";
import { useUserProfile } from "./UserProfileContext";

const UserProfileFriends = () => {
  const { friends } = useUserProfile();

  return (
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
  );
};

export default UserProfileFriends;
