# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copia solution
COPY *.sln .

# copia TODOS os csproj
COPY CasaDeAxeAPI/*.csproj ./CasaDeAxeAPI/
COPY CasaDeAxe.Domain/*.csproj ./CasaDeAxe.Domain/
COPY CasaDeAxe.Application/*.csproj ./CasaDeAxe.Application/
COPY CasaDeAxe.Infrastructure/*.csproj ./CasaDeAxe.Infrastructure/

# restore
RUN dotnet restore

# copia tudo
COPY . .

# build
WORKDIR /app/CasaDeAxeAPI
RUN dotnet publish -c Release -o /app/out

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "CasaDeAxeAPI.dll"]