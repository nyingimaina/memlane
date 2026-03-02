import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { ZestResponsiveLayout } from "jattac.libs.web.zest-responsive-layout";
import Sidebar from "@/components/Sidebar";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Memlane Dashboard",
  description: "Extensible backup and synchronization utility",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable}`}>
        <ZestResponsiveLayout sidebar={<Sidebar />}>
          <main className="main-content">
            {children}
          </main>
        </ZestResponsiveLayout>
      </body>
    </html>
  );
}
