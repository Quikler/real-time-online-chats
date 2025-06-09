import { Link } from "react-router-dom";
import LoginForm, { LoginFormData } from "./LoginForm";
import { useState } from "react";
import Logo from "@src/components/ui/Logo";
import { useAuth } from "@src/hooks/useAuth";
import { GOOGLE_RECAPTCHA_CLIENT_KEY } from "@src/services/google/googleConstants";

export default function LoginPage() {
  const { loginUser } = useAuth();

  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
    rememberMe: false,
  });

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    window.grecaptcha.enterprise.ready(async () => {
      const token = await window.grecaptcha.enterprise.execute(GOOGLE_RECAPTCHA_CLIENT_KEY, { action: 'LOGIN' });
      console.log('Token:', token);

      await loginUser(formData, { headers: { reCAPTCHAToken: token } });
    });
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4 sm:p-8">
      <div className="grid md:grid-cols-2 items-center gap-16 max-w-6xl w-full">
        <div className="text-center md:text-left">
          <Logo className="flex md:justify-start justify-center mt-10 lg:mt-0" scale={2} />
          <h2 className="text-3xl lg:text-4xl font-extrabold text-white mt-8 bg-clip-text bg-gradient-to-r from-blue-400 to-purple-500">
            Seamless Login for Exclusive Access
          </h2>
          <p className="text-sm text-gray-300 mt-6">
            Immerse yourself in a hassle-free login journey with our intuitively designed login
            form. Effortlessly access your account.
          </p>
          <p className="text-sm text-gray-300 mt-6">
            Don't have an account?&nbsp;
            <Link
              to="/signup"
              className="text-blue-400 font-semibold hover:text-blue-300 transition-colors duration-200"
            >
              Register here
            </Link>
          </p>
        </div>

        <LoginForm
          formData={formData}
          setFormData={setFormData}
          onSubmit={handleSubmit}
          className="max-w-md lg:mx-16 mt-8 mx-auto"
        />
      </div>
    </div>
  );
}
