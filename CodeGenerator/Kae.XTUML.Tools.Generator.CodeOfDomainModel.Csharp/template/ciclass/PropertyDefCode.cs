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
    partial class PropertyDef
    {
        private string indent;
        private CIMClassO_OBJ objDef;
        private bool genImplCode;

        public PropertyDef(string indent, CIMClassO_OBJ objDef, bool genImplCode)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genImplCode = genImplCode;
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
    }

}
