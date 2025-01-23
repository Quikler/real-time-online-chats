import { Users } from "../../assets/images/svgr/common";

export default function ChatsHeaderSection() {
  return (
    <section className="w-full h-screen flex justify-center items-center m-auto max-w-screen-xl">
      <div className="-z-10 w-full h-full flex overflow-hidden absolute right-0 top-0">
        <div
          style={{ backgroundSize: "100% 100%" }}
          className="w-full bg-no-repeat bg-[url('/images/Lovepik_com-500361605-blue-banner-posters-background.jpg')]"
        ></div>
      </div>

      <div className="flex flex-col gap-12 px-[10%] text-white w-full">
        <div className="flex items-center gap-6">
          <Users width={96} height={96} className="text-white" />
          <div className="text-6xl tracking-[16px]">
            <p>CHATS SECTION</p>
          </div>
        </div>
        <div>
          <p className="text-2xl font-light text-justify leading-[140%]">
            Hello in chats section! If you don't have an account don't worry you
            can chat with anyone anywhere without registration needed! Simply
            find the chat or create one, but be sure after the browser close
            your chats and data will be gone!
          </p>
          <div className="flex gap-2">
            <button className="mt-8 w-auto bg-lightGreen-100 hover:bg-lightGreen-200 text-white font-bold py-2 px-4 rounded">
              Create chat
            </button>
            <button className="mt-8 w-auto bg-maroon-100 hover:bg-maroon-200 text-white font-bold py-2 px-4 rounded">
              Find chat
            </button>
          </div>
        </div>
      </div>
    </section>
  );
}
