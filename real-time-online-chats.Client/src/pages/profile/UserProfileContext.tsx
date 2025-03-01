import React, { useContext, useEffect, useMemo, useState } from "react";
import { UserProfileType } from "./profile.types";
import { UserService } from "@src/services/api/UserService";
import { useParams } from "react-router-dom";
import { LoaderScreen } from "@src/components/ui/Loader";
import ErrorScreen from "@src/components/ui/ErrorScreen";

type UserProfileContextType = UserProfileType & {
  refreshUser: () => Promise<void>;
};

const UserProfileContext = React.createContext({} as UserProfileContextType);

type UserProfileProviderProps = { children: React.ReactNode };

export const UserProfileProvider = ({ children }: UserProfileProviderProps) => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  const [userProfile, setUserProfile] = useState<UserProfileType | undefined | null>();

  const { userId } = useParams<{ userId: string }>();

  const fetchUserProfile = async (signal?: AbortSignal) => {
    if (!userId) return;

    await UserService.getUserProfile(userId, { signal: signal })
      .then((data) => {
        if (data) {
          setUserProfile(data);
          setLoading(false);
        }
      })
      .catch((e) => {
        console.error("Error fetching profile:", e.message);
        setUserProfile(null);
        setLoading(false);
        setError(true);
      });
  };

  useEffect(() => {
    if (!userId) return;

    const abortController = new AbortController();

    fetchUserProfile(abortController.signal);

    return () => abortController.abort();
  }, [userId]);

  const refreshUser = async () => await fetchUserProfile();

  const value = useMemo(
    () => ({
      ...userProfile!,
      refreshUser,
    }),
    [userProfile]
  );

  if (loading) return <LoaderScreen />;
  if (error) return <ErrorScreen />;

  return <UserProfileContext.Provider value={value}>{children}</UserProfileContext.Provider>;
};

export const useUserProfile = () => {
  const userProfileContext = useContext(UserProfileContext);

  if (!userProfileContext) {
    throw new Error("useUserProfile must be used within a UserProfileContextProvider");
  }

  return userProfileContext;
};
