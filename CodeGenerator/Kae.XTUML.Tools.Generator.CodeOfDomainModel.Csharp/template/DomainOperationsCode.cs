// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Tools.Generator.Coloring;
using System;
using System.Collections.Generic;
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
        ColoringManager coloringManager;

        public DomainOperations(string version, string nameSpace, string domainFacadeClassName, IEnumerable<CIClassDef> syncDefs, ColoringManager coloringManager)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.domainFacadeClassName = domainFacadeClassName;
            this.syncClassDefs = syncDefs;
            this.coloringManager = coloringManager;
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
                var actDescripGen = new ActDescripGenerator(actDef, "target", "    ", "        ", coloringManager);
                string code = actDescripGen.Generate();
            }
        }

        public void prototype()
        {
            foreach(var syncClassDef in syncClassDefs)
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
        }
    }
}
