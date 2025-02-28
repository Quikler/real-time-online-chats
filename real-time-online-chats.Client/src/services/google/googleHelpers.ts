import { GOOGLE_CLIENT_GSI, GOOGLE_CLIENT_ID } from "./googleConstants";

export const initGoogleGSIScript = (onScriptLoad: () => void) => {
  const script = document.createElement("script");
  script.src = GOOGLE_CLIENT_GSI;
  script.async = true;
  script.defer = true;
  script.onload = onScriptLoad;
  document.body.appendChild(script);
};

export const initGoogleAuth = (callback: (response: any) => void) => {
  window.google.accounts.id.initialize({
    client_id: GOOGLE_CLIENT_ID,
    callback: callback,
  });
};

export const renderGoogleButton = (customize?: any) => {
  window.google.accounts.id.renderButton(
    document.getElementsByClassName("g_id_signin")[0],
    customize // Customize the button
  );
};
