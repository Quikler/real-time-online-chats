import axios from "axios";
import { toast } from "react-toastify";

export function isNullOrWhitespace(input?: string) {
  return !input || !input.trim();
}

export const handleError = (error: any) => {
  if (axios.isAxiosError(error)) {
    var err = error.response;
    if (Array.isArray(err?.data.errors)) {
      for (let val of err?.data.errors) {
        toast.error(val);
      }
    } else if (typeof err?.data.errors === "object") {
      for (let e in err?.data.errors) {
        toast.error(err?.data.errors[e][0]);
      }
    } else if (err?.data) {
      toast.error(err?.data);
    } else if (err?.status == 401) {
      toast.error("Please login");
      window.history.pushState({}, "LoginPage", "/login");
    } else if (err) {
      toast.error(err?.data);
    }
  }
};

export const throwIfErrorNotCancelError = (error: any) => {
  if (!axios.isCancel(error)) {
    throw error;
  }
};

export const loadScript = (src: string) => {
  return new Promise((resolve, reject) => {
    if (document.querySelector(`script[src="${src}"]`)) return resolve(undefined);
    const script = document.createElement("script");
    script.src = src;
    script.onload = () => resolve(undefined);
    script.onerror = (err) => reject(err);
    document.body.appendChild(script);
  });
};
