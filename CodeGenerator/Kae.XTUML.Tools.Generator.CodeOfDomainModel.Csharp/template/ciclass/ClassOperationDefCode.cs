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
        private Logger logger;

        public ClassOperationDef(string indent, CIMClassO_OBJ objDef, bool genLogicCode, IDictionary<string, CIMClassS_EE> usedEEs, ColoringManager coloringManager, Logger logger)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genLogicCode = genLogicCode;
            this.usedExternalEntities = usedEEs;
            this.coloringManager = coloringManager;
            this.logger = logger;
        }
    }
}
