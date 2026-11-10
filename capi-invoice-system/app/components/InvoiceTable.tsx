import { matchesInvoiceFilter, overdueDays } from "../interfaces/domain";
import {
  colors,
  date,
  initials,
  money,
  type Invoice,
} from "../interfaces/shared";

type InvoiceTableProps = {
  invoices: Invoice[];
  loading: boolean;
  tab: string;
  setTab: (value: string) => void;
  query: string;
  setQuery: (value: string) => void;
  onPaid: (id: string) => void;
};

export function InvoiceTable({
  invoices,
  loading,
  tab,
  setTab,
  query,
  setQuery,
  onPaid,
}: InvoiceTableProps) {
  const counts = {
    all: invoices.length,
    due: invoices.filter((invoice) => invoice.status === "overdue").length,
    sent: invoices.filter((invoice) => invoice.status === "sent").length,
    paid: invoices.filter((invoice) => invoice.status === "paid").length,
  };
  const filtered = invoices.filter((invoice) =>
    matchesInvoiceFilter(invoice, tab, query),
  );
  const tabs = [
    ["All invoices", counts.all],
    ["Unpaid", counts.sent],
    ["Overdue", counts.due],
    ["Paid", counts.paid],
  ] as const;

  return (
    <section className="invoice-panel">
      <div className="panel-heading">
        <div>
          <h2>
            Invoices <span className="total-count">{filtered.length}</span>
          </h2>
          <p>Keep every payment on your radar.</p>
        </div>
        <div className="panel-actions">
          <label className="search">
            <span>⌕</span>
            <input
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Search invoices..."
            />
            <kbd>⌘ K</kbd>
          </label>
          <button
            className="filter-button"
            onClick={() =>
              setTab(tab === "All invoices" ? "Overdue" : "All invoices")
            }
          >
            ☷ <span>Filter</span>
            <span className="filter-down">⌄</span>
          </button>
        </div>
      </div>

      <div className="tabs">
        {tabs.map(([name, count]) => (
          <button
            key={name}
            className={`tab ${tab === name ? "selected" : ""}`}
            onClick={() => setTab(name)}
          >
            {name}
            <span>{count}</span>
          </button>
        ))}
      </div>

      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              <th>
                
              </th>
              <th>INVOICE</th>
              <th>CLIENT</th>
              <th>ISSUED</th>
              <th>DUE DATE</th>
              <th>AMOUNT</th>
              <th>STATUS</th>
              <th />
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={8} className="empty">
                  Loading invoices…
                </td>
              </tr>
            ) : filtered.length === 0 ? (
              <tr>
                <td colSpan={8} className="empty">
                  No invoices here yet. Create one to get started.
                </td>
              </tr>
            ) : (
              filtered.map((invoice, index) => (
                <tr key={invoice.id}>
                  <td>
                    
                  </td>
                  <td>
                    <a className="invoice-id">{invoice.id}</a>
                    <small className="description">
                      {invoice.description}
                    </small>
                  </td>
                  <td>
                    <div className="client">
                      <span
                        className="client-avatar"
                        style={{ background: colors[index % colors.length] }}
                      >
                        {initials(invoice.client)}
                      </span>
                      <span>{invoice.client}</span>
                    </div>
                  </td>
                  <td className="muted-cell">{date(invoice.issuedDate)}</td>
                  <td className={invoice.status === "overdue" ? "late" : ""}>
                    {date(invoice.dueDate)}
                    {invoice.status === "overdue" && (
                      <small className="days-late">
                        {overdueDays(invoice.dueDate)} days ago
                      </small>
                    )}
                  </td>
                  <td className="amount">
                    {money(invoice.amount, invoice.currency)}
                  </td>
                  <td>
                    <span className={`status-pill ${invoice.status}-pill`}>
                      <i />
                      {invoice.status === "sent"
                        ? "Awaiting"
                        : invoice.status.charAt(0).toUpperCase() +
                          invoice.status.slice(1)}
                    </span>
                  </td>
                  <td>
                    {invoice.status !== "paid" ? (
                      <button
                        className="row-more"
                        title="Mark paid"
                        onClick={() => onPaid(invoice.id)}
                      >
                        ···
                      </button>
                    ) : (
                      <span className="paid-check">✓</span>
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="table-footer">
        
      </div>
    </section>
  );
}
