FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ApisOdoo/OdooCls.API.csproj
RUN dotnet publish ApisOdoo/OdooCls.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar IBM iSeries Access ODBC Driver
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    gnupg \
    apt-transport-https \
    ca-certificates \
    && curl https://public.dhe.ibm.com/software/ibmi/products/odbc/debs/dists/1.1.0/ibmi-acs-1.1.0.list | tee /etc/apt/sources.list.d/ibmi-acs-1.1.0.list \
    && apt-get update \
    && apt-get install -y \
    ibm-iaccess \
    unixodbc \
    unixodbc-dev \
    iputils-ping \
    net-tools \
    && rm -rf /var/lib/apt/lists/*

# Copiar configuración ODBC
COPY odbc.ini /etc/odbc.ini
COPY odbcinst.ini /etc/odbcinst.ini

# Configurar permisos
RUN chmod 644 /etc/odbc.ini /etc/odbcinst.ini

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OdooCls.API.dll"]