import { Link } from "react-router-dom";

export interface LogoProps extends React.HTMLAttributes<HTMLDivElement> {
  scale?: number;
  href?: string;
}

export default function Logo({ scale = 1, href = "", className, ...rest }: LogoProps) {
  return (
    <div className={className} {...rest}>
      <Link
        to={href}
        className={`flex items-center ${
          href ? "cursor-pointer" : "cursor-default"
        }`}
      >
        <svg height={scale * 25} width={scale * 25}>
          <ellipse rx={scale * 25} ry={scale * 25} style={{ fill: "yellow" }} />
        </svg>
        <span
          style={{ fontSize: `${1.25 * scale}rem` }}
          className="self-center font-semibold whitespace-nowrap text-white"
        >
          ROC
        </span>
      </Link>
    </div>
  );
}
