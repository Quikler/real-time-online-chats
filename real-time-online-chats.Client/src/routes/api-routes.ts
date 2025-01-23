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
}

export abstract class MessageRoutes {
  static readonly base = `messages`;
}
