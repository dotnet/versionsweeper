# .NET version sweeper

![build & test](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/build%20&%20test/badge.svg)

## Get started

This is intended to be used as a GitHub action that will run as a [schedule CRON job](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#onschedule). Ideally, once every few months to align with .NET version updates.

There are several required command-line switched (options) when running this action, all of which can be overridden by environment variables.

| Option            | Environment variable              | Details                                                                                                      | Example    |
|-------------------|-----------------------------------|--------------------------------------------------------------------------------------------------------------|------------|
| `-o` or `owner`   | `DOTNET_VERSIONSWEEPER_OWNER`     | The owner of the repo. Assign from `github.repository_owner`.                                                | `dotnet`   |
| `-n` or `name`    | `DOTNET_VERSIONSWEEPER_NAME`      | The repository name. Assign from `github.repository`.                                                        | `samples`  |
| `-b` or `branch`  | `DOTNET_VERSIONSWEEPER_BRANCH`    | The branch name. Assign from `github.ref`.                                                                   | `main`     |
| `-d` or `dir`     | `DOTNET_VERSIONSWEEPER_DIRECTORY` | The root directory, defaults to `.`.                                                                         | `.`        |
| `-p` or `pattern` | `DOTNET_VERSIONSWEEPER_PATTERN`   | The search pattern, defaults to `*.csproj`.                                                                  | `*.csproj` |
| `-t` or `token`   | `GITHUB_TOKEN`                    | The GitHub personal-access token (PAT), or the token from GitHub action context. Assign from `github.token`. |            |
