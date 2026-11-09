import Link from "next/link";
import type { View } from "../interfaces/shared";

type AppSidebarProps = {
  view: View;
  allCount: number;
  overdueCount: number;
};

export function AppSidebar({
  view,
  allCount,
  overdueCount,
}: AppSidebarProps) {
  return (
    <aside className="sidebar">
      <div className="brand">
        <span className="brand-mark">
          l<span>.</span>
        </span>
        <span>Capi Invoice</span>
      </div>

      <nav>
        <Link
          className={`nav-item ${view === "invoices" ? "active" : ""}`}
          href="/"
        >
          <span className="nav-icon">▤</span>
          Invoices
          <span className="nav-count">{allCount}</span>
        </Link>
        <Link
          className={`nav-item ${view === "overdue" ? "active" : ""}`}
          href="/overdue"
        >
          <span className="nav-icon">◷</span>
          Overdue
          {overdueCount > 0 && (
            <span className="nav-alert">{overdueCount}</span>
          )}
        </Link>
        <Link
          className={`nav-item ${view === "payments" ? "active" : ""}`}
          href="/payments"
        >
          <span className="nav-icon">✓</span>
          Payments
        </Link>
        <Link
          className={`nav-item ${view === "activity" ? "active" : ""}`}
          href="/activity"
        >
          <span className="nav-icon">◉</span>
          Activity
        </Link>
      </nav>
    </aside>
  );
}
