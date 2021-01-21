using System.Xml.Serialization;

namespace DotNet.Versions
{
    [XmlRoot]
    public class Project
    {
        [XmlAttribute]
        public string? Sdk { get; set; }

        public PropertyGroup[]? PropertyGroup { get; set; }
    }

    public class PropertyGroup
    {
        /// <summary>
        /// The target framework of the project, SDK style.
        /// </summary>
        public string? TargetFramework { get; set; }

        /// <summary>
        /// A semicolon (netstandard2.0;net5.0) delimited list of target frameworks of the project, SDK style.
        /// </summary>
        public string? TargetFrameworks { get; set; }

        /// <summary>
        /// The target framework version of the project, non-SDK style.
        /// </summary>
        public string? TargetFrameworkVersion { get; set; }
    }
}
