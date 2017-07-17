using CommandLineParser.Arguments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var arg = new FileArgument('i', "inputFile")
            {
                FileMustExist = true
            };
            var parser = new CommandLineParser.CommandLineParser
            {
                AcceptEqualSignSyntaxForValueArguments = true,
                AcceptHyphen = true,
                AcceptSlash = true
            };
            parser.Arguments.Add(arg);

            try
            {
                parser.ParseCommandLine(args);
                if (!arg.Parsed)
                    throw new NotSupportedException("No argument provided.");

                var icons = Collect(arg.Value.FullName).ToList();
                Generate(icons);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process was aborted mid-way. Some icons may have been generated.");
                Console.WriteLine(e.Message);
                return;
            }
            Console.WriteLine("All done!");
        }

        private static void Generate(List<Icon> icons)
        {
            foreach (var ico in icons)
            {
                ico.Generate();
            }
        }

        private static IEnumerable<Icon> Collect(string inputFile)
        {
            var fileDir = new FileInfo(inputFile).DirectoryName;
            var lines = File.ReadAllLines(inputFile);

            var variables = lines.TakeWhile(l => l != "---").ToArray();
            var transforms = lines.SkipWhile(l => l != "---").Skip(1).ToArray();

            var vars = new Dictionary<string, string>();
            foreach (var v in variables)
            {
                var idx = v.IndexOf('=');
                if (idx == -1)
                    throw new NotSupportedException("Key/Value pair does not contain required char '='.");
                var name = v.Substring(0, idx);
                var value = v.Substring(idx + 1);
                name = $"%{name}%";
                if (vars.ContainsKey(name))
                    throw new NotSupportedException($"Variable '{name}' already declared.");

                vars.Add(name, value);
            }

            foreach (var transform in transforms)
            {
                if (transform.StartsWith("#"))
                    continue;

                var line = transform.Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (!line.Contains("->"))
                    throw new NotSupportedException($"Missing '->' in line '{line}'.");

                var split = line.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                    throw new NotSupportedException($"More than one '->' in line '{line}'.");

                var input = split[0];
                var output = split[1];

                string[] inputFiles;
                if (input.Contains("*"))
                {
                    var idx = input.IndexOf('*');
                    var dir = Path.Combine(fileDir, input.Substring(0, idx));
                    var filter = input.Substring(idx);
                    inputFiles = Directory.GetFiles(dir, filter);
                }
                else
                {
                    inputFiles = new[] { Path.Combine(fileDir, input) };
                }

                var lastBracket = output.LastIndexOf('(');
                var path = Path.Combine(fileDir, output.Substring(0, lastBracket).Trim());
                var size = output.Substring(lastBracket + 1);
                size = size.Substring(0, size.IndexOf(')'));
                var spl = size.Split('x');
                var w = int.Parse(spl[0]);
                var h = int.Parse(spl[1]);

                foreach (var v in vars)
                {
                    path = path.Replace(v.Key, v.Value);
                }

                bool addName = false;
                if (inputFiles.Length > 1)
                {
                    if (!path.EndsWith("*"))
                        throw new NotSupportedException("Output path must end with '*' when input path contains '*'.");
                    path = path.Substring(0, path.Length - 1);
                    addName = true;
                }

                foreach (var f in inputFiles)
                {
                    if (!File.Exists(f))
                        throw new NotSupportedException($"Input file '{f}' not found.");

                    var name = Path.GetFileName(f);
                    yield return new Icon(f, addName ? path + name : path, w, h);
                }
            }
        }
    }
}