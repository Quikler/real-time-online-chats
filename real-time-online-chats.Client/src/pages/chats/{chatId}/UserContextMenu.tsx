import { Link } from "react-router-dom";

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
      <ul className="divide-y divide-slate-600">
        <li>
          <button className="w-full px-6 py-3 text-white hover:bg-slate-600 transition-colors duration-200 text-left">
            <Link to={`/profile/${userId}`}>Visit profile</Link>
          </button>
        </li>
      </ul>
    </div>
  );
};

export default UserContextMenu;
