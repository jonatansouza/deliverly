FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DeliverlyCore/DeliverlyCore.csproj", "DeliverlyCore/"]
RUN dotnet restore "DeliverlyCore/DeliverlyCore.csproj"
COPY . .
WORKDIR "/src/DeliverlyCore"
RUN dotnet build "DeliverlyCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliverlyCore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliverlyCore.dll"]
