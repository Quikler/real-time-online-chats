import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { ChatService } from "../../../services/chat-service";
import { toast } from "react-toastify";
import { MessageService } from "../../../services/message-service";
import { CreateMessageRequest } from "../../../contracts/message-contract";

export interface CreateMessageFormData {
  message: string;
}

interface ChatData {
  title: string;
  ownerId: string;
  id: string;
}

const MainChatPage = () => {
  const { chatId } = useParams<{ chatId: string }>();
  const [chatData, setChatData] = useState<ChatData>();

  const [formData, setFormData] = useState<CreateMessageFormData>({
    message: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  useEffect(() => {
    if (chatId) {
      const { observable, abort } = ChatService.getChat(chatId);

      const subscription = observable.subscribe({
        next: (response) => setChatData(response.data),
        error: (error) => toast.error(error.message),
      });

      return () => {
        subscription.unsubscribe();
        abort();
      };
    }
  }, [chatId]);

  const handleCreateMessageFormSubmit = (
    e: React.FormEvent<HTMLFormElement>
  ) => {
    e.preventDefault();

    let request: CreateMessageRequest = {
      chatId: chatId!,
      content: formData.message,
      contentType: "msg",
    };

    MessageService.createMessage(request).observable.subscribe({
      next: () => console.log("Message created successfully"),
      error: (error) => console.log("Error sending message", error),
    });
  };

  return (
    <div className="flex flex-col bg-darkBlue-100 lg:px-16 pt-16">
      <div className="flex flex-col h-full flex-grow">
        <div className="bg-lightGreen-100 items-center gap-4 flex justify-between pt-16 px-6 pb-3">
          <div className="">
            <svg
              fill="#fff"
              height="32px"
              width="32px"
              version="1.1"
              id="Layer_1"
              viewBox="0 0 476.213 476.213"
            >
              <polygon points="476.213,223.107 57.427,223.107 151.82,128.713 130.607,107.5 0,238.106 130.607,368.714 151.82,347.5   57.427,253.107 476.213,253.107 " />
            </svg>
          </div>
          <div className="text-white flex flex-col flex-grow">
            <p className="text-3xl">{chatData?.title}</p>
            <p className="text-lg">3/52 members online</p>
          </div>
          <div>
            <svg
              width="32px"
              height="32px"
              viewBox="0 0 16 16"
              xmlns="http://www.w3.org/2000/svg"
              fill="#fff"
            >
              <path d="M9.5 13a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0zm0-5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z" />
            </svg>
          </div>
        </div>
        <div className="bg-maroon-100 text-white flex-grow">
          <div className="p-4 flex justify-end">
            <div className="flex gap-2">
              <div className="flex flex-col gap-2">
                <div className="flex gap-2 justify-end">
                  <svg
                    viewBox="0 0 2 2"
                    fill="white"
                    xmlns="http://www.w3.org/2000/svg"
                    width={16}
                  >
                    <circle cx="1" cy="1" r="1" />
                  </svg>
                  <p className="text-white">User1</p>
                </div>
                <div className="flex flex-col gap-2 text-black">
                  <div className="bg-white py-2 px-3">
                    <p>Hello guys</p>
                  </div>
                  <div className="bg-white py-2 px-3">
                    <p>How are you? &lt;3</p>
                  </div>
                </div>
              </div>
              <img
                className="w-12 h-12 rounded-full object-cover"
                src="/images/test-profile.jpg"
              />
            </div>
          </div>
          <div className="p-4 flex">
            <div className="flex gap-2">
              <div className="flex flex-col gap-2 order-2">
                <div className="flex gap-2">
                  <svg
                    viewBox="0 0 2 2"
                    fill="white"
                    xmlns="http://www.w3.org/2000/svg"
                    width={16}
                  >
                    <circle cx="1" cy="1" r="1" />
                  </svg>
                  <p className="text-white">User1</p>
                </div>
                <div className="flex flex-col gap-2 text-black">
                  <div className="bg-white py-2 px-3">
                    <p>Hello guys</p>
                  </div>
                  <div className="bg-white py-2 px-3">
                    <p>How are you? &lt;3</p>
                  </div>
                </div>
              </div>
              <img
                className="w-12 h-12 rounded-full object-cover"
                src="/images/test-profile.jpg"
              />
            </div>
          </div>
        </div>
        <div className="bg-lightGreen-100">
          <form
            className="text-white flex justify-between items-center px-4 py-2 gap-4"
            onSubmit={handleCreateMessageFormSubmit}
          >
            <svg width="48" height="48" viewBox="0 0 48 48" fill="#fff">
              <path d="M33 12v23c0 4.42-3.58 8-8 8s-8-3.58-8-8v-25c0-2.76 2.24-5 5-5s5 2.24 5 5v21c0 1.1-.89 2-2 2-1.11 0-2-.9-2-2v-19h-3v19c0 2.76 2.24 5 5 5s5-2.24 5-5v-21c0-4.42-3.58-8-8-8s-8 3.58-8 8v25c0 6.08 4.93 11 11 11s11-4.92 11-11v-23h-3z" />
              <path d="M0 0h48v48h-48z" fill="none" />
            </svg>
            <input
              name="message"
              value={formData.message}
              onChange={handleChange}
              className="border-0 flex-grow focus:outline-none text-2xl bg-transparent placeholder-white outline-0"
              placeholder="Send a message..."
            />
            <button className="text-6xl">➢</button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default MainChatPage;
