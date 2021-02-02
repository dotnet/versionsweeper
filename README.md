# 🎯 LTS - .NET version sweeper

![build & test](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/build%20&%20test/badge.svg)
![.net version sweeper](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/.net%20version%20sweeper/badge.svg)

## Get started

This is intended to be used as a GitHub action that will run as a [scheduled CRON job](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#onschedule). Ideally, once every few months or as often as necessary to align with .NET version updates.

## Required inputs

| Option          | Details                                                                                                                |
|:----------------|:-----------------------------------------------------------------------------------------------------------------------|
| `-o`, `owner`   | The owner of the repo.<br>Assign from `${{ github.repository_owner }}`. Example, `"dotnet"`.                           |
| `-n`, `name`    | The repository name.<br>Assign from `${{ github.repository }}`. Example, `"dotnet/samples"`.                           |
| `-b`, `branch`  | The branch name.<br>Assign from `${{ github.ref }}`. Example, `"main"`.                                                |
| `-t`, `token`   | The GitHub personal-access token (PAT), or the token from GitHub action context.<br>Assign from `${{ github.token }}`. |

## Optional inputs

| Option          | Details                                                         |
|:----------------|:----------------------------------------------------------------|
| `-d`, `dir`     | The root directory, defaults to `"/github/workspace"`.          |
| `-p`, `pattern` | The search pattern, defaults to `"*.csproj;*.fsproj;*.vbproj"`. |

## Example workflow

```yml
name: '.net version sweeper'

on:
  schedule:
  - cron: '0 0 1 * *' # run on the 1st of the month

env:
  DOTNET_VERSION: '5.0.102' # SDK version

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET Core
      uses: actions/setup-dotnet@v1.7.2
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: .NET version sweeper
      uses: ./
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        owner: ${{ github.repository_owner }}
        name: ${{ github.repository }}
        branch: ${{ github.ref }}
```

## Configure action

Create a file at the root of the repository, named *dotnet-versionsweeper.json* and add an `ignore` entry following the [globbing patterns detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).

```json
{
  "ignore": [
    "**/*ThisShouldNeverBeFlagged.csproj"
  ]
}
```

## Acknowledgements

| NuGet Package | URL & License |
|:-|:-|
| `CommandLineParser` | [https://www.nuget.org/packages/CommandLineParser](https://www.nuget.org/packages/CommandLineParser) ([MIT](https://www.nuget.org/packages/CommandLineParser/2.8.0/License)) |
| `Nito.AsyncEx` | [https://www.nuget.org/packages/Nito.AsyncEx](https://www.nuget.org/packages/Nito.AsyncEx) ([MIT](https://licenses.nuget.org/MIT)) |
| `MarkdownBuilder` | [https://www.nuget.org/packages/MarkdownBuilder](https://www.nuget.org/packages/MarkdownBuilder) ([MIT](https://licenses.nuget.org/MIT)) |
| `NuGet.Versioning` | [https://www.nuget.org/packages/NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning) ([Apache-2.0](https://licenses.nuget.org/Apache-2.0)) |
| `Octokit` | [https://www.nuget.org/packages/Octokit](https://www.nuget.org/packages/Octokit) ([MIT](https://licenses.nuget.org/MIT)) |
| `Octokit.Extensions` | [https://www.nuget.org/packages/Octokit.Extensions](https://www.nuget.org/packages/Octokit.Extensions) ([MIT](https://github.com/mirsaeedi/octokit.net.Extensions/blob/master/LICENSE)) |
