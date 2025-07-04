import useVariant from "@src/hooks/useVariant";
import { memo } from "react";
import { twMerge } from "tailwind-merge";

type InputProps = React.InputHTMLAttributes<HTMLInputElement> & {
  variant?: "primary" | "secondary";
};

const Input = ({ variant = "primary", className, ...rest }: InputProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "",
      },
      {
        key: "secondary",
        style: "opacity-50",
      },
    ],
    variant,
    "bg-gray-100 text-gray-800 px-4 py-3 rounded-md outline-blue-500"
  );

  return <input {...rest} className={twMerge(v, className)} />;
};

export default memo(Input);
