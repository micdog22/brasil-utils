
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/BrasilUtils.Api/BrasilUtils.Api.csproj src/BrasilUtils.Api/
COPY tests/BrasilUtils.Api.Tests/BrasilUtils.Api.Tests.csproj tests/BrasilUtils.Api.Tests/
RUN dotnet restore src/BrasilUtils.Api/BrasilUtils.Api.csproj
COPY . .
RUN dotnet build src/BrasilUtils.Api/BrasilUtils.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/BrasilUtils.Api/BrasilUtils.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BrasilUtils.Api.dll"]
