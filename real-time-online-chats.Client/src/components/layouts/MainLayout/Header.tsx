import { useAuth } from "@src/hooks/useAuth";
import { memo, useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { ShortArrowDown } from "@src/components/svg/SVGCommon";
import Button from "@src/components/ui/Button";
import ButtonLink from "@src/components/ui/ButtonLink";
import Logo from "@src/components/ui/Logo";

export default memo(function Header() {
  const { user, logoutUser, isUserLoggedIn } = useAuth();

  const isLinkActive = (path: string) => location.pathname === path;

  const [isScrolled, setIsScrolled] = useState(false);
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isMenuActuallyOpen, setIsMenuActuallyOpen] = useState(false);

  const [isLargeScreen, setIsLargeScreen] = useState(window.innerWidth >= 1024);

  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => setIsScrolled(window.scrollY > 0);
    const handleResize = () => setIsLargeScreen(window.innerWidth >= 1024);

    window.addEventListener("scroll", handleScroll);
    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("scroll", handleScroll);
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  const toggleMenu = () => setIsMenuOpen(!isMenuOpen);
  const toggleUserMenu = () => setIsUserMenuOpen(!isUserMenuOpen);

  useEffect(() => {
    if (isMenuOpen) {
      if (isScrolled) {
        setIsMenuActuallyOpen(true);
      } else if (isLargeScreen) {
        setIsMenuActuallyOpen(false);
      } else {
        setIsMenuActuallyOpen(true);
      }
    } else if (isScrolled) {
      setIsMenuActuallyOpen(true);
    } else {
      setIsMenuActuallyOpen(false);
    }
  }, [isMenuOpen, isScrolled, isLargeScreen]);

  const getBackgroundColor = () => {
    if (isMenuActuallyOpen) {
      return "bg-slate-800";
    }
    return "bg-transparent";
  };

  const menuButtonRef = useRef<HTMLDivElement>(null)

  const handleLogout = () => {
    logoutUser();
    setIsUserMenuOpen(false);
  }

  return (
    <header className={`fixed z-[999] w-full ${getBackgroundColor()}`}>
      <nav
        className={`bg-transparent border-gray-200 px-4 lg:px-6 py-2.5 overflow-y-auto lg:h-auto ${
          isMenuOpen && "h-screen"
        }`}
      >
        <div className="flex flex-wrap justify-between items-center mx-auto max-w-screen-xl">
          <Logo href="/" />
          <div className="flex items-center lg:order-2">
            <div className="">
              {isUserLoggedIn() ? (
                <div ref={menuButtonRef}>
                  <Button
                    onClick={toggleUserMenu}
                    type="button"
                  >
                    {user?.email}
                    <ShortArrowDown width="10" height="10" />
                  </Button>
                  {/* Dropdown menu */}
                  {isUserMenuOpen && (
                    <div style={{width: menuButtonRef.current?.clientWidth}}
                      className={`z-10 absolute bg-white divide-y divide-gray-100 shadow w-44 dark:bg-gray-700 dark:divide-gray-600 ${
                        isUserMenuOpen ? "" : "hidden"
                      }`}
                    >
                      <div className="px-4 py-3 text-sm text-white">
                        {/* <div>
                          {user?.firstName} {user?.lastName}
                        </div> */}
                        <div className="font-medium truncate">{user?.email}</div>
                      </div>
                      <ul
                        className="py-2 text-sm text-gray-200"
                      >
                        <li>
                          <Link
                            to={`/profile/${user?.id}`}
                            className="block px-4 py-2 hover:bg-gray-600"
                          >
                            Profile
                          </Link>
                        </li>
                        <li>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-600">
                            Settings
                          </a>
                        </li>
                        <li>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-600">
                            Earnings
                          </a>
                        </li>
                      </ul>
                      <div className="py-2 text-gray-200">
                        <button
                          onClick={handleLogout}
                          className="block px-4 py-2 w-full text-left hover:bg-gray-600"
                        >
                          Log out
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              ) : (
                <div className="flex gap-2 items-center">
                  <ButtonLink to="/login" className="font-medium text-sm">
                    Log in
                  </ButtonLink>
                  <ButtonLink variant="secondary" to="/signup" className="font-medium text-sm">
                    Sign up
                  </ButtonLink>
                </div>
              )}
            </div>
            <button
              onClick={toggleMenu}
              type="button"
              className="inline-flex items-center p-2 ml-1 text-sm rounded-lg lg:hidden hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-gray-200 dark:text-gray-400  dark:hover:bg-gray-700 dark:focus:ring-gray-600"
              aria-controls="mobile-menu-2"
            >
              <span className="sr-only">Open main menu</span>
              <svg
                className="w-6 h-6"
                fill="currentColor"
                viewBox="0 0 20 20"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  fillRule="evenodd"
                  d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                  clipRule="evenodd"
                ></path>
              </svg>
            </button>
          </div>

          <div
            className={`${
              isMenuOpen ? "flex justify-between" : "hidden"
            } w-full lg:flex lg:w-auto lg:order-1 px-4 lg:px-5 py-2 lg:py-2.5`}
            id="mobile-menu-2"
          >
            <div>
              <ul className="flex flex-col my-4 lg:my-0 h-auto font-medium lg:flex-row lg:space-x-8 md:h-auto">
                <li>
                  <Link
                    onClick={toggleMenu}
                    to="/"
                    className={`block py-2 pr-4 pl-3 lg:bg-transparent lg:p-0 text-gray-300 ${
                      isLinkActive("/")
                        ? ""
                        : "opacity-60 hover:opacity-100 lg:hover:bg-transparent hover:bg-slate-500"
                    }`}
                  >
                    Home
                  </Link>
                </li>
                <li>
                  <Link
                    onClick={toggleMenu}
                    to="/chats"
                    className={`block py-2 pr-4 pl-3 lg:bg-transparent lg:p-0 text-gray-300 ${
                      isLinkActive("/chats")
                        ? ""
                        : "opacity-60 hover:opacity-100 lg:hover:bg-transparent hover:bg-slate-500"
                    }`}
                  >
                    Chats
                  </Link>
                </li>
                <li>
                  <a
                    className="block py-2 pr-4 pl-3 text-gray-300 lg:hover:bg-transparent lg:border-0 lg:hover:text-primary-700 lg:p-0 hover:bg-lightGreen-100 hover:text-white"
                  >
                    Team
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </nav>
    </header>
  );
});
