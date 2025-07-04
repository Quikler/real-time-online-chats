import Button from "@src/components/ui/Button";
import Input from "@src/components/ui/Input";
import { useNavigate } from "react-router";

const ForgotPasswordPage = () => {
  const navigate = useNavigate();

  return <div className="flex-grow flex px-14 py-10 bg-gradient-to-tr from-green-500 via-violet-600 to-green-700">
      <div className="flex flex-row w-full gap-4 p-4 bg-white bg-[url('https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fstatic.vecteezy.com%2Fsystem%2Fresources%2Fpreviews%2F023%2F883%2F541%2Fnon_2x%2Fabstract-background-illustration-abstract-green-background-illustration-simple-green-background-for-wallpaper-display-landing-page-banner-or-layout-design-graphic-for-display-free-vector.jpg&f=1&nofb=1&ipt=b5a266d30663f025fb0d1a2431d4348978897c2d97999235b6b03790620ee395')] bg-no-repeat bg-cover">
      {/* Left side */}
      <div className="w-1/2 flex items-center">
        <img src="/images/svg/forgot-password.svg" />
      </div>

      {/* Right side */}
      <div className="w-1/2 flex items-center">
        <div className="flex flex-col w-full p-4">
          <div className="mb-10 space-y-2">
          <h1 className="text-4xl" style={{ lineHeight: "" }}>Forgot your password?</h1>
          <p className="italic font-thin">Enter your email address. We will send you link to reset password.</p>
          </div>
          <Input className="mb-6 border border-slate-400" placeholder="Email address" />
          <Button className="mb-2">Reset password</Button>
          <div className="w-auto text-center">
          <button onClick={() => navigate('/login')}>Go back</button>
          </div>
        </div>
      </div>
    </div>
  </div>
}

export default ForgotPasswordPage;
