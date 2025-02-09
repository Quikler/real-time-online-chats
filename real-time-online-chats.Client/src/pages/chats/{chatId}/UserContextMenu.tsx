import ButtonLink from "@src/components/ui/ButtonLink";

type UserContextMenuProps = React.HTMLAttributes<HTMLDivElement> & {
  isVisible: boolean;
  position: { x: number; y: number };
  userId: string;
};

const UserContextMenu = ({ isVisible, position, userId, ...rest }: UserContextMenuProps) => {
  if (!isVisible) return null;

  return (
    <div
      {...rest}
      style={{
        top: position.y,
        left: position.x,
      }}
      className="absolute z-[999] bg-slate-700 rounded-lg shadow-lg overflow-hidden border border-slate-600"
    >
    <ButtonLink to={`/profile/${userId}`}>Visit profile</ButtonLink>
    </div>
  );
};

export default UserContextMenu;
