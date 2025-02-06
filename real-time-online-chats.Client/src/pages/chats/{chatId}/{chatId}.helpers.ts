import { UserChat } from "./{chatId}.types";

export const createMessageChatFromUserChat = (user: UserChat, content: string) => {
  return {
    id: (Math.random() + 1).toString(36),
    content: content,
    user: {
      id: user.id,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
    },
  };
};
