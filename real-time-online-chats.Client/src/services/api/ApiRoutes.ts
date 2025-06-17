export abstract class AuthRoutes {
  static readonly base = `identity`;

  static readonly signup = `${this.base}/signup`;
  static readonly login = `${this.base}/login`;
  static readonly refreshToken = `${this.base}/refresh`;
  static readonly me = `${this.base}/me`;
  static readonly logout = `${this.base}/logout`;
}

export abstract class ChatRoutes {
  static readonly base = `chats`;
  static readonly detailed = `detailed`;
  static readonly owned = `${this.base}/owned`;

  static byId (chatId: string): string {
      return `${this.base}/${chatId}`;
  }

  static info (chatId: string): string {
      return `${this.byId(chatId)}/info`;
  }
}

export abstract class ChatMessagesRoutes {
  static readonly chats = `chats`;
  static readonly messages = `messages`;

  static byChatId (chatId: string) {
      return `${this.chats}/${chatId}/${this.messages}`;
  }

  static byId (chatId: string, messageId: string) {
      return `${this.byChatId(chatId)}/${messageId}`;
  }
}

export abstract class ChatUsersRoutes {
  static readonly chats = `chats`;
  static readonly users = `users`;

  static byChatId (chatId: string) {
      return `${this.chats}/${chatId}/${this.users}`;
  }

  static byId (chatId: string, userId: string) {
      return `${this.byChatId(chatId)}/${userId}`;
  }
}

export abstract class UserRoutes {
  static readonly base = `users`;
  static readonly profile = `profile`;
}
