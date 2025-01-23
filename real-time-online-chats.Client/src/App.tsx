import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { AuthProvider } from "./contexts/auth-context";

function App() {
  return (
    <AuthProvider>
      <ToastContainer />
      <Outlet />
    </AuthProvider>
  );
}

export default App;
