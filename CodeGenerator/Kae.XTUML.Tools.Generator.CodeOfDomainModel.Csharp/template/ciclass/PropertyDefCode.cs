// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Tools.Generator.Coloring;
using Kae.Tools.Generator.Coloring.DomainWeaving;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.ciclass
{
    partial class PropertyDef
    {
        private string indent;
        private CIMClassO_OBJ objDef;
        private bool genImplCode;
        IDictionary<string, CIMClassS_EE> usedExternalEntities;
        private ColoringManager coloringManager;
        private Logger logger;

        public PropertyDef(string indent, CIMClassO_OBJ objDef, bool genImplCode, IDictionary<string, CIMClassS_EE> usedEEs, ColoringManager colringManager, Logger logger)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genImplCode = genImplCode;
            this.usedExternalEntities = usedEEs;
            this.coloringManager = colringManager;
            this.logger = logger;
        }

        public void prototypeAction()
        {
            var attrDefs = objDef.LinkedFromR102();
            foreach(var attrDef in attrDefs)
            {
                var subAttrDef = attrDef.SubClassR106();
                if (subAttrDef is CIMClassO_BATTR)
                {
                    var subBattrDef = ((CIMClassO_BATTR)subAttrDef).SubClassR107();
                    if (subBattrDef is CIMClassO_DBATTR)
                    {
                        var dbattrDef = (CIMClassO_DBATTR)subBattrDef;
                        var dabDef = dbattrDef.LinkedFromR693();
                        if (dabDef != null)
                        {
                            string actionDescrip = dbattrDef.Attr_Action_Semantics;
                            var actDef = dabDef.CIMSuperClassACT_ACT();
                            var actGen = new ActDescripGenerator(actDef, "this", "", "", usedExternalEntities, coloringManager, logger);
                            string actGenCode = actGen.Generate();
                        }
                    }
                }
                var attrDtDef = DomainDataTypeDefs.GetBaseDT(attrDef);
                var subDtDef = attrDtDef.SubClassR17();
                if (subDtDef is CIMClassS_SDT)
                {
                    
                }
            }
        }
    }

    public class AttributeDef
    {
        public CIMClassO_ATTR AttrDef { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsReferential { get; set; }
        public string DataTypeName { get; set; }
        public bool IsUniqueId { get; set; }
        public bool IsState { get; set; }
        public CIMClassACT_ACT ActDef { get; set; }
        public string ActionSemantics { get; set; }
        public bool Writable { get; set; } = true;
        public bool IsStructured { get; set; }
    }

}
