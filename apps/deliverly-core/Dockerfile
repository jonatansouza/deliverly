FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Melhore o Cache: Copie apenas o projeto e restaure primeiro
COPY ["DeliverlyCore/DeliverlyCore.csproj", "DeliverlyCore/"]
RUN dotnet restore "DeliverlyCore/DeliverlyCore.csproj"

# 2. Copie o restante do código
COPY . .

# 3. Mude para a pasta do projeto para compilar de forma limpa
WORKDIR "/src/DeliverlyCore"
RUN dotnet build "DeliverlyCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliverlyCore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 4. CORREÇÃO: A DLL está na raiz de /app, não em uma subpasta
ENTRYPOINT ["dotnet", "DeliverlyCore.dll"]