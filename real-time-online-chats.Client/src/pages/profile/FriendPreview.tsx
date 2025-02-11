import { Link } from "react-router-dom";

type FriendPreviewProps = {
  firstName: string;
  lastName: string;
  email: string;
  id: string;
  avatarUrl: string;
};

const FriendPreview = ({ firstName, lastName, email, id, avatarUrl }: FriendPreviewProps) => {
  return (
    <div className="bg-slate-600 p-6 flex gap-6 rounded-2xl shadow-lg hover:shadow-xl transition-shadow duration-300">
      <img
        src={avatarUrl}
        alt="Profile"
        className="object-cover rounded-full w-20 h-20 border-4 border-slate-600"
      />
      <div className="flex flex-col gap-2 flex-1 overflow-hidden">
        <Link to={`/profile/${id}`} className="text-white text-2xl font-semibold truncate">
          {firstName && lastName ? (
            <>
              {firstName} {lastName}
            </>
          ) : (
            <>{email}</>
          )}
        </Link>

        <p className="text-white text-opacity-80 text-sm truncate">
          Online | "Always up for a chat! Let's connect and share ideas."
        </p>

        <div className="flex gap-3 mt-2">
          <button className="bg-slate-700 text-white px-4 py-2 rounded-lg hover:bg-slate-500 transition-colors duration-300">
            Message
          </button>
          <button className="bg-slate-700 text-white px-4 py-2 rounded-lg hover:bg-slate-500 transition-colors duration-300">
            Remove
          </button>
        </div>
      </div>
    </div>
  );
};

export default FriendPreview;
