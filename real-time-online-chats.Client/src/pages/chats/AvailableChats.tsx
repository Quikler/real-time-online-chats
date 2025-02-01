import { useEffect, useState } from "react";
import { ChatService } from "@src/services/api/ChatService";
import ChatPreview from "@src/pages/chats/ChatPreview";
import { LoaderScreen } from "@src/components/ui/Loader";

export default function ChatsSection() {
  const [chats, setChats] = useState<{ id: string; title: string }[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const abortController = new AbortController();
    ChatService.getChats(1, 9, { signal: abortController.signal })
      .then((data) => {
        setChats(data?.items);
        setLoading(false);
      })
      .catch((e) => console.error("Get chats:", e.message));

    return () => abortController.abort();
  }, []);

  if (loading) return <LoaderScreen />;

  return (
    <section className="py-20 px-6 bg-slate-600 text-white">
      <div className="max-w-screen-xl mx-auto flex flex-col lg:gap-8 gap-4">
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-white">Available chats</h2>
          <p className="text-lg opacity-80 mt-2">Discover new chats and friend around the world.</p>
        </div>
        <div className="grid lg:grid-cols-3 md:grid-cols-2 grid-cols-1 lg:gap-x-20 gap-x-10 gap-y-5">
          {chats?.map((value, index) => (
            <ChatPreview key={index} id={value.id} title={value.title} />
          ))}
        </div>
      </div>
    </section>
  );
}
