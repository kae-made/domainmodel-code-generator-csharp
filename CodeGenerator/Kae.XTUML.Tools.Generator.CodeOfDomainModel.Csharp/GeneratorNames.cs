﻿using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp
{
    internal class GeneratorNames
    {
        public static string GetDomainClassBaseInterfaceName()
        {
            return "DomainClassDef";
        }
        public static string GetDomainClassName(CIMClassO_OBJ objDef)
        {
            return $"DomainClass{objDef.Attr_Key_Lett}";
        }

        public static string GetDomainClassImplName(CIMClassO_OBJ objDef)
        {
            return $"DomainClass{objDef.Attr_Key_Lett}Base";
        }
        public static string GetSubRelInterfaceName(CIMClassR_REL relDef)
        {
            return $"SubClassR{relDef.Attr_Numb}";
        }

        public static string GetSubRelClassMethodName(CIMClassR_REL relDef)
        {
            return $"GetSubR{relDef.Attr_Numb}";
        }

        public static string GetGetSuperClassMethodName(CIMClassR_REL relDef, CIMClassO_OBJ objDef)
        {
            return $"GetSuperClassR{relDef.Attr_Numb}";
        }

        public static string GetTakeEventMethodName()
        {
            return "TakeEvent";
        }

        public static string GetAttrPropertyName(CIMClassO_ATTR attrDef)
        {
            return $"Attr_{attrDef.Attr_Name}";
        }
        public static string GetAttrPropertyLocalName(CIMClassO_ATTR attrDef)
        {
            return $"attr_{attrDef.Attr_Name}";
        }

        public static string GetTfrOpName(CIMClassO_TFR tfrDef)
        {
            string tfrName = tfrDef.Attr_Name;
            return $"{tfrName.Substring(0,1)}{tfrName.Substring(1)}";
        }

        public static string GetEventEnumLabelName(CIMClassO_OBJ objDef, CIMClassSM_EVT evtDef)
        {
            return $"{objDef.Attr_Key_Lett}{evtDef.Attr_Numb}";
        }

        public static string GetEventClassName(CIMClassO_OBJ objDef, CIMClassSM_EVT evtDef)
        {
            return $"{ objDef.Attr_Key_Lett}{ evtDef.Attr_Numb}_{ToProgramAvailableString(evtDef.Attr_Mning)}";
        }
        public static string GetStateEnumLabelName(CIMClassSM_STATE stateDef)
        {
            return ToProgramAvailableString(stateDef.Attr_Name);
        }

        public static string GetStateActionMethodName(CIMClassSM_STATE stateDef)
        {
            return $"Action{ToProgramAvailableString(stateDef.Attr_Name)}";
        }

        public static string GetStateArgsInterfaceName(CIMClassSM_STATE stateDef)
        {
            return $"IEventArgs{ToProgramAvailableString(stateDef.Attr_Name)}";
        }

        public static string ToProgramAvailableString(string content)
        {
            string result = "";
            if (string.IsNullOrEmpty(content))
            {
                return result;
            }
            var frags = content.Split(new char[] { ' ' });
            foreach(var frag in frags)
            {
                result += frag.Substring(0, 1).ToUpper() + frag.Substring(1);
            }
            return result;
        }

        public enum RelLinkMethodType
        {
            Linked,
            Link,
            Unlink
        }
        public static string GetRelationshipMethodName(CIMClassR_REL relDef, string side, string phrase, RelLinkMethodType methodType)
        {
            string result = "";
            string prefix = "Unknown";
            switch (methodType)
            {
                case RelLinkMethodType.Link:
                    prefix = "Link";
                    break;
                case RelLinkMethodType.Unlink:
                    prefix = "Unlink";
                    break;
                case RelLinkMethodType.Linked:
                    prefix = "Linked";
                    break;
            }
            result = $"{prefix}R{relDef.Attr_Numb}";
            if (!string.IsNullOrEmpty(side))
            {
                result += side;
            }
            if (!string.IsNullOrEmpty(phrase))
            {
                result += ToProgramAvailableString(phrase);
            }
            return result;            
        }

        public static string GetSuperTypeMethodName(CIMClassR_SUBSUP subSupDef)
        {
            var relDef = subSupDef.CIMSuperClassR_REL();
            var targetObjDef = subSupDef.LinkedFromR212().CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
            return $"GetSuperTypeR{relDef.Attr_Numb}{targetObjDef.Attr_Key_Lett}";
        }

        public static string GetComplexDataTypeName(CIMClassS_SDT sdtDef)
        {
            return $"DomainType{sdtDef.CIMSuperClassS_DT().Attr_Name}";
        }

        public static string GetEnumDataTypeName(CIMClassS_EDT edtDef)
        {
            return $"DomainType{edtDef.CIMSuperClassS_DT().Attr_Name}";
        }

        public static string DescripToCodeComment(string indent, string descrip)
        {
            string result = "";
            if (!string.IsNullOrEmpty(descrip))
            {
                using (var writer = new StringWriter())
                {
                    using (var reader = new StringReader(descrip))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            string xformed = line;
                            if (!line.StartsWith("//"))
                            {
                                xformed = $"// {line}";
                            }
                            writer.WriteLine($"{indent}{xformed}");
                        }
                    }
                    result = writer.ToString();
                }
            }
            return result;
        }

        public static string GetStateMachineClassName(CIMClassO_OBJ objDef)
        {
            return $"DomainClass{objDef.Attr_Key_Lett}StateMachine";
        }

        public static string GetRelLocalVariableName(CIMClassR_REL relDef, CIMClassO_OBJ targetObjDef, string txtPhrase)
        {
            return $"relR{relDef.Attr_Numb}{targetObjDef.Attr_Key_Lett}{ToProgramAvailableString(txtPhrase)}";
        }
    }
}
