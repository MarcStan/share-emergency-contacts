using CommandLine;
using System;
using System.IO;
using System.Xml.Linq;

namespace VersionPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = Parser.Default.ParseArguments<Options>(args);
            if (r.Tag == ParserResultType.Parsed)
            {
                // weird way to do this, but okay
                var options = ((Parsed<Options>)r).Value;
                if (options.Version == null)
                {
                    Console.WriteLine("Version is a required attribute");
                    return;
                }
                if (options.AppCode != null && options.AndroidManifest == null)
                {
                    Console.WriteLine("AndroidManifest path (a|android) is required when setting appCode value");
                    return;
                }
                if (options.AssemblyInfo != null && !File.Exists(options.AssemblyInfo))
                {
                    Console.WriteLine("Invalid path for assemblyInfo file!");
                    return;
                }
                if (options.AndroidManifest != null && !File.Exists(options.AndroidManifest))
                {
                    Console.WriteLine("Invalid path for AndroidManifest file!");
                    return;
                }
                if (options.IOSManifest != null && !File.Exists(options.IOSManifest))
                {
                    Console.WriteLine("Invalid path for iOS plist file!");
                    return;
                }
                if (options.UwpManifest != null && !File.Exists(options.UwpManifest))
                {
                    Console.WriteLine("Invalid path for Package.appxmanifest file!");
                    return;
                }
                try
                {
                    Process(options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process was aborted mid-way. Some values may have been patched.");
                    Console.WriteLine(e.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid command line options!");
                return;
            }
            Console.WriteLine("All done!");
        }


        public static void Process(Options options)
        {
            // asserted that options is now valid
            if (options.AssemblyInfo != null)
            {
                PatchCSharpVersion(options.AssemblyInfo, options.Version);
                Console.WriteLine($"Successfully patched AssemblyInfo file '{options.AssemblyInfo}' with new version '{options.Version}'");
            }
            if (options.AndroidManifest != null)
            {
                PatchAndroidVersion(options.AndroidManifest, options.Version, options.AppCode);
                Console.WriteLine($"Successfully patched AndroidManifest file '{options.AndroidManifest}' with new version '{options.Version}'");
            }
            if (options.IOSManifest != null)
            {
                PatchiOSVersion(options.IOSManifest, options.Version);
                Console.WriteLine($"Successfully patched Info.plist file '{options.IOSManifest}' with new version '{options.Version}'");
            }
            if (options.UwpManifest != null)
            {
                PatchUwpVersion(options.UwpManifest, options.Version);
                Console.WriteLine($"Successfully patched appxmanifest file '{options.UwpManifest}' with new version '{options.Version}'");
            }
        }

        /// <summary>
        /// Opens the Package.appxmanifest and sets the new version number.
        /// </summary>
        /// <param name="uwpManifest"></param>
        /// <param name="version"></param>
        private static void PatchUwpVersion(string uwpManifest, string version)
        {
            XDocument doc = XDocument.Load(uwpManifest);

            const string ns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
            var identityElement = XName.Get("Identity", ns);
            var versionElement = doc.Root.Element(identityElement).Attribute("Version");
            // UWP requires the version element to have 4 digits
            Version v;
            if (!Version.TryParse(version, out v))
            {
                throw new NotSupportedException($"Cannot set value '{version}' in package as it is not a valid version");
            }
            // if user didn't set all 4 values parser returns -1 for any missing
            // override that with 0
            var toZero = new Func<int, int>(i => i == -1 ? 0 : i);
            v = new Version(toZero(v.Major), toZero(v.Minor), toZero(v.Build), toZero(v.Revision));
            version = v.ToString(4);
            versionElement.SetValue(version);
            doc.SaveAsUtf8(uwpManifest);
        }

        /// <summary>
        /// Opens the Info.plist file and replaces the CFBundleShortVersionString and CFBundleVersion to the provided version.
        /// </summary>
        /// <param name="iosManifest"></param>
        /// <param name="version"></param>
        private static void PatchiOSVersion(string iosManifest, string version)
        {
            var doc = XDocument.Load(iosManifest);
            var dict = doc.Element("plist").Element("dict");
            var child = dict.FirstNode;
            while (child.NextNode != null)
            {
                var name = ((XElement)child).Value;
                if (name == "CFBundleShortVersionString" ||
                    name == "CFBundleVersion")
                {
                    // next node has value
                    child = child.NextNode;
                    var e = ((XElement)child);
                    if (e.Name != "string")
                    {
                        throw new NotSupportedException($"After key next entry must be <string> but found '{e.Name}'");
                    }
                    e.SetValue(version);
                }
                child = child.NextNode;
            }
            doc.SaveAsUtf8(iosManifest);
        }

        /// <summary>
        /// Opens the android manifest and updates the versioName with SemVer of the provided version.
        /// If newAppCode is not null also updates the versionCode.
        /// </summary>
        /// <param name="androidManifest"></param>
        /// <param name="version"></param>
        /// <param name="newAppCode">Must either be literal '++' or valid integer. If '++' will increment the existing value by 1.</param>
        private static void PatchAndroidVersion(string androidManifest, string version, string newAppCode)
        {
            // either increment existing value
            bool patchCode = newAppCode == "++";
            int? code = null;
            // or check if a valid int is provided
            if (newAppCode != null && int.TryParse(newAppCode, out int x))
            {
                patchCode = true;
                code = x;
            }
            if (!patchCode && !string.IsNullOrEmpty(newAppCode))
            {
                throw new NotSupportedException("Invalid value for appcode: " + newAppCode);
            }
            const string ns = "http://schemas.android.com/apk/res/android";

            var doc = XDocument.Load(androidManifest);
            var versionNameAttribute = XName.Get("versionName", ns);
            doc.Root.SetAttributeValue(versionNameAttribute, version);
            // only patch it if a version was provided
            if (patchCode)
            {
                var versionCodeAttribute = XName.Get("versionCode", ns);
                string codeString;
                if (!code.HasValue)
                {
                    // no int value provided, increment existing value
                    var oldValue = doc.Root.Attribute(versionCodeAttribute).Value;
                    if (!int.TryParse(oldValue, out int oldV))
                    {
                        throw new NotSupportedException($"Existing versionCode value must be valid int! (Found: {oldValue})");
                    }
                    codeString = (oldV + 1).ToString();
                }
                else
                {
                    // use user provided value
                    codeString = code.Value.ToString();
                }
                Console.WriteLine($"Setting Android appCode to: " + codeString);
                doc.Root.SetAttributeValue(versionCodeAttribute, codeString);
            }
            doc.SaveAsUtf8(androidManifest);
        }

        /// <summary>
        /// Opens the AssemblyInfo.cs file from the path and inserts the provided value into the fields AssemblyVersion, AssemblyFileVersion and AssemblyInformationalVersion.
        /// Only AssemblyVersion must exist (and must be replaced), the other two fields will only be replaced if the are found.
        /// </summary>
        /// <param name="assemblyInfo"></param>
        /// <param name="version"></param>
        private static void PatchCSharpVersion(string assemblyInfo, string version)
        {
            // could probably be done with a regex, but string parsing is simpler to write
            var text = File.ReadAllText(assemblyInfo);
            var v = "AssemblyVersion(";
            var fv = "AssemblyFileVersion(";
            var iv = "AssemblyInformationalVersion(";

            var newVersion = $"\"{version}\"";

            var index1 = text.IndexOf(v);
            if (index1 == -1)
            {
                throw new NotSupportedException($"Could not find required text '{v}' in assemblyInfo file '{assemblyInfo}'");
            }
            string front;
            int endIndex;
            string end;
            text = ReplaceVersion(text, index1, v, newVersion);

            var index2 = text.IndexOf(fv);
            // now do the same for the two optional values
            if (index2 != -1)
            {
                front = text.Substring(0, index2 + fv.Length);
                endIndex = text.IndexOf(')', index2 + fv.Length);
                if (endIndex == -1)
                {
                    throw new NotSupportedException($"Missing closing bracket ')' after {fv}");
                }
                end = text.Substring(endIndex);
                text = front + newVersion + end;
            }
            var index3 = text.IndexOf(iv);
            if (index3 != -1)
            {
                front = text.Substring(0, index3 + iv.Length);
                endIndex = text.IndexOf(')', index3 + iv.Length);
                if (endIndex == -1)
                {
                    throw new NotSupportedException($"Missing closing bracket ')' after {iv}");
                }
                end = text.Substring(endIndex);
                text = front + newVersion + end;
            }

            File.WriteAllText(assemblyInfo, text);
        }

        /// <summary>
        /// Replaces the existing version string inside fullText with the new version.
        /// Expects a ')' to be found after the identifier somewhere and expects the content to be replaced to be a valid version of  "1.0.0.0" or similar (including "").
        /// </summary>
        /// <param name="fulllText">The text in which to replace.</param>
        /// <param name="startIndex">The index at which the identifier was found.</param>
        /// <param name="identifier">The identifier itself.</param>
        /// <param name="newVersion">The new version.</param>
        /// <returns></returns>
        private static string ReplaceVersion(string fulllText, int startIndex, string identifier, string newVersion)
        {
            var front = fulllText.Substring(0, startIndex + identifier.Length);
            // find closing bracket
            var endIndex = fulllText.IndexOf(')', startIndex + identifier.Length);
            if (endIndex == -1)
            {
                throw new NotSupportedException($"Missing closing bracket ')' after {identifier}");
            }
            // before blindly replacing stuff, assert that we are infact only replacing the version
            var content = fulllText.Substring(startIndex + identifier.Length, endIndex - (startIndex + identifier.Length));
            if (!content.StartsWith("\"") || !content.EndsWith("\""))
            {
                throw new NotSupportedException($"When replacing version, the existing version must already be valid. Expected something like \"1.0.0.0\" (including \"\") but found: {content}");
            }
            var oldVersion = content.Substring(1, content.Length - 2);
            if (!Version.TryParse(oldVersion, out Version x))
            {
                throw new NotSupportedException($"Tried to read old version first, but found invalid version: '{x}' for {identifier}");
            }
            var end = fulllText.Substring(endIndex);
            fulllText = front + newVersion + end;
            return fulllText;
        }
    }
}