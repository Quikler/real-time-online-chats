import { Facebook } from "@src/components/svg/SVGAuthProviders";
import { GitHub } from "@src/components/svg/SVGSocMediaReferences";
import { useAuth } from "@src/hooks/useAuth";
import React from "react";
import { useNavigate } from "react-router-dom";
import { twMerge } from "tailwind-merge";

type ProfileUserCard = React.HTMLAttributes<HTMLDivElement> & {
  firstName?: string;
  lastName?: string;
  email?: string;
  activityStatus?: string;
  casualStatus?: string;
  moodStatus?: string;
  workStatus?: string;
  gamingStatus?: string;
  avatarUrl?: string;
  socialLinks: {
    github: string;
    facebook: string;
  };
};

const ProfileUserCard = ({
  firstName,
  lastName,
  email,
  activityStatus,
  casualStatus,
  moodStatus,
  workStatus,
  gamingStatus,
  avatarUrl,
  socialLinks,
  className,
  ...rest
}: ProfileUserCard) => {
  const { user } = useAuth();

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
        <h3 className="text-2xl font-bold text-white">
          {firstName} {lastName}
        </h3>
        <p className="text-sm text-slate-300 mt-1">{email}</p>

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

        <div className="flex justify-center gap-4 mt-6">
          <a
            href={socialLinks.github}
            target="_blank"
            rel="noopener noreferrer"
            className="text-white hover:text-slate-300 transition-colors duration-300"
          >
            <GitHub className="w-6 h-6" />
          </a>
          <a
            href={socialLinks.facebook}
            target="_blank"
            rel="noopener noreferrer"
            className="text-blue-500 hover:text-blue-400 transition-colors duration-300"
          >
            <Facebook className="w-6 h-6" />
          </a>
        </div>

        <div className="mt-6">
          <button
            type="button"
            onClick={() => navigate(`/profile/${user?.id}/edit`)}
            className="bg-slate-600 text-white px-6 py-2 rounded-lg hover:bg-slate-500 transition-colors duration-300"
          >
            Edit Profile
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProfileUserCard;
