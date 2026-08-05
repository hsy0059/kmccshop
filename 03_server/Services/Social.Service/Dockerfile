FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 53215

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Common/Campus.Common/Campus.Common.csproj", "Common/Campus.Common/"]
COPY ["Common/Campus.Infrastructure/Campus.Infrastructure.csproj", "Common/Campus.Infrastructure/"]
COPY ["Services/Social.Service/Social.Service.csproj", "Services/Social.Service/"]
RUN dotnet restore "Services/Social.Service/Social.Service.csproj"
COPY . .
WORKDIR "/src/Services/Social.Service"
RUN dotnet build "Social.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Social.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Social.Service.dll"]
