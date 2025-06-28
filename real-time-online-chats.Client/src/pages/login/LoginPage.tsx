import Logo from "@src/components/ui/Logo";
import { Link } from "react-router";
import LoginForm from "./LoginForm";

export default function LoginPage() {
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

        <LoginForm className="max-w-md lg:mx-16 mt-8 mx-auto" />
      </div>
    </div>
  );
}
