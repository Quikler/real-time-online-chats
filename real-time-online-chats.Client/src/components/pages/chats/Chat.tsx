import { useNavigate, useParams } from "react-router-dom";
import React, { useEffect, useState } from "react";
import { ChatService } from "../../../services/api/chat-service";
import { CreateMessageRequest, GetMessageResponse } from "../../../models/dtos/Message";
import { useAuth } from "../../../contexts/auth-context";
import { isNullOrWhitespace } from "../../../utils/helpers";
import Button from "@src/components/ui/Button";
import ButtonLink from "@src/components/ui/ButtonLink";
import Modal from "@src/components/ui/Modal";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { MessageService } from "@src/services/api/message-service";
import { toast } from "react-toastify";

export interface CreateMessageFormData {
  message: string;
}

interface MessageResult {
  id: string;
  content: string;
  user: UserResponse;
}

interface UserResponse {
  id?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
}

interface ChatData {
  title: string;
  ownerId: string;
  id: string;
  creationTime: Date;
  messages: MessageResult[];
  users: UserResponse[];
}

const Chat = () => {
  const { user, token } = useAuth();
  const { chatId } = useParams<{ chatId: string }>();
  const [chatData, setChatData] = useState<ChatData>();

  const navigate = useNavigate();

  const [connection, setConnection] = useState<HubConnection>();

  const [messageFormData, setMessageFormData] = useState<CreateMessageFormData>({
    message: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setMessageFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleChatLeave = () => {
    if (chatId) {
      ChatService.leaveChat(chatId)
        .then(() => {
          toast.success("Leaved chat successfully");
          navigate("/chats");
        })
        .catch((e) => console.error("Error leaving chat:", e.message));
    }
  };

  const handleChatDelete = () => {
    if (chatId) {
      ChatService.deleteChat(chatId)
        .then(() => {
          toast.success("Chat deleted successfully");
          navigate("/chats");
        })
        .catch((e) => console.error("Error deleting chat:", e.message));
    }
  };

  useEffect(() => {
    // Only initialize SignalR connection if token is available
    if (!token) {
      console.log("Token is not available yet.");
      return;
    }

    const abortController = new AbortController();

    // Retrieve chat data
    if (chatId) {
      ChatService.getChatDetailed(chatId, { signal: abortController.signal })
        .then((data) => setChatData(data))
        .catch((e) => console.log(e.message));
    }

    // Initialize SignalR connection with the server
    const newConnection = new HubConnectionBuilder()
      .withUrl("https://localhost:7207/messageHub", {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    // Cleanup function
    return () => {
      abortController.abort();
      if (newConnection) {
        newConnection
          .stop()
          .then(() => console.log("[SignalR]: Connection stopped"))
          .catch((e) => console.error("[SignalR]: Error stopping connection", e.message));
      }
    };
  }, [token]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          connection
            .invoke("JoinChatGroup", chatId)
            .then(() => console.log("Joined chat group:", chatId))
            .catch((e) => e.message);

          connection.on("SendMessage", (message: GetMessageResponse) => {
            console.log("SendMessage:", message);

            setChatData((prevChatData) => {
              if (!prevChatData) {
                console.error("chatData is not available");
                return prevChatData; // Return the previous state if it's undefined
              }

              // Find the sender in the latest state
              const sender = prevChatData.users.find((u) => u.id === message.userId);

              const newMessage: MessageResult = {
                id: message.userId,
                content: message.content,
                user: {
                  id: message.userId,
                  email: sender?.email,
                  firstName: sender?.firstName!,
                  lastName: sender?.lastName!,
                },
              };

              // Return the updated state
              return {
                ...prevChatData,
                messages: [...prevChatData.messages, newMessage],
              };
            });
          });

          connection.on("JoinChat", (userId: string) => {
            setChatData((prevChatData) => {
              if (!prevChatData) {
                console.error("chatData is not available");
                return prevChatData; // Return the previous state if it's undefined
              }

              // Find the sender in the latest state
              const user = prevChatData.users.find((u) => u.id === userId);

              const newMessage: MessageResult = {
                id: userId,
                content: `<!-- User ${user?.email} joined chat -->`,
                user: {
                  id: userId,
                  email: user?.email,
                  firstName: user?.firstName!,
                  lastName: user?.lastName!,
                },
              };

              // Return the updated state
              return {
                ...prevChatData,
                messages: [...prevChatData.messages, newMessage],
              };
            });
          });

          connection.on("LeaveChat", (userId: string) => {
            setChatData((prevChatData) => {
              if (!prevChatData) {
                console.error("chatData is not available");
                return prevChatData; // Return the previous state if it's undefined
              }

              // Find the sender in the latest state
              const user = prevChatData.users.find((u) => u.id === userId);

              const newMessage: MessageResult = {
                id: userId,
                content: `<!-- User ${user?.email} left the chat -->`,
                user: {
                  id: userId,
                  email: user?.email,
                  firstName: user?.firstName!,
                  lastName: user?.lastName!,
                },
              };

              // Return the updated state
              return {
                ...prevChatData,
                messages: [...prevChatData.messages, newMessage],
              };
            });
          });
        })
        .catch((e) => console.error("[SignalR]:", e.message));
    }
  }, [connection]);

  const handleCreateMessageFormSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (isNullOrWhitespace(messageFormData.message)) return;

    let request: CreateMessageRequest = {
      chatId: chatId!,
      content: messageFormData.message,
      contentType: "msg",
    };

    // Create message (send to the server and store in database)
    MessageService.createMessage(request)
      .then(() => {
        console.log("Message created successfully");
        setMessageFormData({ ...messageFormData, message: "" });
      })
      .catch((e) => console.log(e));
  };

  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleModalOpen = () => setIsModalOpen(!isModalOpen);

  return (
    <div
      className="flex flex-col bg-darkBlue-100 lg:px-16 pt-16 flex-grow"
      style={{ minHeight: "720px" }}
    >
      <div className="flex flex-col h-full flex-grow">
        <div className="bg-lightGreen-100 items-center gap-4 flex justify-between p-6">
          <ButtonLink to="/chats" className="text-3xl">
            ←
          </ButtonLink>
          <div className="text-white flex flex-col flex-grow">
            <p className="text-3xl">{chatData?.title}</p>
            <p className="text-lg">3/52 members online</p>
          </div>
          <div className="w-auto">
            <Button className="text-3xl" onClick={handleModalOpen}>
              ⋮
            </Button>
            <Modal
              className="flex  flex-col gap-2"
              title={chatData?.title}
              isModalOpen={isModalOpen}
              setIsModalOpen={setIsModalOpen}
            >
              <ul className="text-white">
                {chatData?.users.map((value, index) => (
                  <li
                    key={index}
                    className={`flex cursor-default overflow-ellipsis items-center gap-3 p-2 border-b-2 border-darkBlue-200 ${
                      chatData.ownerId === value.id
                        ? "bg-green-500 hover:bg-green-600"
                        : "hover:bg-darkBlue-200"
                    }`}
                  >
                    <img
                      src="/images/test-profile.jpg"
                      className="w-12 h-12 rounded-full object-cover"
                    />
                    <div className="overflow-x-auto text-wrap break-words">{value.email}</div>
                  </li>
                ))}
              </ul>
              <div className="flex gap-2">
                <Button variant="primary" onClick={handleChatLeave}>
                  Leave
                </Button>
                {user?.id === chatData?.ownerId && (
                  <Button variant="danger" onClick={handleChatDelete}>
                    Delete chat
                  </Button>
                )}
              </div>
            </Modal>
          </div>
        </div>
        <div className="bg-maroon-100 text-white flex-grow p-4">
          {chatData?.messages.map((value, index) => {
            return (
              <div
                key={index}
                className={`p-4 flex ${value.user.id === user?.id ? "justify-end" : ""}`}
              >
                <div className="flex gap-2">
                  <div
                    className={`flex flex-col gap-2 ${value.user.id === user?.id ? "" : "order-2"}`}
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
                    <div
                      className={`flex flex-col gap-2 text-black ${
                        value.user.id === user?.id ? "items-end" : "items-start"
                      }`}
                    >
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
              name="message"
              autoComplete="off"
              value={messageFormData.message}
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

export default Chat;
