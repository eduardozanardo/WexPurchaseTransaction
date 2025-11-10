# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia todos os arquivos da pasta src direto para /src
COPY ./src/ .

# Restaura dependências do WebApi
WORKDIR /src/WebApi
RUN dotnet restore

# Build e publish
RUN dotnet publish -c Release -o /app/publish

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia banco SQLite
COPY ./Data /app/Data

# Copia aplicação publicada
COPY --from=build /app/publish ./

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "WebApi.dll"]
