// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Tools.Generator.Coloring;
using Kae.Tools.Generator.Coloring.DomainWeaving;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.DomainClassBase;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.ciclass
{
    partial class PropertyDef
    {
        private string indent;
        private CIMClassO_OBJ objDef;
        private bool genImplCode;
        IDictionary<string, CIMClassS_EE> usedExternalEntities;
        private ColoringManager coloringManager;
        private bool isAzureDigitalTwins;
        private bool isAzureIoTHub;
        private Logger logger;

        public static void ResolveIoTHubColoring(AttributeDef attributeDef)
        {
            attributeDef.IsDesiredProperty = false;
            var objDef = attributeDef.AttrDef.LinkedToR102();
            if (objDef.Attr_Descrip.StartsWith("@iotpnp"))
            {
                attributeDef.IsDesiredProperty = true;
                var attrDef = attributeDef.AttrDef;
                string descrip = attrDef.Attr_Descrip;
                string colorKey = "iotpnp";
                int startPos = descrip.IndexOf($"@{colorKey}");
                if (startPos >= 0)
                {
                    string part = descrip.Substring(startPos);
                    int lpPos = part.IndexOf("(");
                    int rpPos = part.IndexOf(")");
                    if (lpPos >= 0 && rpPos >= 0 && (rpPos - lpPos) > 2)
                    {
                        part = part.Substring(lpPos + 1, rpPos - lpPos - 1);
                        var colors = part.Split(new char[] { ',' });
                        foreach (var c in colors)
                        {
                            switch (c)
                            {
                                case "deviceid":
                                case "readonly":
                                case "exclude":
                                case "telemetry":
                                    attributeDef.IsDesiredProperty = false;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("iotpnp coloring should be '@iotpnp(item,item,...)'. item should be 'deviceid'|'readonly'|'exclude'|'telemetry' ");
                            }
                        }
                    }
                }
            }
        }

        public static string GetAttrNameForDeviceId(CIMClassO_OBJ objDef)
        {
            string attrNameForDeviceId = "";
            var attrDefs = objDef.LinkedFromR102();
            foreach (var attrDef in attrDefs)
            {
                string descrip = attrDef.Attr_Descrip;
                string colorKey = "iotpnp";
                int startPos = descrip.IndexOf($"@{colorKey}");
                if (startPos >= 0)
                {
                    string part = descrip.Substring(startPos);
                    int lpPos = part.IndexOf("(");
                    int rpPos = part.IndexOf(")");
                    if (lpPos >= 0 && rpPos >= 0 && (rpPos - lpPos) > 2)
                    {
                        part = part.Substring(lpPos + 1, rpPos - lpPos - 1);
                        var colors = part.Split(new char[] { ',' });
                        foreach (var c in colors)
                        {
                            if (c == "deviceid")
                            {
                                attrNameForDeviceId = GeneratorNames.GetAttrPropertyName(attrDef);
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(attrNameForDeviceId))
                    {
                        break;
                    }
                }
            }
            return attrNameForDeviceId;
        }

        public PropertyDef(string indent, CIMClassO_OBJ objDef, bool genImplCode, IDictionary<string, CIMClassS_EE> usedEEs, ColoringManager colringManager, bool azureDigitalTwins, bool azureIoTHub, Logger logger)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genImplCode = genImplCode;
            this.usedExternalEntities = usedEEs;
            this.coloringManager = colringManager;
            this.isAzureDigitalTwins = azureDigitalTwins;
            this.isAzureIoTHub= azureIoTHub;
            this.logger = logger;
        }

        public void prototypeAction()
        {
            var attrDefs = objDef.LinkedFromR102();
            foreach(var attrDef in attrDefs)
            {
                string attrName = attrDef.Attr_Name;
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
                            var actGen = new ActDescripGenerator(actDef, "this", "", "", usedExternalEntities, coloringManager, isAzureDigitalTwins, isAzureIoTHub, logger);
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
        public bool IsDesiredProperty { get; set; } = false;
    }


}
