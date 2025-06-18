export type ChatMessage = {
  id: string;
  content: string;
  user: ChatUser;
};

export type ChatUser = {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  avatarUrl: string;
};

export type ChatInfo = {
  title: string;
  ownerId: string;
  id: string;
  creationTime: Date;
};
