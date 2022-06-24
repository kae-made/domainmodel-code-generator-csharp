using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class DomainClassOperations
    {
        string version;
        string nameSpace;
        CIMClassO_OBJ objDef;

        public DomainClassOperations(string version, string nameSpace, CIMClassO_OBJ objDef)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.objDef = objDef;
        }

        public void prototype()
        {
            string domainClassBaseName = GeneratorNames.GetDomainClassImplName(objDef);
            var tfrDefs = objDef.LinkedFromR115();
            foreach (var tfrDef in tfrDefs)
            {
                string descrip = "";
                if (!string.IsNullOrEmpty(tfrDef.Attr_Action_Semantics))
                {
                    descrip = GeneratorNames.DescripToCodeComment("            ", tfrDef.Attr_Action_Semantics);
                    if (descrip.EndsWith(Environment.NewLine))
                    {
                        descrip=descrip.Substring(0, descrip.Length - Environment.NewLine.Length);
                    }
                }
                
                string opName = GeneratorNames.GetTfrOpName(tfrDef);
                var retDtDef = tfrDef.LinkedToR116();
                var retDtTypeName = DomainDataTypeDefs.GetDataTypeName(retDtDef);
                var tparmDefs = tfrDef.LinkedFromR117();
                // var descrip = GeneratorNames.DescripToCodeComment("        ", tfrDef.Attr_Descrip);
                string opArgs = "";
                foreach(var tparamDef in tparmDefs)
                {
                    if (!string.IsNullOrEmpty(opArgs))
                    {
                        opArgs += ", ";
                    }
                    var parmDtDef = tparamDef.LinkedToR118();
                    var parmDtTypeName = DomainDataTypeDefs.GetDataTypeName(parmDtDef);
                    opArgs += $"{parmDtTypeName} {tparamDef.Attr_Name}";
                    // throw new NotImplementedException();
                }
            }
        }
    }
}
