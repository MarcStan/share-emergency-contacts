using CommandLine;

namespace VersionPatcher
{
    public class Options
    {
        [Option('c', "csharp", Default = null, HelpText = "Path to AssemblyInfo.cs containing at least \"[assembly: AssemblyVersion(\"")]
        public string AssemblyInfo { get; set; }

        [Option('a', "android", Default = null, HelpText = "Path to AndroidManifest.xml")]
        public string AndroidManifest { get; set; }

        [Option('i', "ios", Default = null, HelpText = "Path to iOS Info.plist file")]
        public string IOSManifest { get; set; }

        [Option('u', "uwp", Default = null, HelpText = "Path to UWP Package.appxmanifest file")]
        public string UwpManifest { get; set; }

        [Option('v', "version", Default = null, HelpText = "Version to be set. Should be valid version (e.g. \"1.2.3\", \"1.0\", \"1.0.0.0\", etc.).")]
        public string Version { get; set; }

        [Option("appCode", Default = null, HelpText = "Accepted value: int or literal \"++\". The new appCode (if \"++\" then the existing code is incremented by 1). Used only by android (when set, a|android must also be set)")]
        public string AppCode { get; set; }
    }
}