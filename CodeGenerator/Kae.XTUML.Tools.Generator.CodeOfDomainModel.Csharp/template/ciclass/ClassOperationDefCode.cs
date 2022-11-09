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
    partial class ClassOperationDef
    {
        private string indent;
        private CIMClassO_OBJ objDef;
        private bool genLogicCode;
        private IDictionary<string, CIMClassS_EE> usedExternalEntities;
        private ColoringManager coloringManager;
        private bool isAzureIoTHub;
        private Logger logger;

        public ClassOperationDef(string indent, CIMClassO_OBJ objDef, bool genLogicCode, IDictionary<string, CIMClassS_EE> usedEEs, ColoringManager coloringManager, bool azureIoTHub, Logger logger)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genLogicCode = genLogicCode;
            this.usedExternalEntities = usedEEs;
            this.isAzureIoTHub = azureIoTHub;
            this.coloringManager = coloringManager;
            this.logger = logger;
        }

        public void Prototype(CIMClassO_TFR tfrDef)
        {
            var tparmDefs = tfrDef.LinkedFromR117();
            int index = 0;
            foreach (var tparmDef in tparmDefs)
            {
                string paramName = tparmDef.Attr_Name;
                if (++index < tparmDefs.Count())
                {

                }
            }
            var payloadJson = new { x = 10 };
            string payload = Newtonsoft.Json.JsonConvert.SerializeObject(payloadJson);
            var retDtDef = tfrDef.LinkedToR116();
            if (retDtDef.Attr_Name == "void")
            {

            }
            else
            {

            }
            string attrNameForDeviceId = PropertyDef.GetAttrNameForDeviceId(tfrDef.LinkedToR115());
            objDef.Attr_Descrip.StartsWith("@iotpnp");
        }
    }
}
