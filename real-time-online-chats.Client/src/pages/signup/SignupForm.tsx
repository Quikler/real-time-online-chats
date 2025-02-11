import Button from "@src/components/ui/Button";
import Checkbox from "@src/components/ui/Checkbox";
import Input from "@src/components/ui/Input";
import Label from "@src/components/ui/Label";
import useFormValidation from "@src/hooks/useFormValidation";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";

export interface SignupFormData {
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  password: string;
  confirmPassword: string;
  rememberMe: boolean;
}

type SignupFormProps = React.FormHTMLAttributes<HTMLFormElement> & {
  formData: SignupFormData;
  setFormData: React.Dispatch<React.SetStateAction<SignupFormData>>;
};

const SignupForm = ({ onSubmit, formData, setFormData, ...rest }: SignupFormProps) => {
  const { validate, isValid, validationErrors } = useFormValidation<SignupFormData>({
    email: {
      errorMessage: "Email is required",
      condition: (form) => form.email !== "",
    },
    phone: {
      errorMessage: "Phone must be in valid format",
      condition: (form) => /^\d{3,5}\d{3}\d{4}$/.test(form.phone),
    },
    password: {
      errorMessage: "Password must be at least 8 characters long",
      condition: (form) => form.password === form.confirmPassword && form.password?.length >= 8,
    },
    confirmPassword: {
      errorMessage: "Passwords should match",
      condition: (form) => form.password === form.confirmPassword && form.password?.length >= 8,
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

  function handleFormSubmit(e: React.FormEvent<HTMLFormElement>): void {
    e.preventDefault();

    if (!isValid) {
      toast.error("Please fill out required fields", {
        autoClose: 2000,
      });
      return;
    }

    onSubmit?.(e);
  }

  return (
    <form {...rest} className="sm:p-8 p-4 w-full" onSubmit={handleFormSubmit}>
      <div className="flex flex-col gap-3 mb-12">
        <h3 className="text-black text-4xl font-bold">Registration</h3>
        <p className="text-gray-600 text-sm">
          Fill free to create new account if you don't have one
        </p>
      </div>
      <div className="grid lg:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label>First Name</Label>
          <Input
            variant="secondary"
            value={formData.firstName}
            onChange={handleChange}
            name="firstName"
            type="text"
            className="w-full"
            placeholder="Enter name"
          />
        </div>
        <div className="space-y-2">
          <Label>Last Name</Label>
          <Input
            variant="secondary"
            value={formData.lastName}
            onChange={handleChange}
            name="lastName"
            type="text"
            className="w-full"
            placeholder="Enter last name"
          />
        </div>
        <div className="space-y-2">
          <Label>Email</Label>
          <Input
            value={formData.email}
            onChange={handleChange}
            name="email"
            type="email"
            className="w-full"
            placeholder="Enter email"
          />
          <div
            className={
              "text-red-500 text-sm" + ` ${validationErrors["email"].errorMessage ? "mt-1" : ""}`
            }
          >
            {validationErrors["email"].errorMessage}
          </div>
        </div>
        <div className="space-y-2">
          <Label>Mobile Phone</Label>
          <Input
            value={formData.phone}
            onChange={handleChange}
            name="phone"
            type="tel"
            className="w-full"
            placeholder="380977777777"
          />
          <div
            className={
              "text-red-500 text-sm" + ` ${validationErrors["phone"].errorMessage ? "mt-1" : ""}`
            }
          >
            {validationErrors["phone"].errorMessage}
          </div>
        </div>
        <div className="space-y-2">
          <Label>Password</Label>
          <Input
            value={formData.password}
            onChange={handleChange}
            name="password"
            type="password"
            className="w-full"
            placeholder="Enter password"
          />
          <div
            className={
              "text-red-500 text-sm" + ` ${validationErrors["password"].errorMessage ? "mt-1" : ""}`
            }
          >
            {validationErrors["password"].errorMessage}
          </div>
        </div>
        <div className="space-y-2">
          <Label>Confirm Password</Label>
          <Input
            value={formData.confirmPassword}
            onChange={handleChange}
            name="confirmPassword"
            type="password"
            className="w-full"
            placeholder="Enter confirm password"
          />
          <div
            className={
              "text-red-500 text-sm" +
              ` ${validationErrors["confirmPassword"].errorMessage ? "mt-1" : ""}`
            }
          >
            {validationErrors["confirmPassword"].errorMessage}
          </div>
        </div>
      </div>
      <div className="mt-6 flex justify-between items-center">
        <div className="flex items-center gap-1">
          <Checkbox
            checked={formData.rememberMe}
            onChange={(e) => setFormData({ ...formData, rememberMe: e.target.checked })}
            name="rememberMe"
            type="checkbox"
          />
          <Label>Remember me</Label>
        </div>
        <Link to="/login" className="text-blue-600 text-sm font-semibold hover:underline">
          Already have an acount?
        </Link>
      </div>
      <Button className="mt-6" type="submit" variant="secondary">
        Sign up
      </Button>
    </form>
  );
};

export default SignupForm;
