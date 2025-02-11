import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";

type CheckboxProps = React.InputHTMLAttributes<HTMLInputElement> & {
  variant?: "primary" | "secondary";
};

const Checkbox = ({
  variant = "primary",
  className,
  ...rest
}: CheckboxProps) => {
	const v = useVariant(
    [
      { key: "primary", style: "accent-slate-500" },
    ],
    variant,
    "h-4 w-4 shrink-0 rounded"
  );

  return <input {...rest} className={twMerge(`${v} ${className}`)} />;
};

export default Checkbox;
