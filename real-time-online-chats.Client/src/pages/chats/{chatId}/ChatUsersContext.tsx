import { createContext, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import { ChatUser } from "./{chatId}.types";
import { handleError, useCustomHook } from "@src/utils/helpers";
import { ChatUsersService } from "@src/services/api/ChatUsersService";
import ErrorScreen from "@src/components/ui/ErrorScreen";

type ChatUsersContextType = {
    chatUsers: ChatUser[];
    setChatUsers: React.Dispatch<React.SetStateAction<ChatUser[]>>;
};

type Props = { children: React.ReactNode };

const ChatUsersContext = createContext({} as ChatUsersContextType);

export const ChatUsersContextProvider = ({ children }: Props) => {
    console.count("ChatUsersContextProvider render");
    const { chatId } = useParams<{ chatId: string }>();

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(false);

    const [chatUsers, setChatUsers] = useState<ChatUser[]>([]);

    useEffect(() => {
        if (!chatId) return;

        const abort = new AbortController();

        const joinChatAndGetUsers = async () => {
            const joinChat = async () => {
                try {
                    await ChatUsersService.addMe(chatId, { signal: abort.signal });
                    console.log("Joined chat:", chatId);
                } catch (e: any) {
                    if (e?.response?.data?.errors[0] == "User already in chat") {
                        console.log('User already in chat');
                    } else {
                        handleError(e);
                        setError(true);
                    }
                }
            };

            await joinChat();

            const fetchChatUsers = async () => {
                try {
                    const users = await ChatUsersService.getAllUsersByChatId(chatId, { signal: abort.signal });
                    setChatUsers(users);
                    setLoading(false)
                } catch (e: any) {
                    handleError(e);
                    setError(true);
                }
            };

            await fetchChatUsers();
        }

        joinChatAndGetUsers();

        return () => {
            abort.abort();
        }
    }, [chatId]);

    if (error) <ErrorScreen />

    const value = useMemo(() => ({
        chatUsers,
        setChatUsers
    }), [chatUsers]);

    return <ChatUsersContext.Provider value={value}>{loading ? null : children}</ChatUsersContext.Provider>
}

export const useChatUsers = () => useCustomHook(ChatUsersContext, useChatUsers.name);
