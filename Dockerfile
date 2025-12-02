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

# Instalar dependencias ODBC
RUN apt-get update && apt-get install -y \
    unixodbc \
    unixodbc-dev \
    iputils-ping \
    net-tools \
    libpam0g \
    && rm -rf /var/lib/apt/lists/*

# Crear directorio para drivers IBM
RUN mkdir -p /opt/ibm/iaccess/lib64

# Copiar drivers IBM desde contexto local
COPY ibm-iaccess-libs/ /opt/ibm/iaccess/lib64/

# Copiar configuración ODBC
COPY odbc.ini /etc/odbc.ini
COPY odbcinst.ini /etc/odbcinst.ini

# Configurar permisos
RUN chmod 644 /etc/odbc.ini /etc/odbcinst.ini && \
    chmod 755 /opt/ibm/iaccess/lib64/*.so

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OdooCls.API.dll"]