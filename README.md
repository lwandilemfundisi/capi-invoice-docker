# Run the Capi invoice apps with Docker

This bundle contains the .NET API and Next.js frontend from the two repositories, plus Docker configuration and a SQL Server service.

## Start

Install Docker Desktop, then run these commands from this folder:

```sh
docker compose up --build
```

Open the frontend at <http://localhost:3000>. The API is at <http://localhost:5080>; Swagger is available at <http://localhost:5080/swagger>.

The API applies its Entity Framework migrations during startup. SQL Server data persists in the `sqlserver-data` volume. To stop the containers, press `Ctrl+C` and run `docker compose down`. To remove the database volume as well, run `docker compose down -v`.

## Database password

The compose file has a development-only default password. Set `MSSQL_SA_PASSWORD` in the shell or in a `.env` file beside `docker-compose.yml` before starting if you want to change it. It must meet SQL Server's password complexity requirements.

## API URL

The frontend uses `http://localhost:5080` so the browser can reach the API published by Docker. If you change the API's host port, update `NEXT_PUBLIC_API_URL` in the frontend build arguments in `docker-compose.yml` and rebuild the frontend.
