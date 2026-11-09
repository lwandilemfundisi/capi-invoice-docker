import { money, type Invoice } from "../interfaces/shared";

type StatCard = {
  label: string;
  symbol: string;
  tone: string;
  value: number;
  note: string;
  pill?: boolean;
};

export function InvoiceStats({ invoices }: { invoices: Invoice[] }) {
  const overdue = invoices.filter((invoice) => invoice.status === "overdue");
  const sent = invoices.filter((invoice) => invoice.status === "sent");
  const paid = invoices.filter((invoice) => invoice.status === "paid");

  const sum = (items: Invoice[]) =>
    items.reduce((total, invoice) => total + invoice.amount, 0);

  const cards: StatCard[] = [
    {
      label: "OUTSTANDING",
      symbol: "↗",
      tone: "violet",
      value: sum(invoices.filter((invoice) => invoice.status !== "paid")),
      note: `Across ${invoices.length - paid.length} unpaid invoices`,
    },
    {
      label: "OVERDUE",
      symbol: "!",
      tone: "coral",
      value: sum(overdue),
      note: `${overdue.length} need attention`,
      pill: true,
    },
    {
      label: "AWAITING PAYMENT",
      symbol: "◷",
      tone: "blue",
      value: sum(sent),
      note: `${sent.length} invoices in progress`,
    },
    {
      label: "PAID THIS MONTH",
      symbol: "✓",
      tone: "green",
      value: sum(paid),
      note: `↑ Looking good · ${paid.length} invoices`,
    },
  ];

  return (
    <div className="stats">
      {cards.map((card) => (
        <div className="stat-card" key={card.label}>
          <div className="stat-top">
            <span>{card.label}</span>
            <span className={`stat-symbol ${card.tone}`}>{card.symbol}</span>
          </div>

          <strong>{money(card.value)}</strong>

          <div className="stat-bottom">
            {card.pill ? (
              <span className="status-pill overdue-pill">
                <i />
                {card.note}
              </span>
            ) : (
              <span
                className={
                  card.tone === "green" ? "positive" : "stat-note"
                }
              >
                {card.note}
              </span>
            )}
          </div>

          <div className={`stat-line ${card.tone}-line`} />
        </div>
      ))}
    </div>
  );
}