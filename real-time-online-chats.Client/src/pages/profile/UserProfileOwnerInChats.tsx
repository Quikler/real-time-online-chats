import { useEffect, useState } from "react";
import { useUserProfile } from "./UserProfileContext";
import { ChatPreviewResponse } from "@src/models/dtos/Chat";
import { UserService } from "@src/services/api/UserService";
import ChatPreview from "../chats/ChatPreview";
import { PaginationResponse } from "@src/models/dtos/Shared";

const UserProfileOwnerInChats = () => {
  const { id } = useUserProfile();

  const [ownerChatsPagination, setOwnerChatsPagination] = useState<PaginationResponse<ChatPreviewResponse>>();

  useEffect(() => {
    const abort = new AbortController();

    UserService.getUserOwnerChats(id, { signal: abort.signal })
      .then(data => setOwnerChatsPagination(data))
      .catch(e => console.error(`Unable to get chats: ${e.message}`))

    return () => abort.abort();
  }, [])

  return (
    <div className="flex w-full flex-grow flex-col gap-6 p-8 bg-slate-700 rounded-2xl shadow-lg">
      <h2 className="text-4xl font-bold text-white text-center">Owned chats</h2>
      <section className="relative flex flex-col gap-12 m-auto">
          {ownerChatsPagination?.items.length !== 0 ? ownerChatsPagination?.items?.map((value, index) => (
            <ChatPreview key={index} id={value.id} title={value.title} />
          )) : "No chats"}
      </section>
    </div>
  );
};

export default UserProfileOwnerInChats;
