#!/bin/sh
set -eu

dotnet ef database update \
  --project ../Capi.Assessment.Invoice.Persistence/Capi.Assessment.Invoice.Persistence.csproj \
  --startup-project Capi.Assessment.Invoice.Api.csproj \
  --context CapiInvoiceContext \
  --configuration Release

exec dotnet bin/Release/net10.0/Capi.Assessment.Invoice.Api.dll
