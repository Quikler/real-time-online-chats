import { Facebook } from "@src/components/svg/SVGAuthProviders";
import { GitHub } from "@src/components/svg/SVGSocMediaReferences";
import Button from "@src/components/ui/Button";
import React, { useRef } from "react";
import { twMerge } from "tailwind-merge";

type EditProfileUserCardProps = React.HTMLAttributes<HTMLDivElement> & {
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
  onActivityStatusChange: (value: string) => void;
  onCasualStatusChange: (value: string) => void;
  onMoodStatusChange: (value: string) => void;
  onWorkStatusChange: (value: string) => void;
  onGamingStatusChange: (value: string) => void;
  onAvatarChange: (file: File) => void;
};

const EditProfileUserCard = ({
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
  onActivityStatusChange,
  onCasualStatusChange,
  onMoodStatusChange,
  onWorkStatusChange,
  onGamingStatusChange,
  onAvatarChange,
  ...rest
}: EditProfileUserCardProps) => {
  const inputFileRef = useRef<HTMLInputElement>(null);
  const avatarRef = useRef<HTMLImageElement>(null);

  const handleChooseAvatar = () => inputFileRef.current?.click();

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const file = e.target.files[0];

      const reader = new FileReader();
      reader.onload = function (e) {
        if (avatarRef.current) {
          const src = e?.target?.result as string;
          avatarRef.current.src = src;
          onAvatarChange(file);
        }
      };
      reader.readAsDataURL(file);
    }
  };

  return (
    <div
      {...rest}
      className={twMerge("bg-slate-700 rounded-2xl shadow-2xl overflow-hidden", className)}
    >
      <div className="relative h-48 bg-slate-600">
        <img
          ref={avatarRef}
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

        <div className="py-2">
          <Button type="button" onClick={handleChooseAvatar}>
            Choose avatar
          </Button>
          <input
            accept="image/*"
            onChange={handleFileChange}
            className="hidden"
            type="file"
            ref={inputFileRef}
          />
        </div>

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
          <button className="bg-slate-600 text-white px-6 py-2 rounded-lg hover:bg-slate-500 transition-colors duration-300">
            Submit
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditProfileUserCard;
