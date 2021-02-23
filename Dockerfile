FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/* ./
RUN dotnet publish -c Release -o out

# Label the container
LABEL maintainer="David Pine <david.pine@microsoft.com>"
LABEL repository="https://github.com/dotnet/versionsweeper"
LABEL homepage="https://github.com/dotnet/versionsweeper"

LABEL com.github.actions.name=".NET version sweeper"
LABEL com.github.actions.description="A Github action that scans .NET projects, and creates issues that report versions that are not within long term support."
LABEL com.github.actions.icon="alert-circle"
LABEL com.github.actions.color="yellow"

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/DotNet.VersionSweeper.dll" ]