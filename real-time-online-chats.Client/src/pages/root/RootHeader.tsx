import { MessageCircle } from "../../assets/images/svgr/common";
import ButtonLink from "../../components/ui/ButtonLink";

export default function HeaderSection(props: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <section
      className="w-full lg:h-screen lg:my-0 my-24 flex justify-center items-center m-auto max-w-screen-xl" // 60% slate-600
      {...props}
    >
      {/* <div className="-z-10 w-full h-full flex overflow-hidden absolute right-0 top-0">
        <div
          style={{ backgroundSize: "100% 100%" }}
          className="w-full bg-no-repeat bg-slate-500"
        ></div>
      </div> */}

      <div className="flex flex-col gap-6 px-[10%]">
        <div className="flex items-center gap-6 md:flex-row flex-col">
          <MessageCircle className="text-slate-700 size-[96px]" />
          <div className="text-6xl text-slate-700">
            <p>FREE REALTIME</p>
            <p>ONLINE CHAT</p>
          </div>
        </div>

        <p className="text-2xl font-light text-justify lg:leading-[140%] text-slate-700">
          Chat with anyone, anywhere – instantly and effortlessly! Not registration needed. Just
          pick a nickname and dive into the conversation. Make new friends, share ideas, and stay
          connected – the world is just a chat away!
        </p>

        <ButtonLink className="self-start" to="chats">Get started</ButtonLink>
      </div>
    </section>
  );
}
