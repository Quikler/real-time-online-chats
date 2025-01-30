import { createContext, useContext, useEffect, useLayoutEffect, useMemo, useState } from "react";
import { UserProfile } from "../models/User";
import { useNavigate } from "react-router-dom";
import { AuthService } from "../services/api/auth-service";
import { toast } from "react-toastify";
import api from "@services/axios/instance";
import { AuthRoutes } from "../routes/api-routes";
import { LoginRequest, SignupRequest } from "@src/models/dtos/Auth";

interface AuthContextType {
  token: string | null;
  user: UserProfile | null;
  loginUser: (request: LoginRequest) => void;
  signupUser: (request: SignupRequest) => void;
  logoutUser: () => void;
  isUserLoggedIn: () => boolean;
}

type Props = { children: React.ReactNode };

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: Props) => {
  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isReady, setIsReady] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

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
        setIsAuthenticated(true); // Mark authentication as complete
      })
      .catch((e) => {
        console.error("Fetch me:", e.message);
        setIsAuthenticated(true); // Even if fetching fails, mark authentication as complete
      });

    return () => abortController.abort();
  }, []);

  // Set up request interceptor
  useLayoutEffect(() => {
    if (!isAuthenticated) return;

    const authInterceptor = api.interceptors.request.use((config: any) => {
      console.group("[Interceptor] [Request]");
      console.log("Token is", token);
      console.groupEnd();

      config.headers.Authorization =
        !config._retry && token ? `Bearer ${token}` : config.headers.Authorization;
      return config;
    });

    return () => api.interceptors.request.eject(authInterceptor);
  }, [token, isAuthenticated]);

  // Set up response interceptor
  useLayoutEffect(() => {
    if (!isAuthenticated) return;

    const refreshInterceptor = api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        if (
          error.response &&
          error.response.status === 401 &&
          originalRequest.url !== AuthRoutes.refreshToken
        ) {
          console.group("[Interceptor] [Response]");
          console.log("[Error]: 401");
          try {
            const response = await api.post(AuthRoutes.refreshToken, {}, { withCredentials: true });

            setToken(response.data.token);

            originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
            originalRequest._retry = true;

            console.log("Token refreshed");

            return api(originalRequest);
          } catch (e: any) {
            if (e.name !== "CanceledError") {
              console.log("Token not refreshed");

              setToken(null);
              setUser(null);
            }
          } finally {
            console.groupEnd();
          }
        }

        return Promise.reject(error);
      }
    );

    return () => api.interceptors.response.eject(refreshInterceptor);
  }, [isAuthenticated]);

  // Mark the app as ready only after all initialization is complete
  useEffect(() => {
    if (isAuthenticated) {
      setIsReady(true);
    }
  }, [isAuthenticated]);

  const signupUser = (request: SignupRequest) => {
    AuthService.signup(request)
      .then((response) => {
        if (response) {
          setToken(response.token);
          setUser(response.user);

          toast.success("Signed up successfully");
        }
      })
      .catch((e) => toast.error(e));
  };

  const loginUser = (request: LoginRequest) => {
    AuthService.login(request)
      .then((response) => {
        if (response) {
          setToken(response.token);
          setUser(response.user);

          toast.success("Logged in successfully");
        }
      })
      .catch((e) => toast.error(e));
  };

  const isUserLoggedIn = () => !!user;

  const logoutUser = () => {
    AuthService.logout()
      .then(() => {
        setUser(null);
        setToken(null);
        navigate("/");
      })
      .catch((e) => toast.error(e));
  };

  const value = useMemo(
    () => ({
      token,
      user,
      loginUser,
      signupUser,
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
