// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.IO;

public interface ISolutionFileReader
{
    ValueTask<Solution> ReadSolutionAsync(string solutionPath);
}
