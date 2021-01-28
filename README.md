# .NET version sweeper

![build & test](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/build%20&%20test/badge.svg)

## Get started

This is intended to be used as a GitHub action that will run as a [scheduled CRON job](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#onschedule). Ideally, once every few months or as often as necessary to align with .NET version updates.

There are several required command-line switches (options) when running this action, all of which can be overridden by environment variables.

| Option          | Environment variable | Details                                                                                                      |
|-----------------|----------------------|--------------------------------------------------------------------------------------------------------------|
| `-o`, `owner`   | `INPUT_OWNER`        | The owner of the repo. Assign from `github.repository_owner`. Example, `"dotnet"`.                           |
| `-n`, `name`    | `INPUT_NAME`         | The repository name. Assign from `github.repository`. Example, `"samples"`.                                  |
| `-b`, `branch`  | `INPUT_BRANCH`       | The branch name. Assign from `github.ref`. Example, `"main"`.                                                |
| `-d`, `dir`     | `INPUT_DIRECTORY`    | The root directory, defaults to `"."`.                                                                       |
| `-p`, `pattern` | `INPUT_PATTERN`      | The search pattern, defaults to `"*.csproj"`.                                                                |
| `-t`, `token`   | `GITHUB_TOKEN`       | The GitHub personal-access token (PAT), or the token from GitHub action context. Assign from `github.token`. |
