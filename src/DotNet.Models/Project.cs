using System;
using SystemFile = System.IO.File;
using SystemPath = System.IO.Path;

namespace DotNet.Models
{
    public record Project
    {
        string _fullPath = null!;

        public string FullPath
        {
            get => _fullPath;
            set
            {
                _fullPath = value;
                if (value is { Length: > 0 } && SystemFile.Exists(value))
                {
                    Extension = SystemPath.GetExtension(value);
                }
            }
        }

        public string Extension { get; private set; } = null!;

        public int TfmLineNumber { get; init; } = -1;

        public string RawTargetFrameworkMonikers { get; init; } = null!;

        public string[] Tfms => 
            RawTargetFrameworkMonikers?.Split(";", StringSplitOptions.RemoveEmptyEntries) ??
            Array.Empty<string>();
    }
}
