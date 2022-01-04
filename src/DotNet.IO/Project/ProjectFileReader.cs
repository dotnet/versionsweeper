// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.IO;

public class ProjectFileReader : IProjectFileReader
{
    static readonly RegexOptions _options =
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
    static readonly Regex _projectSdkExpression =
        new(@"\<Project Sdk=""(?<sdk>.+?)""", _options);
    static readonly Regex _targetFrameworkExpression =
        new(@"TargetFramework(.*)>(?<tfm>.+?)</", _options);

    public async ValueTask<Project> ReadProjectAsync(string projectPath)
    {
        Project project = new()
        {
            FullPath = projectPath
        };

        if (SystemFile.Exists(projectPath))
        {
            var projectContent = await SystemFile.ReadAllTextAsync(projectPath);
            return project.Extension switch
            {
                ".json" => ParseJson(project, projectContent),
                _ => ParseXml(project, projectContent)
            };
        }

        return project;

        static Project ParseXml(Project project, string projectContent)
        {
            var (index, rawTfms) = MatchExpression(_targetFrameworkExpression, projectContent, "tfm");
            var lineNumber = GetLineNumberFromIndex(projectContent, index);
            var (_, sdk) = MatchExpression(_projectSdkExpression, projectContent, "sdk");

            return project with
            {
                TfmLineNumber = lineNumber,
                RawTargetFrameworkMonikers = rawTfms!,
                Sdk = sdk.NullIfEmpty()
            };
        }

        static Project ParseJson(Project project, string projectContent)
        {
            var projectJson = projectContent.FromJson<ProjectJson>();
            if (projectJson is null or { Frameworks: { Count: 0 } })
            {
                return project;
            }

            var rawTfms = string.Join(";", projectJson.Frameworks.Keys);
            var lineNumber = GetLineNumberFromProjectJson(
                projectJson.Frameworks.Keys.First(), projectContent);

            return project with
            {
                TfmLineNumber = lineNumber,
                RawTargetFrameworkMonikers = rawTfms
            };
        }
    }

    static int GetLineNumberFromProjectJson(string tfm, string json)
    {
        var lineNumber = 0;
        if (json is { Length: > 0 })
        {
            var lines = json.Split('\n');
            for (var i = 0; i < lines.Length; ++i)
            {
                if (lines[i]?.Contains(tfm, StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    lineNumber = i + 1;
                    break;
                }
            }
        }
        return lineNumber;
    }

    static (int Index, string? Value) MatchExpression(
        Regex expression, string content, string groupName)
    {
        var match = expression?.Match(content);
        if (match is not null)
        {
            var group = match.Groups[groupName];
            return (group.Index, group.Value);
        }

        return (0, null);
    }

    static int GetLineNumberFromIndex(string xml, int index)
    {
        var lineNumber = 1;
        for (var i = 0; i < index; ++i)
        {
            if (xml[i] == '\n') ++lineNumber;
        }
        return lineNumber;
    }
}
