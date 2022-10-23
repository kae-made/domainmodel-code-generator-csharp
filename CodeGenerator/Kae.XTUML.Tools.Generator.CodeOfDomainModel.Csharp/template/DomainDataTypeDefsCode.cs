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
    partial class DomainDataTypeDefs
    {
        string version;
        string nameSpace;
        IEnumerable<CIClassDef> classDtDefs;
        private static IDictionary<string, string> dtToDTDLDataType = new Dictionary<string, string>() {
            { "boolean", "bool" },
            { "unique_id", "string" },
            { "integer", "int" },
            { "real", "double" },
            { "timestamp", "DateTime" },
            { "date", "DateTime" },
            { "string", "string" },
            { "state<State_Model>", "int" },
            { "inst_ref<Timer>", "Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer" },
            { "inst<Mapping>", "string" },
            {"inst<Event>", "EventData" },
            { "void", "void" } };


        public DomainDataTypeDefs(string version, string nameSpace, IEnumerable<CIClassDef> classDtDefs)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.classDtDefs = classDtDefs;
        }

        public void prototype()
        {
            foreach (var classDtDef in classDtDefs)
            {
                var dtDef = (CIMClassS_DT)classDtDef;
                var subDtDef = dtDef.SubClassR17();
                
                string descrip = "";
                if (!string.IsNullOrEmpty(dtDef.Attr_Descrip))
                {
                    descrip = GeneratorNames.DescripToCodeComment("    ", dtDef.Attr_Descrip);
                }
                if (subDtDef is CIMClassS_CDT)
                {
                    // Do Nothing
                }
                else if (subDtDef is CIMClassS_UDT)
                {
                    // Do Nothing
                }
                else if (subDtDef is CIMClassS_EDT)
                {
                    if (!string.IsNullOrEmpty(descrip))
                    {

                    }
                    int index = 0;
                    var edtDef = (CIMClassS_EDT)subDtDef;
                    string dtName = GeneratorNames.GetEnumDataTypeName(edtDef);
                    var enumDefs = edtDef.LinkedFromR27();
                    foreach(var enumDef in enumDefs)
                    {
                        string enumName = enumDef.Attr_Name;
                        index++;
                        if (index == enumDefs.Count())
                        {

                        }
                    }
                }
                else if (subDtDef is CIMClassS_SDT)
                {
                    if (!string.IsNullOrEmpty(descrip))
                    {

                    }
                    var sdtDef = (CIMClassS_SDT)subDtDef;
                    string dtName = GeneratorNames.GetComplexDataTypeName(sdtDef);
                    var mbrDefs = sdtDef.LinkedFromR44();
                    foreach(var mbrDef in mbrDefs)
                    {
                        var mbrDtDef = mbrDef.LinkedToR45();
                        string mbrDtName = GetDataTypeName(mbrDtDef);
                        string mbrName = mbrDef.Attr_Name;
                    }
                }
            }
        }

        public static string GetDataTypeName(CIMClassS_DT dtDef)
        {
            string result = "";
            var subDtDef = dtDef.SubClassR17();
            if (subDtDef is CIMClassS_CDT)
            {
                // return GeneratorNames.GetComplexDataTypeName((CIMClassS_CDT)subDtDef);
                return dtToDTDLDataType[dtDef.Attr_Name];
            }
            else if (subDtDef is CIMClassS_UDT)
            {
                if (dtToDTDLDataType.ContainsKey(dtDef.Attr_Name))
                {
                    return dtToDTDLDataType[dtDef.Attr_Name];
                }
                var origDtDef = ((CIMClassS_UDT)subDtDef).LinkedToR18();
                return GetDataTypeName(origDtDef);
            }
            else if (subDtDef is CIMClassS_EDT)
            {
                return GeneratorNames.GetEnumDataTypeName((CIMClassS_EDT)subDtDef);
            }
            else if (subDtDef is CIMClassS_SDT)
            {
                return GeneratorNames.GetComplexDataTypeName((CIMClassS_SDT)subDtDef);
            }
            return dtToDTDLDataType[dtDef.Attr_Name];
        }

        public static CIMClassS_DT GetBaseDT(CIMClassO_ATTR attrDef)
        {
            var subAttrDef = attrDef.SubClassR106();
            if (subAttrDef is CIMClassO_BATTR)
            {
                var battrDef = (CIMClassO_BATTR)subAttrDef;
                var dtDef = attrDef.LinkedToR114();
                return dtDef;
            }
            else if (subAttrDef is CIMClassO_RATTR)
            {
                var rattrDef = (CIMClassO_RATTR)subAttrDef;
                var battrDef = rattrDef.LinkedToR113();
                if (battrDef == null)
                {
                    var dtDef = attrDef.LinkedToR114();
                    return dtDef;
                }
                else
                {
                    var referedAttrDef = rattrDef.LinkedToR113().CIMSuperClassO_ATTR();
                    var dtDef = referedAttrDef.LinkedToR114();
                    return dtDef;
                }
            }
            else
            {
                throw new IndexOutOfRangeException($"O_ATTR[{attrDef.Attr_Attr_ID}]'s subtype is wrong!");
            }
        }

        public static bool IsStructuredDataType(CIMClassS_DT dtDef)
        {
            var subDtDef = dtDef.SubClassR17();
            if (subDtDef is CIMClassS_SDT)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
