declare global {
  interface Window {
    google: any;
    grecaptcha: { enterprise: any };
  }
}

export const GOOGLE_CLIENT_ID =
  "915197020819-23914pt8hnh35qemg74m6hi2tf0q0v65.apps.googleusercontent.com";

export const GOOGLE_CLIENT_GSI = "https://accounts.google.com/gsi/client";

export const GOOGLE_RECAPTCHA_CLIENT_KEY = "6LcrkFkrAAAAALrrEbm0btuqH6UVaiz_7rKqv33m";
