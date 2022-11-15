// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Tools.Generator.Coloring;
using Kae.Tools.Generator.Coloring.DomainWeaving;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class DomainOperations
    {
        string version;
        string nameSpace;
        string domainFacadeClassName;
        IEnumerable<CIClassDef> syncClassDefs;
        IDictionary<string, CIMClassS_EE> usedExternalEntities;
        ColoringManager coloringManager;
        bool isAzureIoTHub;
        Logger logger;

        public DomainOperations(string version, string nameSpace, string domainFacadeClassName, IEnumerable<CIClassDef> syncDefs, IDictionary<string, CIMClassS_EE> usedEEs, ColoringManager coloringManager, bool azureIoTHub, Logger logger)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.domainFacadeClassName = domainFacadeClassName;
            this.syncClassDefs = syncDefs;
            this.usedExternalEntities = usedEEs;
            this.coloringManager = coloringManager;
            this.isAzureIoTHub = azureIoTHub;
            this.logger = logger;
        }

        public void prototypeAction()
        {
            foreach (var syncClassDef in syncClassDefs)
            {
                var syncDef = (CIMClassS_SYNC)syncClassDef;
                var fnbDef = syncDef.LinkedFromR695();
                var actDef = fnbDef.CIMSuperClassACT_ACT();
                var retDtDef = syncDef.LinkedToR25();
                if (retDtDef.Attr_Name == "void") ;
                string syncName = syncDef.Attr_Name;
                var actDescripGen = new ActDescripGenerator(actDef, "target", "    ", "        ", usedExternalEntities, coloringManager, isAzureIoTHub, logger);
                string code = actDescripGen.Generate();
            }
        }

        public static (string New, string Namespace) GetExternalEntityConstructorName(CIMClassS_EE eeDef)
        {
            string constructorName = "";
            string nameSpace = "";
            string eeKeyLetter = eeDef.Attr_Key_Lett;
            var marked = ExternalEntityDef.GetColorMark(eeDef);
            if (marked.ContainsKey("constructor"))
            {
                constructorName = marked["constructor"]["new"];
                nameSpace = marked["constructor"]["namespace"];
            }
            else
            {
                if (eeDef.Attr_Key_Lett == "TIM")
                {
                    constructorName = "Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.impl.TIMImpl";
                    nameSpace = "Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM";
                }
                else if (eeDef.Attr_Key_Lett == "STR")
                {
                    constructorName = "Kae.DomainModel.Csharp.Framework.ExternalEntity.STR.STRImpl";
                    nameSpace = "Kae.DomainModel.Csharp.Framework.ExternalEntity.STR";
                }
                else if (eeDef.Attr_Key_Lett == "RND")
                {
                    constructorName = "Kae.DomainModel.Csharp.Framework.ExternalEntity.RND.RNDImpl";
                    nameSpace = "Kae.DomainModel.Csharp.Framework.ExternalEntity.RND";
                }
            }
            return (constructorName, nameSpace);
        }

        public void prototype()
        {
            foreach (var syncClassDef in syncClassDefs)
            {
                var syncDef = (CIMClassS_SYNC)syncClassDefs;
                var retDtDef = syncDef.LinkedToR25();
                string retDTName = DomainDataTypeDefs.GetDataTypeName(retDtDef);
                var sparmDefs = syncDef.LinkedFromR24();
                string actionSemantics = GeneratorNames.DescripToCodeComment("", syncDef.Attr_Action_Semantics);
                string methodName = syncDef.Attr_Name;
                string args = "";
                foreach (var sparmDef in sparmDefs)
                {
                    var pDtDef = sparmDef.LinkedToR26();
                    var pDTName = DomainDataTypeDefs.GetDataTypeName(pDtDef);
                    if (!string.IsNullOrEmpty(args))
                    {
                        args += ", ";
                    }
                    args += $"{pDTName} {sparmDef.Attr_Name}";
                }
            }

            foreach(var eeKey in usedExternalEntities.Keys)
            {
                var eeDef = usedExternalEntities[eeKey];
                string eeKeyLetter = eeDef.Attr_Key_Lett;
                using (var reader = new StringReader(eeDef.Attr_Descrip))
                {
                    string readline = null;
                    while ((readline = reader.ReadLine()) != null)
                    {
                        if (readline.StartsWith("@constructor"))
                        {
                            string constructorName = readline.Substring(readline.IndexOf("("));
                            constructorName = constructorName.Substring(0, constructorName.Length - 1);
                        }
                    }
                }
            }
        }
    }
}
