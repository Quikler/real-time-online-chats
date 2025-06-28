import useVariant from "@src/hooks/useVariant";
import { memo } from "react";
import { twMerge } from "tailwind-merge";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "danger";
};

const Button = ({ variant = "primary", className, ...rest }: ButtonProps) => {
  const v = useVariant(
    [
      { key: "primary", style: "bg-slate-600 hover:bg-slate-500" },
      { key: "secondary", style: "bg-slate-400 hover:bg-slate-500" },
      { key: "danger", style: "bg-rose-600 hover:bg-rose-700" },
    ],
    variant,
    "text-white px-6 py-2 inline-flex items-center justify-center gap-2 disabled:opacity-50 rounded-lg transition-colors duration-300"
  );

  return <button {...rest} className={twMerge(v, className)} />;
};

export default memo(Button);
