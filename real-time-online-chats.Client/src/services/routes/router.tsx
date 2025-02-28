import { createBrowserRouter, Outlet } from "react-router-dom";
import WhyUsSection from "@src/pages/root/WhyUs";
import Chat from "@src/pages/chats/{chatId}/Chat";
import Forbidden from "@src/pages/errors/forbidden";
import NotFound from "@src/pages/errors/not-found";
import LoginPage from "@src/pages/login/LoginPage";
import SignupPage from "@src/pages/signup/SignupPage";
import ProtectedRoute from "./ProtectedRoute";
import App from "@src/App";
import HeaderSection from "@src/pages/root/RootHeader";
import { LoaderScreen } from "@src/components/ui/Loader";
import ChatsHeaderSection from "@src/pages/chats/ChatsHeader";
import ChatsSection from "@src/pages/chats/AvailableChats";
import EditUserProfile from "@src/pages/profile/EditUserProfile";
import UserProfile from "@src/pages/profile/UserProfile";
import MainLayout from "@src/components/layouts/MainLayout/MainLayout";
import BaseUserProfile from "@src/pages/profile/BaseUserProfile";

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
        ],
      },
      {
        path: "chats/:chatId", // Moved outside the `MainLayout` wrapper
        element: (
          <ProtectedRoute>
            <Chat />
          </ProtectedRoute>
        ),
      },
      {
        path: "profile/:userId",
        element: (
          <MainLayout>
            <BaseUserProfile />
          </MainLayout>
        ),
        // children: [
        //   { path: ":userId", element: <UserProfile /> },
        //   { path: ":userId/edit", element: <EditUserProfile /> },
        // ],
        children: [
          { path: "", element: <UserProfile /> },
          { path: "edit", element: <EditUserProfile /> },
        ],
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
