﻿// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Tools.Generator;
using Kae.Tools.Generator.Context;
using Kae.Tools.Generator.utility;
using Kae.Utility.Logging;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.adaptor;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp
{
    public class CsharpCodeGenerator : GeneratorBase
    {
        public static readonly string CPKeyProjectName = "project-name";
        public static readonly string CPKeyDotNetVersion = "dotnet-ver";
        public static readonly string CPKeyOverWrite = "overwrite";
        public static readonly string CPKeyActionGen = "action-gen";
        public static readonly string CPKeyBackup = "backup";
        public static readonly string CPKeyAdaptorGen = "adoptor-gen";

        public CsharpCodeGenerator(Logger logger, string version) : base(logger, version)
        {

        }

        private string ProjectName;
        private string DotNetVersion;
        private bool IsOverWriteActionFile = true;
        private bool IsGenCode = true;
        private bool IsBackup = true;
        private bool isAdaptorGen = false;
        private GenFolder.WriteMode behaviorFileWriteMode = GenFolder.WriteMode.Overwrite;

        protected override void CreateAdditionalContext()
        {
            var genContext = GetContext();
            var cpProjItem = new StringParam(CPKeyProjectName);
            genContext.AddOption(cpProjItem);
            var cpDNetVItem = new StringParam(CPKeyDotNetVersion);
            genContext.AddOption(cpDNetVItem);
            var cpOverWriteItem = new BooleanParam(CPKeyOverWrite);
            genContext.AddOption(cpOverWriteItem);
            var cpActionGen = new BooleanParam(CPKeyActionGen);
            genContext.AddOption(cpActionGen);
            var cpBackup = new BooleanParam(CPKeyBackup);
            genContext.AddOption(cpBackup);
            var cpAdaptorGen = new BooleanParam(CPKeyAdaptorGen);
            genContext.AddOption(cpAdaptorGen);
        }

        protected override bool AdditionalWorkForResloveContext()
        {
            int index = 0;
            foreach (var cp in GetContext().Options)
            {
                if (cp.ParamName == CPKeyProjectName)
                {
                    ProjectName = ((StringParam)cp).Value;
                    index++;
                }
                else if (cp.ParamName == CPKeyDotNetVersion)
                {
                    DotNetVersion = ((StringParam)cp).Value;
                    index++;
                }
                else if (cp.ParamName == CPKeyOverWrite)
                {
                    IsOverWriteActionFile = ((BooleanParam)cp).Value;
                }
                else if (cp.ParamName == CPKeyActionGen)
                {
                    IsGenCode = ((BooleanParam)cp).Value;
                }
                else if (cp.ParamName == CPKeyBackup)
                {
                    IsBackup = ((BooleanParam)cp).Value;
                }
                else if (cp.ParamName == CPKeyAdaptorGen)
                {
                    isAdaptorGen = ((BooleanParam)cp).Value;
                }
            }
            if (IsGenCode)
            {
                IsOverWriteActionFile = true;
                if (IsBackup)
                {
                    behaviorFileWriteMode = GenFolder.WriteMode.Backup;
                }
            }
            if (IsOverWriteActionFile == false)
            {
                behaviorFileWriteMode = GenFolder.WriteMode.None;
            }
            if (index == 2)
            {
                return true;
            }
            return false;
        }

        protected override bool AdditionalWorkForMetaModel()
        {
            // Do Nothing
            return true;
        }

        protected override bool AdditionalWorkForDomainModels()
        {
            // Do Nothing
            return true;
        }

        protected override bool GenerateEnvironmentYourOwn()
        {
            string projectPath = ProjectName;
            genFolder.CreateFolder(projectPath);
            var projectFile = new ProjectFile(Version, projectPath, DotNetVersion, new List<ProjectFile.Library>()
            { new ProjectFile.Library() { Name = "Kae.StateMachine", Version = "0.3.0" },
              new ProjectFile.Library() { Name = "Kae.Utility.Logging", Version = "1.0.0"},
              new ProjectFile.Library(){ Name = "Kae.DomainModel.Csharp.Framework", Version="5.1.0"},
              new ProjectFile.Library() { Name = "Newtonsoft.Json", Version="13.0.1" }
            });

            var extPackages = this.coloringManager.GetExternalPackages();
            foreach(var extPackage in extPackages)
            {
                projectFile.AddLibrary(extPackage);
            }

            var projectFileCode = projectFile.TransformText();
            string fileName = $"{ProjectName}.csproj";
            genFolder.WriteContentAsync(projectPath, fileName, projectFileCode, GenFolder.WriteMode.Overwrite).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");
            

            return true;
        }

        protected override void GenerateContentsYourOwn()
        {
            string projectPath = ProjectName;
            bool overwriteHandCodingFiles = IsOverWriteActionFile;

            var modelRepository = this.modelResolver.ModelRepository;

            var classObjDefs = modelRepository.GetCIInstances(CIMOOAofOOADomainName, "O_OBJ");
            var instanceRepository = new InstanceRepository(Version, ProjectName, classObjDefs);
            var instanceRepositoryCode = instanceRepository.TransformText();
            string fileName = "InstanceRepositoryInMemory.cs";
            genFolder.WriteContentAsync(projectPath, fileName, instanceRepositoryCode, GenFolder.WriteMode.Overwrite).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");

            var classDtDefs = modelRepository.GetCIInstances(CIMOOAofOOADomainName, "S_DT");
            var domainDataTypeDefs = new DomainDataTypeDefs(Version, ProjectName, classDtDefs);
            var domainDataTypeDefsCode = domainDataTypeDefs.TransformText();
            fileName = "DomainDataTypes.cs";
            genFolder.WriteContentAsync(projectPath, fileName, domainDataTypeDefsCode, GenFolder.WriteMode.Overwrite).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");

            var classRelDefs = modelRepository.GetCIInstances(CIMOOAofOOADomainName, "R_REL");
            var superTypeDefs = new SuperTypeDefs(ProjectName, Version, classRelDefs);
            //superTypeDefs.prototype();
            var superTypeDefsCode = superTypeDefs.TransformText();
            fileName = "SubClassDefs.cs";
            genFolder.WriteContentAsync(projectPath, fileName, superTypeDefsCode, GenFolder.WriteMode.Overwrite).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");

            var domainClassDefs = new DomainClassDefs(Version, ProjectName, classObjDefs, ColoringManagerForDomainWeaving);
            //domainClassDefs.prototype();
            var domainClassDefsCode = domainClassDefs.TransformText();
            fileName = "DomainClassDefs.cs";
            genFolder.WriteContentAsync(projectPath, fileName, domainClassDefsCode, GenFolder.WriteMode.Overwrite).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");

            foreach (var classObjDef in classObjDefs)
            {
                var objDef = (CIMClassO_OBJ)classObjDef;
                var domainClassBase = new DomainClassBase(Version, ProjectName, objDef, ColoringManagerForDomainWeaving);
                var domainClassBaseCode = domainClassBase.TransformText();
                fileName = $"DomainClass{objDef.Attr_Key_Lett}Base.cs";
                genFolder.WriteContentAsync(projectPath, fileName, domainClassBaseCode, GenFolder.WriteMode.Overwrite).Wait();
                Console.WriteLine($"Generated - {fileName}");
                logger.LogInfo($"Generated - {fileName}");

                var tfrDefs = objDef.LinkedFromR115();
                bool hasOperations = false;
                foreach (var tfrDef in tfrDefs)
                {
                    hasOperations = true;
                    break;
                }
                if (hasOperations)
                {
                    var domainClassOperations = new DomainClassOperations(Version, ProjectName, objDef, ColoringManagerForDomainWeaving);
                    //domainClassOperations.prototype();
                    //domainClassOperations.prototypeAct();
                    var domainClassOperationsCode = domainClassOperations.TransformText();

                    fileName = $"DomainClass{objDef.Attr_Key_Lett}BaseOperations.cs";
                    genFolder.WriteContentAsync(projectPath, fileName, domainClassOperationsCode, behaviorFileWriteMode).Wait();
                    Console.WriteLine($"Generated - {fileName}");
                    logger.LogInfo($"Generated - {fileName}");
                }

                CIMClassSM_SM smDef = null;
                var ismDef = objDef.LinkedFromR518();
                if (ismDef != null)
                {
                    smDef = ismDef.CIMSuperClassSM_SM();
                }
                // To support Class State Machine,
                // use asmDef by objDef.LinkedFromR519();
                if (smDef != null)
                {
                    var domainClassStateMachine = new DomainClassStateMachine(Version, ProjectName, objDef, smDef);
                    var domainClassStateMachineCode = domainClassStateMachine.TransformText();
                    fileName = $"DomainClass{objDef.Attr_Key_Lett}StateMachine.cs";
                    genFolder.WriteContentAsync(projectPath, fileName, domainClassStateMachineCode, GenFolder.WriteMode.Overwrite).Wait();
                    // domainClassStateMachine.prototype();
                    Console.WriteLine($"Generated - {fileName}");
                    logger.LogInfo($"Generated - {fileName}");

                    var domainClassActions = new DomainClassActions(Version, ProjectName, objDef, smDef, IsGenCode, ColoringManagerForDomainWeaving);
                    // domainClassActions.prototype();
                    // domainClassActions.prototypeAction();
                    var domainClassActionsCode = domainClassActions.TransformText();
                    fileName = $"DomainClass{objDef.Attr_Key_Lett}StateMachineActions.cs";
                    genFolder.WriteContentAsync(projectPath, fileName, domainClassActionsCode, behaviorFileWriteMode).Wait();
                    Console.WriteLine($"Generated - {fileName}");
                    logger.LogInfo($"Generated - {fileName}");
                }
            }
            string domainFacadeClassName = GeneratorNames.GetDomainFacadeClassName(ProjectName);
            var domainFacade = new DomainFacade(Version, ProjectName, domainFacadeClassName);
            var domainFacadeCode = domainFacade.TransformText();
            fileName = $"{domainFacadeClassName}.cs";
            genFolder.WriteContentAsync(projectPath, fileName, domainFacadeCode, GenFolder.WriteMode.Overwrite).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");

            var syncDefs = modelRepository.GetCIInstances(CIMOOAofOOADomainName, "S_SYNC");
            var domainOperations = new DomainOperations(Version, ProjectName, domainFacadeClassName, syncDefs, ColoringManagerForDomainWeaving);
            var domainOperationsCode = domainOperations.TransformText();
            // domainOperations.prototypeAction();
            fileName = $"{domainFacadeClassName}Operations.cs";
            genFolder.WriteContentAsync(projectPath, fileName, domainOperationsCode, behaviorFileWriteMode).Wait();
            Console.WriteLine($"Generated - {fileName}");
            logger.LogInfo($"Generated - {fileName}");

            string eeDefFolderName = Path.Join(projectPath, ExternalEntityDef.GetFolderName());
            genFolder.CreateFolder(eeDefFolderName);
            var eeDefs = modelRepository.GetCIInstances(CIMOOAofOOADomainName, "S_EE");
            foreach (var eeCIDef in eeDefs)
            {
                var eeDef = (CIMClassS_EE)eeCIDef;
                if (!(eeDef.Attr_Key_Lett == "TIM" && eeDef.Attr_Name == "Time"))
                {
                    var eeDefGen = new ExternalEntityDef(Version, ProjectName, eeDef);
                    //eeDefGen.prototype();
                    string eeDefGenCode = eeDefGen.TransformText();
                    fileName = $"{GeneratorNames.GetExternalEntityWrappterClassName(eeDef, false)}.cs";
                    genFolder.WriteContentAsync(eeDefFolderName, fileName, eeDefGenCode, GenFolder.WriteMode.Overwrite).Wait();
                    Console.WriteLine($"Generated - {fileName}");
                    logger.LogInfo($"Generated - {fileName}");
                }
#if false
                var brgDefs = eeDef.LinkedFromR19();
                foreach(var brgDef in brgDefs)
                {
                    string brgName = brgDef.Attr_Name;
                    var bparmDefs = brgDef.LinkedFromR21();
                    var brgRetDtDef = brgDef.LinkedToR20();
                    string brgRetDtName = DomainDataTypeDefs.GetDataTypeName(brgRetDtDef);
                    foreach(var bparmDef in bparmDefs)
                    {
                        string bparmName = bparmDef.Attr_Name;
                        var parmDtDef = bparmDef.LinkedToR22();
                        string parmDtName = DomainDataTypeDefs.GetDataTypeName}(parmDtDef);
                    }
                }
#endif
            }

            if (isAdaptorGen)
            {
                var adaptorGen = new AdaptorDef(Version, ProjectName, "    ", syncDefs, classObjDefs);
                string adaptorGenCode = adaptorGen.TransformText();
                string adaptorFolderName = Path.Join(projectPath,AdaptorDef.GetFolderName());
                genFolder.CreateFolder(adaptorFolderName);
                string adaptorFileName = $"{ProjectName}Adaptor.cs";
                genFolder.WriteContentAsync(adaptorFolderName, adaptorFileName, adaptorGenCode, GenFolder.WriteMode.Overwrite).Wait();
                Console.WriteLine($"Generated - {adaptorFileName}");
                logger.LogInfo($"Generated - {fileName}");
            }
        }

    }
}
