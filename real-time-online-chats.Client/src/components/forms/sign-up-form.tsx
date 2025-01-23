import { useState } from "react";
import { Apple, Facebook, Google } from "../../assets/images/svgr/auth-with";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";
import { useAuth } from "../../contexts/auth-context";

export interface SignupFormData {
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  password: string;
  confirmPassword: string;
  rememberMe: boolean;
}

interface SignUpFormDataErrors {
  [key: string]: {
    key: string;
    errorMessage: string;
    validation: (value: string, formData: SignupFormData) => string;
    isValid: boolean;
  };
}

export default function SignUpForm() {
  const { signupUser } = useAuth();
  // if (isUserLoggedIn()) {
  //   return <Navigate to="/" />
  // }

  const [formData, setFormData] = useState<SignupFormData>({
    email: "",
    firstName: "",
    lastName: "",
    phone: "",
    password: "",
    confirmPassword: "",
    rememberMe: true,
  });

  const validationErrors = {
    email: "Email is required",
    phone: "Phone must be in valid format",
    password: "Password must be at least 8 characters long",
    confirmPassword: "Passwords should match",
  };

  const [formDataErrors, setFormDataErrors] = useState<SignUpFormDataErrors>({
    firstName: {
      key: "firstName",
      errorMessage: "",
      validation: () => "",
      isValid: true,
    },
    lastName: {
      key: "lastName",
      errorMessage: "",
      validation: () => "",
      isValid: true,
    },
    email: {
      key: "email",
      errorMessage: validationErrors.email,
      validation: function (value: string) {
        return (this.isValid = value != "") ? "" : validationErrors.email;
      },
      isValid: false,
    },
    phone: {
      key: "phone",
      errorMessage: validationErrors.phone,
      validation: function (value: string) {
        return (this.isValid = /^\d{3,5}\d{3}\d{4}$/.test(value))
          ? ""
          : validationErrors.phone;
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
    confirmPassword: {
      key: "confirmPassword",
      errorMessage: validationErrors.confirmPassword,
      validation: function (value: string, formData: SignupFormData) {
        return (this.isValid = value?.length >= 8 && value == formData.password)
          ? ""
          : validationErrors.confirmPassword;
      },
      isValid: false,
    },
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setFormData((prevData) => {
      const updatedData = { ...prevData, [name]: value };

      setFormDataErrors((prevErrors) => {
        const updatedErrors = { ...prevErrors };

        updatedErrors[name].errorMessage = updatedErrors[name].validation(
          value,
          updatedData
        );

        // Recalculate the dependent field
        if (name === "password") {
          updatedErrors.confirmPassword.errorMessage =
            updatedErrors.confirmPassword.validation(
              updatedData.confirmPassword,
              updatedData
            );
        } else if (name === "confirmPassword") {
          updatedErrors.password.errorMessage =
            updatedErrors.password.validation(
              updatedData.password,
              updatedData
            );
        }

        return updatedErrors;
      });

      return updatedData;
    });
  };

  function handleFormSubmit(e: React.FormEvent<HTMLFormElement>): void {
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

    signupUser(formData);
  }

  return (
    <div className="flex flex-col justify-center items-center bg-darkBlue-100 lg:px-16 lg:py-32 pt-16">
      <div className="grid md:grid-cols-2 items-center gap-y-8 bg-white max-w-7xl w-full shadow-[0_2px_10px_-3px_rgba(6,81,237,0.3)] lg:rounded-md overflow-hidden">
        <div className="max-md:order-1 flex flex-col sm:p-8 p-4 bg-gradient-to-r from-lightGreen-100 to-darkBlue-200 w-full h-full">
          <div className="max-w-md space-y-12 mx-auto">
            <div>
              <h4 className="text-white text-lg font-semibold">
                Create Your Account
              </h4>
              <p className="text-[13px] text-white mt-2">
                Welcome to our registration page! Get started by creating your
                account.
              </p>
            </div>
            <div>
              <h4 className="text-white text-lg font-semibold">
                Simple &amp; Secure Registration
              </h4>
              <p className="text-[13px] text-white mt-2">
                Our registration process is designed to be straightforward and
                secure. We prioritize your privacy and data security.
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
              <button
                type="button"
                className="w-full px-5 py-2.5 flex items-center justify-center rounded-md text-gray-800 text-base tracking-wider font-semibold border-none outline-none bg-gray-100 hover:bg-gray-200"
              >
                <Google />
                Continue with Google
              </button>
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
        <form className="sm:p-8 p-4 w-full" onSubmit={handleFormSubmit}>
          <div className="mb-12">
            <h3 className="text-blue-500 text-3xl font-extrabold max-md:text-center">
              Registration
            </h3>
          </div>
          <div className="grid lg:grid-cols-2 gap-6">
            <div>
              <label className="text-gray-800 text-sm mb-2 block">
                First Name
              </label>
              <input
                value={formData.firstName}
                onChange={handleChange}
                name="firstName"
                type="text"
                className="bg-gray-100 w-full text-gray-800 text-sm px-4 py-3 rounded-md outline-blue-500"
                placeholder="Enter name"
              />
            </div>
            <div>
              <label className="text-gray-800 text-sm mb-2 block">
                Last Name
              </label>
              <input
                value={formData.lastName}
                onChange={handleChange}
                name="lastName"
                type="text"
                className="bg-gray-100 w-full text-gray-800 text-sm px-4 py-3 rounded-md outline-blue-500"
                placeholder="Enter last name"
              />
            </div>
            <div>
              <label className="text-gray-800 text-sm mb-2 block">Email</label>
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
                Mobile Phone
              </label>
              <input
                value={formData.phone}
                onChange={handleChange}
                name="phone"
                type="tel"
                className="bg-gray-100 w-full text-gray-800 text-sm px-4 py-3 rounded-md outline-blue-500"
                placeholder="380977777777"
              />
              <div
                className={
                  "text-red-500 text-sm" +
                  ` ${formDataErrors["phone"].errorMessage ? "mt-1" : ""}`
                }
              >
                {formDataErrors["phone"].errorMessage}
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
            <div>
              <label className="text-gray-800 text-sm mb-2 block">
                Confirm Password
              </label>
              <input
                value={formData.confirmPassword}
                onChange={handleChange}
                name="confirmPassword"
                type="password"
                className="bg-gray-100 w-full text-gray-800 text-sm px-4 py-3 rounded-md outline-blue-500"
                placeholder="Enter confirm password"
              />
              <div
                className={
                  "text-red-500 text-sm" +
                  ` ${
                    formDataErrors["confirmPassword"].errorMessage ? "mt-1" : ""
                  }`
                }
              >
                {formDataErrors["confirmPassword"].errorMessage}
              </div>
            </div>
          </div>
          <div className="mt-6 flex justify-between items-center">
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
              to="/login"
              className="text-blue-600 text-sm font-semibold hover:underline"
            >
              Already have an acount?
            </Link>
          </div>
          <div className="mt-6">
            <button
              type="submit"
              className="py-3 px-6 text-sm tracking-wide font-semibold rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none"
            >
              Sign up
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
