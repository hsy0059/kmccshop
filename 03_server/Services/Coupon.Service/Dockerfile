FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 53225

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Common/Campus.Common/Campus.Common.csproj", "Common/Campus.Common/"]
COPY ["Common/Campus.Infrastructure/Campus.Infrastructure.csproj", "Common/Campus.Infrastructure/"]
COPY ["Services/Coupon.Service/Coupon.Service.csproj", "Services/Coupon.Service/"]
RUN dotnet restore "Services/Coupon.Service/Coupon.Service.csproj"
COPY . .
WORKDIR "/src/Services/Coupon.Service"
RUN dotnet build "Coupon.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Coupon.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Coupon.Service.dll"]
