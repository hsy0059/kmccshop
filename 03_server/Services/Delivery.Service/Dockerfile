FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 53221

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Common/Campus.Common/Campus.Common.csproj", "Common/Campus.Common/"]
COPY ["Common/Campus.Infrastructure/Campus.Infrastructure.csproj", "Common/Campus.Infrastructure/"]
COPY ["Services/Delivery.Service/Delivery.Service.csproj", "Services/Delivery.Service/"]
RUN dotnet restore "Services/Delivery.Service/Delivery.Service.csproj"
COPY . .
WORKDIR "/src/Services/Delivery.Service"
RUN dotnet build "Delivery.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Delivery.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Delivery.Service.dll"]
