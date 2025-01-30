import Logo from "../../ui/Logo";
import { Link } from "react-router-dom";
import { useAuth } from "../../../contexts/auth-context";
import LoginForm, { LoginFormData } from "./LoginForm";
import { mapResponseToDTO } from "@src/utils/mappers";
import { useState } from "react";

export default function LoginPage() {
  const { loginUser } = useAuth();

  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
    rememberMe: false,
  });

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    loginUser(mapResponseToDTO(formData));
  };

  return (
    <div className="bg-gradient-to-r from-darkBlue-100 via-purple-800 to-darkBlue-200 text-gray-800 lg:px-16 pt-12">
      <div className="min-h-screen flex fle-col items-center justify-center sm:p-8 p-4">
        <div className="grid md:grid-cols-2 items-center gap-10 max-w-6xl w-full">
          <div className="md:order-none order-1 mx-auto">
            <Logo className="self-start" scale={2} />
            <h2 className="lg:text-2xl text-2xl font-extrabold lg:leading-[50px] text-white">
              Seamless Login for Exclusive Access
            </h2>
            <p className="text-sm mt-6 text-white">
              Immerse yourself in a hassle-free login journey with our
              intuitively designed login form. Effortlessly access your account.
            </p>
            <p className="text-sm mt-6 text-white">
              Don't have an account
              <Link
                to="/signup"
                className="text-white font-semibold underline ml-1"
              >
                Register here
              </Link>
            </p>
          </div>
          <LoginForm
            formData={formData}
            setFormData={setFormData}
            onSubmit={handleSubmit}
          />
        </div>
      </div>
    </div>
  );
}
