# This file is intentionally not valid.
# It's only purpose is for dog fooding.

FROM mcr.microsoft.com/dotnet/aspnet:3.1.30-bionic AS build-env
WORKDIR /App

# Copy everything
COPY . ./

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.16

# Restore as distinct layers
RUN dotnet restore

FROM mcr.microsoft.com/dotnet/framework/sdk:4.7.1

# Build and publish a release
RUN dotnet publish -c Release -o out
FROM mcr.microsoft.com/azure/bits:6.0

COPY --from=mcr.microsoft.com/dotnet/framework/runtime:3.5-20221011-windowsservercore-ltsc2019 /usr/share/dotnet/shared

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-20221011-windowsservercore-ltsc2022

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]
