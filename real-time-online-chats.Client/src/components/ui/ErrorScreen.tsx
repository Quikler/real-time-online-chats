import { memo } from "react";

const ErrorScreen = () => {
  return (
    <div className="bg-transparent w-full flex items-center justify-center flex-grow">
      <div className="flex flex-col gap-2">
        <p className="text-3xl text-gray-300">Something got wrong...</p>
        <p className="text-xl text-gray-300">Page migt not exist or was removed</p>
      </div>
    </div>
  );
};

export default memo(ErrorScreen);
