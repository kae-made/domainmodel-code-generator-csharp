// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Tools.Generator.Context;
using Kae.Utility.Logging;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

        private static readonly string version = "0.1.0";
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
            var contextParams = generator.ContextParams;
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
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyOOAofOOAModelFilePath).First();
                    requiredOptions.Remove(args[index]);
                    ((PathSelectionParam)cp).Path = args[++index];
                }
                else if (args[index] == "--meta-datatype")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyMetaDataTypeDefFilePath).First();
                    ((PathSelectionParam)cp).Path = args[++index];
                }
                else if (args[index] == "--base-datatype")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyBaseDataTypeDefFilePaht).First();
                    requiredOptions.Remove(args[index]);
                    ((PathSelectionParam)cp).Path = args[++index];
                }
                else if (args[index] == "--domainmodel")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyDomainModelFilePath).First();
                    requiredOptions.Remove(args[index]);
                    ((PathSelectionParam)cp).Path = args[++index];
                    // domainModelFilePath = args[index];
                }
                else if (args[index] == "--project")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyProjectName).First();
                    requiredOptions.Remove(args[index]);
                    ((StringParam)cp).Value = args[++index];
                }
                else if (args[index] == "--dotnetver")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyDotNetVersion).First();
                    requiredOptions.Remove(args[index]);
                    ((StringParam)cp).Value = args[++index];
                }
                else if (args[index] == "--gen-folder")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyGenFolderPath).First();
                    requiredOptions.Remove(args[index]);
                    ((PathSelectionParam)cp).Path = args[++index];
                }
                else if (args[index] == "--colors")
                {
                    colorsFileName = args[++index];
                }
                else if (args[index] == "--action-gen")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyActionGen).First();
                    ((BooleanParam)cp).Value = bool.Parse(args[++index]);
                }
                else if (args[index] == "--overwrite")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyOverWrite).First();
                    ((BooleanParam)cp).Value = bool.Parse(args[++index]);
                }
                else if (args[index] == "--backup")
                {
                    var cp = contextParams.Where(c => c.ParamName == CsharpCodeGenerator.CPKeyBackup).First();
                    ((BooleanParam)cp).Value = bool.Parse(args[++index]);
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
