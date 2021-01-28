FROM mcr.microsoft.com/dotnet/runtime:5.0

LABEL maintainer="David Pine <david.pine@microsoft.com>"
LABEL repository="https://github.com/IEvangelist/dotnet-versionsweeper"
LABEL homepage="https://github.com/IEvangelist/dotnet-versionsweeper"

LABEL com.github.actions.name=".NET version sweeper"
LABEL com.github.actions.description="A Github action that scans .NET projects, and creates issues that report versions that are not within long term support."
LABEL com.github.actions.icon="alert-circle"
LABEL com.github.actions.color="yellow"

COPY ../../dist/* /

ENTRYPOINT [ "dotnet", "/DotNet.VersionSweeper.exe" ]