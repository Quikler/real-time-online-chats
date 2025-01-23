import { useState } from "react";
import { Apple, Facebook, Google } from "../../assets/images/svgr/auth-with";
import Logo from "../common/logo";
import { Link } from "react-router-dom";
import { useAuth } from "../../contexts/auth-context";
import { toast } from "react-toastify";

export interface LoginFormData {
  email: string;
  password: string;
  rememberMe: boolean;
}

interface LoginFormDataErrors {
  [key: string]: {
    key: string;
    errorMessage: string;
    validation: (value: string) => string;
    isValid: boolean;
  };
}

export default function LogInForm() {
  const { loginUser } = useAuth();

  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
    rememberMe: true,
  });

  const validationErrors = {
    email: "Email is required",
    password: "Password must be at least 8 characters long",
  };

  const [formDataErrors, setFormDataErrors] = useState<LoginFormDataErrors>({
    email: {
      key: "email",
      errorMessage: validationErrors.email,
      validation: function (value: string) {
        return (this.isValid = value != "") ? "" : validationErrors.email;
      },
      isValid: false,
    },
    password: {
      key: "password",
      errorMessage: validationErrors.password,
      validation: function (value: string) {
        return (this.isValid = value?.length >= 8)
          ? ""
          : validationErrors.password;
      },
      isValid: false,
    },
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));

    setFormDataErrors((prevErrors) => ({
      ...prevErrors,
      [name]: {
        ...prevErrors[name],
        errorMessage: prevErrors[name].validation(value),
      },
    }));
  };

  const handleFormSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const invalidFields = Object.values(formDataErrors)
      .filter((value) => !value.isValid)
      .map((value) => value.key);

    if (invalidFields.length > 0) {
      toast.error("Please fill out required fields", {
        autoClose: 2000,
      });
      console.log("Fields required", invalidFields);
      return;
    }

    loginUser(formData);
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
              Don't have an account{" "}
              <Link
                to="/signup"
                className="text-white font-semibold underline ml-1"
              >
                Register here
              </Link>
            </p>
          </div>
          <form
            className="bg-white rounded-xl px-6 py-8 space-y-6 max-w-md md:ml-auto w-full mx-auto"
            onSubmit={handleFormSubmit}
          >
            <h3 className="text-black text-3xl font-extrabold max-md:text-center">
              Log in
            </h3>
            <div className="space-y-3 gap-6 flex flex-col">
              <div>
                <label className="text-gray-800 text-sm mb-2 block">
                  Email
                </label>
                <input
                  value={formData.email}
                  onChange={handleChange}
                  name="email"
                  type="email"
                  className="bg-gray-100 w-full text-gray-800 text-sm px-4 py-3 rounded-md outline-blue-500"
                  placeholder="Enter email"
                />
                <div
                  className={
                    "text-red-500 text-sm" +
                    ` ${formDataErrors["email"].errorMessage ? "mt-1" : ""}`
                  }
                >
                  {formDataErrors["email"].errorMessage}
                </div>
              </div>
              <div>
                <label className="text-gray-800 text-sm mb-2 block">
                  Password
                </label>
                <input
                  value={formData.password}
                  onChange={handleChange}
                  name="password"
                  type="password"
                  className="bg-gray-100 w-full text-gray-800 text-sm px-4 py-3 rounded-md outline-blue-500"
                  placeholder="Enter password"
                />
                <div
                  className={
                    "text-red-500 text-sm" +
                    ` ${formDataErrors["password"].errorMessage ? "mt-1" : ""}`
                  }
                >
                  {formDataErrors["password"].errorMessage}
                </div>
              </div>
            </div>
            <div className="text-sm text-right">
              <div className="flex justify-between">
                <div className="flex items-center">
                  <input
                    checked={formData.rememberMe}
                    onChange={(e) =>
                      setFormData({ ...formData, rememberMe: e.target.checked })
                    }
                    name="rememberMe"
                    type="checkbox"
                    className="h-4 w-4 shrink-0 rounded"
                  />
                  <label className="ml-3 block text-sm">Remember me</label>
                </div>
                <Link
                  to="jajvascript:void(0);"
                  className="text-blue-600 font-semibold hover:underline"
                >
                  Forgot your password?
                </Link>
              </div>
            </div>
            <div>
              <button
                type="submit"
                className="w-full shadow-xl py-3 px-6 text-sm font-semibold rounded-lg text-white bg-lightGreen-100 hover:bg-lightGreen-200 focus:outline-none"
              >
                Sign in
              </button>
            </div>
            <p className="my-6 text-sm text-gray-400 text-center">
              or continue with
            </p>
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
        </div>
      </div>
    </div>
  );
}
