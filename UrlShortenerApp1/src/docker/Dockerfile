﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "UrlShortenerApp1.csproj"
RUN dotnet build "UrlShortenerApp1.csproj" -c Release -o /app/build
RUN dotnet publish "UrlShortenerApp1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrlShortenerApp1.dll"]