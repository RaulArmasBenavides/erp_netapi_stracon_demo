# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia csproj a sus carpetas correctas
COPY ["PaymentServiceNet/PaymentServiceNet.csproj", "PaymentServiceNet/"]
COPY ["PaymentServiceNet.Infraestructure/PaymentServiceNet.Infrastructure.csproj", "PaymentServiceNet.Infraestructure/"]
COPY ["PaymentServiceNet.Core/PaymentServiceNet.Core.csproj", "PaymentServiceNet.Core/"]
COPY ["PaymentServiceNet.Application/PaymentServiceNet.Application.csproj", "PaymentServiceNet.Application/"]
COPY ["PaymentServiceNet.CrossCutting/PaymentServiceNet.CrossCutting.csproj", "PaymentServiceNet.CrossCutting/"]
# Restaura SOLO el proyecto web (evitas el .dcproj)
RUN dotnet restore "PaymentServiceNet/PaymentServiceNet.csproj"

# Copia el resto del código
COPY . .

# Limpia bin/obj que vengan del host (evita errores raros)
RUN find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +

# Publica
RUN dotnet publish "PaymentServiceNet/PaymentServiceNet.csproj" -c Release -o /app/out /p:UseAppHost=false

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "PaymentServiceNet.dll"]