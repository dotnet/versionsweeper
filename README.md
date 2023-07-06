# 🎯 LTS (or STS) versions
## .NET version sweeper

[![build & test](https://github.com/dotnet/versionsweeper/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/dotnet/versionsweeper/actions/workflows/build-and-test.yml)
[![target supported version](https://github.com/dotnet/versionsweeper/actions/workflows/dog-food.yml/badge.svg)](https://github.com/dotnet/versionsweeper/actions/workflows/dog-food.yml)
[![code-ql analysis](https://github.com/dotnet/versionsweeper/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/dotnet/versionsweeper/actions/workflows/codeql-analysis.yml)
[![GitHub marketplace](https://img.shields.io/badge/marketplace-.NET%20version%20sweeper-green?colorA=24292e&colorB=97ca00&style=flat&longCache=true&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAM6wAADOsB5dZE0gAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAERSURBVCiRhZG/SsMxFEZPfsVJ61jbxaF0cRQRcRJ9hlYn30IHN/+9iquDCOIsblIrOjqKgy5aKoJQj4O3EEtbPwhJbr6Te28CmdSKeqzeqr0YbfVIrTBKakvtOl5dtTkK+v4HfA9PEyBFCY9AGVgCBLaBp1jPAyfAJ/AAdIEG0dNAiyP7+K1qIfMdonZic6+WJoBJvQlvuwDqcXadUuqPA1NKAlexbRTAIMvMOCjTbMwl1LtI/6KWJ5Q6rT6Ht1MA58AX8Apcqqt5r2qhrgAXQC3CZ6i1+KMd9TRu3MvA3aH/fFPnBodb6oe6HM8+lYHrGdRXW8M9bMZtPXUji69lmf5Cmamq7quNLFZXD9Rq7v0Bpc1o/tp0fisAAAAASUVORK5CYII=)](https://github.com/marketplace/actions/net-version-sweeper)
[![GitHub license](https://img.shields.io/github/license/dotnet/versionsweeper)](https://github.com/dotnet/versionsweeper/blob/main/LICENSE)
[![GitHub contributors](https://img.shields.io/github/contributors/dotnet/versionsweeper.svg)](https://GitHub.com/dotnet/versionsweeper/graphs/contributors/)
[![GitHub repo size](https://img.shields.io/github/repo-size/dotnet/versionsweeper)](https://github.com/dotnet/versionsweeper)
[![GitHub issues-opened](https://img.shields.io/github/issues/dotnet/versionsweeper.svg)](https://GitHub.com/dotnet/versionsweeper/issues?q=is%3Aissue+is%3Aopened)
[![GitHub issues-closed](https://img.shields.io/github/issues-closed/dotnet/versionsweeper.svg)](https://GitHub.com/dotnet/versionsweeper/issues?q=is%3Aissue+is%3Aclosed)
[![GitHub pulls-opened](https://img.shields.io/github/issues-pr/dotnet/versionsweeper.svg)](https://GitHub.com/dotnet/versionsweeper/pulls?q=is%3Aissue+is%3Aopened)
[![GitHub pulls-merged](https://img.shields.io/github/issues-search/dotnet/versionsweeper?label=merged%20pull%20requests&query=is%3Apr%20is%3Aclosed%20is%3Amerged&color=darkviolet)](https://github.com/dotnet/versionsweeper/pulls?q=is%3Apr+is%3Aclosed+is%3Amerged)
[![GitHub pulls-unmerged](https://img.shields.io/github/issues-search/dotnet/versionsweeper?label=unmerged%20pull%20requests&query=is%3Apr%20is%3Aclosed%20is%3Aunmerged&color=red)](https://github.com/dotnet/versionsweeper/pulls?q=is%3Apr+is%3Aclosed+is%3Aunmerged)

## Get started

The .NET version sweeper is designed to alert repositories (by either creating issues, pull requests, or both) that there are projects targeting versions that are no longer supported (or won't be soon). For example, projects targeting .NET Core 3.0 or .NET Framework 4.5.1 could trigger an issue to be created to update these projects to supported versions, or even a pull request that upgrades it for you. For example issues, see [issues created in this repo based on the *non-lts* directory](https://github.com/dotnet/versionsweeper/issues?q=is%3Aopen+is%3Aissue+label%3Adotnet-target-version).

This is intended to be used as a GitHub action that will run as a [scheduled CRON job](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#onschedule). Ideally, once a month or as often as necessary to align with .NET version updates.

A schedule/cron job that runs on the first of every month is detailed below in the [example workflow](#example-workflow), `'0 0 1 * *'`.

## Required inputs

| Option         | Details                                                                                                                |
|:---------------|:-----------------------------------------------------------------------------------------------------------------------|
| `-o`, `owner`  | The owner of the repo.<br>Assign from `${{ github.repository_owner }}`. Example, `"dotnet"`.                           |
| `-n`, `name`   | The repository name.<br>Assign from `${{ github.repository }}`. Example, `"dotnet/samples"`.                           |
| `-b`, `branch` | The branch name.<br>Assign from `${{ github.ref }}`. Example, `"main"`.                                                |
| `-t`, `token`  | The GitHub personal-access token (PAT), or the token from GitHub action context.<br>Assign from `${{ github.token }}`. |

## Optional inputs

| Option                 | Details                                                                                |
|:-----------------------|:---------------------------------------------------------------------------------------|
| `-d`, `dir`            | The root directory, defaults to `"/github/workspace"`.                                 |
| `-p`, `pattern`        | The search pattern, defaults to `"*.csproj;*.fsproj;*.vbproj;*.xproj;project.json"`.   |
| `-s`, `sdk-compliance` | Whether or not to report projects that are not using the new SDK-style project format. |

## GitHub Action outputs

| Output               | Type       | Details                                                                                                    |
|:---------------------|:-----------|:-----------------------------------------------------------------------------------------------------------|
| `has-remaining-work` | `bool`     | When `true`, the `upgrade-projects` array will be populated with project directories that require upgrade. |
| `upgrade-projects`   | `string[]` | An array of project directories that are in need of being upgraded.                                        |

> **Note**
> Outputs are only present when configured to run as pull request mode. For more information, see [Configure action](#configure-action).

## Example workflow

```yml
# This is a basic workflow to help you get started with Actions

name: "target supported version"

# Controls when the action will run.
on:
  # Triggers the workflow on push or pull request events but only for the default branch
  schedule:
    - cron: "0 0 1 * *"
  workflow_dispatch:
    inputs:
      reason:
        description: "The reason for running the workflow"
        required: true
        default: "Manual run"
      support:
        description: "The support level to target (STS, LTS, or Preview)."
        required: true
        default: "STS"

# A workflow run is made up of one or more jobs that can run sequentially or in parallel
jobs:
  # This workflow contains a single job called "build"
  version-sweep:
    # The type of runner that the job will run on
    runs-on: ubuntu-latest
    permissions:
      issues: write
      pull-requests: write

    # Steps represent a sequence of tasks that will be executed as part of the job
    steps:
      # Checks-out your repository under $GITHUB_WORKSPACE, so your job can access it
      - uses: actions/checkout@v3

      # Runs a single command using the runners shell
      - name: "Print manual run reason"
        if: ${{ github.event_name == 'workflow_dispatch' }}
        run: |
          echo 'Reason: ${{ github.event.inputs.reason }}'

      # Start the .NET version sweeper, scan projects/slns for non-LTS (or currrent) versions
      - name: .NET version sweeper
        id: dotnet-version-sweeper
        uses: dotnet/versionsweeper@main
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          owner: ${{ github.repository_owner }}
          name: ${{ github.repository }}
          branch: ${{ github.ref }}
      
      - name: Create pull requests
        if: steps.dotnet-version-sweeper.outputs.has-remaining-work == 'true'
        run: |
          upgradeProjects: ${{ steps.dotnet-version-sweeper.outputs.upgrade-projects }}

          # Install .NET Upgrade Assistant global tool
          dotnet tool install --global upgrade-assistant

          # Iterate all upgrade projects
          for projectDir in "${upgradeProjects[@]}"; do
            echo "Project Directory: $projectDir"

            # Create a new branch
            git checkout -b upgrade/$projectDir

            # Perform the upgrade using upgrade-assistant
            upgrade-assistant upgrade "$projectDir" --non-interactive -t ${{ inputs.support }}

            # Commit the changes
            git add .
            git commit -m ".NET Version Sweeper: Upgraded $projectDir"

            # Push the branch to the repository
            git push origin upgrade/$projectDir

            # Create a pull request
            gh pr create \
              --base main \
              --head upgrade/$projectDir \
              --title "Upgraded $projectDir" \
              --body "Proposed upgrade for $projectDir"
          done
```

## Project and solution discovery

The .NET version sweeper currently supports reporting on all of the following types:

- C# project files: `*.csproj`
- F# project files: `*.fsproj`
- VB project files: `*.vbproj`
- DNX project files: `*.xproj`
- Project JSON files: `project.json`
- Solution files: `*.sln`

## Dockerfile discovery

The .NET version sweeper also supports reporting _Dockerfile_ target frameworks that are out-of-support, or soon to be, for example:

- `FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8`
- `COPY --from=mcr.microsoft.com/dotnet/framework/runtime:3.5-20221011-windowsservercore-ltsc2019`

## Configure action

To configure the action, you can create a file at the root of the repository named *dotnet-versionsweeper.json*. This config file contains a node, named `"ignore"` that is an array of patterns following the [globbing matcher detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).

The file can also contain a value `outOfSupportWithinDays` to specify the number of additional days to consider flagging versions that will be out of support.

```json
{
  "ignore": [
    "**/SomePath/**/*.csproj",
    "**/*ThisShouldNeverBeFlagged.csproj",
    "IgnoreDir/**/*.*"
  ],
  "outOfSupportWithinDays": 90,
  "actionType": "All"
}
```

For an example config file, see [dotnet/samples/dotnet-versionsweeper.json](https://github.com/dotnet/samples/blob/master/dotnet-versionsweeper.json).

| Configuration | Type |  Details |
|:--|:--|:--|
| `ignore` | `string[]` | Glob patterns to ignore. |
| `outOfSupportWithinDays` | `string[]` | The number of days in advance to monitor for. |
| `actionType` | `ActionType` <br><br> &nbsp;&nbsp; `"CreateIssue"` <br> &nbsp;&nbsp; `"PullRequest"` <br> &nbsp;&nbsp; `"All"` | The type of action to take, defaults to `CreateIssue`.  |

## Label auto-generation

This tool will create a label named `dotnet-target-version` for easier tracking of issues and pull requests that it creates. The label is created with the following description and color by default, please do not change the name - as that is what is used to determine whether or not to create a new label.

- `Description`: "Issues and PRs automatically generated from the .NET version sweeper."
- `Color`: [Official .NET purple #512bd4](https://hexcolorcodes.org/hex-code/512BD4)

### Example labels in the wild

- [dotnet/doc](https://github.com/dotnet/docs/labels/dotnet-target-version)
- [dotnet/samples](https://github.com/dotnet/samples/labels/dotnet-target-version)
- [dotnet/versionsweeper](https://github.com/dotnet/versionsweeper/labels/dotnet-target-version)

## Example issues

This repo serves as a sample, as it contains a directory *non-lts* with projects and solutions that are intentionally targeting unsupported versions. There are issues created against these to exemplify how they render. For more information, see [these issues](https://github.com/IEvangelist/dotnet-versionsweeper/issues?q=is%3Aopen+is%3Aissue+label%3Aexample-issue).

## Official .NET support policies

This action is intended to help determine non-LTS (or STS) versions, but it _isn't_ perfect. When in doubt, please refer to the [official .NET support policies](https://dotnet.microsoft.com/platform/support/policy).

## Acknowledgements

| Name | NuGet package URL & license |
|:-|:-|
| `CommandLineParser` | [https://www.nuget.org/packages/CommandLineParser](https://www.nuget.org/packages/CommandLineParser) ([MIT](https://www.nuget.org/packages/CommandLineParser/2.8.0/License)) |
| `MarkdownBuilder` | [https://www.nuget.org/packages/MarkdownBuilder](https://www.nuget.org/packages/MarkdownBuilder) ([MIT](https://licenses.nuget.org/MIT)) |
| `NSubstitute` | [https://www.nuget.org/packages/NSubstitute](https://www.nuget.org/packages/NSubstitute) ([LICENSE](https://github.com/nsubstitute/NSubstitute/blob/master/LICENSE.txt)) |
| `Octokit` | [https://www.nuget.org/packages/Octokit](https://www.nuget.org/packages/Octokit) ([MIT](https://licenses.nuget.org/MIT)) |
| `Octokit.Extensions` | [https://www.nuget.org/packages/Octokit.Extensions](https://www.nuget.org/packages/Octokit.Extensions) ([MIT](https://github.com/mirsaeedi/octokit.net.Extensions/blob/master/LICENSE)) |
