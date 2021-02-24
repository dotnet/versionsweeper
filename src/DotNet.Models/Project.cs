using System;
using SystemFile = System.IO.File;
using SystemPath = System.IO.Path;

namespace DotNet.Models
{
    public record Project
    {
        string _fullPath = null!;

        /// <summary>
        /// The SDK value that the project is specifying.
        /// </summary>
        public string? Sdk { get; init; }

        /// <summary>
        /// Gets a value indicating whether the project is the new SDK-style format.
        /// </summary>
        public bool IsSdkStyle => Sdk is not null and { Length: > 0 };

        /// <summary>
        /// The fully qualified path of the project.
        /// </summary>
        public string FullPath
        {
            get => _fullPath;
            set
            {
                _fullPath = value;
                if (value is { Length: > 0 })
                {
                    Extension = SystemPath.GetExtension(value);
                }
            }
        }

        /// <summary>
        /// The file extension of the project, for example; ".csproj".
        /// </summary>
        public string Extension { get; private set; } = null!;

        /// <summary>
        /// The line number in the project file where the TargetFramework(s) element exists.
        /// </summary>
        public int TfmLineNumber { get; init; } = -1;

        /// <summary>
        /// The raw string representation of the TargetFramework(s) element in the project file.
        /// </summary>
        public string RawTargetFrameworkMonikers { get; init; } = null!;

        /// <summary>
        /// The parsed target framework monikers.
        /// </summary>
        public string[] Tfms =>
            RawTargetFrameworkMonikers?.Split(";", StringSplitOptions.RemoveEmptyEntries) ??
            Array.Empty<string>();
    }
}
