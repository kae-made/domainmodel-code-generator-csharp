using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class ExternalEntityDef
    {
        string version;
        string nameSpace;
        CIMClassS_EE eeDef;
        static readonly string folderName = "ExternalEntities";

        public ExternalEntityDef(string version, string nameSpace,CIMClassS_EE eeDef)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.eeDef = eeDef;
        }

        public static string GetFolderName() { return folderName; }

        public void prototype()
        {
            string wrapperClassName = GeneratorNames.GetExternalEntityWrappterClassName(eeDef);
            string keyLett = eeDef.Attr_Key_Lett;
            var brgDefs = eeDef.LinkedFromR19();
            foreach(var brgDef in brgDefs){
                string brgName = brgDef.Attr_Name;
                var retDtDef = brgDef.LinkedToR20();
                string retDtName = DomainDataTypeDefs.GetDataTypeName(retDtDef);
                string paramsCode = "";
                var bparmDefs = brgDef.LinkedFromR21();
                foreach (var bparmDef in bparmDefs)
                {
                    var parmDtDef = bparmDef.LinkedToR22();
                    var parmDtName= DomainDataTypeDefs.GetDataTypeName(parmDtDef);
                    if (!string.IsNullOrEmpty(paramsCode))
                    {
                        paramsCode += ", ";
                    }
                    paramsCode += $"{parmDtName} {bparmDef.Attr_Name}";
                }
            }
        }
    }
}
