import Button from "@src/components/ui/Button";
import { useAuth } from "@src/hooks/useAuth";
import React from "react";
import { useNavigate } from "react-router-dom";
import { twMerge } from "tailwind-merge";
import { useUserProfile } from "./UserProfileContext";

type UserProfileCardProps = React.HTMLAttributes<HTMLDivElement> & {};

const UserProfileCard = ({ className, ...rest }: UserProfileCardProps) => {
  const { user } = useAuth();
  const {
    id,
    avatarUrl,
    firstName,
    lastName,
    email,
    activityStatus,
    casualStatus,
    moodStatus,
    workStatus,
    gamingStatus,
  } = useUserProfile();

  const navigate = useNavigate();

  return (
    <div
      {...rest}
      className={twMerge("bg-slate-700 rounded-2xl shadow-2xl overflow-hidden", className)}
    >
      <div className="relative h-48 bg-slate-600">
        <img
          referrerPolicy="no-referrer"
          src={avatarUrl}
          alt={`${firstName} ${lastName}`}
          className="w-48 h-48 rounded-full object-cover border-4 border-slate-700 absolute -bottom-12 left-1/2 transform -translate-x-1/2"
        />
      </div>

      <div className="p-6 pt-16 text-center">
        <h3 className="text-2xl break-all font-bold text-white">{firstName}</h3>
        <h3 className="text-2xl break-all font-bold text-white">{lastName}</h3>
        <p className="text-sm break-all text-slate-300 mt-1">{email}</p>

        <div className="grid grid-cols-3 items-center gap-2 mt-4">
          <span className="inline-block px-4 py-1 bg-slate-600 text-white text-sm rounded-full">
            {activityStatus}
          </span>
          <span className="inline-block px-4 py-1 bg-slate-600 text-white text-sm rounded-full">
            {casualStatus}
          </span>
          <span className="inline-block px-4 py-1 bg-slate-600 text-white text-sm rounded-full">
            {moodStatus}
          </span>
          <span className="inline-block px-4 py-1 bg-slate-600 text-white text-sm rounded-full">
            {workStatus}
          </span>
          <span className="inline-block px-4 py-1 bg-slate-600 text-white text-sm rounded-full">
            {gamingStatus}
          </span>
        </div>

        <div className="mt-6">
          {user?.id === id && (
            <Button type="button" onClick={() => navigate(`/profile/${user?.id}/edit`)}>
              Edit Profile
            </Button>
          )}
        </div>
      </div>
    </div>
  );
};

export default UserProfileCard;
