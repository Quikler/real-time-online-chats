import { useEffect } from "react";
import { initGoogleGSIScript, initGoogleAuth, renderGoogleButton } from "./googleHelpers";
import { useAuth } from "@src/hooks/useAuth";
import { toast } from "react-toastify";

const GoogleSignup = () => {
  const { signupGoogle } = useAuth();

  const googleAuthCallback = async (response: any) => {
    try {
      await signupGoogle(response.credential);
    } catch (e: any) {
      toast.error(e.message);
    }
  };

  const googleButtonStyle = {};

  useEffect(() => {
    if (window.google) {
      initGoogleAuth(googleAuthCallback);
      renderGoogleButton(googleButtonStyle);
    } else {
      initGoogleGSIScript(() => {
        initGoogleAuth(googleAuthCallback);
        renderGoogleButton(googleButtonStyle);
      });
    }
  }, []);

  return (
    <>
      <div
        id="g_id_onload"
        data-client_id="915197020819-23914pt8hnh35qemg74m6hi2tf0q0v65.apps.googleusercontent.com"
        data-context="signup"
        data-ux_mode="popup"
        data-login_uri="https://localhost:7207/api/v1/identity/signup-google"
        data-itp_support="true"
      ></div>

      <div
        className="g_id_signin"
        data-type="standard"
        data-shape="rectangular"
        data-theme="outline"
        data-text="signup_with"
        data-size="large"
        data-logo_alignment="left"
      ></div>
    </>
  );
};

export default GoogleSignup;
