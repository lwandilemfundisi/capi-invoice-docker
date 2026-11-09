export type InvoiceRecord = {
  id: string;
  client: string;
  email: string;
  amount: number;
  currency: string;
  issuedDate: string;
  dueDate: string;
  description: string;
  status: "sent" | "overdue" | "paid";
};

export function matchesInvoiceFilter(
  invoice: InvoiceRecord,
  tab: string,
  query: string,
) {
  const normalizedQuery = query.trim().toLowerCase();
  const matchesQuery =
    !normalizedQuery ||
    [invoice.id, invoice.client, invoice.description].some((value) =>
      value.toLowerCase().includes(normalizedQuery),
    );

  const matchesTab =
    tab === "All invoices" ||
    (tab === "Unpaid"
      ? invoice.status === "sent" || invoice.status === "overdue"
      : invoice.status.toLowerCase() === tab.toLowerCase());

  return matchesQuery && matchesTab;
}

export function overdueDays(dueDate: string) {
  const due = new Date(`${dueDate}T00:00:00`);
  return Math.max(1, Math.floor((Date.now() - due.getTime()) / 86_400_000));
}
