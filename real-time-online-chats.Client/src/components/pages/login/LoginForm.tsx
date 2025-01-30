import { Google, Apple, Facebook } from "@src/assets/images/svgr/auth-with";
import Button from "@components/ui/Button";
import { Link } from "react-router-dom";
import useFormValidation from "@src/hooks/useFormValidation";
import { toast } from "react-toastify";
import Input from "@src/components/ui/Input";
import { useState } from "react";

export interface LoginFormData {
  email: string;
  password: string;
  rememberMe: boolean;
}

type LoginFormProps = React.HTMLAttributes<HTMLFormElement> & {
  formData: LoginFormData;
  setFormData: React.Dispatch<React.SetStateAction<LoginFormData>>;
};

const LoginForm = ({ onSubmit, formData, setFormData, ...rest }: LoginFormProps) => {
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const updatedData = {
      ...formData,
      [e.target.name]: e.target.value,
    };

    setFormData(updatedData);
    validate(updatedData);
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!isValid) {
      toast.error("Please fill out required fields", {
        autoClose: 2000,
      });
      return;
    }

    onSubmit?.(e);
  };

  return (
    <form
      {...rest}
      className="bg-white rounded-xl px-6 py-8 space-y-6 max-w-md md:ml-auto w-full mx-auto"
      onSubmit={handleSubmit}
    >
      <h3 className="text-black text-3xl font-extrabold max-md:text-center">Log in</h3>
      <div className="space-y-3 gap-6 flex flex-col">
        <div>
          <label className="text-gray-800 text-sm mb-2 block">Email</label>
          <Input
            value={formData.email}
            onChange={handleChange}
            name="email"
            type="email"
            placeholder="Enter email"
            className="w-full text-sm"
          />
          <div
            className={
              "text-red-500 text-sm" + ` ${validationErrors["email"].errorMessage ? "mt-1" : ""}`
            }
          >
            {validationErrors["email"].errorMessage}
          </div>
        </div>
        <div>
          <label className="text-gray-800 text-sm mb-2 block">Password</label>
          <Input
            value={formData.password}
            onChange={handleChange}
            name="password"
            type="password"
            placeholder="Enter password"
            className="w-full text-sm"
          />
          <div
            className={
              "text-red-500 text-sm" + ` ${validationErrors["password"].errorMessage ? "mt-1" : ""}`
            }
          >
            {validationErrors["password"].errorMessage}
          </div>
        </div>
      </div>
      <div className="text-sm text-right">
        <div className="flex justify-between">
          <div className="flex items-center">
            <input
              checked={formData.rememberMe}
              onChange={(e) => setFormData({ ...formData, rememberMe: e.target.checked })}
              name="rememberMe"
              type="checkbox"
              className="h-4 w-4 shrink-0 rounded"
            />
            <label className="ml-3 block text-sm">Remember me</label>
          </div>
          <Link to="jajvascript:void(0);" className="text-blue-600 font-semibold hover:underline">
            Forgot your password?
          </Link>
        </div>
      </div>
      <div>
        <Button type="submit" className="w-full">
          Sign in
        </Button>
      </div>
      <p className="my-6 text-sm text-gray-400 text-center">or continue with</p>
      <div className="space-x-6 flex justify-center items-center">
        <button type="button" className="border-none outline-none">
          <Google width={32} />
        </button>
        <button type="button" className="border-none outline-none">
          <Apple width={32} fill="black" />
        </button>
        <button type="button" className="border-none outline-none">
          <Facebook width={32} className="fill-blue-600" />
        </button>
      </div>
    </form>
  );
};

export default LoginForm;
