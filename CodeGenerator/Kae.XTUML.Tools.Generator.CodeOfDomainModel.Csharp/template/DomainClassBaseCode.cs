// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class DomainClassBase
    {
        string version;
        string nameSpace;
        CIMClassO_OBJ objDef;

        public DomainClassBase(string version, string nameSpace, CIMClassO_OBJ objDef)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.objDef = objDef;
        }

        public void prototype()
        {
            var domainClassName = GeneratorNames.GetDomainClassName(objDef);
            var domainBaseInterfaceName = GeneratorNames.GetDomainClassBaseInterfaceName();
            var subClassInterfaces = DomainClassDefs.GetSubClassInterfaces(objDef);
            var domainClassBaseName = GeneratorNames.GetDomainClassImplName(objDef);

            bool hasStateMachine = false;
            var ismDef = objDef.LinkedFromR518();
            if (ismDef != null)
            {
                hasStateMachine = true;
            }

            var implAttrDefs = new List<AttributeDef>();
            var attrDefs = objDef.LinkedFromR102();
            foreach (var attrDef in attrDefs)
            {
                var implAttrDef = new AttributeDef() { AttrDef = attrDef, IsIdentity = false, IsReferential = false, IsUniqueId = false, IsState = false };
                var attrPropName = GeneratorNames.GetAttrPropertyName(attrDef);
                var dtDef = DomainDataTypeDefs.GetBaseDT(attrDef);
                var attrPropDataTypeName = DomainDataTypeDefs.GetDataTypeName(dtDef);
                implAttrDef.DataTypeName = attrPropDataTypeName;
                bool generate = true;
                if (hasStateMachine)
                {
                    if (dtDef.Attr_Name == "state<State_Model>")
                    {
                        generate = false;
                        implAttrDef.IsState = true;
                    }
                }

                if (generate)
                {
                    var subAttrDef = attrDef.SubClassR106();
                    if (subAttrDef is not CIMClassO_RATTR)
                    {
                        var oidaDefs = attrDef.LinkedOneSideR105();
                        foreach (var oidaDef in oidaDefs)
                        {
                            var oidDef = oidaDef.LinkedOneSideR105();
                            implAttrDef.IsIdentity = true;
                            if (dtDef.Attr_Name == "unique_id")
                            {
                                implAttrDef.IsUniqueId = true;
                            }
                        }
                    }
                    else
                    {
                        implAttrDef.IsReferential = true;
                    }
                }
                implAttrDefs.Add(implAttrDef);
            }
            // Constructor Gen
            string constructorArgs = "";
            foreach (var implAttrDef in implAttrDefs)
            {
                if (!implAttrDef.IsReferential)
                {
                    if (!string.IsNullOrEmpty(constructorArgs))
                    {
                        constructorArgs += ", ";
                    }
                    constructorArgs += implAttrDef.AttrDef.Attr_Name;
                }
            }
            // Constructor Signature
            // Constructor Body
            foreach(var implAttrDef in implAttrDefs)
            {
                string propName = GeneratorNames.GetAttrPropertyName(implAttrDef.AttrDef);
                string propLocalName = GeneratorNames.GetAttrPropertyLocalName(implAttrDef.AttrDef);
                if (implAttrDef.IsIdentity)
                {
                    if (implAttrDef.IsUniqueId)
                    {
                        var newGuid = Guid.NewGuid().ToString();
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (implAttrDef.IsState)
                    {
                        string stateMachineClassName = GeneratorNames.GetStateMachineClassName(objDef);
                        string stateMachineLocalVariableName = "stateMachine";
                    }
                    else
                    {
                        if (implAttrDef.IsReferential)
                        {

                        }
                        else
                        {

                        }
                    }
                }
            }

            // local member Attr Gen
            foreach(var implAttrDef in implAttrDefs)
            {
                string localVarName = GeneratorNames.GetAttrPropertyLocalName(implAttrDef.AttrDef);

            }
            // property implementation
            foreach(var implAttrDef in implAttrDefs)
            {
                string localVarName = GeneratorNames.GetAttrPropertyLocalName(implAttrDef.AttrDef);
                string propName = GeneratorNames.GetAttrPropertyName(implAttrDef.AttrDef);

                if (implAttrDef.IsIdentity)
                {

                }
                else
                {
                    if (implAttrDef.IsState)
                    {

                    }
                    else
                    {
                        if (implAttrDef.IsReferential)
                        {

                        }
                        else
                        {

                        }
                    }
                }
            }

            var logInstanceCreation = new logging.Logging("logger", "", objDef, "newInstance", logging.Logging.Mode.InstanceLifeCycle, "create");
            var logInstanceCreationGen = logInstanceCreation.TransformText();

            var joinedRgos = DomainClassDefs.GetJoinedRGOs(objDef);
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
                        var targetObjDef = partDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                        var targetClassName = GeneratorNames.GetDomainClassName(targetObjDef);
                        if (partDef.Attr_Mult == 0)
                        {
                            string relVarName = GeneratorNames.GetRelLocalVariableName(relDef, targetObjDef, partDef.Attr_Txt_Phrs);
                        }
                        if (partDef.Attr_Cond==0)
                        {

                        }
                    }
                }
                else if (subRgo is CIMClassR_SUB)
                {
                    var rsupDef = (CIMClassR_SUB)subRgo;
                    var rsubsupDef = rsupDef.LinkedToR213();
                    var relDef = rsubsupDef.CIMSuperClassR_REL();
                    var targetObjDef = rsubsupDef.LinkedFromR212().CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    string targetClassName = GeneratorNames.GetDomainClassName(targetObjDef);
                    string relVarName = GeneratorNames.GetRelLocalVariableName(relDef, targetObjDef, "");
                }
                else if (subRgo is CIMClassR_ASSR)
                {
                    var rassrDef = (CIMClassR_ASSR)subRgo;
                    var rassocDef = rassrDef.LinkedToR211();
                    var relDef = rassocDef.CIMSuperClassR_REL();
                    var raoneDef = rassocDef.LinkedFromR209();
                    var raothDef = rassocDef.LinkedFromR210();

                    var oneObjDef = raoneDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    var oneObjTypeName = GeneratorNames.GetDomainClassName(oneObjDef);
                    var oneArgName = $"oneInstance{GeneratorNames.ToProgramAvailableString(raoneDef.Attr_Txt_Phrs)}";
                    string relVarName = GeneratorNames.GetRelLocalVariableName(relDef, oneObjDef, raoneDef.Attr_Txt_Phrs);


                    var otherObjDef = raothDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    var otherObjTypeName = GeneratorNames.GetDomainClassName(otherObjDef);
                    var otherArgName = $"otherInstance{GeneratorNames.ToProgramAvailableString(raothDef.Attr_Txt_Phrs)}";
                    relVarName = GeneratorNames.GetRelLocalVariableName(relDef, otherObjDef, raothDef.Attr_Txt_Phrs);

                }
            }

            if (joinedRgos.Count() > 0)
            {

            }
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
                        string condition = GetRelCondition(rgo, targetClassName);
                        string unRelCondtion = GetUnrelCondition(rgo, targetClassName, "instance");
                        if (partDef.Attr_Mult == 0)
                        {
                            string returnType = targetClassName;
                            string methodNameLink = GeneratorNames.GetRelationshipMethodName(relDef, "", partDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Link);
                            string methodNameUnLink = GeneratorNames.GetRelationshipMethodName(relDef, "", partDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Unlink);

                            var orefDefs = rgo.LinkedOtherSideR111();
                            foreach(var orefDef in orefDefs)
                            {
                                var thisAttrDef = orefDef.LinkedToR108().CIMSuperClassO_ATTR();
                                var tgtAttrDef = orefDef.LinkedOtherSideR111().LinkedOtherSideR110().LinkedOtherSideR105();
                                string thisAttrName = GeneratorNames.GetAttrPropertyLocalName(thisAttrDef);
                                string tgtAttrName = GeneratorNames.GetAttrPropertyName(tgtAttrDef);
                            }
                            var logLink = new logging.Logging("logger", "", objDef, "this", logging.Logging.Mode.LinkLifeCycle, "link");
                            logLink.oneObjDef = targetObjDef;
                            logLink.oneVarName = "instance";
                            var logLinkGen = logLink.TransformText();

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
                    string condition = GetRelCondition(rgo, targetClassName);
                    string unRelCondtion = GetUnrelCondition(rgo, targetClassName, "instance");
                    string relVarName = GeneratorNames.GetRelLocalVariableName(relDef, targetObjDef, "");
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
                    string condition = GetRelCondition(rgo, oneObjTypeName);

                    string methodNameLinkedOther = GeneratorNames.GetRelationshipMethodName(relDef, "Other", raothDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                    var otherObjDef = raothDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    var otherObjTypeName = GeneratorNames.GetDomainClassName(otherObjDef);
                    var otherArgName = $"otherInstance{GeneratorNames.ToProgramAvailableString(raothDef.Attr_Txt_Phrs)}";
                    condition = GetRelCondition(rgo, otherObjTypeName);

                    var orefDefs = rgo.LinkedOtherSideR111();
                    foreach (var orefDef in orefDefs)
                    {
                        var thisRefAttrDef = orefDef.LinkedToR108().CIMSuperClassO_ATTR();
                        var tgtRtdaDef = orefDef.LinkedOtherSideR111();
                        var tgtAttrDef = tgtRtdaDef.LinkedOtherSideR110().LinkedOtherSideR105();
                        var thisAttrPropLocalName = GeneratorNames.GetAttrPropertyLocalName(thisRefAttrDef);
                        var tgtAttrPropName = GeneratorNames.GetAttrPropertyName(tgtAttrDef);
                        var tgtObjDef = tgtAttrDef.LinkedToR102();
                        var tgtObjTypeName = GeneratorNames.GetDomainClassName(tgtObjDef);
                        var subRtoDef = tgtRtdaDef.LinkedOneSideR110().SubClassR204();
                        if (subRtoDef is CIMClassR_AONE)
                        {

                        }
                        else if (subRtoDef is CIMClassR_AOTH)
                        {

                        }
                    }
                }
            }

            var joinedRtos = DomainClassDefs.GetJoinedRTOs(objDef);
            if (joinedRtos.Count() > 0)
            {

            }
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
                        string condition = GetRelCondition(rformDef.CIMSuperClassR_RGO(), targetClassName, false);

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
                    string condtion = GetRelCondition(rassocDef.LinkedFromR211().CIMSuperClassR_RGO(), targetTypeName, false);
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
                    string condtion = GetRelCondition(rassocDef.LinkedFromR211().CIMSuperClassR_RGO(), targetType, false);
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
                    var subsupDef = superDef.LinkedToR212();
                    var subsupRelDef = subsupDef.CIMSuperClassR_REL();
                    var subClassIFName = GeneratorNames.GetSubRelInterfaceName(subsupRelDef);
                    var subClassGetMethodName = GeneratorNames.GetSubRelClassMethodName(subsupRelDef);
                    var subDefs = subsupDef.LinkedFromR213();
                    string subClassKeys = "";
                    foreach (var subDef in subDefs)
                    {
                        if (!string.IsNullOrEmpty(subClassKeys))
                        {
                            subClassKeys += ", ";
                        }
                        var subObjDef = subDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                        subClassKeys += $"\"{subObjDef.Attr_Key_Lett}\"";
                    }
                    string superGetMethodName = GeneratorNames.GetGetSuperClassMethodName(subsupRelDef,objDef);

                }
            }

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
                            if (rformDef.Attr_Cond == 0)
                            {

                            }
                        }
                        else
                        {
                            if (rformDef.Attr_Cond == 0)
                            {

                            }
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
                    string condtion = GetRelCondition(rassocDef.LinkedFromR211().CIMSuperClassR_RGO(), targetTypeName, false);
                    if (raothDef.Attr_Mult == 0)
                    {
                        if (raothDef.Attr_Cond == 0)
                        {

                        }
                    }
                    else
                    {
                        if (raothDef.Attr_Cond == 0)
                        {

                        }
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
                    string condtion = GetRelCondition(rassocDef.LinkedFromR211().CIMSuperClassR_RGO(), targetType, false);
                    if (raoneDef.Attr_Mult == 0)
                    {
                        if (raoneDef.Attr_Cond == 0)
                        {

                        }
                    }
                    else
                    {
                        if (raoneDef.Attr_Cond == 0)
                        {

                        }
                    }
                }
                else if (subRto is CIMClassR_SUPER)
                {
                    var superDef = (CIMClassR_SUPER)subRto;
                    var subsupDef = superDef.LinkedToR212();
                    var subsupRelDef = subsupDef.CIMSuperClassR_REL();
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

        public static string GetRelCondition(CIMClassR_RGO rgo, string targetClassName, bool rgoToRto = true)
        {
            var orefDefs = rgo.LinkedOtherSideR111();
            string condition = "";
            foreach (var orefDef in orefDefs)
            {
                var rgoAttrDef = orefDef.LinkedToR108().CIMSuperClassO_ATTR();
                var tgtAttrDef = orefDef.LinkedOtherSideR111().LinkedOtherSideR110().LinkedOtherSideR105();
                var tgtObjDef = tgtAttrDef.LinkedToR102();
                string tgtClassName = GeneratorNames.GetDomainClassName(tgtObjDef);
                if (tgtClassName == targetClassName)
                {
                    var rgoAttrPropVarName = GeneratorNames.GetAttrPropertyName(rgoAttrDef);
                    var tgtAttrPropVarName = GeneratorNames.GetAttrPropertyName(tgtAttrDef);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        condition += " && ";
                    }
                    if (rgoToRto)
                    {
                        condition += $"this.{rgoAttrPropVarName}==(({targetClassName})inst).{tgtAttrPropVarName}";
                    }
                    else
                    {
                        string castType = targetClassName;
                        //if (rgo.SubClassR205() is CIMClassR_FORM)
                        //{
                            var otherSideObjDef = rgo.CIMSuperClassR_OIR().LinkedOtherSideR201();
                            castType = GeneratorNames.GetDomainClassName(otherSideObjDef);
                        //}
                        condition += $"this.{tgtAttrPropVarName}==(({castType})inst).{rgoAttrPropVarName}";
                    }
                }
            }
            return condition;
        }

        public void prototypeForStorage()
        {
            string changedStateClassName = "";
            string changedStateVarName = "";
            GeneratorNames.GetChangedStoreVariable(ref changedStateClassName, ref changedStateVarName);
            var storageAttrDefs = objDef.LinkedFromR102();
            foreach (var attrDef in storageAttrDefs)
            {
                var dtDef = attrDef.LinkedToR114();
                var subDtDef = dtDef.SubClassR17();
                string dataTypeName = DomainDataTypeDefs.GetDataTypeName(dtDef);
                string localPropVarName = GeneratorNames.GetAttrPropertyLocalName(attrDef);
                string stateOfPropVarName = GeneratorNames.GetPropertyStateVariableName(attrDef);
            }
        }

        public static string GetUnrelCondition(CIMClassR_RGO rgo, string targetClassName, string targetVarName)
        {
            var orefDefs = rgo.LinkedOtherSideR111();
            string condition = "";
            foreach (var orefDef in orefDefs)
            {
                var thisAttrDef = orefDef.LinkedToR108().CIMSuperClassO_ATTR();
                var tgtAttrDef = orefDef.LinkedOtherSideR111().LinkedOtherSideR110().LinkedOtherSideR105();
                var tgtObjDef = tgtAttrDef.LinkedToR102();
                string tgtClassName = GeneratorNames.GetDomainClassName(tgtObjDef);
                if (tgtClassName == targetClassName)
                {
                    var thisAttrPropVarName = GeneratorNames.GetAttrPropertyName(thisAttrDef);
                    var tgtAttrPropVarName = GeneratorNames.GetAttrPropertyName(tgtAttrDef);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        condition += " && ";
                    }
                    condition += $"this.{thisAttrPropVarName}=={targetVarName}.{tgtAttrPropVarName}";
                }
            }
            return condition;
        }

        public static string GetPropertyValueDomainRegexPattern(string descrip)
        {
            string pattern = null;
            using (var reader = new StringReader(descrip))
            {
                string coloring = "@domain:";
                string line = null;
                while((line=reader.ReadLine()) != null)
                {
                    if (line.StartsWith(coloring))
                    {
                        string patternDef = line.Substring(coloring.Length);
                        string[] frags = patternDef.Split(new char[] { '=' });
                        if (frags.Length == 2)
                        {
                            if (frags[0] == "pattern")
                            {
                                pattern = frags[1];
                                pattern = pattern.Substring(1, pattern.Length - 2);
                            }
                        }
                    }
                }
            }
            return pattern;
        }

        public static string GetIdentityPropertiesArgsInFormattedString(CIMClassO_OBJ objDef, string varName)
        {
            string result = "";
            var idAttrs = new Dictionary<string, CIMClassO_ATTR>();
            var oidDefs = objDef.LinkedFromR104();
            foreach (var oidDef in oidDefs)
            {
                if (oidDef.Attr_Oid_ID == 0)
                {
                    var oidaDefs = oidDef.LinkedOtherSideR105();
                    foreach (var oidaDef in oidaDefs)
                    {
                        var attrDef = oidaDef.LinkedOtherSideR105();
                        if (!idAttrs.ContainsKey(attrDef.Attr_Name))
                        {
                            idAttrs.Add(attrDef.Attr_Name, attrDef);
                        }
                    }
                }
            }
            foreach (var attrName in idAttrs.Keys)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ",";
                }
                string propName = GeneratorNames.GetAttrPropertyName(idAttrs[attrName]);
                result += $"{idAttrs[attrName].Attr_Name}=" +"{" + $"{varName}.{propName}"+ "}";
            }
            return result;
        }

        public static string GetLinkIdentitiePropertiesArgsInFormattedString(string varName, IEnumerable<CIMClassO_ATTR> idAttrDefs)
        {
            string result = "";
            foreach (var attrDef in idAttrDefs)
            {
                string propName = GeneratorNames.GetAttrPropertyName(attrDef);
                if (!string.IsNullOrEmpty(result))
                {
                    result += ",";
                    result += "{" + $"{varName}.{propName}:{attrDef.Attr_Name}" + "}";
                }
            }
            return result;
        }

        public static bool IsIdentityAttribute(CIMClassO_ATTR attrDef)
        {
            bool isIdentity = false;
            var oidDefs = attrDef.LinkedToR102().LinkedFromR104();
            foreach(var oidDef in oidDefs)
            {
                var oidaDefs = oidDef.LinkedOtherSideR105();
                foreach(var oidaDef in oidaDefs)
                {
                    var idAttrDef = oidaDef.LinkedOtherSideR105();
                    if (attrDef.Attr_Attr_ID == idAttrDef.Attr_Attr_ID)
                    {
                        isIdentity = true;
                        break;
                    }
                }
            }
            return isIdentity;
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
}
