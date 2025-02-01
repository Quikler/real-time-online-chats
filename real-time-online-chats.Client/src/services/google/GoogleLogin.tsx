import { useEffect } from "react";
import { GoogleService } from "./GoogleService";
import { initGoogleAuth, renderGoogleButton, initGoogleGSIScript } from "./googleHelpers";

const GoogleLogin = () => {
  const googleAuthCallback = (response: any) => {
    GoogleService.login(response.credential)
      .then((data) => console.log(data))
      .catch((e) => console.error("google error", e.message));
  };

  const googleButtonStyle = { type: "icon", shape: "circle" };

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
        data-login_uri="https://localhost:7207/api/v1/identity/login-google"
        data-auto_select="true"
        data-itp_support="true"
      ></div>

      <div
        className="g_id_signin"
        data-type="icon"
        data-shape="circle"
        data-theme="outline"
        data-text="signin_with"
        data-size="large"
      ></div>
    </>
  );
};

export default GoogleLogin;
