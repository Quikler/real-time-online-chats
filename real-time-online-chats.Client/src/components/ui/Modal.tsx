import React from "react";
import { twMerge } from "tailwind-merge";

type ModalProps = React.HTMLAttributes<HTMLDivElement> & {
  title?: string;
  children: React.ReactNode;
  isModalOpen: boolean;
  setIsModalOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const Modal = ({
  title,
  children,
  isModalOpen,
  setIsModalOpen,
  className,
  ...rest
}: ModalProps) => {
  const modalBackgroundStyles = `fixed inset-0 bg-slate-900/50 transition-opacity z-40 ${
    isModalOpen ? "opacity-100 backdrop-blur-sm" : "opacity-0 pointer-events-none"
  }`;

  const modalContainerStyles = `${
    isModalOpen ? "flex" : "hidden"
  } overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 justify-center items-center w-full md:inset-0 h-[calc(100%-1rem)] max-h-full`;

  const modalContentStyles = "relative bg-slate-800 rounded-lg shadow-lg w-full max-w-md";

  const closeButtonStyles =
    "text-slate-400 bg-transparent hover:bg-slate-700 hover:text-slate-200 rounded-lg text-sm w-8 h-8 ms-auto inline-flex justify-center items-center";

  return (
    <>
      <div className={modalBackgroundStyles} onClick={() => setIsModalOpen(false)}></div>

      <div
        role="dialog"
        aria-modal="true"
        aria-hidden={!isModalOpen}
        tabIndex={-1}
        className={modalContainerStyles}
      >
        <div className="relative p-4 w-full max-w-md max-h-full">
          <div className={modalContentStyles}>
            <div className="flex items-center justify-between p-4 md:p-5 border-b border-slate-700 rounded-t">
              <h3 className="text-xl font-semibold text-slate-200">{title}</h3>
              <button
                onClick={() => setIsModalOpen(false)}
                type="button"
                className={closeButtonStyles}
                aria-label="Close modal"
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
              </button>
            </div>

            <div className={twMerge("p-4 md:p-5", className)} {...rest}>
              {children}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Modal;
