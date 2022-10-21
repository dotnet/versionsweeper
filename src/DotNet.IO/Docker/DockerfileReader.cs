// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.IO;

public class DockerfileReader : IDockerfileReader
{
    static readonly RegexOptions _options =
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
    static readonly Regex _dockerfileTfmsExpression =
        new(@"FROM .+?dotnet.+?:(?<tfm>.+?)[\s|\n]", _options);

    public async ValueTask<Dockerfile> ReadDockerfileAsync(string dockerfilePath)
    {
        Dockerfile dockerfile = new()
        {
            FullPath = dockerfilePath
        };
        
        if (SystemFile.Exists(dockerfilePath))
        {
            var dockerfileContent = await SystemFile.ReadAllTextAsync(dockerfilePath);
            var matches = _dockerfileTfmsExpression.Matches(dockerfileContent);

            foreach (Match match in matches)
            {
                var group = match.Groups["tfm"];
                var (index, tfm) = (group.Index, group.Value);
                if (tfm is not null)
                {
                    if (dockerfile.ImageDetails is null)
                    {
                        dockerfile = dockerfile with
                        {
                            ImageDetails = new HashSet<ImageDetails>()
                        };
                    }

                    var lineNumber = GetLineNumberFromIndex(dockerfileContent, index);

                    dockerfile.ImageDetails.Add(
                        new TfmAndLineNumberPair(tfm, lineNumber));
                }
            }
        }

        return dockerfile;
    }

    static int GetLineNumberFromIndex(string content, int index)
    {
        var lineNumber = 1;
        for (var i = 0; i < index; ++i)
        {
            if (content[i] == '\n') ++lineNumber;
        }
        return lineNumber;
    }
}
