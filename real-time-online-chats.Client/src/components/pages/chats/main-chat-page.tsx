import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { ChatService } from "../../../services/api/chat-service";
import { MessageService } from "../../../services/api/message-service";
import { CreateMessageRequest } from "../../../contracts/message-contract";
import { router } from "../../../routes/router";
import { useAuth } from "../../../contexts/auth-context";
import { isNullOrWhitespace } from "../../../utils/helpers";
import { LoaderScreen } from "@components/ui/Loader"

export interface CreateMessageFormData {
  message: string;
}

interface MessageResult {
  id: string;
  content: string;
  user: UserResponse;
}

interface UserResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}

interface ChatData {
  title: string;
  ownerId: string;
  id: string;
  messages: MessageResult[];
}

const MainChatPage = () => {
  const { user } = useAuth();
  const { chatId } = useParams<{ chatId: string }>();
  const [chatData, setChatData] = useState<ChatData>();
  const [isLoading, setIsLoading] = useState(true);

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
      const { observable, abort } = ChatService.getChatRestrict(chatId);

      observable.subscribe({
        next: (response) => {
          setChatData(response.data);
          setIsLoading(false);
        },
        error: () => router.navigate("/forbidden"),
      });

      return () => abort();
    }
  }, [chatId]);

  const handleCreateMessageFormSubmit = (
    e: React.FormEvent<HTMLFormElement>
  ) => {
    e.preventDefault();

    if (isNullOrWhitespace(formData.message)) {
      e.currentTarget.reportValidity();
      return;
    }

    //const input = document.getElementById("mes") as HTMLInputElement;
    //input.setCustomValidity()

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

  if (isLoading) return <LoaderScreen />;

  return (
    <div className="flex flex-col bg-darkBlue-100 lg:px-16 pt-16 flex-grow">
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
        <div className="bg-maroon-100 text-white flex-grow p-4">
          {chatData?.messages.map((value, index) => {
            return (
              <div
                key={index}
                className={`p-4 flex ${
                  value.user.id === user?.id ? "justify-end" : ""
                }`}
              >
                <div className="flex gap-2">
                  <div
                    className={`flex flex-col gap-2 ${
                      value.user.id === user?.id ? "" : "order-2"
                    }`}
                  >
                    <div
                      className={`flex gap-2 items-center ${
                        value.user.id === user?.id ? "justify-end" : ""
                      }`}
                    >
                      <svg
                        viewBox="0 0 2 2"
                        fill="white"
                        xmlns="http://www.w3.org/2000/svg"
                        width={16}
                      >
                        <circle cx="1" cy="1" r="1" />
                      </svg>
                      <p className="text-white">{value.user.email}</p>
                    </div>
                    <div className="flex flex-col gap-2 text-black items-end">
                      <div className="bg-white py-2 px-3">
                        <p>{value.content}</p>
                      </div>
                    </div>
                  </div>
                  <img
                    className="w-12 h-12 rounded-full object-cover"
                    src="/images/test-profile.jpg"
                  />
                </div>
              </div>
            );
          })}
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
              required
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
