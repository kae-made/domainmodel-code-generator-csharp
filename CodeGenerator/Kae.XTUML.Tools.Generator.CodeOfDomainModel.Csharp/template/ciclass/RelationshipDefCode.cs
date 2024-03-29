﻿// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.ciclass
{
    partial class RelationshipDef
    {
        string indent;
        CIMClassO_OBJ objDef;
        bool genImplCode;

        public RelationshipDef(string indent, CIMClassO_OBJ objDef, bool genImplCode)
        {
            this.indent = indent;
            this.objDef = objDef;
            this.genImplCode = genImplCode;
        }

        public void prototype(CIMClassR_SUB subDef)
        {
            var subObjDef = subDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
            string subObjKey = subObjDef.Attr_Key_Lett;
        }

        public static string GetRelCondition(CIMClassR_RGO rgo, string targetClassName, string phrase, bool rgoToRto = true)
        {
            var orefDefs = rgo.LinkedOtherSideR111();
            string condition = "";
            foreach (var orefDef in orefDefs)
            {
                var rtoDef = orefDef.LinkedOtherSideR111().LinkedOneSideR110();
                var subRtoDef = rtoDef.SubClassR204();
                string targetTxtPhrs = "";
                if (subRtoDef is CIMClassR_AONE)
                {
                    targetTxtPhrs = ((CIMClassR_AONE)subRtoDef).Attr_Txt_Phrs;
                }
                else if (subRtoDef is CIMClassR_AOTH)
                {
                    targetTxtPhrs = ((CIMClassR_AOTH)subRtoDef).Attr_Txt_Phrs;
                }
                else
                {
                    targetTxtPhrs = phrase;
                }
                var rgoAttrDef = orefDef.LinkedToR108().CIMSuperClassO_ATTR();
                var tgtAttrDef = orefDef.LinkedOtherSideR111().LinkedOtherSideR110().LinkedOtherSideR105();
                var tgtObjDef = tgtAttrDef.LinkedToR102();
                string tgtClassName = GeneratorNames.GetDomainClassName(tgtObjDef);
                if (tgtClassName == targetClassName && targetTxtPhrs == phrase)
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

        public static string GetUnrelCondition(CIMClassR_RGO rgo, string targetClassName, string targetVarName, string phrase)
        {
            var orefDefs = rgo.LinkedOtherSideR111();
            string condition = "";
            foreach (var orefDef in orefDefs)
            {
                var rtoDef = orefDef.LinkedOtherSideR111().LinkedOneSideR110();
                var subRtoDef = rtoDef.SubClassR204();
                string targetTxtPhrs = "";
                if (subRtoDef is CIMClassR_AONE)
                {
                    targetTxtPhrs = ((CIMClassR_AONE)subRtoDef).Attr_Txt_Phrs;
                }
                else if (subRtoDef is CIMClassR_AOTH)
                {
                    targetTxtPhrs = ((CIMClassR_AOTH)subRtoDef).Attr_Txt_Phrs;
                }
                else
                {
                    targetTxtPhrs = phrase;
                }
                var thisAttrDef = orefDef.LinkedToR108().CIMSuperClassO_ATTR();
                var tgtAttrDef = orefDef.LinkedOtherSideR111().LinkedOtherSideR110().LinkedOtherSideR105();
                var tgtObjDef = tgtAttrDef.LinkedToR102();
                string tgtClassName = GeneratorNames.GetDomainClassName(tgtObjDef);
                if (tgtClassName == targetClassName && targetTxtPhrs == phrase)
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

        public static CIMClassO_ATTR GetFormalizedO_ATTRForR_SUPER(CIMClassR_SUPER superDef)
        {
            var oirDef = superDef.CIMSuperClassR_RTO().LinkedToR109();
            if (oirDef != null)
            {
                if (oirDef.Attr_Oid_ID == 0)
                {
                    var oidaDefs = oirDef.LinkedOtherSideR105();
                    foreach(var oidaDef in oidaDefs)
                    {
                        var attrDef = oidaDef.LinkedOtherSideR105();
                        return attrDef;
                    }
                }
            }
            return null;
        }
    }
}
