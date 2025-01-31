import { twMerge } from "tailwind-merge";

type ModalProps = React.HTMLAttributes<HTMLDivElement> & {
  title?: string;
  children: React.ReactNode;
  isModalOpen: boolean;
  setIsModalOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const Modal = ({ title, children, isModalOpen, setIsModalOpen, className, ...rest }: ModalProps) => {
  // Reusable styles for the modal background
  const modalBackgroundStyles = `fixed inset-0 bg-darkBlue-200 transition-opacity z-40 ${
    isModalOpen ? "opacity-75 backdrop-blur-sm" : "opacity-0 pointer-events-none"
  }`;

  // Reusable styles for the modal container
  const modalContainerStyles = `${
    isModalOpen ? "flex" : "hidden"
  } overflow-y-auto overflow-x-hidden lg:pt-0 pt-10 fixed top-0 right-0 left-0 z-50 justify-center items-center w-full md:inset-0 h-[calc(100%-1rem)] max-h-full`;

  // Reusable styles for the modal content
  const modalContentStyles = "relative bg-darkBlue-100 rounded-lg shadow-lg";

  // Reusable styles for the close button
  const closeButtonStyles =
    "text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm w-8 h-8 ms-auto inline-flex justify-center items-center dark:hover:bg-gray-600 dark:hover:text-white";

  return (
    <>
      {/* Darkening background */}
      <div className={modalBackgroundStyles}></div>

      {/* Main modal */}
      <div
        id="authentication-modal"
        tabIndex={-1}
        aria-hidden="true"
        className={modalContainerStyles}
      >
        <div className="relative p-4 pt-16 w-full max-w-md max-h-full">
          {/* Modal content */}
          <div className={modalContentStyles}>
            {/* Modal header */}
            <div className="flex items-center justify-between p-4 md:p-5 rounded-t">
              <h3 className="text-xl font-semibold text-white">{title}</h3>
              <button
                onClick={() => setIsModalOpen(false)}
                type="button"
                className={closeButtonStyles}
                data-modal-hide="authentication-modal"
              >
                <svg
                  className="w-3 h-3"
                  aria-hidden="true"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 14 14"
                >
                  <path
                    stroke="currentColor"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="m1 1 6 6m0 0 6 6M7 7l6-6M7 7l-6 6"
                  />
                </svg>
                <span className="sr-only">Close modal</span>
              </button>
            </div>

            {/* Modal body */}
            <div className={twMerge(`p-4 md:p-5 ${className}`)} {...rest}>
              {children}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Modal;