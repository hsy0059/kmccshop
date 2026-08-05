FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 53211

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Common/Campus.Common/Campus.Common.csproj", "Common/Campus.Common/"]
COPY ["Common/Campus.Infrastructure/Campus.Infrastructure.csproj", "Common/Campus.Infrastructure/"]
COPY ["Services/Campus.Service/Campus.Service.csproj", "Services/Campus.Service/"]
RUN dotnet restore "Services/Campus.Service/Campus.Service.csproj"
COPY . .
WORKDIR "/src/Services/Campus.Service"
RUN dotnet build "Campus.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Campus.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Campus.Service.dll"]
