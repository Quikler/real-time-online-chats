import { createBrowserRouter, Outlet } from "react-router-dom";
import UserProfile from "@src/components/profile/UserProfile";
import WhyUsSection from "@src/pages/root/WhyUs";
import MainLayout from "@src/layouts/main-layout";
import AccountPage from "@src/pages/account/account-page";
import Chat from "@src/pages/chats/{chatId}/Chat";
import Forbidden from "@src/pages/errors/forbidden";
import NotFound from "@src/pages/errors/not-found";
import LoginPage from "@src/pages/login/LoginPage";
import SignupPage from "@src/pages/signup/SignupPage";
import ProtectedRoute from "./protected-routes";
import App from "@src/App";
import HeaderSection from "@src/pages/root/RootHeader";
import { LoaderScreen } from "@src/components/ui/Loader";
import ChatsHeaderSection from "@src/pages/chats/ChatsHeader";
import ChatsSection from "@src/pages/chats/AvailableChats";

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
            element: (
              <ProtectedRoute>
                <Chat />
              </ProtectedRoute>
            ),
          },
        ],
      },
      {
        path: "profile",
        element: (
          <MainLayout>
            <AccountPage />
          </MainLayout>
        ),
        children: [{ path: ":userId", element: <UserProfile /> }],
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
    element: <LoaderScreen />,
  },
]);
