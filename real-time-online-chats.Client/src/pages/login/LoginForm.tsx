import { Apple, Facebook } from "@src/components/svg/SVGAuthProviders";
import Button from "@components/ui/Button";
import { Link } from "react-router-dom";
import useFormValidation from "@src/hooks/useFormValidation";
import { toast } from "react-toastify";
import Input from "@src/components/ui/Input";
import GoogleLogin from "@src/services/google/GoogleLogin";
import { twMerge } from "tailwind-merge";
import Label from "@src/components/ui/Label";
import Checkbox from "@src/components/ui/Checkbox";
import { GOOGLE_RECAPTCHA_CLIENT_KEY } from "@src/services/google/googleConstants";
import { useEffect, useRef, useState } from "react";
import { useAuth } from "@src/hooks/useAuth";

export interface LoginFormData {
  email: string;
  password: string;
  rememberMe: boolean;
}

type LoginFormProps = React.HTMLAttributes<HTMLFormElement>;

const LoginForm = ({ className }: LoginFormProps) => {
  const { validationErrors, validate, isValid } = useFormValidation<LoginFormData>({
    email: {
      errorMessage: "Email is required",
      condition: (form) => form.email !== "",
    },
    password: {
      errorMessage: "Password must be at least 8 characters long",
      condition: (form) => form.password?.length >= 8,
    },
  });

  const [captchaToken, setCaptchaToken] = useState<string | undefined>(undefined);
  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
    rememberMe: false,
  });

  const reCAPTCHARef = useRef<HTMLDivElement>(null);

  const { loginUser } = useAuth();

  useEffect(() => {
    if (window.grecaptcha && !window.recaptchaWidgetId) {
      window.recaptchaWidgetId = 'rendered';
      window.grecaptcha.ready(() => {
        window.grecaptcha.render(reCAPTCHARef.current, {
          sitekey: GOOGLE_RECAPTCHA_CLIENT_KEY,
          callback: (token: string) => {
            console.log('Captcha resolved:', token);
            setCaptchaToken(token);
            reCAPTCHARef.current!.children[0].classList.remove("border", "border-solid", "border-red-500");
          }
        })
      })
    }

    return () => {
      window.recaptchaWidgetId = '';
    }
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const updatedData = {
      ...formData,
      [e.target.name]: e.target.value,
    };

    setFormData(updatedData);
    validate(updatedData);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!isValid) {
      toast.error("Please fill out required fields", {
        autoClose: 2000,
      });
      return;
    }

    if (!captchaToken) {
      reCAPTCHARef.current!.children[0].classList.add("border", "border-solid", "border-red-500");
      return;
    }

    await loginUser(formData, { headers: { reCAPTCHAToken: captchaToken } });
    reCAPTCHARef.current!.children[0].classList.remove("border", "border-solid", "border-red-500");
    window.grecaptcha.reset();
  };

  return (
    <form
      className={twMerge(
        "bg-white rounded-2xl px-8 py-10 space-y-8 shadow-2xl border border-gray-100",
        className
      )}
      onSubmit={handleSubmit}
    >
      <div className="flex flex-col gap-3">
        <h3 className="text-black text-4xl font-bold text-center">Welcome Back</h3>
        <p className="text-gray-600 text-sm text-center">Log in to your account to continue</p>
      </div>

      <div className="space-y-2">
        <Label>Email</Label>
        <Input
          value={formData.email}
          onChange={handleChange}
          name="email"
          type="email"
          placeholder="Enter your email"
          className="w-full"
        />
        {validationErrors["email"].errorMessage && (
          <div className="text-red-500 text-sm mt-1">{validationErrors["email"].errorMessage}</div>
        )}
      </div>

      <div className="space-y-2">
        <Label>Password</Label>
        <Input
          value={formData.password}
          onChange={handleChange}
          name="password"
          type="password"
          placeholder="Enter your password"
          className="w-full"
        />
        {validationErrors["password"].errorMessage && (
          <div className="text-red-500 text-sm mt-1">
            {validationErrors["password"].errorMessage}
          </div>
        )}
      </div>

      <div className="flex justify-between items-center">
        <div className="flex gap-1 items-center">
          <Checkbox
            checked={formData.rememberMe}
            onChange={(e) => setFormData({ ...formData, rememberMe: e.target.checked })}
            name="rememberMe"
            type="checkbox"
          />
          <Label>Remember me</Label>
        </div>
        <Link
          to="javascript:void(0);"
          className="text-blue-600 text-sm font-semibold hover:underline hover:text-blue-700 transition-colors duration-200"
        >
          Forgot your password?
        </Link>
      </div>

      <div ref={reCAPTCHARef} />

      <Button type="submit" className="w-full">
        Sign in
      </Button>

      <div className="flex items-center">
        <div className="flex-grow border-t border-gray-300"></div>
        <span className="mx-4 text-sm text-gray-500">or continue with</span>
        <div className="flex-grow border-t border-gray-300"></div>
      </div>

      <div className="flex justify-center items-center gap-6">
        <button
          type="button"
          className="p-3 rounded-lg border border-gray-300 hover:bg-gray-50 transition-colors duration-200"
        >
          <GoogleLogin />
        </button>
        <button
          type="button"
          className="p-3 rounded-lg border border-gray-300 hover:bg-gray-50 transition-colors duration-200"
        >
          <Apple width={24} fill="black" />
        </button>
        <button
          type="button"
          className="p-3 rounded-lg border border-gray-300 hover:bg-gray-50 transition-colors duration-200"
        >
          <Facebook width={24} className="fill-blue-600" />
        </button>
      </div>
    </form>
  );
};

export default LoginForm;
