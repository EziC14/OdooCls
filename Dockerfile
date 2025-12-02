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

# Instalar dependencias ODBC y librerías de sistema necesarias para IBM iSeries
RUN apt-get update && apt-get install -y \
    unixodbc \
    unixodbc-dev \
    iputils-ping \
    net-tools \
    libpam0g \
    libedit2 \
    libkrb5-3 \
    libgssapi-krb5-2 \
    openssl \
    libssl3 \
    libcurl4 \
    && rm -rf /var/lib/apt/lists/*

# Crear directorio para drivers IBM
RUN mkdir -p /opt/ibm/iaccess/lib64

# Copiar drivers IBM desde contexto local
COPY ibm-iaccess-libs/ /opt/ibm/iaccess/lib64/

# Crear symlinks por si las librerías buscan versiones específicas
RUN cd /opt/ibm/iaccess/lib64 && \
    ln -sf libcwbcore.so libcwbcore.so.1 2>/dev/null || true && \
    ln -sf libcwbodbc.so libcwbodbc.so.1 2>/dev/null || true && \
    ln -sf libcwbodbcs.so libcwbodbcs.so.1 2>/dev/null || true

# Configurar librerías (NOTA: odbc.ini se montará como volumen desde el host)
RUN chmod 755 /opt/ibm/iaccess/lib64/*.so && \
    echo "/opt/ibm/iaccess/lib64" > /etc/ld.so.conf.d/ibm-iaccess.conf && \
    ldconfig

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OdooCls.API.dll"]