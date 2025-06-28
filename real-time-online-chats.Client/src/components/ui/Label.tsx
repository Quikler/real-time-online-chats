import useVariant from "@src/hooks/useVariant";
import { memo } from "react";
import { twMerge } from "tailwind-merge";

type LabelProps = React.HTMLAttributes<HTMLLabelElement> & {
  variant?: "primary";
};

const Label = ({ variant = "primary", className, ...rest }: LabelProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "text-gray-800 font-medium",
      },
    ],
    variant,
    "text-sm block"
  );

  return <label className={twMerge(v, className)} {...rest} />;
};

export default memo(Label);
