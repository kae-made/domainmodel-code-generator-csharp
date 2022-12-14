using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
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
        bool isAzureDigitalTwins;

        public ExternalEntityDef(string version, string nameSpace,CIMClassS_EE eeDef, bool azureDigitalTwins)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.eeDef = eeDef;
            this.isAzureDigitalTwins = azureDigitalTwins;
        }

        public static string GetFolderName() { return folderName; }

        public static bool IsBuiltInExternalEntity(IDictionary<string, IDictionary<string, string>> marking)
        {
            bool result = false;

            if (marking.ContainsKey("builtin"))
            {
                if (marking["builtin"].Values.ElementAt(0).ToLower() == "true")
                {
                    result = true;
                }
            }

            return result;
        }
        public static (string initCode, string retCode) GetImplReturnCode(CIMClassS_BRG brgDef)
        {
            var dtDef = brgDef.LinkedToR20();
            string initCode = "";
            string retCode = "";
            string resultVarName = "result";
            string retDTName = DomainDataTypeDefs.GetDataTypeName(dtDef);
            var subDtDef = dtDef.SubClassR17();
            if (subDtDef is CIMClassS_UDT)
            {
                subDtDef = ((CIMClassS_UDT)subDtDef).LinkedToR18().SubClassR17();
            }
            if (subDtDef is CIMClassS_CDT)
            {
                if (retDTName != "void")
                {
                    if (retDTName == "DateTime" || retDTName == "EventData" || retDTName.EndsWith("Timer"))
                    {
                        initCode = $"{retDTName} {resultVarName} = null;";
                    }
                    else
                    {
                        switch (retDTName)
                        {
                            case "bool":
                                initCode = $"bool {resultVarName} = true;";
                                break;
                            case "int":
                                initCode = $"int {resultVarName} = 0;";
                                break;
                            case "double":
                                initCode = $"double {resultVarName} = 0.0;";
                                break;
                            case "string":
                                initCode = $"string {resultVarName} = \"\";";
                                break;
                            default:
                                initCode = $"// Unknown datatype - {retDTName}";
                                break;

                        }
                    }
                    retCode = $"return {resultVarName};";
                }
            }
            else if (subDtDef is CIMClassS_SDT)
            {
                initCode = $"{retDTName} {resultVarName} = null;";
                retCode = $"return {resultVarName};";
            }
            else if (subDtDef is CIMClassS_EDT)
            {
                var firstEnumDef = ((CIMClassS_EDT)subDtDef).LinkedFromR27().FirstOrDefault();
                initCode = $"{retDTName} {resultVarName} = {retDTName}.{firstEnumDef.Attr_Name};";
                retCode = $"return {resultVarName};";
            }
            return (initCode, retCode);
        }

        public static IDictionary<string, IDictionary<string, string>> GetColorMark(CIMClassS_EE eeDef)
        {
            var results = new Dictionary<string, IDictionary<string, string>>();
            string descrip = eeDef.Attr_Descrip;
            using (var reader = new StringReader(descrip))
            {
                string line = "";
                while((line=reader.ReadLine()) != null)
                {
                    if (line.StartsWith("@"))
                    {
                        var frags = line.Substring(1).Split(new char[] { ';' });
                        bool builtin = false;
                        foreach(var frag in frags)
                        {
                            if (frag.StartsWith("builtin"))
                            {
                                if (frag.ToLower().IndexOf("true") > 0)
                                {
                                    builtin = true;
                                    break;
                                }
                            }
                        }
                        if (builtin == false)
                        {
                            foreach (var frag in frags)
                            {
                                int op = frag.IndexOf("(");
                                int cp = frag.LastIndexOf(")");
                                string markName = frag.Substring(0, op);
                                if (!results.ContainsKey(markName))
                                {
                                    results.Add(markName, new Dictionary<string, string>());
                                }
                                string markArgs = frag.Substring(op + 1, (cp - op) - 1);
                                var margs = markArgs.Split(new char[] { ',' });
                                foreach (var marg in margs)
                                {
                                    string margName = marg.Trim();
                                    string margValue = marg.Trim();
                                    if (marg.IndexOf(":") > 0)
                                    {
                                        margName = marg.Substring(0, marg.IndexOf(":"));
                                        margValue = marg.Substring(marg.IndexOf(":") + 1);
                                    }
                                    if (!results[markName].ContainsKey(margName))
                                    {
                                        results[markName].Add(margName, "");
                                    }
                                    results[markName][margName] = margValue;
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }

        public void prototype()
        {
            string wrapperClassName = GeneratorNames.GetExternalEntityWrappterClassName(eeDef, isAzureDigitalTwins);
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
                var implOfReturn = GetImplReturnCode(brgDef);
                string initCode = implOfReturn.initCode;
                string retCode = implOfReturn.retCode;
            }
        }
    }
}
