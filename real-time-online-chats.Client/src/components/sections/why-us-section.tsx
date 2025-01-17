import React from "react";
import {
  CheckCircle,
  Coffee,
  Globe,
  Heart,
  Lock,
  MessageCircle,
  Users,
  Zap,
} from "../../assets/images/svgr/common";
import FeatureItem, { FeatureItemProps } from "../feature-item";

export default function WhyUsSection() {
  const features: FeatureItemProps[] = [
    {
      title: "100% Free",
      description:
        "No hidden fees or premium features. Everything is accessible to everyone.",
      icon: <CheckCircle fill="white" opacity=".7" />,
      className: "lg:pl-2",
    },
    {
      title: "Real-time messaging",
      description:
        "Your conversations are private and secure. We don't store any chat history.",
      icon: <Lock fill="white" opacity=".7" />,
      className: "lg:pl-10",
    },
    {
      title: "Privacy focused",
      description:
        "Start chatting instantly - no registration or downloads required.",
      icon: <Coffee fill="white" opacity=".7" />,
      className: "lg:pl-16",
    },
    {
      title: "Works on all devices",
      description: "Developed with ❤️ by Quikler.",
      icon: <Heart fill="white" opacity=".7" />,
      className: "lg:pl-20",
    },
    {
      title: "Real-time Chat",
      description: "Instant messaging with no delays or refreshing needed.",
      icon: <MessageCircle fill="white" opacity=".7" />,
      className: "lg:pl-20",
    },
    {
      title: "Group Chats",
      description: "Create public or private rooms for group conversations.",
      icon: <Users fill="white" opacity=".7" />,
      className: "lg:pl-16",
    },
    {
      title: "Global Access",
      description: "Connect with people from around the world, anytime.",
      icon: <Globe fill="white" opacity=".7" />,
      className: "lg:pl-10",
    },
    {
      title: "Lightning Fast",
      description: "Optimized performance for smooth chatting experience.",
      icon: <Zap fill="white" opacity=".7" />,
      className: "lg:pl-2",
    },
  ];

  return (
    <section className="py-14 px-[2%] relative w-full flex items-center m-auto bg-black text-white text-opacity-70">
      <div className="flex justify-around w-full items-center lg:flex-row flex-col lg:gap-0 gap-8">
        <div>
          <div
            className="lg:text-4xl sm:text-2xl w-full bg-no-repeat bg-[url('./images/cool-circle.svg')]"
            style={{ backgroundSize: "100% 100%" }}
          >
            <div className="py-[45%] px-[90px] lg:px-[130px]">
              <p>
                ROC - free opensource and friendly community. Ready to enhance.
                Ready to go!
              </p>
            </div>
          </div>
        </div>
        <div className="flex flex-col lg:w-3/4 w-full gap-8">
          {features.map((val, index) => {
            return (
              <React.Fragment key={index}>
                <FeatureItem
                  className={val.className}
                  title={val.title}
                  description={val.description}
                  icon={val.icon}
                />
              </React.Fragment>
            );
          })}
        </div>
      </div>
    </section>
  );
}
