import { MessageCircle } from "../../components/svg/SVGCommon";
import ButtonLink from "../../components/ui/ButtonLink";

export default function HeaderSection(props: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <section
      className="w-full lg:h-screen lg:my-0 my-24 flex justify-center items-center m-auto max-w-screen-xl"
      {...props}
    >
      <div className="flex flex-col gap-6 px-[10%] text-gray-300">
        <div className="flex items-center gap-6 md:flex-row flex-col">
          <MessageCircle className="size-[96px]" />
          <div className="text-6xl">
            <p>FREE REALTIME</p>
            <p>ONLINE CHAT</p>
          </div>
        </div>

        <p className="text-2xl font-light text-justify lg:leading-[140%]">
          Chat with anyone, anywhere – instantly and effortlessly! Not registration needed. Just
          pick a nickname and dive into the conversation. Make new friends, share ideas, and stay
          connected – the world is just a chat away!
        </p>

        <ButtonLink className="self-start" to="chats">
          Get started
        </ButtonLink>
      </div>
    </section>
  );
}
