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

RUN apt-get update && apt-get install -y \
    unixodbc \
    unixodbc-dev \
    freetds-bin \
    freetds-dev \
    tdsodbc \
    curl \
    wget \
    && rm -rf /var/lib/apt/lists/*

COPY odbc.ini /etc/odbc.ini
COPY odbcinst.ini /etc/odbcinst.ini

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OdooCls.API.dll"]