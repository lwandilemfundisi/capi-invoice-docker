FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

COPY . .
RUN dotnet restore Capi.Assessment.Invoice.Api/Capi.Assessment.Invoice.Api.csproj \
    && dotnet build Capi.Assessment.Invoice.Api/Capi.Assessment.Invoice.Api.csproj \
        --configuration Release --no-restore \
    && dotnet tool install --tool-path /tools dotnet-ef --version 10.0.14

ENV PATH="/tools:${PATH}"
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

WORKDIR /src/Capi.Assessment.Invoice.Api
ENTRYPOINT ["/bin/sh", "/src/docker-entrypoint.sh"]
