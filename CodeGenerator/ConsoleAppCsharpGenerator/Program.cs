// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Tools.Generator;
using Kae.Tools.Generator.Context;
using Kae.Utility.Logging;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleAppCsharpGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            if (p.ResolveArgs(args))
            {
                p.Work();
            }
        }

        private static readonly string version = "1.0.0";
        CsharpCodeGenerator generator;
        public Program()
        {
            generator = new CsharpCodeGenerator(ConsoleLogger.CreateLogger(), version);
        }

        private void Work()
        {
            generator.ResolveContext();
            generator.LoadMetaModel();
            generator.LoadDomainModels();
            generator.GenerateEnvironment();
            generator.GenerateContents();
        }

        private string colorsFileName;
        public bool ResolveArgs(string[] args)
        {
            var genContext = generator.GetContext();
            if (args.Length == 0)
            {
                // ShowCommandline();
                return false;
            }

            var requiredOptions = new List<string>() { "--metamodel", "--base-datatype", "--domainmodel", "--project", "--dotnetver", "--gen-folder" };
            int index = 0;
            while (index < args.Length)
            {
                if (args[index] == "--metamodel")
                {
                    requiredOptions.Remove(args[index]);
                    genContext.SetOptionValue(GeneratorBase.CPKeyOOAofOOAModelFilePath, (args[++index], false));
                }
                else if (args[index] == "--meta-datatype")
                {
                    genContext.SetOptionValue(GeneratorBase.CPKeyMetaDataTypeDefFilePath, (args[++index], false));
                }
                else if (args[index] == "--base-datatype")
                {
                    requiredOptions.Remove(args[index]);
                    genContext.SetOptionValue(GeneratorBase.CPKeyBaseDataTypeDefFilePaht, (args[++index], false));
                }
                else if (args[index] == "--domainmodel")
                {
                    requiredOptions.Remove(args[index]);
                    string domainModelPath = args[++index];
                    genContext.SetOptionValue(GeneratorBase.CPKeyDomainModelFilePath, (domainModelPath, !File.Exists(domainModelPath)));
                }
                else if (args[index] == "--project")
                {
                    requiredOptions.Remove(args[index]);
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyProjectName, args[++index]);
                }
                else if (args[index] == "--dotnetver")
                {
                    requiredOptions.Remove(args[index]);
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyDotNetVersion, args[++index]);
                }
                else if (args[index] == "--gen-folder")
                {
                    requiredOptions.Remove(args[index]);
                    genContext.SetOptionValue(GeneratorBase.CPKeyGenFolderPath, (args[++index], true));
                }
                else if (args[index] == "--colors")
                {
                    genContext.SetOptionValue(GeneratorBase.CPKeyColoringFilePath, (args[++index], false));
                }
                else if (args[index] == "--action-gen")
                {
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyActionGen, bool.Parse(args[++index]));
                }
                else if (args[index] == "--overwrite")
                {
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyOverWrite, bool.Parse(args[++index]));
                }
                else if (args[index] == "--backup")
                {
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyBackup, bool.Parse(args[++index]));
                }
                else if (args[index] == "--adoptor-gen")
                {
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyAdaptorGen, bool.Parse(args[++index]));
                }
                else if (args[index] == "--azuredigitaltwins")
                {
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyAzureDigitalTwins, args[++index]);
                }
                else if (args[index]=="--azure-iot-hub")
                {
                    genContext.SetOptionValue(CsharpCodeGenerator.CPKeyAzureIoTHub, bool.Parse(args[++index]));
                }
                else
                {
                    // ShowCommandline();
                }
                index++;
            }
            if (requiredOptions.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
