import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "danger";
};

export default function Button({ variant = "primary", className, ...rest }: ButtonProps) {
  const v = useVariant(
    [
      { key: "primary", style: "bg-emerald-500 hover:bg-emerald-600" },
      { key: "secondary", style: "bg-indigo-600 hover:bg-indigo-700" },
      { key: "danger", style: "bg-rose-600 hover:bg-rose-700" },
    ],
    variant,
    "text-white rounded-md py-2 px-4 px-4 lg:px-5 py-2 lg:py-2.5 font-bold"
  );

  return <button {...rest} className={twMerge(`${v} ${className}`)} />;
}
