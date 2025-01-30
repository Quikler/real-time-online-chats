import { createBrowserRouter, Outlet } from "react-router-dom";
import App from "../App";
import HeaderSection from "../components/sections/header-section";
import LoginPage from "../components/pages/login/LoginPage";
import SignupPage from "@src/components/pages/signup/SignupPage";
import ChatsSection from "../components/sections/chats-section";
import MainLayout from "../layouts/main-layout";
import WhyUsSection from "../components/sections/why-us-section";
import ChatsHeaderSection from "../components/sections/chats-header-section";
import UserProfile from "../components/profile/user-profile";
import AccountPage from "../components/pages/account/account-page";
import ProtectedRoute from "./protected-routes";
import NotFound from "../components/pages/errors/not-found";
import Chat from "../components/pages/chats/Chat";
import Forbidden from "../components/pages/errors/forbidden";
import { LoaderScreen } from "../components/ui/Loader";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      {
        path: "",
        element: (
          <MainLayout>
            <HeaderSection />
            <WhyUsSection />
          </MainLayout>
        ),
      },
      {
        path: "login",
        element: (
          <MainLayout>
            <LoginPage />
          </MainLayout>
        ),
      },
      {
        path: "signup",
        element: (
          <MainLayout>
            <SignupPage />
          </MainLayout>
        ),
      },
      {
        path: "chats",
        element: (
          <MainLayout>
            <Outlet />
          </MainLayout>
        ),
        children: [
          {
            path: "",
            element: (
              <>
                <ChatsHeaderSection />
                <ChatsSection />
              </>
            ),
          },
          {
            path: ":chatId",
            element: <Chat />,
          },
        ],
      },
      {
        path: "profile",
        element: (
          <MainLayout>
            <ProtectedRoute>
              <AccountPage />
            </ProtectedRoute>
          </MainLayout>
        ),
        children: [{ path: "", element: <UserProfile /> }],
      },
    ],
  },
  {
    path: "*",
    element: <NotFound />,
  },
  {
    path: "forbidden",
    element: <Forbidden />,
  },
  {
    path: "loader",
    element: <LoaderScreen />
  }
]);
