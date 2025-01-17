import ChatPreview from "../chats/chat-preview";

export default function ChatsSection() {
  return (
    <section className="bg-black w-full">
      <div className="max-w-screen-xl mx-auto py-12 px-6 flex flex-col lg:gap-8 gap-4">
        <p className="text-center text-5xl text-white">Available chats</p>
        <div className="grid lg:grid-cols-3 md:grid-cols-2 grid-cols-1 lg:gap-x-20 gap-x-10 gap-y-5">
          <ChatPreview />
          <ChatPreview />
          <ChatPreview />
          <ChatPreview />
          <ChatPreview />
          <ChatPreview />
          <ChatPreview />
        </div>
      </div>
    </section>
  );
}
