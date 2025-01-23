import React, { useState } from "react";

export interface CreateChatFormData {
  title: string;
}

type Props = {
  onSubmit: (e: React.FormEvent<HTMLFormElement>, data: CreateChatFormData) => void;
  isChatFormOpen: boolean;
  setIsChatFormOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const CreateChatForm = ({
  onSubmit,
  isChatFormOpen,
  setIsChatFormOpen,
}: Props) => {
  const [formData, setFormData] = useState<CreateChatFormData>({
    title: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleCreateChatFormSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    onSubmit(e, formData);
  };

  return (
    <>
      {/* Darkening background */}
      <div
        className={`fixed inset-0 bg-darkBlue-200 transition-opacity z-40 ${
          isChatFormOpen
            ? "opacity-75 backdrop-blur-sm"
            : "opacity-0 pointer-events-none"
        }`}
      ></div>

      {/* Main modal */}
      <div
        id="authentication-modal"
        tabIndex={-1}
        aria-hidden="true"
        className={`${
          isChatFormOpen ? "flex" : "hidden"
        } overflow-y-auto overflow-x-hidden lg:pt-0 pt-10 fixed top-0 right-0 left-0 z-50 justify-center items-center w-full md:inset-0 h-[calc(100%-1rem)] max-h-full`}
      >
        <div className="relative p-4 w-full max-w-md max-h-full">
          {/* Modal content */}
          <div className="relative bg-darkBlue-100 rounded-lg shadow">
            {/* Modal header */}
            <div className="flex items-center justify-between p-4 md:p-5 rounded-t">
              <h3 className="text-xl font-semibold text-white">Create chat</h3>
              <button
                onClick={() => setIsChatFormOpen(false)}
                type="button"
                className="end-2.5 text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm w-8 h-8 ms-auto inline-flex justify-center items-center dark:hover:bg-gray-600 dark:hover:text-white"
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
            <div className="p-4 md:p-5">
              <form className="space-y-4" onSubmit={handleCreateChatFormSubmit}>
                <div>
                  <label className="block mb-2 text-sm font-medium text-white">
                    Title
                  </label>
                  <input
                    onChange={handleChange}
                    value={formData.title}
                    type="text"
                    name="title"
                    id="title"
                    className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5"
                    placeholder="Enter title"
                  />
                </div>
                <button
                  type="submit"
                  className="w-full text-white bg-lightGreen-100 hover:bg-lightGreen-200 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                >
                  Create chat
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default CreateChatForm;
