# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia csproj a sus carpetas correctas
COPY ["SupplierServiceNet/SupplierServiceNet.csproj", "SupplierServiceNet/"]
COPY ["SupplierServiceNet.Infraestructure/SupplierServiceNet.Infrastructure.csproj", "SupplierServiceNet.Infraestructure/"]
COPY ["SupplierServiceNet.Core/SupplierServiceNet.Core.csproj", "SupplierServiceNet.Core/"]
COPY ["SupplierServiceNet.Application/SupplierServiceNet.Application.csproj", "SupplierServiceNet.Application/"]
COPY ["SupplierServiceNet.CrossCutting/SupplierServiceNet.CrossCutting.csproj", "SupplierServiceNet.CrossCutting/"]
# Restaura SOLO el proyecto web (evitas el .dcproj)
RUN dotnet restore "SupplierServiceNet/SupplierServiceNet.csproj"

# Copia el resto del código
COPY . .

# Limpia bin/obj que vengan del host (evita errores raros)
RUN find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +

# Publica
RUN dotnet publish "SupplierServiceNet/SupplierServiceNet.csproj" -c Release -o /app/out /p:UseAppHost=false

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "SupplierServiceNet.dll"]