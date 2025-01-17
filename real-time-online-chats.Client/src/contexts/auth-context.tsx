import {
  Children,
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";
import { UserProfile } from "../models/user";
import { useNavigate } from "react-router-dom";
import { AuthService } from "../services/auth-service";
import { toast } from "react-toastify";
import axios from "axios";
import { LoginFormData } from "../components/forms/log-in-form";
import { SignupFormData } from "../components/forms/sign-up-form";

// interface AuthContextType {
//   token: string | null | undefined;
//   user: any;
//   setToken: (token: string | null | undefined) => void;
//   setUser: (user: any) => void;
//   logout: () => void;
//   isLoggedin: () => boolean;
// }

// const AuthContext = createContext<AuthContextType>({} as AuthContextType);

// export const useUserAuth = () => {
//   const authContext = useContext(AuthContext);

//   if (!authContext) {
//     throw new Error('useUserAuth must be used within a UserAuthProvider');
//   }

//   return authContext;
// };

// export const UserAuthProvider = ({ children }: { children: ReactNode }) => {
//   const [token, setToken] = useState<string | undefined | null>();
//   const [user, setUser] = useState<UserProfile | null>();

//   // useEffect(() => {
//   //   const fetchMe = async () => {
//   //     console.log("Fetch me");

//   //     try {
//   //       const response = await api.get(AuthService.meUrl);
//   //       setToken(response.data.token);
//   //     } catch {
//   //       setToken(null);
//   //     }
//   //   };

//   //   fetchMe();
//   // }, []);

//   useLayoutEffect(() => {
//     const authInterceptor = api.interceptors.request.use((config: any) => {
//       config.headers.Authorization = !config._retry && token ?
//         `Bearer ${token}`:
//         config.headers.Authorization;
//       return config;
//     });

//     return () => {
//       api.interceptors.request.eject(authInterceptor);
//     };
//   }, [token]);

//   useLayoutEffect(() => {
//     const refreshInterceptor = api.interceptors.response.use(
//       (response) => response,
//       async (error) => {
//         const originalRequest = error.config;

//         if (
//           error.response.status === 401 //&&
//           //originalRequest.url === AuthService.refreshTokenUrl
//         ) {
//           try {
//             const response = await api.post(AuthService.refreshTokenUrl);

//             setToken(response.data.token);

//             originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
//             originalRequest._retry = true;

//             return api(originalRequest);
//           } catch {
//             setToken(null);
//           }
//         }

//         return Promise.reject(error);
//       },
//     )

//     return () => {
//       api.interceptors.response.eject(refreshInterceptor);
//     };
//   });

//   const logoutUser = () => {
//     setToken(null);
//     setUser(null);
//   };

//   const isUserLoggedIn = () => !!user;

//   const value = {
//     token,
//     user,
//     setToken,
//     setUser,
//     logout: logoutUser,
//     isLoggedin: isUserLoggedIn,
//   };

//   return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
// };

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
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const user = localStorage.getItem("user");
    const token = localStorage.getItem("token");

    if (user && token) {
      setUser(JSON.parse(user));
      setToken(token);
      axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    }

    setIsReady(true);
  }, []);

  const signupUser = (formData: SignupFormData) => {
    AuthService.signup(formData).subscribe({
      next: (response) => {
        localStorage.setItem("token", response.data.token);
        const userObj = {
          email: response.data.user.email,
          firstName: response.data.user.firstName,
          lastName: response.data.user.lastName,
        };
        localStorage.setItem("user", JSON.stringify(userObj));

        setToken(response.data.token);
        setUser(userObj);

        toast.success("Signed up successfully");
      },
      error: (error) => toast.error(error),
    });
  };

  const loginUser = (formData: LoginFormData) => {
    AuthService.login(formData).subscribe({
      next: (response) => {
        localStorage.setItem("token", response.data.token);
        const userObj = {
          email: response.data.user.email,
          firstName: response.data.user.firstName,
          lastName: response.data.user.lastName,
        };
        localStorage.setItem("user", JSON.stringify(userObj));

        setToken(response.data.token);
        setUser(userObj);

        toast.success("Logged in successfully");
      },
      error: (error) => toast.error(error),
    });
  };

  const isUserLoggedIn = () => !!user;

  const logoutUser = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    setToken(null);
    navigate("/");
  };

  const value = {
    token: token,
    user: user,
    loginUser: loginUser,
    signupUser: signupUser,
    logoutUser: logoutUser,
    isUserLoggedIn: isUserLoggedIn,
  };

  return (
    <AuthContext.Provider value={value}>
      {isReady ? children : null}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const authContext = useContext(AuthContext);

  if (!authContext) {
    throw new Error("useUserAuth must be used within a UserAuthProvider");
  }

  return authContext;
};
