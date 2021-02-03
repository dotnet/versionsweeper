# 🎯 LTS - .NET version sweeper

![👌 build & test](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/build%20&%20test/badge.svg)
![🧹 .net version sweeper](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/.net%20version%20sweeper/badge.svg)
![🔒 code ql](https://github.com/IEvangelist/dotnet-versionsweeper/workflows/%F0%9F%94%92%20code%20ql/badge.svg)

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
# The name used in the GitHub UI for the workflow
name: '.net version sweeper'

# When to run this action:
# - Scheduled on the first of every month
# - Manually runable from the GitHub UI with a reason
on:
  schedule:
  - cron: '0 0 1 * *'
  workflow_dispatch:
    inputs:
      reason:
        description: 'The reason for running the workflow'
        required: true
        default: 'Manual run'

# Run on the latest version of Ubuntu
jobs:
  build:
    runs-on: ubuntu-latest

    # Checkout the repo into the workspace within the VM
    steps:
    - uses: actions/checkout@v2

    # If triggered manually, print the reason why
    - name: 'Print manual run reason'
      if: ${{ github.event_name == 'workflow_dispatch' }}
      run: |
        echo "Reason: ${{ github.event.inputs.reason }}"

    # Run the .NET version sweeper
    # Issues will be automatically created for any non-ignored projects that are targeting non-LTS versions
    - name: .NET version sweeper
      id: dotnet-version-sweeper
      uses: ./
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        owner: ${{ github.repository_owner }}
        name: ${{ github.repository }}
        branch: ${{ github.ref }}
```

## Configure action

To configure the action, you can create a file at the root of the repository named *dotnet-versionsweeper.json*. This config file contains a node, named `"ignore"` that is an array of patterns following the [globbing matcher detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).

```json
{
  "ignore": [
    "**/SomePath/**/*.csproj",
    "**/*ThisShouldNeverBeFlagged.csproj",
    "IgnoreDir/**/*.*"
  ]
}
```

For an example config file, see [dotnet/samples/dotnet-versionsweeper.json](https://github.com/dotnet/samples/blob/master/dotnet-versionsweeper.json).

## Acknowledgements

| NuGet Package | URL & License |
|:-|:-|
| `CommandLineParser` | [https://www.nuget.org/packages/CommandLineParser](https://www.nuget.org/packages/CommandLineParser) ([MIT](https://www.nuget.org/packages/CommandLineParser/2.8.0/License)) |
| `MarkdownBuilder` | [https://www.nuget.org/packages/MarkdownBuilder](https://www.nuget.org/packages/MarkdownBuilder) ([MIT](https://licenses.nuget.org/MIT)) |
| `Nito.AsyncEx` | [https://www.nuget.org/packages/Nito.AsyncEx](https://www.nuget.org/packages/Nito.AsyncEx) ([MIT](https://licenses.nuget.org/MIT)) |
| `NuGet.Versioning` | [https://www.nuget.org/packages/NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning) ([Apache-2.0](https://licenses.nuget.org/Apache-2.0)) |
| `Octokit` | [https://www.nuget.org/packages/Octokit](https://www.nuget.org/packages/Octokit) ([MIT](https://licenses.nuget.org/MIT)) |
| `Octokit.Extensions` | [https://www.nuget.org/packages/Octokit.Extensions](https://www.nuget.org/packages/Octokit.Extensions) ([MIT](https://github.com/mirsaeedi/octokit.net.Extensions/blob/master/LICENSE)) |
