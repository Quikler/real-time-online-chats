import { ShortArrowDown } from "@src/components/svg/SVGCommon";
import { scrollToBottomOfBody } from "@src/utils/helpers";
import { useChat } from "./ChatContext";

type CountOfNewMessagesProps = {
  onCountOfNewMessagesReset?: () => void;
};

const CountOfNewMessages = ({ onCountOfNewMessagesReset }: CountOfNewMessagesProps) => {
  const { countOfNewMessages, setCountOfNewMessages } = useChat();

  const onClick = () => {
    scrollToBottomOfBody();
    setCountOfNewMessages(0);

    onCountOfNewMessagesReset?.();
  };

  return (
    <>
      {countOfNewMessages !== 0 && (
        <button
          onClick={onClick}
          className="fixed text-white left-8 bottom-24 bg-slate-800 w-16 h-16 rounded-full"
        >
          <div className="h-full relative">
            <div className="h-full flex items-center justify-center">
              <ShortArrowDown width={12} height={12} />
            </div>
            <div className="absolute right-0 top-0">
              <div className="bg-red-500 rounded-full py-1 px-3">{countOfNewMessages}</div>
            </div>
          </div>
        </button>
      )}
    </>
  );
};

export default CountOfNewMessages;
