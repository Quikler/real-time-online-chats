import { Link } from "react-router-dom";

const ChatPreview = () => {
  return (
    <div className="flex flex-col gap-2">
      <div className="relative bg-darkBlue-100 p-6">
        <Link to='/chats/2ffe4a07-ea50-42d5-a309-2f172a37f0be' className="absolute px-6 lg:px-12 lg:py-1 left-6 top-3 text-white bg-lightGreen-100 hover:bg-lightGreen-200">
          Join
        </Link>
        <div className="flex flex-col gap-4 pt-6">
          <div className="flex flex-col items-end gap-1">
            <div className="flex gap-2">
              <svg
                viewBox="0 0 2 2"
                fill="white"
                xmlns="http://www.w3.org/2000/svg"
                width={16}
              >
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
              <svg
                viewBox="0 0 2 2"
                fill="white"
                xmlns="http://www.w3.org/2000/svg"
                width={16}
              >
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
      <p className="text-center text-white">Chat #1</p>
    </div>
  );
};

export default ChatPreview;
