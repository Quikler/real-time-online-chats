import { useState } from "react";

type FormValidations<T> = {
  [key: string]: {
    errorMessage: string;
    condition: (form: T) => boolean;
  };
};

type KeyErrors = {
  [key: string]: {
    errorMessage: string;
  };
};

const useFormValidation = <T>(validations: FormValidations<T>) => {
  const [validationErrors, setValidationErrors] = useState<KeyErrors>(
    Object.keys(validations).reduce((err, key) => {
      err[key] = { errorMessage: validations[key].errorMessage };
      return err;
    }, {} as KeyErrors)
  );

  const [isValid, setIsValid] = useState(false);

  const validate = (formData: T): boolean => {
    let allValid = true;
    for (const key in validations) {
      const isValid = validations[key].condition(formData);
      if (!isValid) {
        allValid = false;
      }
      
      setValidationErrors((prevErrors) => ({
        ...prevErrors,
        [key]: {
          ...prevErrors[key],
          errorMessage: isValid ? "" : validations[key].errorMessage,
        },
      }));
    }

    setIsValid(allValid);
    return allValid;
  };

  return { validationErrors, validate, isValid };
};

export default useFormValidation;
