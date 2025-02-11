import React, { ReactNode } from "react";
import Footer from "./Footer";
import Header from "./Header";

interface MainLayoutProps {
  children: ReactNode;
}

const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  return (
    <>
      <Header />
      <main id="main" className="bg-gradient-to-br from-slate-700 to-slate-900">{children}</main>
      <Footer />
    </>
  );
};

export default MainLayout;