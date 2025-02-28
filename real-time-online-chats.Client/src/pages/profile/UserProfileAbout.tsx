import { useUserProfile } from "./UserProfileContext";

const UserProfileAbout = () => {
  const { aboutMe } = useUserProfile();

  return (
    <div className="flex w-full flex-grow flex-col gap-6 p-8 bg-slate-700 rounded-2xl shadow-lg">
      <section className="relative flex flex-col gap-12 m-auto">
        <h2 className="text-4xl font-bold text-white text-center">About Me</h2>
        <div className="flex gap-8 lg:flex-row flex-col">
          <div className="w-full flex flex-col gap-6 p-8 bg-slate-600 rounded-2xl shadow-lg">
            <p className="text-white leading-relaxed text-opacity-90">
              {aboutMe != null ? <>{aboutMe}</> : <>There is nothing yet...</>}
            </p>
          </div>
        </div>
      </section>
    </div>
  );
};

export default UserProfileAbout;
