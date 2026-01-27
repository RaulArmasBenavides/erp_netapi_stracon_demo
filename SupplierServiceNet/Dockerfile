# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ["SupplierServiceNet/SupplierServiceNet.csproj", "SupplierServiceNet/"]
COPY ["SupplierServiceNet.Infraestructure/SupplierServiceNet.Infrastructure.csproj", "SupplierServiceNet.Infraestructure/"]
COPY ["SupplierServiceNet.Core/SupplierServiceNet.Core.csproj", "SupplierServiceNet.Core/"]
COPY ["SupplierServiceNet.Application/SupplierServiceNet.Application.csproj", "SupplierServiceNet.Application/"]
COPY ["SupplierServiceNet.CrossCutting/SupplierServiceNet.CrossCutting.csproj", "SupplierServiceNet.CrossCutting/"]

RUN dotnet restore "SupplierServiceNet/SupplierServiceNet.csproj"
COPY . .
RUN dotnet publish "SupplierServiceNet/SupplierServiceNet.csproj" -c Release -o /app/publish

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Configuración crítica para Docker
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Para logging más detallado
ENV Serilog__MinimumLevel__Default=Debug
ENV Serilog__MinimumLevel__Override__Microsoft=Information

# Conexión a la base de datos (ajusta según tu entorno)
ENV ConnectionStrings__ConexionSql="Server=host.docker.internal,14333;Database=stracon_db;User Id=sa;Password=P@ssw0rd?ts;Encrypt=False;TrustServerCertificate=True"

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SupplierServiceNet.dll"]