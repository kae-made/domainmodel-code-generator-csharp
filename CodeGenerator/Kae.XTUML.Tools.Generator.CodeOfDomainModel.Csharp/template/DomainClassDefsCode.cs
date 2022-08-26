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
    partial class DomainClassDefs
    {
        string version;
        string nameSpace;
        IEnumerable<CIClassDef> classObjDefs;

        public DomainClassDefs(string version, string nameSpace, IEnumerable<CIClassDef> classObjDef)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.classObjDefs = classObjDef;
        }

        public void prototype_get_identities()
        {
            foreach (var classObjDef in classObjDefs)
            {
                string identities = "";
                var objDef = (CIMClassO_OBJ)classObjDef;
                var oidDefs = objDef.LinkedFromR104();
                foreach(var oidDef in oidDefs)
                {
                    if (oidDef.Attr_Oid_ID == 0)
                    {
                        var oidaDefs = oidDef.LinkedOtherSideR105();
                        foreach (var oidaDef in oidaDefs)
                        {
                            var attrDef = oidaDef.LinkedOtherSideR105();
                            string propName = GeneratorNames.GetAttrPropertyName(attrDef);
                            if (!string.IsNullOrEmpty(identities))
                            {
                                identities += ",";
                            }
                            identities += $"{attrDef.Attr_Name}=this.{propName}";
                        }
                    }
                }

            }
        }
        public void prototype_properties()
        {
            foreach (var classObjDef in classObjDefs)
            {
                var objDef = (CIMClassO_OBJ)classObjDef;
                var propertiesGen = new ciclass.PropertyDef("        ", objDef, false);
                string propertiesGenCode = propertiesGen.TransformText();
            }
        }
        public void prototype_relationship()
        {
            foreach (var classObjDef in classObjDefs)
            {
                var objDef = (CIMClassO_OBJ)classObjDef;
                var relathinshipDef = new ciclass.RelationshipDef("        ", objDef, false);
                string propertiesGenCode = relathinshipDef.TransformText();
            }
        }
        public void prototype_operations()
        {
            foreach (var classObjDef in classObjDefs)
            {
                var objDef = (CIMClassO_OBJ)classObjDef;
                var operationsGen = new ciclass.ClassOperationDef("        ", objDef, false);
                string operationGenCode = operationsGen.TransformText();

            }
        }
        public void prototype()
        {
            foreach (var classObjDef in classObjDefs)
            {
                var objDef = (CIMClassO_OBJ)classObjDef;
                var domainClassName = GeneratorNames.GetDomainClassName(objDef);
                var domainBaseInterfaceName = GeneratorNames.GetDomainClassBaseInterfaceName();
                var subClassInterfaces = GetSubClassInterfaces(objDef);

                bool hasStateMachine = false;
                var ismDef = objDef.LinkedFromR518();
                if (ismDef != null)
                {
                    hasStateMachine = true;
                }

                var attrDefs = objDef.LinkedFromR102();
                foreach (var attrDef in attrDefs)
                {
                    var attrPropName = GeneratorNames.GetAttrPropertyName(attrDef);
                    var dtDef = DomainDataTypeDefs.GetBaseDT(attrDef);
                    var attrPropDataTypeName = DomainDataTypeDefs.GetDataTypeName(dtDef);
                    bool generate = true;
                    if (hasStateMachine)
                    {
                        if (dtDef.Attr_Name == "state<State_Model>")
                        {
                            generate = false;
                        }
                    }

                    if (generate)
                    {
                        string accessors = "get;";
                        var subAttrDef = attrDef.SubClassR106();
                        if (subAttrDef is not CIMClassO_RATTR)
                        {
                            bool isIdentity = false;
                            var oidaDefs = attrDef.LinkedOneSideR105();
                            foreach (var oidaDef in oidaDefs)
                            {
                                var oidDef = oidaDef.LinkedOneSideR105();
                                isIdentity = true;
                            }
                            if (isIdentity == false)
                            {
                                accessors += " set;";
                            }
                        }

                    }

                }

                var joinedRgos = GetJoinedRGOs(objDef);
                foreach (var rgo in joinedRgos)
                {
                    var subRgo = rgo.SubClassR205();
                    if (subRgo is CIMClassR_FORM)
                    {
                        var rformDef = (CIMClassR_FORM)subRgo;
                        var rsimpDef = rformDef.LinkedToR208();
                        var relDef = rsimpDef.CIMSuperClassR_REL();
                        var partDefs = rsimpDef.LinkedFromR207();
                        if (partDefs.Count() > 0)
                        {
                            if (partDefs.Count() > 1)
                            {
                                // TODO: ???
                            }
                            var partDef = partDefs.First();
                            string methodNameLinked = GeneratorNames.GetRelationshipMethodName(relDef, "", partDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                            var targetObjDef = partDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                            var targetClassName = GeneratorNames.GetDomainClassName(targetObjDef);
                            if (partDef.Attr_Mult == 0)
                            {
                                string returnType = targetClassName;
                                string methodNameLink = GeneratorNames.GetRelationshipMethodName(relDef, "", partDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Link);
                                string methodNameUnLink = GeneratorNames.GetRelationshipMethodName(relDef, "", partDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Unlink);
                            }
                            else
                            {
                                string returnType = $"IEnumerable<{targetClassName}>";
                            }
                        }
                    }
                    else if (subRgo is CIMClassR_SUB)
                    {
                        var rsupDef = (CIMClassR_SUB)subRgo;
                        var rsubsupDef = rsupDef.LinkedToR213();
                        var relDef = rsubsupDef.CIMSuperClassR_REL();
                        string methodNameLink = GeneratorNames.GetRelationshipMethodName(relDef, "", "", GeneratorNames.RelLinkMethodType.Link);
                        string methodNameUnlink = GeneratorNames.GetRelationshipMethodName(relDef, "", "", GeneratorNames.RelLinkMethodType.Unlink);
                        var targetObjDef = rsubsupDef.LinkedFromR212().CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                        string targetClassName = GeneratorNames.GetDomainClassName(targetObjDef);
                        string methodNameLinked = GeneratorNames.GetSuperTypeMethodName(rsubsupDef);
                    }
                    else if (subRgo is CIMClassR_ASSR)
                    {
                        var rassrDef = (CIMClassR_ASSR)subRgo;
                        var rassocDef = rassrDef.LinkedToR211();
                        var relDef = rassocDef.CIMSuperClassR_REL();
                        var raoneDef = rassocDef.LinkedFromR209();
                        var raothDef = rassocDef.LinkedFromR210();
                        string methodNameLink = GeneratorNames.GetRelationshipMethodName(relDef, "", "", GeneratorNames.RelLinkMethodType.Link);
                        string methodNameUnlink = GeneratorNames.GetRelationshipMethodName(relDef, "", "", GeneratorNames.RelLinkMethodType.Unlink);

                        string methodNameLinkedOne = GeneratorNames.GetRelationshipMethodName(relDef, "One", raoneDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                        var oneObjDef = raoneDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                        var oneObjTypeName = GeneratorNames.GetDomainClassName(oneObjDef);
                        var oneArgName = $"oneInstance{GeneratorNames.ToProgramAvailableString(raoneDef.Attr_Txt_Phrs)}";

                        string methodNameLinkedOther = GeneratorNames.GetRelationshipMethodName(relDef, "Other", raothDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                        var otherObjDef = raothDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                        var otherObjTypeName = GeneratorNames.GetDomainClassName(otherObjDef);
                        var otherArgName = $"otherInstance{GeneratorNames.ToProgramAvailableString(raothDef.Attr_Txt_Phrs)}";

                    }
                }

                var joinedRtos = GetJoinedRTOs(objDef);
                foreach (var rto in joinedRtos)
                {
                    var subRto = rto.SubClassR204();
                    if (subRto is CIMClassR_PART)
                    {
                        var rpartDef = (CIMClassR_PART)subRto;
                        var rsimpDef = rpartDef.LinkedToR207();
                        var relDef = rsimpDef.CIMSuperClassR_REL();
                        var rformDef = rsimpDef.LinkedFromR208();
                        if (rformDef != null)
                        {
                            var targetObjDef = rformDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                            string targetClassName = GeneratorNames.GetDomainClassName(targetObjDef);
                            string methodNameLinked = GeneratorNames.GetRelationshipMethodName(relDef, "", rformDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                            if (rformDef.Attr_Mult == 0)
                            {
                                string returnType = targetClassName;
                            }
                            else
                            {
                                string returnType = $"IEnumerable<{targetClassName}>";

                            }
                        }
                    }
                    else if (subRto is CIMClassR_AONE)
                    {
                        var raoneDef = (CIMClassR_AONE)subRto;
                        var rassocDef = raoneDef.LinkedToR209();
                        var relDef = rassocDef.CIMSuperClassR_REL();
                        var targetObjDef = rassocDef.LinkedFromR211().CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                        var raothDef = rassocDef.LinkedFromR210();
                        string targetTypeName = GeneratorNames.GetDomainClassName(targetObjDef);
                        string methodNameLinked = GeneratorNames.GetRelationshipMethodName(relDef, "One", raothDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                        if (raothDef.Attr_Mult == 0)
                        {
                            string returnType = targetTypeName;
                        }
                        else
                        {
                            string returnType = $"IEnumerable<{targetTypeName}>";
                        }
                    }
                    else if (subRto is CIMClassR_AOTH)
                    {
                        var raothDef = (CIMClassR_AOTH)subRto;
                        var rassocDef = raothDef.LinkedToR210();
                        var relDef = rassocDef.CIMSuperClassR_REL();
                        var targetObjDef = rassocDef.LinkedFromR211().CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                        string targetType = GeneratorNames.GetDomainClassName(targetObjDef);
                        var raoneDef = rassocDef.LinkedFromR209();
                        string methodNameLinked = GeneratorNames.GetRelationshipMethodName(relDef, "Other", raoneDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                        if (raoneDef.Attr_Mult == 0)
                        {
                            string returnType = targetType;
                        }
                        else
                        {
                            string returnType = $"IEnumerable<{targetType}>";
                        }
                    }
                    else if (subRto is CIMClassR_SUPER)
                    {
                        var superDef = (CIMClassR_SUPER)subRto;
                        var subsupRelDef = superDef.LinkedToR212().CIMSuperClassR_REL();
                        var subClassIFName = GeneratorNames.GetSubRelInterfaceName(subsupRelDef);
                        var subClassGetMethodName = GeneratorNames.GetSubRelClassMethodName(subsupRelDef);

                        var subDefs = superDef.LinkedToR212().LinkedFromR213();
                        foreach (var subDef in subDefs)
                        {
                            var subObjDef = subDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                            string methodNameLinked = GeneratorNames.GetRelationshipMethodName(subsupRelDef, "", "", GeneratorNames.RelLinkMethodType.Linked) + subObjDef.Attr_Key_Lett;
                            string returnType = GeneratorNames.GetDomainClassName(subObjDef);
                        }

                    }
                }

                var tfrDefs = objDef.LinkedFromR115();
                foreach (var tfrDef in tfrDefs)
                {
                    string opName = tfrDef.Attr_Name;
                    var retDtDef = tfrDef.LinkedToR116();
                    var retTypeName = DomainDataTypeDefs.GetDataTypeName(retDtDef);
                    var tparmDefs = tfrDef.LinkedFromR117();
                    string args = "";
                    foreach (var tparmDef in tparmDefs)
                    {
                        var paramTypeDef = tparmDef.LinkedToR118();
                        string paramTypeName = DomainDataTypeDefs.GetDataTypeName(paramTypeDef);
                        string frag = $"{paramTypeName} {tparmDef.Attr_Name}";
                        if (!string.IsNullOrEmpty(args))
                        {
                            args += $", {frag}";
                        }
                    }
                }
            }
        }

        public static string GetSubClassInterfaces(CIMClassO_OBJ objDef)
        {
            string result = "";
            var attrDefs = objDef.LinkedFromR102();
            foreach (var attrDef in attrDefs)
            {
                var subAttrDef = attrDef.SubClassR106();
                if (subAttrDef is CIMClassO_RATTR)
                {
                    var refDefs = ((CIMClassO_RATTR)subAttrDef).LinkedFromR108();
                    foreach (var refDef in refDefs)
                    {
                        var rgoDef = refDef.LinkedOneSideR111();
                        var subRgoDef = rgoDef.SubClassR205();
                        if (subRgoDef is CIMClassR_SUB)
                        {
                            var subDef = (CIMClassR_SUB)subRgoDef;
                            var relDef = subDef.LinkedToR213().CIMSuperClassR_REL();
                            if (!string.IsNullOrEmpty(result))
                            {
                                result += ", ";
                            }
                            result = GeneratorNames.GetSubRelInterfaceName(relDef);
                        }
                    }
                }
            }
            return result;
        }

        public static IEnumerable<CIMClassR_RGO> GetJoinedRGOs(CIMClassO_OBJ objDef)
        {
            var rgos = new List<CIMClassR_RGO>();
            var foundRels = new List<CIMClassR_REL>();

            var attrDefs = objDef.LinkedFromR102();
            foreach (var attrDef in attrDefs)
            {
                var subAttrDef = attrDef.SubClassR106();
                if (subAttrDef is CIMClassO_RATTR)
                {
                    var orefDefs = ((CIMClassO_RATTR)(subAttrDef)).LinkedFromR108();
                    foreach (var orefDef in orefDefs)
                    {
                        CIMClassR_REL relDef = null;
                        var rgoDef = orefDef.LinkedOneSideR111();
                        var subRgoDef = rgoDef.SubClassR205();
                        if (subRgoDef is CIMClassR_FORM)
                        {
                            relDef = ((CIMClassR_FORM)subRgoDef).LinkedToR208().CIMSuperClassR_REL();
                        }
                        else if (subRgoDef is CIMClassR_SUB)
                        {
                            relDef = ((CIMClassR_SUB)subRgoDef).LinkedToR213().CIMSuperClassR_REL();
                        }
                        else if (subRgoDef is CIMClassR_ASSR)
                        {
                            relDef = ((CIMClassR_ASSR)subRgoDef).LinkedToR211().CIMSuperClassR_REL();
                        }
                        var joined = foundRels.Where(r => r.Attr_Numb == relDef.Attr_Numb);
                        if (joined.Count() == 0)
                        {
                            foundRels.Add(relDef);
                            rgos.Add(rgoDef);
                        }
                    }
                }
            }

            return rgos;
        }

        public static IEnumerable<CIMClassR_RTO> GetJoinedRTOs(CIMClassO_OBJ objDef)
        {
            var rtos = new List<CIMClassR_RTO>();
            var joinedRels = new List<string>();

            var attrDefs = objDef.LinkedFromR102();
            foreach (var attrDef in attrDefs)
            {
                var oidaDefs = attrDef.LinkedOneSideR105();
                foreach (var oidaDef in oidaDefs)
                {
                    var rtidaDefs = oidaDef.LinkedOneSideR110();
                    foreach (var rtidaDef in rtidaDefs)
                    {
                        var rtoDef = rtidaDef.LinkedOneSideR110();
                        string relName = null;
                        var subRtoDef = rtoDef.SubClassR204();
                        if (subRtoDef is CIMClassR_PART)
                        {
                            var relDef = ((CIMClassR_PART)subRtoDef).LinkedToR207().CIMSuperClassR_REL();
                            relName = $"R{relDef.Attr_Numb}";
                        }
                        else if (subRtoDef is CIMClassR_SUPER)
                        {
                            var relDef = ((CIMClassR_SUPER)subRtoDef).LinkedToR212().CIMSuperClassR_REL();
                            relName = $"R{relDef.Attr_Numb}";
                        }
                        else if (subRtoDef is CIMClassR_AONE)
                        {
                            var relDef = ((CIMClassR_AONE)subRtoDef).LinkedToR209().CIMSuperClassR_REL();
                            relName = $"R{relDef.Attr_Numb}One";
                        }
                        else if (subRtoDef is CIMClassR_AOTH)
                        {
                            var relDef = ((CIMClassR_AOTH)subRtoDef).LinkedToR210().CIMSuperClassR_REL();
                            relName = $"R{relDef.Attr_Numb}{relDef.Attr_Numb}Other";
                        }
                        if (!joinedRels.Contains(relName))
                        {
                            rtos.Add(rtoDef);
                            joinedRels.Add(relName);
                        }
                    }
                }
            }
            return rtos;
        }
    }
}

