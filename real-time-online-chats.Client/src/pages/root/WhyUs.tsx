import {
  CheckCircle,
  Coffee,
  Globe,
  Heart,
  Lock,
  MessageCircle,
  Users,
  Zap,
} from "@src/assets/images/svgr/common";

export default function WhyUsSection() {
  const features = [
    {
      title: "100% Free",
      description: "No hidden fees or premium features. Everything is accessible to everyone.",
      Icon: <CheckCircle className="text-white opacity-70" />,
    },
    {
      title: "Real-time messaging",
      description: "Your conversations are private and secure. We don't store any chat history.",
      Icon: <Lock className="text-white opacity-70" />,
    },
    {
      title: "Privacy Focused",
      description: "Start chatting instantly - no registration or downloads required.",
      Icon: <Coffee className="text-white opacity-70" />,
    },
    {
      title: "Works on all devices",
      description: "Developed with ❤️ by Quikler.",
      Icon: <Heart className="text-white opacity-70" />,
    },
    {
      title: "Real-time Chat",
      description: "Instant messaging with no delays or refreshing needed.",
      Icon: <MessageCircle className="text-white opacity-70" />,
    },
    {
      title: "Group Chats",
      description: "Create public or private rooms for group conversations.",
      Icon: <Users className="text-white opacity-70" />,
    },
    {
      title: "Global Access",
      description: "Connect with people from around the world, anytime.",
      Icon: <Globe className="text-white opacity-70" />,
    },
    {
      title: "Lightning Fast",
      description: "Optimized performance for a smooth chatting experience.",
      Icon: <Zap className="text-white opacity-70" />,
    },
  ];

  return (
    <section className="py-20 px-6 bg-slate-600 text-white">
      <div className="text-center mb-12">
        <h2 className="text-4xl font-bold text-white">Why Us?</h2>
        <p className="text-lg opacity-80 mt-2">Discover what makes our platform unique.</p>
      </div>

      <div className="max-w-screen-xl mx-auto grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {features.map((feature, index) => (
          <div
            key={index}
            className="flex items-start cursor-default gap-4 p-6 bg-slate-800 rounded-xl shadow-lg hover:bg-slate-700 transition-all"
          >
            {feature.Icon}
            <div className="flex flex-col gap-2">
              <h3 className="font-bold text-xl">{feature.title}</h3>
              <p className="opacity-80">{feature.description}</p>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}
