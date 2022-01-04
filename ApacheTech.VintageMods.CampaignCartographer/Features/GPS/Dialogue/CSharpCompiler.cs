using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Dialogue
{
    public sealed class CSharpCompiler
    {
        private readonly List<string> _references;

        public CSharpCompiler()
        {
            _references = new List<string>
            {
                "System.dll",
                "System.Core.dll",
                "System.Data.dll",
                "System.Drawing.dll",
                "System.Xml.dll",
                "System.Xml.Linq.dll",
                "System.Net.Http.dll",

                "0Harmony.dll",
                "AnimatedGif.dll",
                "cairo-sharp.dll",
                "Newtonsoft.Json.dll",
                "OpenTK.dll",
                "protobuf-net.dll",
                "Tavis.JsonPatch.dll",

                "Vintagestory.exe",
                "VintagestoryAPI.dll",
                "VintagestoryLib.dll",
                Path.Combine("Mods", "VSCreativeMod.dll"),
                Path.Combine("Mods", "VSEssentials.dll"),
                Path.Combine("Mods", "VSSurvivalMod.dll")
            };
        }

        private CSharpCodeProvider Compiler { get; } = new (new Dictionary<string, string>
        {
            {
                "CompilerVersion",
                "v4.0"
            }
        });

        public Assembly CompileFromFiles(ModContainer mod, IEnumerable<string> files)
        {
            return BuildAssembly(mod, GenerateCodeFromFile(files));
        }

        public Assembly CompileFromFile(ModContainer mod, FileInfo file)
        {
            return BuildAssembly(mod, GenerateCodeFromFile(new[] { file.FullName }));
        }

        public Assembly CompileFromSource(ModContainer mod, string source)
        {
            return BuildAssembly(mod, GenerateCodeFromSource(new[] { source }));
        }

        private static Assembly BuildAssembly(ModContainer mod, CompilerResults compilerResults)
        {
            foreach (var grouping in compilerResults.Errors.Cast<CompilerError>().GroupBy(error => !error.IsWarning ? EnumLogType.Error : EnumLogType.Warning))
            {
                mod.Logger.Log(grouping.Key, "Compiler " + grouping.Key.ToString().ToLowerInvariant() + "s during compilation:");
                foreach (var compilerError in grouping)
                {
                    var text = string.IsNullOrEmpty(compilerError.FileName) ? ""
                        : $"{compilerError.FileName.Substring(mod.FolderPath.Length + 1)}({compilerError.Line}): ";
                    mod.Logger.Log(grouping.Key, $"    {text}{compilerError.ErrorText} [{compilerError.ErrorNumber}]");
                }
            }
            if (compilerResults.Errors.HasErrors)
            {
                throw new Exception("Could not compile from source files due to errors");
            }
            return compilerResults.CompiledAssembly;
        }

        private CompilerResults GenerateCodeFromFile(IEnumerable<string> paths)
        {
            return Compiler.CompileAssemblyFromFile(GetCompilerParameters(), paths.ToArray());
        }

        private CompilerResults GenerateCodeFromSource(IEnumerable<string> sources)
        {
            return Compiler.CompileAssemblyFromSource(GetCompilerParameters(), sources.ToArray());
        }

        private CompilerParameters GetCompilerParameters()
        {
            var compilerParameters = new CompilerParameters
            {
                CompilerOptions = "/unsafe",
                GenerateExecutable = false,
                TreatWarningsAsErrors = false,
                IncludeDebugInformation = true,
                GenerateInMemory = true,
                TempFiles = new TempFileCollection(Path.GetTempPath(), false)
                {
                    KeepFiles = true
                }
            };
            FixReferences();
            compilerParameters.ReferencedAssemblies.AddRange(_references.ToArray());
            return compilerParameters;
        }

        private void FixReferences()
        {
            for (var i = 0; i < _references.Count; i++)
            {
                if (_references[i].StartsWith("System.")) continue;
                if (File.Exists(Path.Combine(GamePaths.Binaries, "Lib", _references[i])))
                {
                    _references[i] = Path.Combine(GamePaths.Binaries, "Lib", _references[i]);
                    continue;
                }

                if (!File.Exists(Path.Combine(GamePaths.Binaries, _references[i])))
                {
                    throw new Exception("Referenced library not found: " + _references[i]);
                }

                _references[i] = Path.Combine(GamePaths.Binaries, _references[i]);
            }
        }
    }
}