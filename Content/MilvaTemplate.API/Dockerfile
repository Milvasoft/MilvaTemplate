#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:xhttpportxx
EXPOSE xhttpportxx/tcp

COPY *.sln .

COPY ["MilvaTemplate.Localization/MilvaTemplate.Localization.csproj", "MilvaTemplate.Localization/"]
COPY ["MilvaTemplate.Entity/MilvaTemplate.Entity.csproj", "MilvaTemplate.Entity/"]
COPY ["MilvaTemplate.Data/MilvaTemplate.Data.csproj", "MilvaTemplate.Data/"]
COPY ["MilvaTemplate.API/MilvaTemplate.API.csproj", "MilvaTemplate.API/"]

RUN dotnet restore

COPY MilvaTemplate.Localization/. MilvaTemplate.Localization/
COPY MilvaTemplate.Entity/. MilvaTemplate.Entity/
COPY MilvaTemplate.Data/. MilvaTemplate.Data/
COPY MilvaTemplate.API/. MilvaTemplate.API/

WORKDIR "/app/MilvaTemplate.API"
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS runtime
WORKDIR /app

COPY --from=build /app/MilvaTemplate.API/out ./


ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

ENTRYPOINT ["dotnet", "MilvaTemplate.API.dll"]