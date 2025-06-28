import { memo } from "react";
import { twMerge } from "tailwind-merge";

type UserAvatarProps = React.HTMLAttributes<HTMLImageElement> & {
  width?: string;
  height?: string;
  avatarUrl: string;
};

const UserAvatar = ({ width = "48px", height = "48px", avatarUrl, className, ...rest }: UserAvatarProps) => (
  <img
    style={{ width: width, height: height }}
    src={avatarUrl}
    className={twMerge("w-12 h-12 rounded-full object-cover", className)}
    {...rest}
  />
);

export default memo(UserAvatar);
