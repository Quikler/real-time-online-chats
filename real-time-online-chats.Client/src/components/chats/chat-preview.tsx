import { ChatService } from "@src/services/api/chat-service";
import { getRandomColor, getRandomNumber } from "@src/utils/helpers";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import Button from "../ui/Button";

type ChatPreviewProps = React.HTMLAttributes<HTMLDivElement> & {
  title: string;
  id: string;
};

const ChatPreview = ({ id, title, ...rest }: ChatPreviewProps) => {
  const navigate = useNavigate();

  const gradientCount = getRandomNumber(1, 3);

  let randomGradient = `linear-gradient(${getRandomNumber(
    0,
    360
  )}deg, ${getRandomColor()}, ${getRandomColor()}),`;

  for (let index = 0; index < gradientCount; index++) {
    randomGradient += ` linear-gradient(${getRandomNumber(
      0,
      360
    )}deg, ${getRandomColor()}, ${getRandomColor()})`;
    if (index + 1 < gradientCount) randomGradient += ", ";
  }

  const handleJoinChatClick = (_e: React.MouseEvent<HTMLButtonElement>) => {
    ChatService.joinChat(id)
      .then(() => {
        toast.success("Joined chat successfully");
        navigate(`/chats/${id}`);
      })
      .catch((e) => console.log(e));
  };

  return (
    <div className="flex flex-col gap-2" {...rest}>
      <div
        className="relative bg-darkBlue-100 p-6"
        style={{ borderRadius: "32px", background: randomGradient }}
      >
        <div className="flex flex-col gap-4">
          <Button className="self-start" onClick={handleJoinChatClick} type="submit">
            Join
          </Button>
          <div className="flex flex-col items-end gap-1">
            <div className="flex gap-2">
              <svg viewBox="0 0 2 2" fill="white" xmlns="http://www.w3.org/2000/svg" width={16}>
                <circle cx="1" cy="1" r="1" />
              </svg>
              <p className="text-white">User1</p>
            </div>
            <div className="flex flex-col gap-2">
              <div className="bg-white py-2 px-3">
                <p>Hello guys</p>
              </div>
              <div className="bg-white py-2 px-3">
                <p>How are you? &lt;3</p>
              </div>
            </div>
          </div>
          <div className="flex flex-col items-start gap-1">
            <div className="flex gap-2">
              <svg viewBox="0 0 2 2" fill="white" xmlns="http://www.w3.org/2000/svg" width={16}>
                <circle cx="1" cy="1" r="1" />
              </svg>
              <p className="text-white">User1</p>
            </div>
            <div className="bg-white py-2 px-3">
              <p>
                Hello User1! I'm awesome.
                <br /> How are you?
              </p>
            </div>
          </div>
        </div>
      </div>
      <p className="text-center text-white">{title}</p>
    </div>
  );
};

export default ChatPreview;
