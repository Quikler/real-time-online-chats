import { createContext, useContext, useEffect, useLayoutEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "@services/axios/instance";
import { LoginRequest, SignupRequest } from "@src/models/dtos/Auth";
import { AuthRoutes } from "@src/services/api/ApiRoutes";
import { AuthService } from "@src/services/api/AuthService";
import { GoogleService } from "@src/services/google/GoogleService";

export type UserProfile = {
  id: string;
  firstName: string | null;
  lastName: string | null;
  email: string;
};

type AuthContextType = {
  token: string | null | undefined;
  user: UserProfile | null | undefined;
  loginUser: (request: LoginRequest) => Promise<void>;
  signupUser: (request: SignupRequest) => Promise<void>;
  loginGoogle: (credential: string) => Promise<void>;
  signupGoogle: (credential: string) => Promise<void>;
  logoutUser: () => Promise<void>;
  isUserLoggedIn: () => boolean;
}

type AuthProviderProps = { children: React.ReactNode };

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserProfile | null | undefined>(undefined);
  const [isReady, setIsReady] = useState(false);

  // Fetch user profile and set token
  useEffect(() => {
    const abortController = new AbortController();

    AuthService.me({
      signal: abortController.signal,
    })
      .then((data) => {
        if (data) {
          setToken(data.token);
          setUser(data.user);
        }
      })
      .catch((e) => {
        console.log("[Me] Error:", e.message);
        setToken(null);
        setUser(null);
      });

    return () => abortController.abort();
  }, []);

  // Set up request interceptor
  useLayoutEffect(() => {
    const authInterceptor = api.interceptors.request.use((config: any) => {
      config.headers.Authorization =
        !config._retry && token ? `Bearer ${token}` : config.headers.Authorization;
      return config;
    });

    return () => api.interceptors.request.eject(authInterceptor);
  }, [token]);

  // Set up response interceptor
  useLayoutEffect(() => {
    const refreshInterceptor = api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        if (
          error.response &&
          error.response.status === 401 &&
          originalRequest.url !== AuthRoutes.refreshToken
        ) {
          console.log("[Interceptor][Response] Error 401.");
          try {
            const response = await api.post(AuthRoutes.refreshToken, {}, { withCredentials: true });

            setToken(response.data.token);

            originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
            originalRequest._retry = true;

            console.log("[Interceptor][Response]: Token refreshed");

            return api(originalRequest);
          } catch (e: any) {
            if (e.name !== "CanceledError") {
              console.log("[Interceptor][Response]: Token not refreshed");

              setToken(null);
              setUser(null);
            }
          }
        }

        return Promise.reject(error);
      }
    );

    return () => api.interceptors.response.eject(refreshInterceptor);
  }, []);

  // Mark the app as ready only if user cannot be fetched from server (missing refreshToken)
  // OR if user is logged in (refreshToken presented and user info retrieved with JWT token)
  useEffect(() => {
    if (user === null || isUserLoggedIn()) {
      setIsReady(true);
    }
  }, [user]);

  const signupUser = async (request: SignupRequest) => {
    try {
      const data = await AuthService.signup(request);
      if (data) {
        setToken(data.token);
        setUser(data.user);
      }
    } catch (e: any) {
      console.error("Unable to signup:", e.message);
    }
  };

  const loginUser = async (request: LoginRequest) => {
    try {
      const data = await AuthService.login(request);
      if (data) {
        setToken(data.token);
        setUser(data.user);
      }
    } catch (e: any) {
      console.error("Unable to login:", e.message);
    }
  };

  const signupGoogle = async (credential: string) => {
    try {
      const data = await GoogleService.signup(credential);
      if (data) {
        setToken(data.token);
        setUser(data.user);
      }
    } catch (e: any) {
      console.error("Unable to signup with google:", e.message);
      throw e;
    }
  };

  const loginGoogle = async (credential: string) => {
    try {
      const data = await GoogleService.login(credential);
      if (data) {
        setToken(data.token);
        setUser(data.user);
      }
    } catch (e: any) {
      console.error("Unable to login with google:", e.message);
      throw e;
    }
  };

  const isUserLoggedIn = () => !!user;

  const logoutUser = async () => {
    try {
      await AuthService.logout();
      setUser(null);
      setToken(null);
      navigate("/");
    } catch (e: any) {
      console.error("Unable to logout:", e.message);
    }
  };

  const value = useMemo(
    () => ({
      token,
      user,
      loginUser,
      signupUser,
      loginGoogle,
      signupGoogle,
      logoutUser,
      isUserLoggedIn,
    }),
    [token, user]
  );

  // Render children only when the app is ready
  return <AuthContext.Provider value={value}>{isReady ? children : null}</AuthContext.Provider>;
};

export const useAuth = () => {
  const authContext = useContext(AuthContext);

  if (!authContext) {
    throw new Error("useUserAuth must be used within a UserAuthProvider");
  }

  return authContext;
};
