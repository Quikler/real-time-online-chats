import { Users } from "@src/assets/images/svgr/common";
import Button from "@src/components/ui/Button";

export default function ChatsHeaderSection() {
  return (
    <section className="w-full lg:h-screen lg:my-0 my-24 flex justify-center items-center m-auto max-w-screen-xl">
      <div className="flex flex-col gap-6 px-[10%] text-white w-full">
        <div className="flex items-center gap-6">
          <Users width={96} height={96} className="text-slate-700" />
          <div className="text-6xl text-slate-700">
            <p>CHATS</p>
            <p>SECTION</p>
          </div>
        </div>

        <p className="text-2xl font-light text-justify text-slate-700">
          Hello in chats section! If you don't have an account don't worry you can chat with anyone
          anywhere without registration needed! Simply find the chat or create one, but be sure
          after the browser close your chats and data will be gone!
        </p>
        
        <div className="flex gap-2">
          <Button>Create chat</Button>
          <Button variant="secondary">Find chat</Button>
        </div>
      </div>
    </section>
  );
}
