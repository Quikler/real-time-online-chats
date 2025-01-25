import {
  createContext,
  useContext,
  useEffect,
  useLayoutEffect,
  useMemo,
  useState,
} from "react";
import { UserProfile } from "../models/user";
import { useNavigate } from "react-router-dom";
import { AuthService } from "../services/api/auth-service";
import { toast } from "react-toastify";
import { LoginFormData } from "../components/forms/log-in-form";
import { SignupFormData } from "../components/forms/sign-up-form";
import api from "@services/axios/instance"
import { AuthRoutes } from "../routes/api-routes";

interface AuthContextType {
  token: string | null;
  user: UserProfile | null;
  loginUser: (formData: LoginFormData) => void;
  signupUser: (formData: SignupFormData) => void;
  logoutUser: () => void;
  isUserLoggedIn: () => boolean;
}

type Props = { children: React.ReactNode };

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: Props) => {
  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserProfile | null>(null);

  useEffect(() => {
    const { observable, abort } = AuthService.me();

    observable.subscribe({
      next: (response) => {
        console.group("Me");
        console.log(response.data);
        console.groupEnd();
        
        setToken(response.data.token); // set jwt auth token into state
        setUser(response.data.user); // set user into state
      },
      error: () => {},
    });

    return () => abort();
  }, []);

  useLayoutEffect(() => {
    if (!token) return ;

    const authInterceptor = api.interceptors.request.use((config: any) => {
      console.group("[Interceptor] Request");
      console.log("Token is", token);
      console.groupEnd();

      config.headers.Authorization =
        !config._retry && token
          ? `Bearer ${token}`
          : config.headers.Authorization;
      return config;
    });

    return () => api.interceptors.request.eject(authInterceptor);
  }, [token]);

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
          console.group("[Interceptor] [Response]");
          console.log("[Error]: 401");
          try {
            const response = await api.post(
              AuthRoutes.refreshToken,
              {},
              { withCredentials: true }
            );

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
        // else if (error.response &&
        //   error.response.status === 403) {
        //     router.navigate("")
        //   }

        return Promise.reject(error);
      }
    );

    return () => api.interceptors.response.eject(refreshInterceptor);
  }, []);

  const signupUser = (formData: SignupFormData) => {
    AuthService.signup(formData).observable.subscribe({
      next: (response) => {
        const userObj = response.data.user;

        setToken(response.data.token);
        setUser(userObj);

        toast.success("Signed up successfully");
      },
      error: (error) => toast.error(error),
    });
  };

  const loginUser = (formData: LoginFormData) => {
    AuthService.login(formData).observable.subscribe({
      next: (response) => {
        const userObj = response.data.user;

        setToken(response.data.token);
        setUser(userObj);

        toast.success("Logged in successfully");
      },
      error: (error) => toast.error(error),
    });
  };

  const isUserLoggedIn = () => !!user;

  const logoutUser = () => {
    setUser(null);
    setToken(null);
    navigate("/");
    AuthService.logout();
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

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const authContext = useContext(AuthContext);

  if (!authContext) {
    throw new Error("useUserAuth must be used within a UserAuthProvider");
  }

  return authContext;
};
