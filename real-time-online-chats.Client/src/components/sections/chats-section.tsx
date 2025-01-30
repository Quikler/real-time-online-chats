import { useEffect, useState } from "react";
import ChatPreview from "../chats/chat-preview";
import { ChatService } from "@src/services/api/chat-service";
import { LoaderScreen } from "../ui/Loader";

export default function ChatsSection() {
  const [chats, setChats] = useState<{ id: string; title: string }[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const abortController = new AbortController();
    ChatService.getChats(1, 10, { signal: abortController.signal })
      .then((data) => {
        setChats(data?.items);
        setLoading(false);
      })
      .catch((e) => console.error("Get chats:", e.message));

    return () => abortController.abort();
  }, []);

  if (loading) return <LoaderScreen />

  return (
    <section className="bg-black w-full">
      <div className="max-w-screen-xl mx-auto py-12 px-6 flex flex-col lg:gap-8 gap-4">
        <p className="text-center text-5xl text-white">Available chats</p>
        <div className="grid lg:grid-cols-3 md:grid-cols-2 grid-cols-1 lg:gap-x-20 gap-x-10 gap-y-5">
          {chats?.map((value, index) => (
            <ChatPreview key={index} id={value.id} title={value.title} />
          ))}
        </div>
      </div>
    </section>
  );
}
