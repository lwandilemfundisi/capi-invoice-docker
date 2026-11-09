import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Capi — Invoices, made easy",
  description: "Create, track and get paid for every invoice.",
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}