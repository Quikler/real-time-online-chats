import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";

type Props = { children: React.ReactNode };

const ProtectedRoute = ({ children }: Props) => {
  const location = useLocation();
  const { isUserLoggedIn } = useAuth();

  return isUserLoggedIn() ? (
    <>{children}</>
  ) : (
    <Navigate to="/login" state={{ from: location }} replace />
  );
};

export default ProtectedRoute;
