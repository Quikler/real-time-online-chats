import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "danger";
};

export default function Button({ variant = "primary", className, ...rest }: ButtonProps) {
  const v = useVariant(
    [
      { key: "primary", style: "bg-slate-600 hover:bg-slate-700" },
      { key: "secondary", style: "bg-slate-400 hover:bg-slate-500" },
      { key: "danger", style: "bg-rose-600 hover:bg-rose-700" },
    ],
    variant,
    "inline-flex items-center justify-center gap-2 text-slate-100 focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 font-medium rounded-lg text-sm px-5 py-2.5 transition-all duration-200 shadow-lg hover:shadow-slate-400/30"
  );

  return <button {...rest} className={twMerge(v, className)} />;
}
