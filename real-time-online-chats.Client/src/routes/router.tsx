import { createBrowserRouter, Outlet } from "react-router-dom";
import App from "../App";
import HeaderSection from "../components/sections/header-section";
import LogInForm from "../components/forms/log-in-form";
import SignUpForm from "../components/forms/sign-up-form";
import ChatsSection from "../components/sections/chats-section";
import MainLayout from "../layouts/main-layout";
import WhyUsSection from "../components/sections/why-us-section";
import ChatsHeaderSection from "../components/sections/chats-header-section";
import { AuthProvider } from "../contexts/auth-context";
import UserProfile from "../components/profile/user-profile";
import AccountPage from "../components/pages/account/account-page";
import ProtectedRoute from "./protected-routes";
import NotFound from "../components/pages/errors/not-found";
import CreateChatForm from "../components/pages/chats/create-chat-form";

export const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <AuthProvider>
        <App />
      </AuthProvider>
    ),
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
            <LogInForm />
          </MainLayout>
        ),
      },
      {
        path: "signup",
        element: (
          <MainLayout>
            <SignUpForm />
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
            path: "create",
            element: <CreateChatForm />,
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
]);
