import { useEffect, useState } from "react";
import Logo from "./common/logo";
import { Link, useLocation } from "react-router-dom";
import { useAuth } from "../contexts/auth-context";

export default function Header() {
  const { user, logoutUser, isUserLoggedIn } = useAuth();

  const location = useLocation();

  const isLinkActive = (path: string) => location.pathname === path;

  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => setIsScrolled(window.scrollY > 0);

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
    <header
      className={`fixed z-[999] w-full ${
        isScrolled || isMenuOpen ? "bg-darkBlue-200" : "bg-transparent"
      }`}
    >
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
                <div>
                  <button
                    onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                    id="dropdownInformationButton"
                    data-dropdown-toggle="dropdownInformation"
                    className="text-white bg-purple-100 hover:bg-purple-200 focus:outline-none font-medium rounded-lg text-sm px-4 lg:px-5 py-2 lg:py-2.5 text-center inline-flex items-center"
                    type="button"
                  >
                    {user?.email}
                    <svg
                      className="w-2.5 h-2.5 ms-3"
                      aria-hidden="true"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 10 6"
                    >
                      <path
                        stroke="currentColor"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="m1 1 4 4 4-4"
                      />
                    </svg>
                  </button>
                  {/* Dropdown menu */}
                  {isUserMenuOpen && (
                    <div
                      id="dropdownInformation"
                      className={`z-10 absolute bg-white divide-y divide-gray-100 shadow w-44 dark:bg-gray-700 dark:divide-gray-600 ${
                        isUserMenuOpen ? "" : "hidden"
                      }`}
                    >
                      <div className="px-4 py-3 text-sm text-gray-900 dark:text-white">
                        <div>
                          {user?.firstName} {user?.lastName}
                        </div>
                        <div className="font-medium truncate">
                          {user?.email}
                        </div>
                      </div>
                      <ul
                        className="py-2 text-sm text-gray-200"
                        aria-labelledby="dropdownInformationButton"
                      >
                        <li>
                          <Link
                            to="/profile"
                            className="block px-4 py-2 hover:bg-gray-600"
                          >
                            Profile
                          </Link>
                        </li>
                        <li>
                          <a
                            href="#"
                            className="block px-4 py-2 hover:bg-gray-600"
                          >
                            Settings
                          </a>
                        </li>
                        <li>
                          <a
                            href="#"
                            className="block px-4 py-2 hover:bg-gray-600"
                          >
                            Earnings
                          </a>
                        </li>
                      </ul>
                      <div className="py-2 text-gray-200">
                        <button
                          onClick={logoutUser}
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
                  <Link
                    to="/login"
                    className="text-white bg-lightGreen-100 hover:bg-lightGreen-200 font-medium rounded-lg text-sm px-4 lg:px-5 py-2 lg:py-2.5"
                  >
                    Log in
                  </Link>
                  <Link
                    to="/signup"
                    className="text-white bg-purple-100 hover:bg-purple-200 font-medium rounded-lg text-sm px-4 lg:px-5 py-2 lg:py-2.5"
                  >
                    Sign up
                  </Link>
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
              isMenuOpen ? "flex" : "hidden"
            } justify-between w-full lg:flex lg:w-auto lg:order-1 px-4 lg:px-5 py-2 lg:py-2.5`}
            id="mobile-menu-2"
          >
            <div>
              <ul className="flex flex-col my-4 lg:my-0 h-auto font-medium lg:flex-row lg:space-x-8 md:h-auto">
                <li>
                  <Link
                    to="/"
                    className={`block py-2 pr-4 pl-3 lg:bg-transparent lg:p-0 ${
                      isLinkActive("/") ? "text-white" : "text-gray-300 lg:hover:bg-transparent hover:bg-lightGreen-100 hover:text-white"
                    }`}
                  >
                    Home
                  </Link>
                </li>
                <li>
                  <Link
                    to="/chats"
                    className={`block py-2 pr-4 pl-3 rounded lg:bg-transparent lg:p-0 ${
                      isLinkActive("/chats") ? "text-white" : "text-gray-300 lg:hover:bg-transparent hover:bg-lightGreen-100 hover:text-white"
                    }`}
                  >
                    Chats
                  </Link>
                </li>
                <li>
                  <a
                    href="#"
                    className="block py-2 pr-4 pl-3 text-gray-300 lg:hover:bg-transparent lg:border-0 lg:hover:text-primary-700 lg:p-0 hover:bg-lightGreen-100 hover:text-white"
                  >
                    Marketplace
                  </a>
                </li>
                <li>
                  <a
                    href="#"
                    className="block py-2 pr-4 pl-3 text-gray-300 lg:hover:bg-transparent lg:border-0 lg:hover:text-primary-700 lg:p-0 hover:bg-lightGreen-100 hover:text-white"
                  >
                    Features
                  </a>
                </li>
                <li>
                  <a
                    href="#"
                    className="block py-2 pr-4 pl-3 text-gray-300 lg:hover:bg-transparent lg:border-0 lg:hover:text-primary-700 lg:p-0 hover:bg-lightGreen-100 hover:text-white"
                  >
                    Team
                  </a>
                </li>
                <li>
                  <a
                    href="#"
                    className="block py-2 pr-4 pl-3 text-gray-300 lg:hover:bg-transparent lg:border-0 lg:hover:text-primary-700 lg:p-0 hover:bg-lightGreen-100 hover:text-white"
                  >
                    Contact
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </nav>
    </header>
  );
}
