# Capi Invoices

Capi Invoices is a small invoice tracking app with a browser interface, a .NET API, and a SQL Server database. The frontend is built with Next.js and React. The API handles invoice commands and queries, while SQL Server stores invoices and reminder activity.

## Features

- **Invoices dashboard:** view invoices, search by invoice ID, client, or description, and filter by all, unpaid, overdue, or paid status.
- **Summary cards:** see outstanding, overdue, awaiting payment, and paid totals.
- **Overdue view:** find invoices whose due date has passed and that have not been paid.
- **Payments view:** focus on paid invoices.
- **Create invoice:** enter the client, email, amount, issue date, due date, and description. The app uses South African rand (ZAR). It sets a new invoice to `sent`, or `overdue` when its due date is already in the past.
- **Mark paid:** use the action at the end of an unpaid invoice row to change its status to paid.
- **Activity and reminders:** view reminder activity and manually simulate overdue reminder processing. This is a demonstration; the app does not send real email.

The Activity screen describes a reminder sequence starting three days overdue and repeating every seven days. In the checked API source, the simulator currently checks that an invoice is at least three days overdue and has not had a reminder logged today; it does not enforce the seven-day interval. The button is manual, so no background reminder job runs on a schedule.

## Run with Docker

Install Docker Desktop and make sure its Linux container engine is running. From this folder, run:

```sh
docker compose up --build
```

Open:

- Frontend: <http://localhost:3000>
- API: <http://localhost:5080>
- Swagger: <http://localhost:5080/swagger>

The API applies Entity Framework migrations during startup. SQL Server data persists in a Docker volume named `sqlserver-data`. Stop the apps with `Ctrl+C`, then run `docker compose down`. To also delete the stored database, run `docker compose down -v`.

The compose file uses a development-only default SQL Server password. To change it, set `MSSQL_SA_PASSWORD` in your environment or in a `.env` file beside `docker-compose.yml` before starting. Choose a password that meets SQL Server's complexity requirements.

## Create an invoice

1. Open <http://localhost:3000> and choose **Create invoice**.
2. Fill in the client name, valid email, amount, issue date, due date, and description. The due date must be on or after the issue date.
3. Choose **Create invoice**. Currency is set to ZAR. The API determines the initial status from the due date.
4. The new invoice appears in the invoice list. Use the search box or status tabs to find it.
5. To record payment, use the row action at the right and choose **Mark paid**.

## Main API routes

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/invoice/create` | Create an invoice |
| `GET` | `/api/invoice/list` | List invoices |
| `PATCH` | `/api/invoice/{id}/paid` | Mark an invoice paid |
| `GET` | `/api/invoice/reminders` | Read reminder activity |
| `POST` | `/api/invoice/reminders/simulate` | Manually simulate reminder processing |

## Project folders

- `capi-invoice-system/` contains the Next.js pages and UI components.
- `Capi.Assessment.Invoice/` contains the .NET solution, API, domain logic, persistence layer, and database migrations.
- `docker-compose.yml` starts the frontend, API, and SQL Server together.
