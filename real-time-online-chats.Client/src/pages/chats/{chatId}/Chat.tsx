import { useNavigate, useParams } from "react-router-dom";
import React, { useEffect, useRef, useState } from "react";
import { HubConnection } from "@microsoft/signalr";
import { MessageService } from "@src/services/api/MessageService";
import { toast } from "react-toastify";
import { useAuth } from "@src/hooks/useAuth";
import { CreateMessageRequest } from "@src/models/dtos/Message";
import { ChatService } from "@src/services/api/ChatService";
import { isNullOrWhitespace, scrollToBottomOfBody } from "@src/utils/helpers";
import { MessageChat, UserChat } from "./{chatId}.types";
import { createMessageChatFromUserChat } from "./{chatId}.helpers";
import useChatDetailed from "./hooks/useChatDetailed";
import useMessageHubConnection from "./hooks/useMessageHubConnection";
import ChatHeader from "./ChatHeader";
import Message from "./Message";
import { Close, ShortArrowDown } from "@src/components/svg/SVGCommon";

const Chat = () => {
  const { user } = useAuth();
  const { chatId } = useParams<{ chatId: string }>();
  const navigate = useNavigate();

  const { chatInfo, messages, users, setMessages, setUsers, setChatInfo } = useChatDetailed(chatId);
  const connection = useMessageHubConnection(chatId);

  const [countOfNewMessages, setCountOfNewMessages] = useState(0);
  const [editableMessage, setEditableMessage] = useState<MessageChat | null>();
  const [message, setMessage] = useState<string | undefined>("");

  useEffect(() => {
    const scrollHandler = () => {
      const scrollPosition = window.scrollY + window.innerHeight;
      const bottomPosition = document.documentElement.scrollHeight;

      if (scrollPosition >= bottomPosition - 40) {
        setCountOfNewMessages(0);
      }
    };

    window.addEventListener("scroll", scrollHandler);

    return () => window.removeEventListener("scroll", scrollHandler);
  }, []);

  // Initialize SignalR event handlers
  useEffect(() => {
    if (!connection || !chatInfo) return;

    const registerSignalREventHandlers = (connection: HubConnection) => {
      connection.off("SendMessage");
      connection.on("SendMessage", (message: MessageChat) => {
        setMessages((prev) => [...prev, message]);
        if (message.user.id !== user?.id) {
          setCountOfNewMessages(prev => prev + 1);
        }
      });

      connection.off("JoinChat");
      connection.on("JoinChat", (joinedUser: UserChat) => {
        const message = createMessageChatFromUserChat(
          joinedUser,
          `<!-- User ${joinedUser.email} joined chat -->`
        );

        setMessages((prev) => [...prev, message]);
        setUsers((prev) => [...prev, joinedUser]);
      });

      connection.off("LeaveChat");
      connection.on("LeaveChat", (leavedUser: UserChat) => {
        console.log("Current owner:", chatInfo.ownerId);
        console.log("Left the chat:", leavedUser.id);

        const newUsers = users.filter((u) => u.id !== leavedUser.id);
        const newOwner = newUsers[0];

        console.log("New owner:", newOwner);

        const message = createMessageChatFromUserChat(
          leavedUser,
          leavedUser.id === chatInfo.ownerId
            ? `<!-- User ${leavedUser.email} left the chat and new owner is ${newOwner.email} -->`
            : `<!-- User ${leavedUser.email} left the chat -->`
        );

        setMessages((prev) => [...prev, message]);

        // ✅ Use functional update to avoid stale state
        setChatInfo((prevChatInfo) =>
          prevChatInfo ? { ...prevChatInfo, ownerId: newOwner.id } : prevChatInfo
        );

        setUsers(newUsers);
      });

      connection.off("DeleteMessage");
      connection.on("DeleteMessage", (messageId: string) => {
        setMessages((prev) => prev.filter((message) => message.id !== messageId));
      });

      connection.off("UpdateMessage");
      connection.on("UpdateMessage", (message: MessageChat) => {
        setMessages((prev) =>
          prev.map((m) => (m.id === message.id ? { ...m, content: message.content } : m))
        );
      });
    };

    registerSignalREventHandlers(connection);
  }, [connection, chatInfo?.ownerId]);

  const hasScrolled = useRef(false);

  useEffect(() => {
    if (messages?.length > 0 && !hasScrolled.current) {
      scrollToBottomOfBody();
      hasScrolled.current = true; // Mark as executed
    }
  }, [messages]);

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

  const handleSendMessage = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (isNullOrWhitespace(message)) return;

    if (editableMessage?.id) {
      const request = {
        chatId: chatId!,
        content: message!,
      };

      MessageService.updateMessage(editableMessage?.id, request)
        .then(() => console.log("Updated message successfully."))
        .catch((e) => console.error("Error updating message:", e.message));

      setEditableMessage(null);

      return;
    }

    const request: CreateMessageRequest = {
      chatId: chatId!,
      content: message!,
    };

    MessageService.createMessage(request)
      .then(() => {
        setMessage("");
        window.scrollTo(0, document.body.scrollHeight);
      })
      .catch((e) => console.error("Error creating message:", e.message));
  };

  const handleMessageDelete = () => {
    setEditableMessage(null);
    setMessage("");
  };

  const handleMessageEdit = (messageId: string) => {
    const message = messages.find((m) => m.id === messageId);
    setEditableMessage(message);
    setMessage(message?.content);
  };

  return (
    <div className="flex flex-col bg-slate-600 lg:px-16 flex-grow" style={{ minHeight: "720px" }}>
      <ChatHeader
        chatInfo={chatInfo}
        users={users}
        onChatDelete={handleChatDelete}
        onChatLeave={handleChatLeave}
      />

      <ul className="flex-grow flex flex-col p-6 overflow-y-auto pt-40 pb-32">
        {messages?.map((message, index) => {
          // If previous message's user id is equal to current
          const isCurrentUserPrevious =
            index > 0 && message.user.id === messages[index - 1].user.id;

          const isCurrentUser = message.user.id === user?.id;

          const showUserInfo = !isCurrentUserPrevious;

          return (
            <li
              key={index}
              className={`flex gap-3 ${isCurrentUserPrevious ? "pt-2" : "pt-8"} ${
                isCurrentUser ? "justify-end" : "justify-start"
              }`}
            >
              <Message
                chatId={chatInfo?.id!}
                onDelete={handleMessageDelete}
                onEdit={handleMessageEdit}
                messageChat={message}
                isCurrentUser={isCurrentUser}
                showUserInfo={showUserInfo}
              />
            </li>
          );
        })}
      </ul>

      {countOfNewMessages !== 0 && (
        <button
          onClick={() => {
            scrollToBottomOfBody();
            setCountOfNewMessages(0);
          }}
          className="fixed text-white left-8 bottom-24 bg-slate-800 w-16 h-16 rounded-full"
        >
          <div className="h-full relative">
            <div className="h-full flex items-center justify-center">
              <ShortArrowDown width={12} height={12} />
            </div>
            <div className="absolute right-0 top-0">
              <div className="bg-red-500 rounded-full py-1 px-3">{countOfNewMessages}</div>
            </div>
          </div>
        </button>
      )}

      <div className="fixed flex flex-col gap-2 bottom-0 right-0 left-0 w-full bg-slate-700 p-4">
        {editableMessage && (
          <div className="flex items-center gap-2 text-white">
            <p>Edit message: {editableMessage.content}</p>
            <button
              className="p-1 px-2 bg-slate-600 text-white rounded-lg hover:bg-slate-500 transition-colors duration-300"
              onClick={() => {
                setEditableMessage(null);
                setMessage("");
              }}
            >
              <Close width="12" />
            </button>
          </div>
        )}
        <form className="flex items-center gap-4" onSubmit={handleSendMessage}>
          {/* {countOfNewMessages !== 0 && (
            <div className="flex items-center justify-center text-white bg-slate-600 w-10 h-10 rounded-full">
              {countOfNewMessages}
            </div>
          )} */}
          <input
            name="message"
            autoComplete="off"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
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
