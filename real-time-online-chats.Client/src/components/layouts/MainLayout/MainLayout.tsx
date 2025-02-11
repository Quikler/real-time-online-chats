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
      <main id="main" style={{ backgroundColor: "slategray" }}>{children}</main>
      <Footer />
    </>
  );
};

export default MainLayout;