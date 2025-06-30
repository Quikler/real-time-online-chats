import { useNavigate } from "react-router-dom";
import Button from "../../components/ui/Button";
import { memo } from "react";

type ChatPreviewProps = React.HTMLAttributes<HTMLDivElement> & {
  title: string;
  id: string;
};

const ChatPreview = ({ id, title, ...rest }: ChatPreviewProps) => {
  const navigate = useNavigate();

  const handleJoinChatClick = (_e: React.MouseEvent<HTMLButtonElement>) => navigate(`/chats/${id}`);

  return (
    <div className="flex flex-col gap-4" {...rest}>
      <div className="relative bg-slate-600 p-6 rounded-3xl shadow-lg">
        <div className="flex flex-col gap-6">
          <Button
            className="self-start bg-slate-500 text-white hover:bg-slate-500 transition-colors duration-300"
            onClick={handleJoinChatClick}
            type="submit"
          >
            Join
          </Button>

          {/*<div className="flex flex-col gap-4">
            <div className="flex flex-col items-end gap-2">
              <div className="flex items-center gap-2">
                <svg viewBox="0 0 2 2" fill="white" xmlns="http://www.w3.org/2000/svg" width={16}>
                  <circle cx="1" cy="1" r="1" />
                </svg>
                <p className="text-white text-sm">User1</p>
              </div>
              <div className="flex flex-col gap-2">
                <div className="bg-slate-600 text-white py-2 px-4 rounded-lg max-w-[80%]">
                  <p>Hello guys</p>
                </div>
                <div className="bg-slate-600 text-white py-2 px-4 rounded-lg max-w-[80%]">
                  <p>How are you? &lt;3</p>
                </div>
              </div>
            </div>

            <div className="flex flex-col items-start gap-2">
              <div className="flex items-center gap-2">
                <svg viewBox="0 0 2 2" fill="white" xmlns="http://www.w3.org/2000/svg" width={16}>
                  <circle cx="1" cy="1" r="1" />
                </svg>
                <p className="text-white text-sm">User2</p>
              </div>
              <div className="bg-slate-600 text-white py-2 px-4 rounded-lg max-w-[80%]">
                <p>
                  Hello User1! I'm awesome.
                  <br /> How are you?
                </p>
              </div>
            </div>
          </div>*/}
         <div className="flex flex-col gap-4">
         {title}
         </div>
        </div>
      </div>
      <p className="text-center text-white text-lg font-semibold">{title}</p>
    </div>
  );
};

export default memo(ChatPreview);
