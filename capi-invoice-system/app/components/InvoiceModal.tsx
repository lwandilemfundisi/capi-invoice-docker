import type { ChangeEvent, FormEvent } from "react";

export type InvoiceForm = {
  client: string;
  email: string;
  amount: string;
  issuedDate: string;
  dueDate: string;
  description: string;
};

type InvoiceModalProps = {
  form: InvoiceForm;
  setForm: (form: InvoiceForm) => void;
  onClose: () => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
};

export function InvoiceModal({
  form,
  setForm,
  onClose,
  onSubmit,
}: InvoiceModalProps) {
  const updateField =
    (key: keyof InvoiceForm) => (event: ChangeEvent<HTMLInputElement>) =>
      setForm({ ...form, [key]: event.target.value });

  return (
    <div
      className="modal-backdrop"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget) onClose();
      }}
    >
      <form className="modal" onSubmit={onSubmit}>
        <div className="modal-top">
          <div>
            <span className="eyebrow">NEW INVOICE</span>
            <h2>Create an invoice</h2>
          </div>
          <button type="button" className="close" onClick={onClose}>
            ×
          </button>
        </div>

        <label>
          Client name
          <input
            required
            placeholder="e.g. Northstar Design Co."
            value={form.client}
            onChange={updateField("client")}
          />
        </label>
        <label>
          Client email
          <input
            type="email"
            required
            placeholder="accounts@company.com"
            value={form.email}
            onChange={updateField("email")}
          />
        </label>

        <div className="form-row">
          <label>
            Amount (ZAR)
            <input
              required
              type="number"
              min="1"
              step="0.01"
              placeholder="0.00"
              value={form.amount}
              onChange={updateField("amount")}
            />
          </label>
          <label>
            Issue date
            <input
              required
              type="date"
              value={form.issuedDate}
              onChange={updateField("issuedDate")}
            />
          </label>
        </div>

        <div className="form-row">
          <label>
            Due date
            <input
              required
              type="date"
              min={form.issuedDate}
              value={form.dueDate}
              onChange={updateField("dueDate")}
            />
          </label>
          <label>
            Description
            <input
              required
              placeholder="What are you billing for?"
              value={form.description}
              onChange={updateField("description")}
            />
          </label>
        </div>

        <div className="modal-actions">
          <button type="button" className="secondary" onClick={onClose}>
            Cancel
          </button>
          <button className="primary" type="submit">
            Create invoice <span>→</span>
          </button>
        </div>
      </form>
    </div>
  );
}
