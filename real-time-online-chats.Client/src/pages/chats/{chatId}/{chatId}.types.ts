export type MessageChat = {
  id: string;
  content: string;
  user: UserChat;
};

export type UserChat = {
  id?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
};

export type ChatInfo = {
  title: string;
  ownerId: string;
  id: string;
  creationTime: Date;
};
