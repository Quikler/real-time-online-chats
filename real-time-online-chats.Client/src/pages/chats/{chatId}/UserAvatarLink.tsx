import { Link } from "react-router-dom";
import UserAvatar from "./UserAvatar";

type UserAvatarLinkProps = React.HTMLAttributes<HTMLAnchorElement> & {
  userId: string;
  width?: string;
  height?: string;
};

const UserAvatarLink = ({
  userId,
  width = "48px",
  height = "48px",
  ...rest
}: UserAvatarLinkProps) => {
  return (
    <Link to={`/profile/${userId}`} {...rest}>
      <UserAvatar width={width} height={height} />
    </Link>
  );
};

export default UserAvatarLink;
