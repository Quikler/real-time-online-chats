import useVariant from '@src/hooks/useVariant';
import { Link, LinkProps } from 'react-router-dom'
import { twMerge } from 'tailwind-merge';

type CustomLinkProps = LinkProps & {
  variant?: "primary" | "secondary" | "danger";
};

export default function ButtonLink({ variant = "primary", className, ...rest }: CustomLinkProps) {
  const v = useVariant(
    [
      { key: "primary", style: "bg-emerald-500 hover:bg-emerald-600" },
      { key: "secondary", style: "bg-indigo-600 hover:bg-indigo-700" },
    ],
    variant,
    "text-white rounded-md py-2 px-4 px-4 lg:px-5 py-2 lg:py-2.5 font-bold"
  );
  
  return (
    <Link {...rest} className={twMerge(`${v} ${className}`)} />
  )
}