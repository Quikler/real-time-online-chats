import { Outlet } from "react-router-dom";
import { UserProfileProvider } from "./UserProfileContext";

const BaseUserProfile = () => {
  return (
    <UserProfileProvider>
      <Outlet />
    </UserProfileProvider>
  );
};

export default BaseUserProfile;
