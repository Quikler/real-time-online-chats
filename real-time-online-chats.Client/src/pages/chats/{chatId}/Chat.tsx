import { useNavigate, useParams } from "react-router-dom";
import React, { useEffect, useRef, useState } from "react";
import { HubConnection } from "@microsoft/signalr";
import { MessageService } from "@src/services/api/MessageService";
import { toast } from "react-toastify";
import { useAuth } from "@src/contexts/AuthContext";
import { CreateMessageRequest } from "@src/models/dtos/Message";
import { ChatService } from "@src/services/api/ChatService";
import { isNullOrWhitespace } from "@src/utils/helpers";
import { MessageChat, UserChat } from "./{chatId}.types";
import { createMessageChatFromUserChat } from "./{chatId}.helpers";
import useChatDetailed from "./hooks/useChatDetailed";
import useMessageHubConnection from "./hooks/useMessageHubConnection";
import ChatHeader from "./ChatHeader";

export interface CreateMessageFormData {
  message: string;
}

const Chat = () => {
  const { user } = useAuth();
  const { chatId } = useParams<{ chatId: string }>();

  const { chatInfo, messages, users, setMessages } = useChatDetailed(chatId);
  const connection = useMessageHubConnection(chatId);

  const lastMessageRef = useRef<HTMLLIElement>(null);

  // Initialize SignalR event handlers
  useEffect(() => {
    if (!connection) return;

    const registerSignalREventHandlers = (connection: HubConnection) => {
      connection.on("SendMessage", (message: MessageChat) => {
        setMessages((prev) => [...prev, message]);
      });

      connection.on("JoinChat", (user: UserChat) => {
        const message = createMessageChatFromUserChat(
          user,
          `<!-- User ${user.email} joined chat -->`
        );
        setMessages((prev) => [...prev, message]);
      });

      connection.on("LeaveChat", (user: UserChat) => {
        const message = createMessageChatFromUserChat(
          user,
          `<!-- User ${user.email} left the chat -->`
        );
        setMessages((prev) => [...prev, message]);
      });
    };

    registerSignalREventHandlers(connection);
  }, [connection]);

  useEffect(() => {
    if (messages) {
      lastMessageRef.current?.scrollIntoView();
    }
  }, [messages]);

  const [messageFormData, setMessageFormData] = useState<CreateMessageFormData>({
    message: "",
  });

  const handleMessageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setMessageFormData({
      ...messageFormData,
      [name]: value,
    });
  };

  const navigate = useNavigate();

  const handleChatLeave = () => {
    if (chatId) {
      ChatService.leaveChat(chatId)
        .then(() => navigate("/chats"))
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

  const handleCreateMessageFormSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (isNullOrWhitespace(messageFormData.message)) return;

    let request: CreateMessageRequest = {
      chatId: chatId!,
      content: messageFormData.message,
    };

    MessageService.createMessage(request)
      .then(() => setMessageFormData({ ...messageFormData, message: "" }))
      .catch((e) => console.log(e));
  };

  return (
    <div className="flex flex-col bg-slate-600 lg:px-16 flex-grow" style={{ minHeight: "720px" }}>
      <ChatHeader chatInfo={chatInfo} users={users} onChatDelete={handleChatDelete} onChatLeave={handleChatLeave} />

      <ul className="flex-grow flex flex-col p-6 overflow-y-auto pt-40 pb-32 gap-8">
        {messages?.map((value, index) => (
          <li
            ref={index === messages.length - 1 ? lastMessageRef : undefined}
            key={index}
            className={`flex gap-3 ${value.user.id === user?.id ? "justify-end" : "justify-start"}`}
          >
            <img
              src="/images/test-profile.jpg"
              className={`w-10 h-10 rounded-full object-cover ${
                value.user.id === user?.id ? "order-2" : ""
              }`}
            />
            <div className="flex flex-col gap-3 max-w-[80%]">
              <p
                className={`text-xl text-white text-opacity-80 flex gap-4 ${
                  value.user.id === user?.id ? "font-medium self-end" : ""
                }`}
              >
                {value.user.email}
              </p>

              <div
                className={`flex flex-col gap-1 ${
                  value.user.id === user?.id ? "items-end" : "items-start"
                }`}
              >
                <p
                  className={`p-3 text-lg rounded-lg break-all ${
                    value.user.id === user?.id
                      ? "bg-slate-700 text-white"
                      : "bg-slate-500 text-white"
                  }`}
                >
                  {value.content}
                </p>
              </div>
            </div>
          </li>
        ))}
      </ul>

      <div className="fixed bottom-0 right-0 left-0 w-full bg-slate-700 p-4">
        <form className="flex items-center gap-4" onSubmit={handleCreateMessageFormSubmit}>
          <input
            name="message"
            autoComplete="off"
            value={messageFormData.message}
            onChange={handleMessageChange}
            className="flex-grow p-3 bg-slate-600 text-white placeholder-white focus:outline-none"
            placeholder="Send a message..."
          />
          <button
            type="submit"
            className="p-3 bg-slate-600 text-white rounded-lg hover:bg-slate-500 transition-colors duration-300"
          >
            ➢
          </button>
        </form>
      </div>
    </div>
  );
};

export default Chat;
