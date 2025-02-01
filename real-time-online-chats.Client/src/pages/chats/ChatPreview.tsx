import { ChatService } from "@src/services/api/ChatService";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import Button from "../../components/ui/Button";

type ChatPreviewProps = React.HTMLAttributes<HTMLDivElement> & {
  title: string;
  id: string;
};

const ChatPreview = ({ id, title, ...rest }: ChatPreviewProps) => {
  const navigate = useNavigate();

  const handleJoinChatClick = (_e: React.MouseEvent<HTMLButtonElement>) => {
    ChatService.joinChat(id)
      .then(() => {
        toast.success("Joined chat successfully");
        navigate(`/chats/${id}`);
      })
      .catch((e) => console.log(e));
  };

  return (
    <div className="flex flex-col gap-4" {...rest}>
      {/* Chat Preview Card */}
      <div className="relative bg-slate-700 p-6 rounded-3xl shadow-lg"> {/* 30% slate-700 */}
        <div className="flex flex-col gap-6">
          {/* Join Button */}
          <Button
            className="self-start bg-slate-600 text-white hover:bg-slate-500 transition-colors duration-300" // 60% slate-600
            onClick={handleJoinChatClick}
            type="submit"
          >
            Join
          </Button>

          {/* Chat Messages */}
          <div className="flex flex-col gap-4">
            {/* User 1 Messages */}
            <div className="flex flex-col items-end gap-2">
              <div className="flex items-center gap-2">
                <svg viewBox="0 0 2 2" fill="white" xmlns="http://www.w3.org/2000/svg" width={16}>
                  <circle cx="1" cy="1" r="1" />
                </svg>
                <p className="text-white text-sm">User1</p> {/* 10% white */}
              </div>
              <div className="flex flex-col gap-2">
                <div className="bg-slate-600 text-white py-2 px-4 rounded-lg max-w-[80%]"> {/* 60% slate-600 */}
                  <p>Hello guys</p>
                </div>
                <div className="bg-slate-600 text-white py-2 px-4 rounded-lg max-w-[80%]"> {/* 60% slate-600 */}
                  <p>How are you? &lt;3</p>
                </div>
              </div>
            </div>

            {/* User 2 Messages */}
            <div className="flex flex-col items-start gap-2">
              <div className="flex items-center gap-2">
                <svg viewBox="0 0 2 2" fill="white" xmlns="http://www.w3.org/2000/svg" width={16}>
                  <circle cx="1" cy="1" r="1" />
                </svg>
                <p className="text-white text-sm">User2</p> {/* 10% white */}
              </div>
              <div className="bg-slate-600 text-white py-2 px-4 rounded-lg max-w-[80%]"> {/* 60% slate-600 */}
                <p>
                  Hello User1! I'm awesome.
                  <br /> How are you?
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Chat Title */}
      <p className="text-center text-white text-lg font-semibold">{title}</p> {/* 10% white */}
    </div>
  );
};

export default ChatPreview;