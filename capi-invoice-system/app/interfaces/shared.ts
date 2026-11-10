import type { InvoiceRecord } from "../interfaces/domain";

export type Invoice = InvoiceRecord;
export type Notice = { id: string; invoiceId: string; client: string; message: string; sentAt: string };
export type View = "invoices" | "overdue" | "payments" | "activity";
export const API = process.env.NEXT_PUBLIC_API_URL || "https://localhost:7022";
export const viewDetails: Record<View, { title: string; tab: string }> = {
    invoices: { title: "Your invoices", tab: "All invoices" }, overdue: { title: "Overdue invoices", tab: "Overdue" },
    payments: { title: "Payments", tab: "Paid" }, activity: { title: "Recent activity", tab: "All invoices" },
};
export const money = (n: number, c = "ZAR") => new Intl.NumberFormat("en-ZA", { style: "currency", currency: c, maximumFractionDigits: 0 }).format(n);
export const date = (s: string) => {
  if (!s) return "N/A";
  const dateOnly = s.split(/[ T]/)[0]; 
  return new Date(`${dateOnly}T00:00:00`).toLocaleDateString("en-ZA", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
};
;
export const initials = (s: string) => s.split(/[ &]/).filter(Boolean).slice(0, 2).map(x => x[0]).join("").toUpperCase();
export const colors = ["#e8e3ff", "#d8f2e8", "#ffebd5", "#dceafe", "#fce1ea"];