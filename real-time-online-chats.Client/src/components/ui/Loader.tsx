import React from "react";
import "./Loader.css";

interface LoaderProps extends React.HTMLAttributes<HTMLDivElement> {
  size?: string;
}

const Loader = ({ size, ...rest }: LoaderProps) => (
  <div style={{ width: size, height: size }} className="loader" {...rest}></div>
);

export default Loader;

export const LoaderScreen = ({ size = "256px", ...rest }: LoaderProps) => (
  <div className="bg-darkBlue-100 flex items-center justify-center flex-grow">
    <Loader size={size} {...rest} />
  </div>
);
