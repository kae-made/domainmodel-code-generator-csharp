// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class InstanceRepository
    {
        string version;
        string nameSpace;
        List<CIMClassO_OBJ> objDefs;

        public InstanceRepository(string version, string nameSpace, IEnumerable<CIClassDef> classDefs)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.objDefs = new List<CIMClassO_OBJ>();
            foreach (var c in classDefs)
            {
                this.objDefs.Add((CIMClassO_OBJ)c);
            }
        }

        public void prototype()
        {
            foreach(var objDef in objDefs)
            {
                string domainClassBaseName = GeneratorNames.GetDomainClassImplName(objDef);
                string domainClassName = objDef.Attr_Key_Lett;
            }
        }
    }
}
