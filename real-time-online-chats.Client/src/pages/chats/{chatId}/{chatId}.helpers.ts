import { ChatUser } from "./{chatId}.types";

export const createMessageChatFromUserChat = (user: ChatUser, content: string) => {
  return {
    id: (Math.random() + 1).toString(36),
    content: content,
    user: user,
  };
};
