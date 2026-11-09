import type { Notice } from "../interfaces/shared";

type ActivitySectionProps = {
  notices: Notice[];
  onSimulate: () => void;
};

export function ActivitySection({
  notices,
  onSimulate,
}: ActivitySectionProps) {
  return (
    <section className="lower">
      <div className="reminder-card">
        <div className="reminder-icon">✉</div>
        <div className="reminder-copy">
          <span className="eyebrow">
            AUTOMATIONS <span className="auto-tag">SIMULATED</span>
          </span>
          <h3>Friendly nudges, right on time.</h3>
          <p>
            We’ll remind clients when an invoice slips past its due date. No
            awkward follow-ups needed.
          </p>
          <div className="reminder-meta">
            <span className="live-dot" /> Reminder sequence <b>3 days overdue</b>{" "}
            <span className="meta-sep">·</span> then every 7 days
          </div>
        </div>
        <button className="simulate" onClick={onSimulate}>
          ↻ Simulate reminders
        </button>
      </div>

      <div className="activity">
        <div className="activity-title">
          <div>
            <h3>Recent activity</h3>
            <p>The latest from your invoices.</p>
          </div>
          <button
            onClick={() =>
              document
                .querySelector(".activity-list")
                ?.scrollIntoView({ behavior: "smooth" })
            }
          >
            View all <span>→</span>
          </button>
        </div>

        <div className="activity-list">
          {notices.length === 0 ? (
            <div className="activity-item">
              <span className="activity-icon blue-bg">◷</span>
              <div>
                <strong>All caught up</strong>
                <p>
                  No reminder activity yet. Simulate reminders to check for
                  overdue invoices.
                </p>
              </div>
            </div>
          ) : (
            notices.slice(0, 3).map((notice, index) => (
              <div className="activity-item" key={notice.id}>
                <span
                  className={`activity-icon ${index ? "green-bg" : "coral-bg"}`}
                >
                  {index ? "✓" : "↗"}
                </span>
                <div>
                  <strong>
                    {index ? "Reminder sent" : "Overdue reminder simulated"}{" "}
                    <span className="activity-invoice">{notice.invoiceId}</span>
                  </strong>
                  <p>
                    {notice.client} · {notice.message}
                  </p>
                </div>
                <time>
                  {new Date(notice.sentAt).toLocaleTimeString("en-ZA", {
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
                </time>
              </div>
            ))
          )}
        </div>
      </div>
    </section>
  );
}
