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
    partial class SuperTypeDefs
    {
        string version;
        string nameSpace;
        IEnumerable<CIClassDef> classRelDefs;

        public SuperTypeDefs(string nameSpace, string version, IEnumerable<CIClassDef> classRelDefs)
        {
            this.nameSpace = nameSpace;
            this.version = version;
            this.classRelDefs = classRelDefs;
        }

        public void prototype()
        {
            bool isFirst = true;
            foreach (var classRelDef in classRelDefs)
            {
                var relDef = (CIMClassR_REL)classRelDef;
                var subRelDef = relDef.SubClassR206();
                if (subRelDef is CIMClassR_SUBSUP)
                {
                    var interfaceName = GeneratorNames.GetSubRelInterfaceName(relDef);
                    var rsuperDef = ((CIMClassR_SUBSUP)subRelDef).LinkedFromR212();
                    var objDef = rsuperDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    var getSuperClassMethodName = GeneratorNames.GetGetSuperClassMethodName(relDef, objDef);
                    var superDomainClassName = GeneratorNames.GetDomainClassName(objDef);
                    if (isFirst == false)
                    {

                    }
                    isFirst = false;
                }
            }
        }
    }
}
