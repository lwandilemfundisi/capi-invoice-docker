"use client";

import { useCallback, useEffect, useMemo, useState, type FormEvent } from "react";
import { ActivitySection } from "./ActivitySection";
import { AppHeader } from "./AppHeader";
import { AppSidebar } from "./AppSidebar";
import { InvoiceModal, type InvoiceForm } from "./InvoiceModal";
import { InvoiceStats } from "./InvoiceStats";
import { InvoiceTable } from "./InvoiceTable";
import { API, type Invoice, type Notice, type View, viewDetails } from "../interfaces/shared";

const emptyForm = (issuedDate = ""): InvoiceForm => ({
  client: "",
  email: "",
  amount: "",
  issuedDate,
  dueDate: "",
  description: "",
});

export default function CapiClient({ view }: { view: View }) {
  const page = viewDetails[view];
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [notices, setNotices] = useState<Notice[]>([]);
  const [tab, setTab] = useState(page.tab);
  const [query, setQuery] = useState("");
  const [modal, setModal] = useState(false);
  const [toast, setToast] = useState<string | string[]>("");
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState<InvoiceForm>(emptyForm);

  const load = useCallback(async () => {
    try {
      const [invoicesResponse, noticesResponse] = await Promise.all([
        fetch(`${API}/api/invoice/list`),
        fetch(`${API}/api/invoice/reminders`),
      ]);

      if (!invoicesResponse.ok || !noticesResponse.ok) {
        throw new Error();
      }

      var invoicesRsp = await invoicesResponse.json();
      setInvoices(invoicesRsp.result);

      var noticesRsp = await noticesResponse.json();
      setNotices(noticesRsp.result);

    } catch {
      setToast("Could not reach the API. Start the .NET service on port 5080.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => setTab(page.tab), [page.tab]);
  useEffect(() => {
    void load();
  }, [load]);
  useEffect(() => {
    setForm((current) =>
      current.issuedDate
        ? current
        : { ...current, issuedDate: new Date().toISOString().slice(0, 10) },
    );
  }, []);
  useEffect(() => {
    if (!toast) return;

    const timer = setTimeout(() => setToast(""), 3800);
    return () => clearTimeout(timer);
  }, [toast]);

  const counts = useMemo(
    () => ({
      all: invoices.length,
      due: invoices.filter((invoice) => invoice.status === "overdue").length,
    }),
    [invoices],
  );

  async function createInvoice(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    try {
      const response = await fetch(`${API}/api/invoice/create`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          ...form,
          amount: Number(form.amount),
          currency: "ZAR",
        }),
      });
      const data = await response.json();

      const notificationMessages = Array.isArray(data.notification)
        ? data.notification
            .map((item: { text?: unknown }) =>
              typeof item?.text === "string" ? item.text : "",
            )
            .filter((text: string) => text.trim().length > 0)
        : [];

        console.log(notificationMessages);
      if (notificationMessages.length > 0) {
        setToast(notificationMessages);
        return;
      }

      if (!response.ok) throw new Error(data.message);

      setModal(false);
      setForm(emptyForm(new Date().toISOString().slice(0, 10)));
      setToast(`${data.id} created and ready to send.`);
      await load();
    } catch (error) {
      setToast(
        error instanceof Error ? error.message : "Could not create invoice.",
      );
    }
  }

  async function simulateReminders() {
    try {
      const response = await fetch(`${API}/api/invoice/reminders/simulate`, {
        method: "POST",
      });
      const data = await response.json();

      setToast(data.message);
      await load();
    } catch {
      setToast("Could not reach the .NET API.");
    }
  }

  async function markPaid(id: string) {
    try {
      const response = await fetch(`${API}/api/invoice/${id}/paid`, {
        method: "PATCH",
      });

      if (!response.ok) throw new Error();

      setToast(`${id} marked as paid.`);
      await load();
    } catch {
      setToast("Could not update invoice.");
    }
  }

  return (
    <main className="shell">
      <AppSidebar
        view={view}
        allCount={counts.all}
        overdueCount={counts.due}
      />

      <section className="main">
        <AppHeader title={page.title} hasNotifications={notices.length > 0} />

        <div className="content">
          <div className="page-heading">
            <div>
              <h1>{page.title}</h1>
              <p className="subtitle">
                A little less admin. A lot more getting paid.
              </p>
            </div>
            <button className="primary" onClick={() => setModal(true)}>
              <span>＋</span> Create invoice
            </button>
          </div>

          {view === "invoices" && <InvoiceStats invoices={invoices} />}

          {view !== "activity" && (
            <InvoiceTable
              invoices={invoices}
              loading={loading}
              tab={tab}
              setTab={setTab}
              query={query}
              setQuery={setQuery}
              onPaid={markPaid}
            />
          )}

          {(view === "invoices" || view === "activity") && (
            <ActivitySection
              notices={notices}
              onSimulate={simulateReminders}
            />
          )}

          <footer>
            © 2026 Capi Invoice Assessment
          </footer>
        </div>
      </section>

      {toast && (
        <div className="toast" role={Array.isArray(toast) ? "alert" : "status"}>
          <span aria-hidden="true">{Array.isArray(toast) ? "!" : "✓"}</span>
          {Array.isArray(toast) ? (
            <ul className="toast-messages">
              {toast.map((message, index) => (
                <li key={`${index}-${message}`}>{message}</li>
              ))}
            </ul>
          ) : (
            <span className="toast-message">{toast}</span>
          )}
          <button aria-label="Dismiss notification" onClick={() => setToast("")}>×</button>
        </div>
      )}

      {modal && (
        <InvoiceModal
          form={form}
          setForm={setForm}
          onClose={() => setModal(false)}
          onSubmit={createInvoice}
        />
      )}
    </main>
  );
}
