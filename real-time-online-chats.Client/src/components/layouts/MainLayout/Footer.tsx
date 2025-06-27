import { GitHub, LinkedIn, Discord, Reddit } from "@src/components/svg/SVGSocMediaReferences";
import Logo from "@src/components/ui/Logo";
import { memo } from "react";

interface FooterItem {
  icon: JSX.Element;
  bgColor: string;
  href: string;
}

const Footer = () => {
  const references: FooterItem[] = [
    { icon: <GitHub />, bgColor: "#333333", href: "https://github.com/Quikler", },
    { icon: <LinkedIn />, bgColor: "#0082ca", href: "https://www.linkedin.com/in/roman-binykow-152895329", },
    { icon: <Discord />, bgColor: "#5665ed", href: "https://discordapp.com/users/479183227995815936", },
    { icon: <Reddit />, bgColor: "#fd4200", href: "https://www.reddit.com/user/IiQuijlerIi", }
  ];

  return (
    <footer className="w-full py-14 bg-slate-800 text-white">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="max-w-3xl mx-auto">
          <Logo className="flex justify-center" />
          <ul className="text-lg flex items-center justify-center flex-col gap-7 md:flex-row md:gap-12 transition-all duration-500 py-16 mb-10 border-b border-gray-200">
            <li>
              <a href="#">Pagedone</a>
            </li>
            <li>
              <a href="#">Products</a>
            </li>
            <li>
              <a href="#">Resources</a>
            </li>
            <li>
              <a href="#">Blogs</a>
            </li>
            <li>
              <a href="#">Support</a>
            </li>
          </ul>
          <div className="flex space-x-10 justify-center items-center mb-14">
            {references.map((value, index) => {
              return (
                <a
                  key={index}
                  href={value.href} target="_blank"
                  type="button"
                  style={{ backgroundColor: value.bgColor }}
                  className="rounded-full relative bg-white p-3 uppercase leading-normal text-surface shadow-dark-3 shadow-black/30 transition duration-150 ease-in-out hover:shadow-dark-1 focus:shadow-dark-1 focus:outline-none focus:ring-0 active:shadow-1 dark:bg-neutral-700 dark:text-white"
                  data-twe-ripple-init=""
                  data-twe-ripple-color="light"
                >
                  <span className="mx-auto [&>svg]:h-5 [&>svg]:w-5">
                    {value.icon}
                  </span>
                </a>
              );
            })}
          </div>
          <span className="text-lg text-gray-500 text-center block">
            ©<a href="/">ROC</a> 2025, All rights reserved.
          </span>
        </div>
      </div>
    </footer>
  );
};

export default memo(Footer);
