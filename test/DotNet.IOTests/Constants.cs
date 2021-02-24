namespace DotNet.IOTests
{
    class Constants
    {
        internal const string TestSolutionXml = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.30803.129
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""test-project"", ""sln-test.csproj"", ""{835DBC44-B11C-45E8-A042-D550FC96F4B0}""
EndProject
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""test-project-json"", ""sln-test.json"", ""{450FAE99-6EFF-41DB-A041-CA520FCEE8BD}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{835DBC44-B11C-45E8-A042-D550FC96F4B0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{835DBC44-B11C-45E8-A042-D550FC96F4B0}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{835DBC44-B11C-45E8-A042-D550FC96F4B0}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{835DBC44-B11C-45E8-A042-D550FC96F4B0}.Release|Any CPU.Build.0 = Release|Any CPU
		{450FAE99-6EFF-41DB-A041-CA520FCEE8BD}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{450FAE99-6EFF-41DB-A041-CA520FCEE8BD}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{450FAE99-6EFF-41DB-A041-CA520FCEE8BD}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{450FAE99-6EFF-41DB-A041-CA520FCEE8BD}.Release|Any CPU.Build.0 = Release|Any CPU		
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{835DBC44-B11C-45E8-A042-D550FC96F4B0} = {503A8641-B4A5-4D57-9CD2-54C5F894E88D}
		{450FAE99-6EFF-41DB-A041-CA520FCEE8BD} = {503A8641-B4A5-4D57-9CD2-54C5F894E88D}
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {70E27639-9B58-4BD1-93FB-05D76A8A32EF}
	EndGlobalSection
EndGlobal
";

        internal const string TestProjectXml = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""16.5.0"" />
    <PackageReference Include=""MSTest.TestAdapter"" Version=""2.1.0"" />
    <PackageReference Include=""MSTest.TestFramework"" Version=""2.1.0"" />
    <PackageReference Include=""coverlet.collector"" Version=""1.2.0"" />
    <PackageReference Include=""xunit"" Version=""2.4.1"" />
    <PackageReference Include=""xunit.runner.visualstudio"" Version=""2.4.1"" />
  </ItemGroup>

</Project>";

        internal const string TestProjectJson = @"{
  ""version"": ""1.0.0-*"",
  ""buildOptions"": {
    ""emitEntryPoint"": true
  },

  ""dependencies"": {
    ""Microsoft.AspNetCore.Server.IISIntegration"": ""1.0.0"",
    ""Microsoft.AspNetCore.Server.Kestrel"": ""1.0.0"",
    ""Microsoft.AspNetCore.Owin"": ""1.0.0""
  },

  ""tools"": {
    ""Microsoft.AspNetCore.Server.IISIntegration.Tools"": ""1.0.0-preview2-final""
  },

  ""frameworks"": {
    ""netcoreapp1.0"": {
      ""dependencies"": {
        ""Microsoft.NETCore.App"": {
          ""version"": ""1.0.0"",
          ""type"": ""platform""
        }
      },
      ""imports"": [
        ""dotnet5.6"",
        ""portable-net45+win8""
      ]
    }
},

  ""publishOptions"": {
    ""include"": [
      ""wwwroot"",
      ""appsettings.json"",
      ""web.config""
    ]
  },

  ""scripts"": {
    ""postpublish"": [ ""dotnet publish-iis --publish-folder %publish:OutputPath% --framework %publish:FullTargetFramework%"" ]
  }
}";

        internal const string TestProjectJsonMultipleTfms = @"{
  ""version"": ""1.0.0-*"",
  ""tooling"": {
    ""defaultNamespace"": ""vscode_aspnet5""
  },

  ""dependencies"": {
    ""Microsoft.AspNet.Diagnostics"": ""1.0.0-rc1-final"",
    ""Microsoft.AspNet.IISPlatformHandler"": ""1.0.0-rc1-final"",
    ""Microsoft.AspNet.Mvc"": ""6.0.0-rc1-final"",
    ""Microsoft.AspNet.Mvc.TagHelpers"": ""6.0.0-rc1-final"",
    ""Microsoft.AspNet.Server.Kestrel"": ""1.0.0-rc1-final"",
    ""Microsoft.AspNet.StaticFiles"": ""1.0.0-rc1-final"",
    ""Microsoft.AspNet.Tooling.Razor"": ""1.0.0-rc1-final"",
    ""Microsoft.Extensions.Configuration.Json"": ""1.0.0-rc1-final"",
    ""Microsoft.Extensions.Logging"": ""1.0.0-rc1-final"",
    ""Microsoft.Extensions.Logging.Console"": ""1.0.0-rc1-final"",
    ""Microsoft.Extensions.Logging.Debug"" : ""1.0.0-rc1-final""
  },

  ""commands"": {
    ""web"": ""Microsoft.AspNet.Server.Kestrel"",
    ""web-dev"": ""Microsoft.AspNet.Server.Kestrel --ASPNET_ENV Development""
  },

  ""frameworks"": {
    ""dnx46"": {},
    ""dnxcore50"": {}
  },

  ""exclude"": [
    ""wwwroot"",
    ""node_modules""
  ],
  ""publishExclude"": [
    ""node_modules"",
    ""**.xproj"",
    ""**.user"",
    ""**.vspscc""
  ],
  ""scripts"": {
    ""postrestore"": [
      ""npm install"",
      ""bower install""
    ],
    ""prepublish"": [
      ""npm install"",
      ""bower install"",
      ""gulp clean"",
      ""gulp min""
    ]
  }
}";
    }
}
