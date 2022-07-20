// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
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
        public ClassOperationDef(string indent, CIMClassO_OBJ objDef, bool genLogicCode)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genLogicCode = genLogicCode;
        }
    }
}
