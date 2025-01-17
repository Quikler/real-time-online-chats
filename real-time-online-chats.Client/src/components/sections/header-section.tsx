import { Link } from "react-router-dom";
import { MessageCircle } from "../../assets/images/svgr/common";

export default function HeaderSection(
  props: React.HTMLAttributes<HTMLDivElement>
) {
  return (
    <section
      className="w-full h-screen flex justify-center items-center m-auto max-w-screen-xl"
      {...props}
    >
      <div className="-z-10 w-full h-full flex overflow-hidden absolute right-0 top-0">
        <div
          style={{ backgroundSize: "100% 100%" }}
          className="w-full bg-no-repeat bg-[url('./images/hd-bg.webp')]"
        ></div>
      </div>

      <div className="flex flex-col lg:gap-12 gap-2 px-[10%] text-white w-full">
        <div className="flex items-center gap-6 max-[520px]:collapse visible">
          <MessageCircle className="text-white size-[96px]" />
          <div className="lg:text-6xl text-2xl tracking-[16px]">
            <p>FREE REALTIME</p>
            <p>ONLINE CHAT</p>
          </div>
        </div>
        <div className="flex flex-col lg:gap-8 gap-2">
          <p className="lg:text-2xl md:text-lg sm:text-sm text-[8px] font-light text-justify lg:leading-[140%]">
            Chat with anyone, anywhere – instantly and effortlessly! Not
            registration needed. Just pick a nickname and dive into the
            conversation. Make new friends, share ideas, and stay connected –
            the world is just a chat away!
          </p>
          <div>
            <Link to="chats">
              <button className="text-white bg-lightGreen-100 hover:bg-lightGreen-200 font-medium rounded-lg text-sm px-4 lg:px-5 py-2 lg:py-2.5">
                Get started
              </button>
            </Link>
          </div>
        </div>
      </div>
    </section>
  );
}
