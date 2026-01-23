# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia csproj a sus carpetas correctas
COPY ["ApiMovies/ApiMovies.csproj", "ApiMovies/"]
COPY ["ApiMovies.Infraestructure/ApiMovies.Infrastructure.csproj", "ApiMovies.Infraestructure/"]
COPY ["ApiMovies.Core/ApiMovies.Core.csproj", "ApiMovies.Core/"]
COPY ["ApiMovies.Application/ApiMovies.Application.csproj", "ApiMovies.Application/"]
COPY ["ApiMovies.CrossCutting/ApiMovies.CrossCutting.csproj", "ApiMovies.CrossCutting/"]
# Restaura SOLO el proyecto web (evitas el .dcproj)
RUN dotnet restore "ApiMovies/ApiMovies.csproj"

# Copia el resto del código
COPY . .

# Limpia bin/obj que vengan del host (evita errores raros)
RUN find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +

# Publica
RUN dotnet publish "ApiMovies/ApiMovies.csproj" -c Release -o /app/out /p:UseAppHost=false

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "ApiMovies.dll"]