import { useState } from "react";
import { Apple, Facebook, Google } from "@src/assets/images/svgr/auth-with";
import { useAuth } from "@src/contexts/AuthContext";
import SignupForm, { SignupFormData } from "./SignupForm";
import GoogleSignup from "@src/services/google/GoogleSignup";

export default function SignupPage() {
  const { signupUser } = useAuth();

  const [formData, setFormData] = useState<SignupFormData>({
    email: "",
    firstName: "",
    lastName: "",
    phone: "",
    password: "",
    confirmPassword: "",
    rememberMe: true,
  });

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    signupUser(formData);
  };

  return (
    <>
      <div className="flex flex-col justify-center items-center bg-darkBlue-100 lg:px-16 lg:py-32 pt-16">
        <div className="grid md:grid-cols-2 items-center gap-y-8 bg-white max-w-7xl w-full shadow-[0_2px_10px_-3px_rgba(6,81,237,0.3)] lg:rounded-md overflow-hidden">
          <div className="max-md:order-1 flex flex-col sm:p-8 p-4 bg-gradient-to-r from-lightGreen-100 to-darkBlue-200 w-full h-full">
            <div className="max-w-md space-y-12 mx-auto">
              <div>
                <h4 className="text-white text-lg font-semibold">Create Your Account</h4>
                <p className="text-[13px] text-white mt-2">
                  Welcome to our registration page! Get started by creating your account.
                </p>
              </div>
              <div>
                <h4 className="text-white text-lg font-semibold">
                  Simple &amp; Secure Registration
                </h4>
                <p className="text-[13px] text-white mt-2">
                  Our registration process is designed to be straightforward and secure. We
                  prioritize your privacy and data security.
                </p>
              </div>
              <div className="space-y-6">
                <button
                  type="button"
                  className="w-full px-5 py-2.5 flex items-center justify-center rounded-md text-white text-base tracking-wider font-semibold border-none outline-none bg-blue-600 hover:bg-blue-700"
                >
                  <Facebook />
                  Continue with Facebook
                </button>
                {/* <button
                type="button"
                className="w-full px-5 py-2.5 flex items-center justify-center rounded-md text-gray-800 text-base tracking-wider font-semibold border-none outline-none bg-gray-100 hover:bg-gray-200"
              >
                <Google />
                Continue with Google
              </button> */}

                <GoogleSignup />

                <button
                  type="button"
                  className="w-full px-5 py-2.5 flex items-center justify-center rounded-md text-white text-base tracking-wider font-semibold border-none outline-none bg-black hover:bg-[#333]"
                >
                  <Apple />
                  Continue with Apple
                </button>
              </div>
            </div>
          </div>
          <SignupForm onSubmit={handleSubmit} formData={formData} setFormData={setFormData} />
        </div>
      </div>
    </>
  );
}
