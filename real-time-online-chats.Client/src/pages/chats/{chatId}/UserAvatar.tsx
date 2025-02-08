import { twMerge } from "tailwind-merge";

type UserAvatarProps = React.HTMLAttributes<HTMLImageElement> & {
  width?: string;
  height?: string;
};

const UserAvatar = ({ width = "48px", height = "48px", className, ...rest }: UserAvatarProps) => (
  <img
    style={{ width: width, height: height }}
    src="/images/test-profile.jpg"
    className={twMerge("w-12 h-12 rounded-full object-cover", className)}
    {...rest}
  />
);

export default UserAvatar;
