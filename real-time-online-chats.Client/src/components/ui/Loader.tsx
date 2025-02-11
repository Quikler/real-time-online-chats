import React from "react";

interface LoaderProps extends React.HTMLAttributes<HTMLDivElement> {
  size?: string;
}

const Loader = ({ size, ...rest }: LoaderProps) => {
  return (
    <div
      className="border-[16px] border-gray-200 border-t-slate-600 rounded-full animate-spin"
      style={{ width: size, height: size }}
      {...rest}
    ></div>
  );
};

export default Loader;

export const LoaderScreen = ({ size = "256px", ...rest }: LoaderProps) => (
  <div className="bg-transparent w-full flex items-center justify-center flex-grow">
    <Loader size={size} {...rest} />
  </div>
);
