// import { createContext, useMemo, useState } from "react";
// import { MessageChat } from "./{chatId}.types";

// type MessageContextType = {
//   messageChat: MessageChat;
// };

// type MessageProviderProps = { children: React.ReactNode; message: MessageChat };

// const MessageContext = createContext({} as MessageContextType);

// export const MessageContextProvider = ({ children, message }: MessageProviderProps) => {
//   const [messageChat, setMessageChat] = useState(message);

//   const value = useMemo(
//     () => ({
//       messageChat,
//     }),
//     [messageChat]
//   );

//   return <MessageContext.Provider value={value}>{children}</MessageContext.Provider>;
// };
