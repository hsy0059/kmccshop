FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 53523

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Common/Campus.Common/Campus.Common.csproj", "Common/Campus.Common/"]
COPY ["Common/Campus.Infrastructure/Campus.Infrastructure.csproj", "Common/Campus.Infrastructure/"]
COPY ["Services/Merchant.Service/Merchant.Service.csproj", "Services/Merchant.Service/"]
RUN dotnet restore "Services/Merchant.Service/Merchant.Service.csproj"
COPY . .
WORKDIR "/src/Services/Merchant.Service"
RUN dotnet build "Merchant.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Merchant.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Merchant.Service.dll"]
