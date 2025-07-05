import Button from "@src/components/ui/Button";
import Input from "@src/components/ui/Input";
import { ResetPasswordRequest } from "@src/models/dtos/Auth";
import { AuthService } from "@src/services/api/AuthService";
import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router";

const ResetPasswordPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const [newPassword, setNewPassword] = useState<string | null>();

  async function handleChangePassword() {
    const email = searchParams.get("email");
    const token = searchParams.get("token");

    if (newPassword && email && token) {
      const request: ResetPasswordRequest = {
        email: email,
        token: token,
        newPassword: newPassword,
      }

      const message = await AuthService.resetPassword(request);
      console.log("Reset password message:", message);
    } else {
      console.error("One or more arguments are missing");
    }
  }
  
  return <div className="flex-grow flex px-14 py-10 bg-gradient-to-tr from-green-500 via-violet-600 to-green-700">
      <div className="flex flex-row w-full gap-4 p-4 bg-white bg-[url('https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fstatic.vecteezy.com%2Fsystem%2Fresources%2Fpreviews%2F023%2F883%2F541%2Fnon_2x%2Fabstract-background-illustration-abstract-green-background-illustration-simple-green-background-for-wallpaper-display-landing-page-banner-or-layout-design-graphic-for-display-free-vector.jpg&f=1&nofb=1&ipt=b5a266d30663f025fb0d1a2431d4348978897c2d97999235b6b03790620ee395')] bg-no-repeat bg-cover">
      {/* Left side */}
      <div className="w-1/2 flex items-center">
        <img src="/images/svg/reset-password.svg" />
      </div>

      {/* Right side */}
      <div className="w-1/2 flex items-center">
        <div className="flex flex-col w-full p-4">
          <div className="mb-10 space-y-2">
          <h1 className="text-4xl" style={{ lineHeight: "" }}>Reset password</h1>
          <p className="italic font-thin">Enter new password. Remember to enter strong typed password.</p>
          </div>
          <Input required onChange={e => setNewPassword(e.target.value)} type="password" className="mb-6 border border-slate-400" placeholder="New password" />
          <Button onClick={handleChangePassword} className="mb-2">Change password</Button>
          <div className="w-auto text-center">
          <button onClick={() => navigate('/login')}>Go back</button>
          </div>
        </div>
      </div>
    </div>
  </div>
};

export default ResetPasswordPage;
