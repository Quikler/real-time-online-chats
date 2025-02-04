import React, { ReactNode } from "react";
import Header from "../components/header";
import Footer from "../components/footer";

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