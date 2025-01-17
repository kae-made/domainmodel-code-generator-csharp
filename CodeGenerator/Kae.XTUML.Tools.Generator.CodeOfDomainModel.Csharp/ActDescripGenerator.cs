﻿// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Tools.Generator.Coloring;
using Kae.Tools.Generator.Coloring.DomainWeaving;
using Kae.Utility.Logging;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp
{
    internal class ActDescripGenerator
    {
        private CIMClassACT_ACT actDef;
        private string baseIndent;
        private string indent;
        private string selfVarNameOnCode;
        private IDictionary<string, CIMClassS_EE> usedExternalEntities;
        private Logger logger;
        private bool isAzureDigitalTwins;
        private bool isAzureIoTHub;

        private List<Dictionary<string, VariableDef>> declaredVariables = new List<Dictionary<string, VariableDef>>();

        private class TempSetVarFolder
        {
            private List<List<string>> declaredTempVariable = new List<List<string>>();
            public void AddBlock()
            {
                declaredTempVariable.Add(new List<string>());
            }
            public void DeleteBlock()
            {
                declaredTempVariable.RemoveAt(declaredTempVariable.Count - 1);
            }
            public void Declared(string varName)
            {
                declaredTempVariable[declaredTempVariable.Count - 1].Add(varName);
            }
            public bool HasDeclared(string varName)
            {
                for (int i = declaredTempVariable.Count - 1; i >= 0; i--)
                {
                    if (declaredTempVariable[i].Contains(varName))
                        return true;
                }
                declaredTempVariable[declaredTempVariable.Count - 1].Add(varName);
                return false;
            }
        }
        private TempSetVarFolder currentTempSetVarFolder;

        ColoringManager coloringManager;

        public ActDescripGenerator(CIMClassACT_ACT actDef, string selfVarName, string baseIndent, string indent, IDictionary<string, CIMClassS_EE> usedEEs, ColoringManager coloringManager, bool azureDigitalTwins, bool azureIoTHub, Logger logger)
        {
            this.actDef = actDef;
            this.selfVarNameOnCode = selfVarName;
            this.baseIndent = baseIndent;
            this.indent = indent;
            this.usedExternalEntities = usedEEs;
            this.coloringManager = coloringManager;
            this.isAzureDigitalTwins = azureDigitalTwins;
            this.isAzureIoTHub = azureIoTHub;
            this.logger = logger;
            this.currentTempSetVarFolder = new TempSetVarFolder();
        }

        Dictionary<string, string> eeReferDefs = new Dictionary<string, string>();
        public string Generate()
        {
            var gen = GenCode();
            if (gen == false)
            {
                return $"{indent}// mark - no generation";
            }

            string resultCode = "";

            bool overwriteByColoring = false;
            var codeBefores = new List<string>();
            var codeAfters = new List<string>();
            if (coloringManager != null)
            {
                IList<ColoringForInstance> colorings;
                if (coloringManager.HasColoring(actDef, out colorings))
                {
                    // 一つでもOverwriteがあれば、Action記述からの生成は行わない
                    // Overwrite 指定されている場合は、Before、Body、After の順で生成する。
                    // Overwrite 指定が無い場合は、Before、Afterで囲み順番に階層を形成する
                    var sbBody = new StringBuilder();
                    var writerBody = new StringWriter(sbBody);
                    var coloredGenCodes = coloringManager.GenerateForColoring(colorings, indent, baseIndent);
                    for (int cIndex = 0; cIndex < colorings.Count; cIndex++)
                    {
                        var ccodeGen = coloredGenCodes[cIndex];
                        var coloring = colorings[cIndex];
                        if ((ccodeGen.AddStyle & GenerationResult.Style.Overwrite) != 0)
                        {
                            if (!string.IsNullOrEmpty(sbBody.ToString()))
                            {
                                writerBody.WriteLine();
                            }
                            overwriteByColoring = true;
                            var sbCodeFrag = new StringBuilder();
                            var writerFrag = new StringWriter(sbCodeFrag);
                            writerFrag.WriteLine($"{indent}// code by coloring of {coloring.Specification.Name}");
                            if ((ccodeGen.AddStyle & GenerationResult.Style.AddBefore) != 0)
                            {
                                writerFrag.WriteLine($"{indent}// code before");
                                writerFrag.Write(ccodeGen.CodeBefore);
                                writerFrag.WriteLine();
                            }
                            writerFrag.WriteLine($"{indent}// code body");
                            writerFrag.Write(ccodeGen.CodeOverwrite);
                            if ((ccodeGen.AddStyle & GenerationResult.Style.AddAfter) != 0)
                            {
                                writerFrag.WriteLine();
                                writerFrag.WriteLine($"{indent}// code after");
                                writerFrag.Write(ccodeGen.CodeAfter);
                            }
                            writerBody.Write(sbCodeFrag.ToString());
                        }
                        else
                        {
                            if ((ccodeGen.AddStyle & GenerationResult.Style.AddBefore) != 0)
                            {
                                var sbCodeFrag = new StringBuilder();
                                var writerFrag = new StringWriter(sbCodeFrag);
                                writerFrag.WriteLine($"{indent}// code before by coloring of {coloring.Specification.Name}");
                                writerFrag.Write(ccodeGen.CodeBefore);
                                codeBefores.Add(writerFrag.ToString());
                            }
                            if ((ccodeGen.AddStyle & GenerationResult.Style.AddAfter) != 0)
                            {
                                var sbCodeFrag = new StringBuilder();
                                var writerFrag = new StringWriter(sbCodeFrag);
                                writerFrag.WriteLine($"{indent}// code after by coloring of {coloring.Specification.Name}");
                                writerFrag.Write(ccodeGen.CodeBefore);
                                codeAfters.Insert(0, writerFrag.ToString());
                            }
                        }
                    }
                    resultCode = sbBody.ToString();
                }
            }

            if (overwriteByColoring == false)
            {
                string code = "";
                var blkDef = actDef.LinkedToR666();
                if (blkDef != null)
                {
                    code = GenerateBlock(blkDef);
                }

                var bicDef = actDef.LinkedOneSideR694();
                var bieDef = actDef.LinkedOtherSideR640();
                var parsed2BlkDef = actDef.LinkedToR650();

                var currentScopeBlkDef = actDef.LinkedToR699();

                indent += baseIndent;

                var sb = new StringBuilder();
                var writer = new StringWriter(sb);
                if (eeReferDefs.Count > 0)
                {
                    writer.WriteLine($"{indent}// External Entities Reference Declarations.");
                    foreach (var eeKey in eeReferDefs.Keys)
                    {
                        writer.WriteLine($"{indent}{eeReferDefs[eeKey]};");
                        writer.WriteLine("");
                    }
                }
                if (hasUsedSelectedVar)
                {
                    writer.WriteLine($"{indent}DomainClassDef selected = null;");
                    writer.WriteLine("");
                }
                writer.Write(code);
                resultCode = sb.ToString();
            }

            if (codeBefores.Count > 0)
            {
                var sb = new StringBuilder();
                var writer = new StringWriter(sb);
                for (int index = 0; index < codeBefores.Count; index++)
                {
                    if (index > 0)
                    {
                        writer.WriteLine();
                    }
                    writer.Write(codeBefores[index]);
                }
                writer.WriteLine();
                writer.Write(resultCode);
                resultCode = sb.ToString();
            }
            if (codeAfters.Count > 0)
            {
                var sb = new StringBuilder();
                var writer = new StringWriter(sb);
                writer.Write(resultCode);
                foreach (var codeAfter in codeAfters)
                {
                    writer.WriteLine();
                    writer.Write(codeAfter);
                }
                resultCode = sb.ToString();
            }

            return resultCode;
        }

        public bool GenCode()
        {
            bool result = true;
            string actionDescrip = "";
            var subActDef = actDef.SubClassR698();
            if (subActDef is CIMClassACT_FNB)
            {
                var syncDef = ((CIMClassACT_FNB)subActDef).LinkedToR695();
                actionDescrip = syncDef.Attr_Action_Semantics;
            }
            else if (subActDef is CIMClassACT_OPB)
            {
                var tfrDef = ((CIMClassACT_OPB)subActDef).LinkedToR696();
                actionDescrip = tfrDef.Attr_Action_Semantics;
            }
            else if (subActDef is CIMClassACT_SAB)
            {
                var smActDef = ((CIMClassACT_SAB)subActDef).LinkedToR691();
                actionDescrip = smActDef.Attr_Action_Semantics;
            }
            else if (subActDef is CIMClassACT_DAB)
            {
                var dbattrDef = ((CIMClassACT_DAB)subActDef).LinkedToR693();
                actionDescrip = dbattrDef.Attr_Action_Semantics;
            }
            if (!string.IsNullOrEmpty(actionDescrip))
            {
                using (var reader = new StringReader(actionDescrip))
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith("//"))
                        {
                            string genColor = "@gen:";
                            int index = line.IndexOf(genColor);
                            if (index > 0)
                            {
                                if (line.Substring(index + genColor.Length).StartsWith("false"))
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        protected int relatedByCodeIndex = 0;

        protected string GenerateBlock(CIMClassACT_BLK blkDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            declaredVariables.Add(new Dictionary<string, VariableDef>());
            blockDepth++;
            currentTempSetVarFolder.AddBlock();

            var varDefs = blkDef.LinkedFromR823();
            foreach (var varDef in varDefs)
            {
                var variableDef = new VariableDef() { Name = varDef.Attr_Name, Declared = false, VarDef = varDef };
                declaredVariables[declaredVariables.Count - 1].Add(variableDef.Name, variableDef);
                var dtDef = varDef.LinkedToR848();
                string dataTypeName = "";
                var subVarDef = varDef.SubClassR814();
                if (subVarDef is CIMClassV_INT)
                {
                    if (variableDef.Name.ToLower() == "self")
                    {
                        variableDef.Name = this.selfVarNameOnCode;
                        variableDef.Declared = true;
                    }
                    var vIntDef = (CIMClassV_INT)subVarDef;
                    var objDef = vIntDef.LinkedToR818();
                    variableDef.Set = false;
                    dataTypeName = GeneratorNames.GetDomainClassName(objDef);
                }
                else if (subVarDef is CIMClassV_INS)
                {
                    var vInsDef = (CIMClassV_INS)subVarDef;
                    var objDef = vInsDef.LinkedToR819();
                    variableDef.Set = true;
                    dataTypeName = $"List<{GeneratorNames.GetDomainClassName(objDef)}>";
                }
                else if (subVarDef is CIMClassV_TRN)
                {
                    var vTrnDef = (CIMClassV_TRN)subVarDef;
                    var sDimDef = vTrnDef.LinkedFromR844();
                    var trnDtDef = vTrnDef.LinkedToR821();
                    if (trnDtDef != null)
                    {
                        dataTypeName = DomainDataTypeDefs.GetDataTypeName(trnDtDef);
                    }
                }
                // writer.WriteLine($"{indent}{dataTypeName} {varDef.Attr_Name};");
            }
            var selectedVarDef = HasDeclaredVariable("selected");
            if (selectedVarDef == null)
            {
                selectedVarDef = new VariableDef() { Declared = false, Name = "selected", Set = false };
                declaredVariables[declaredVariables.Count - 1].Add(selectedVarDef.Name, selectedVarDef);
            }

            var valDefs = blkDef.LinkedFromR826();
            foreach (var valDef in valDefs)
            {
                var dtDef = valDef.LinkedToR820();
                var subValDef = valDef.SubClassR801();
                if (subValDef is CIMClassV_PVL)
                {
                    var vPvl = (CIMClassV_PVL)subValDef;
                    string name = "";
                    var bParm = vPvl.LinkedToR831();
                    if (bParm != null)
                    {
                        name = bParm.Attr_Name;
                    }
                    var sParm = vPvl.LinkedToR832();
                    if (sParm != null)
                    {
                        name = sParm.Attr_Name;
                    }
                    var tParm = vPvl.LinkedToR833();
                    if (tParm != null)
                    {
                        name = tParm.Attr_Name;
                    }
                    if (!declaredVariables[declaredVariables.Count - 1].ContainsKey(name))
                    {
                        var variableDef = new VariableDef() { Name = name, Declared = true, PvlDef = vPvl };
                        declaredVariables[declaredVariables.Count - 1].Add(name, variableDef);
                    }
                }
                else if (subValDef is CIMClassV_EDV)
                {
                    var vEdvDef = (CIMClassV_EDV)subValDef;
                    var vEprDefs = vEdvDef.LinkedFromR834();
                    foreach (var vEprDef in vEprDefs)
                    {
                        var edvdiDef = vEprDef.LinkedToR846();
                        var cppDef = vEprDef.LinkedToR847();
                        if (!declaredVariables[declaredVariables.Count - 1].ContainsKey(edvdiDef.Attr_Name))
                        {
                            var variableDef = new VariableDef() { Name = edvdiDef.Attr_Name, Declared = true, EvtDiDef = edvdiDef };
                            declaredVariables[declaredVariables.Count - 1].Add(variableDef.Name, variableDef);
                        }
                    }
                }
            }

            // indent inc
            indent += baseIndent;

            var teBlkDef = blkDef.LinkedFromR2016();
            var smtDefs = blkDef.LinkedFromR602();
            CIMClassACT_SMT firstSmtDef = null;
            if (smtDefs.Count() > 0)
            {
                firstSmtDef = smtDefs.ElementAt(0);
                while (true)
                {
                    var prevSmtDef = firstSmtDef.LinkedFromR661Succeeds();
                    if (prevSmtDef == null)
                    {
                        break;
                    }
                    firstSmtDef = prevSmtDef;
                }
            }
            if (firstSmtDef != null)
            {
                var currentStmtDef = firstSmtDef;
                while (currentStmtDef != null)
                {
                    writer.WriteLine($"{indent}// Line : {currentStmtDef.Attr_LineNumber}");
                    string code = GenerateStatement(currentStmtDef);
                    writer.WriteLine(code);
                    currentStmtDef = currentStmtDef.LinkedToR661Precedes();
                }
            }

            var actForDef = blkDef.LinkedFromR605();
            var actEDef = blkDef.LinkedFromR606();
            var actIfDef = blkDef.LinkedFromR607();
            var actWhlDef = blkDef.LinkedFromR608();
            var actActDef = blkDef.LinkedFromR650();
            var actElDef = blkDef.LinkedFromR658();
            var actDef666 = blkDef.LinkedFromR666();
            var actDef699 = blkDef.LinkedFromR699();
            var vVarDefs = blkDef.LinkedFromR823();
            var vValDefs = blkDef.LinkedFromR826();
            var bsfDefs = blkDef.LinkedOtherSideR2923();
            var actDef612 = blkDef.LinkedToR612();

            // indent decl
            indent = indent.Substring(baseIndent.Length);

            blockDepth--;
            declaredVariables.RemoveAt(declaredVariables.Count - 1);
            foreach (var dVar in declaredVariables)
            {
                foreach (var vk in dVar.Keys)
                {
                    if (dVar[vk].DeclaredDepth > blockDepth)
                    {
                        dVar[vk].Declared = false;
                    }
                }
            }
            currentTempSetVarFolder.DeleteBlock();

            return sb.ToString();
        }

        protected string GenerateStatement(CIMClassACT_SMT smtDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            string code = "";

            // Bad idea!
            // writer.WriteLine(indent + "{");
            // indent += baseIndent;

            var subSmtDef = smtDef.SubClassR603();
            if (subSmtDef is CIMClassACT_FOR)
            {
                code = GenerateACT_For((CIMClassACT_FOR)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_WHL)
            {
                code = GenerateACT_While((CIMClassACT_WHL)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_IF)
            {
                code = GenerateACT_If((CIMClassACT_IF)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_EL)
            {
                code = GenerateACT_Else((CIMClassACT_EL)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_E)
            {
                code = GenerateACT_E((CIMClassACT_E)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_BRG)
            {
                code = GenerateACT_Bridge((CIMClassACT_BRG)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_FNC)
            {
                code = GenerateACT_Function((CIMClassACT_FNC)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_RET)
            {
                code = GenerateACT_Return((CIMClassACT_RET)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_TFM)
            {
                code = GenerateACT_Transform((CIMClassACT_TFM)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_AI)
            {
                code = GenerateACT_Assign((CIMClassACT_AI)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_DEL)
            {
                code = GenerateACT_Delete((CIMClassACT_DEL)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_CNV)
            {
                code = GenerateACT_CNV((CIMClassACT_CNV)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_CR)
            {
                code = GenerateACT_Create((CIMClassACT_CR)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_SEL)
            {
                code = GenerateACT_SelectRelatedBy((CIMClassACT_SEL)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_FIO)
            {
                code = GenerateACT_SelectFrom((CIMClassACT_FIO)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_FIW)
            {
                // SELECT FROM INSTANECES OF WHERE
                code = GenerateACT_SelectFromWhere((CIMClassACT_FIW)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_URU)
            {
                code = GenerateACT_UnrelateUsing((CIMClassACT_URU)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_UNR)
            {
                code = GenerateACT_Unrelate((CIMClassACT_UNR)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_RU)
            {
                code = GenerateACT_RelateUsing((CIMClassACT_RU)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_REL)
            {
                // Relate Across
                code = GenerateACT_Relate((CIMClassACT_REL)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_CTL)
            {
                code = GenerateACT_CTL((CIMClassACT_CTL)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_BRK)
            {
                code = GenerateACT_Break((CIMClassACT_BRK)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_CON)
            {
                code = GenerateACT_Continue((CIMClassACT_CON)subSmtDef);
            }
            else if (subSmtDef is CIMClassE_ESS)
            {
                // GENERATE EVENT TO instance
                code = GenerateACT_ESS((CIMClassE_ESS)subSmtDef);
            }
            else if (subSmtDef is CIMClassE_GPR)
            {
                code = GenerateACT_GPR((CIMClassE_GPR)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_IOP)
            {
                code = GenerateACT_IOP((CIMClassACT_IOP)subSmtDef);
            }
            else if (subSmtDef is CIMClassACT_SGN)
            {
                code = GenerateACT_SGN((CIMClassACT_SGN)subSmtDef);
            }
            else
            {
                code = $"// {subSmtDef.GetType().Name} is not supported.";
            }

            writer.Write(code);

            // Bad Idea!
            // indent = indent.Substring(baseIndent.Length);
            // writer.WriteLine(indent + "}");

            return sb.ToString();
        }

        protected string GenerateACT_For(CIMClassACT_FOR actForDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            var blkDef = actForDef.LinkedToR605();
            var varLoopDef = actForDef.LinkedToR614();
            var varSetDef = actForDef.LinkedToR652();
            var objDef = actForDef.LinkedToR670();
            string bodyCode = GenerateBlock(blkDef);

            var loopVariableDef = HasDeclaredVariable(varLoopDef.Attr_Name);
            string tmpVarPostfix = "";
            if (loopVariableDef.Declared)
            {
                tmpVarPostfix = "_";
            }
            writer.WriteLine($"{indent}foreach (var {varLoopDef.Attr_Name}{tmpVarPostfix} in {varSetDef.Attr_Name})");
            writer.WriteLine($"{indent}" + "{");
            if (!string.IsNullOrEmpty(tmpVarPostfix))
            {
                writer.WriteLine($"{indent}{baseIndent}{varLoopDef.Attr_Name} = {varLoopDef.Attr_Name}{tmpVarPostfix};");
            }
            writer.Write(bodyCode);
            writer.WriteLine($"{indent}" + "}");

            return sb.ToString();
        }
        protected string GenerateACT_While(CIMClassACT_WHL actWhlDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var blkDef = actWhlDef.LinkedToR608();
            var condValDef = actWhlDef.LinkedToR626();
            bool isDecl;
            string condVarName;
            string condCode = GenerateV_VAL(condValDef, out condVarName);
            string bodyCode = GenerateBlock(blkDef);
            writer.WriteLine($"{indent}while ({condCode})");
            writer.WriteLine(indent + "{");
            writer.Write(bodyCode);
            writer.WriteLine(indent + "}");

            return sb.ToString();
        }
        protected string GenerateACT_If(CIMClassACT_IF actIfDef)
        {
            string code = "";

            var val1Def = actIfDef.LinkedToR625();
            bool isDecl;
            string condVarName;
            string conditionVal = GenerateV_VAL(val1Def, out condVarName);
            var actBlkDef = actIfDef.LinkedToR607();
            string bodyCode = GenerateBlock(actBlkDef);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            writer.WriteLine($"{indent}if ({conditionVal})");
            writer.WriteLine(indent + "{");
            writer.Write(bodyCode);
            writer.WriteLine(indent + "}");

            var actELDefs = actIfDef.LinkedFromR682();
            foreach (var elDef in actELDefs)
            {
                var elBlkDef = elDef.LinkedToR658();
                var elValDef = elDef.LinkedToR659();
                string elBodyCode = GenerateBlock(elBlkDef);
                string elCondValCode = GenerateV_VAL(elValDef, out condVarName);
                writer.WriteLine($"{indent}else if ({elCondValCode})");
                writer.WriteLine(indent + "{");
                writer.Write(elBodyCode);
                writer.WriteLine(indent + "}");
            }

            var actE1Def = actIfDef.LinkedFromR683();
            if (actE1Def != null)
            {
                var elseBlkDef = actE1Def.LinkedToR606();
                bodyCode = GenerateBlock(elseBlkDef);
                writer.WriteLine($"{indent}else");
                writer.WriteLine(indent + "{");
                writer.Write(bodyCode);
                writer.WriteLine(indent + "}");
            }
            var actELDef = actIfDef.LinkedToR690();
            var actE2Def = actIfDef.LinkedToR692();

            return sb.ToString();
        }
        protected string GenerateACT_Else(CIMClassACT_EL actElDef)
        {
            throw new NotImplementedException();
        }
        protected string GenerateACT_E(CIMClassACT_E actEDef)
        {
            throw new NotImplementedException();
        }
        protected string GenerateACT_Bridge(CIMClassACT_BRG actBrgDef)
        {
            var brgDef = actBrgDef.LinkedToR674();
            var paramDefs = actBrgDef.LinkedFromR628();
            var eeDef = brgDef.LinkedToR19();
            string paramCode = "";
            foreach (var paramDef in paramDefs)
            {
                var paramValDef = paramDef.LinkedToR800();
                string varName;
                string paramValCode = GenerateV_VAL(paramValDef, out varName);
                if (!string.IsNullOrEmpty(paramCode))
                {
                    paramCode += ", ";
                }
                paramCode += $"{paramDef.Attr_Name}:{paramValCode}";
            }

            string eeWrapper = GeneratorNames.GetExternalEntityWrappterClassName(eeDef, isAzureDigitalTwins);
            string eeWrapperRefVarName = GeneratorNames.GetExternalEntityWrapperRefVarName(eeDef);
            if (!eeReferDefs.ContainsKey(eeDef.Attr_Key_Lett))
            {
                string eeRefCode = $"var {eeWrapperRefVarName} = ({eeWrapper})instanceRepository.GetExternalEntity(\"{eeDef.Attr_Key_Lett}\")";
                eeReferDefs.Add(eeDef.Attr_Key_Lett, eeRefCode);
            }

            return $"{indent}{eeWrapperRefVarName}.{brgDef.Attr_Name}({paramCode});";

        }
        protected string GenerateACT_Function(CIMClassACT_FNC actFncDef)
        {
            // Invoke Domain Operation without return;
            var syncDef = actFncDef.LinkedToR675();
            var syncParamDefs = actFncDef.LinkedFromR669();
            string paramCode = "";
            foreach (var syncParamDef in syncParamDefs)
            {
                var spValDef = syncParamDef.LinkedToR800();
                string varName;
                string spValCode = GenerateV_VAL(spValDef, out varName);
                if (!string.IsNullOrEmpty(paramCode))
                {
                    paramCode += ", ";
                }
                paramCode += $"{syncParamDef.Attr_Name}:{spValCode}";
            }
            return $"{indent}this.{syncDef.Attr_Name}({paramCode});";
        }
        protected string GenerateACT_Return(CIMClassACT_RET actRetDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var retValDef = actRetDef.LinkedToR668();
            string varName;
            string code = GenerateV_VAL(retValDef, out varName);

            writer.WriteLine($"{indent}instanceRepository.SyncChangedStates(changedStates);");
            writer.WriteLine($"{indent}return {code};");
            return sb.ToString();
        }
        protected string GenerateACT_Transform(CIMClassACT_TFM actTfmDef)
        {
            // Invoke Domain Class Operation
            var vparDefs = actTfmDef.LinkedFromR627();
            var tfrDef = actTfmDef.LinkedToR673();
            var varDef = actTfmDef.LinkedToR667();
            string paramCode = "";
            foreach (var vparDef in vparDefs)
            {
                var valDef = vparDef.LinkedToR800();
                bool isDecl;
                string pVarName;
                string valCode = GenerateV_VAL(valDef, out pVarName);
                if (!string.IsNullOrEmpty(paramCode))
                {
                    paramCode += ", ";
                }
                paramCode += $"{vparDef.Attr_Name}:{valCode}";
            }
            string instVarName = varDef.Attr_Name;
            if (instVarName.ToLower() == "self")
            {
                instVarName = this.selfVarNameOnCode;
            }
            string implCode = $"{indent}{instVarName}.{tfrDef.Attr_Name}({paramCode});";

            return implCode;
        }
        protected string GenerateACT_Assign(CIMClassACT_AI actAiDef)
        {
            string code = $"{indent}";

            var rValDef = actAiDef.LinkedToR609();
            var lValDef = actAiDef.LinkedToR689();

            bool isDeclLVar = false;
            string lVarName;
            string lValCode = $"{GenerateV_VAL(lValDef, out lVarName)}";

            if (!string.IsNullOrEmpty(lVarName))
            {
                var lVarDef = HasDeclaredVariable(lVarName);
                if (lVarDef.Declared == false)
                {
                    code += "var ";
                    DeclaredVariable(lVarDef);
                }
            }

            string rVarName;
            code += $"{lValCode} = {GenerateV_VAL(rValDef, out rVarName)};";

            return code;
        }
        protected string GenerateACT_Delete(CIMClassACT_DEL actDelDef)
        {
            var varDef = actDelDef.LinkedToR634();
            string varName = varDef.Attr_Name;
            if (varName.ToLower() == "self")
            {
                varName= this.selfVarNameOnCode;
            }
            return $"{indent}{varName}.DeleteInstance(changedStates);";
        }
        protected string GenerateACT_CNV(CIMClassACT_CNV actCnvDef)
        {
            var objDef = actCnvDef.LinkedToR672();
            throw new NotImplementedException();
        }
        protected string GenerateACT_Create(CIMClassACT_CR actCrDef)
        {
            string code = $"{indent}";

            var varDef = actCrDef.LinkedToR633();
            var objDef = actCrDef.LinkedToR671();
            string domainClassImplName = GeneratorNames.GetDomainClassImplName(objDef);

            var instVarDef = HasDeclaredVariable(varDef.Attr_Name);
            //           if (instVarDef == null)
            //         {
            //           instVarDef = new VariableDef() { Declared = false, Name = varDef.Attr_Name, Set = false, VarDef = varDef };
            //         declaredVariables[declaredVariables.Count - 1].Add(varDef.Attr_Name, instVarDef);
            //   }
            if (instVarDef.Declared == false)
            {
                code += "var ";
                //                instVarDef.Declared = true;
                DeclaredVariable(instVarDef);
            }
            code += $"{varDef.Attr_Name} = {domainClassImplName}.CreateInstance(instanceRepository, logger, changedStates);";

            return code;
        }
        protected string GenerateACT_SelectRelatedBy(CIMClassACT_SEL actSelDef)
        {
            // SELECT ANY RELATED BY
            // SELECT ONE RELATED BY
            // SELECT MANY RELATED BY
            // SELECT MANY RELATED BY WHERE
            string code = "";

            var lnkDef = actSelDef.LinkedFromR637();
            var valSourceDef = actSelDef.LinkedToR613();
            var varDestDef = actSelDef.LinkedToR638();
            string srcVarName;
            string VarSourceCode = GenerateV_VAL(valSourceDef, out srcVarName);
            var teLnkDef = lnkDef.LinkedFromR2042();
            CIMClassO_OBJ destObjDef = null;

            var subActSelDef = actSelDef.SubClassR664();
            string whereCode = "";
            if (subActSelDef is CIMClassACT_SRW)
            {
                var srwDef = (CIMClassACT_SRW)subActSelDef;
                var whereValDef = srwDef.LinkedToR611();
                string whereVarName;
                whereCode = GenerateV_VAL(whereValDef, out whereVarName);
            }

            code = GenerateRelatedCode(actSelDef.Attr_cardinality, lnkDef, valSourceDef, varDestDef, whereCode);
            while (lnkDef != null)
            {
                destObjDef = lnkDef.LinkedToR678();
                var linkRelDef = lnkDef.LinkedToR681();
                var linkRelPhrase = lnkDef.Attr_Rel_Phrase;
                var linkRelMult = lnkDef.Attr_Mult;
                // TODO: Generate code for each lnk
                lnkDef = lnkDef.LinkedToR604Succeeds();
            }

            var dstVarDef = HasDeclaredVariable(varDestDef.Attr_Name);
            //            if(dstVarDef == null)
            //            {
            //                dstVarDef = new VariableDef() { Declared=false, Name=varDestDef.Attr_Name, VarDef=varDestDef};
            //                if (actSelDef.Attr_cardinality=="one" || actSelDef.Attr_cardinality == "any")
            //                {
            //                    dstVarDef.Set = false;
            //                }
            //                else
            //                {
            //                    dstVarDef.Set = true;
            //                }
            //                declaredVariables[declaredVariables.Count - 1].Add(dstVarDef.Name, dstVarDef);
            //            }
            if (dstVarDef.Declared == false)
            {
                // var
                //                dstVarDef.Declared=true;
                DeclaredVariable(dstVarDef);
            }
            return code;
        }

        bool hasUsedSelectedVar = false;

        protected string GenerateRelatedCode(string cardinality, CIMClassACT_LNK lnkDef, CIMClassV_VAL srcValDef, CIMClassV_VAR dstVarDef, string whereCode)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            string srcVarName;
            bool srcIsSet;
            var srcObjDef = GetObjDefOfValDef(srcValDef, out srcVarName, out srcIsSet);
            var currentLinkDef = lnkDef;
            int linkingVarIndex = 1;
            bool orgSrcSet = srcIsSet;


            var dstVariableDef = HasDeclaredVariable(dstVarDef.Attr_Name);
            var dstVarObjDef = dstVariableDef.GetObjDef();
            string dstVarDomainClassName = GeneratorNames.GetDomainClassName(dstVarObjDef);

            var linkList = new List<RelatedByLinkUnit>();

            while (currentLinkDef != null)
            {
                var relDef = currentLinkDef.LinkedToR681();
                var dstObjDef = currentLinkDef.LinkedToR678();
                var linkUnit = new RelatedByLinkUnit() { Target = currentLinkDef, SetSource = srcIsSet, ObjDefOfDestination = dstObjDef, ObjDefOfSource = srcObjDef };
                string linkPhrase = currentLinkDef.Attr_Rel_Phrase;
                if (linkPhrase.StartsWith("'") && linkPhrase.EndsWith("'"))
                {
                    linkPhrase = linkPhrase.Substring(1, linkPhrase.Length - 2);
                }
                var subRelDef = relDef.SubClassR206();
                if (subRelDef is CIMClassR_SIMP)
                {
                    var rSimpDef = (CIMClassR_SIMP)subRelDef;
                    var rFormDef = rSimpDef.LinkedFromR208();
                    if (rFormDef == null)
                    {
                        throw new ArgumentOutOfRangeException($"It seems that R{relDef.Attr_Numb} has not been formalized. Please check your model");
                    }
                    var rFormObjDef = rFormDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    var rPartDefs = rSimpDef.LinkedFromR207();
                    var rPartDef = rPartDefs.First();
                    var rPartObjDef = rPartDef.CIMSuperClassR_RTO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    string phrase = "";
                    if (srcObjDef.Attr_Key_Lett == dstObjDef.Attr_Key_Lett)
                    {
                        if (linkPhrase == rFormDef.Attr_Txt_Phrs)
                        {
                            linkUnit.SetDestination = (rFormDef.Attr_Mult == 1);
                            linkUnit.CondDestination = (rFormDef.Attr_Cond == 1);
                            phrase = rFormDef.Attr_Txt_Phrs;
                        }
                        else
                        {
                            linkUnit.SetDestination = (rPartDef.Attr_Mult == 1);
                            linkUnit.CondDestination = (rPartDef.Attr_Cond == 1);
                            phrase = rPartDef.Attr_Txt_Phrs;
                        }
                    }
                    else
                    {
                        if (srcObjDef.Attr_Key_Lett == rFormObjDef.Attr_Key_Lett)
                        {
                            linkUnit.SetDestination = (rPartDef.Attr_Mult == 1);
                            linkUnit.CondDestination = (rPartDef.Attr_Cond == 1);
                            phrase = rPartDef.Attr_Txt_Phrs;
                        }
                        else
                        {
                            linkUnit.SetDestination = (rFormDef.Attr_Mult == 1);
                            linkUnit.CondDestination = (rFormDef.Attr_Cond == 1);
                            phrase = rFormDef.Attr_Txt_Phrs;
                        }
                    }
                    linkUnit.MethodName = GeneratorNames.GetRelationshipMethodName(relDef, "", phrase, GeneratorNames.RelLinkMethodType.Linked);
                    linkList.Add(linkUnit);
                }
                else if (subRelDef is CIMClassR_SUBSUP)
                {
                    linkUnit.SetDestination = false;
                    linkUnit.CondDestination = false;
                    var rSubsupDef = (CIMClassR_SUBSUP)subRelDef;
                    var rSuperDef = rSubsupDef.LinkedFromR212();
                    var rSuperObjDef = rSuperDef.CIMSuperClassR_RTO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    if (srcObjDef.Attr_Key_Lett == rSuperObjDef.Attr_Key_Lett)
                    {

                        linkUnit.MethodName = GeneratorNames.GetRelationshipMethodName(relDef, "", "", GeneratorNames.RelLinkMethodType.Linked) + dstObjDef.Attr_Key_Lett;
                    }
                    else
                    {
                        linkUnit.MethodName = GeneratorNames.GetGetSuperClassMethodName(relDef);
                    }
                    linkList.Add(linkUnit);
                }
                else if (subRelDef is CIMClassR_COMP)
                {

                }
                else if (subRelDef is CIMClassR_ASSOC)
                {
                    var rAssocDef = (CIMClassR_ASSOC)subRelDef;
                    var rAssrDef = rAssocDef.LinkedFromR211();
                    var rAoneDef = rAssocDef.LinkedFromR209();
                    var rAothDef = rAssocDef.LinkedFromR210();
                    var assrObjDef = rAssrDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    var aoneObjDef = rAoneDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    var aothObjDef = rAothDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    if (assrObjDef.Attr_Key_Lett == srcObjDef.Attr_Key_Lett)
                    {
                        linkUnit.SetDestination = false;
                        linkUnit.CondDestination = false;
                        string side = "";
                        if (aoneObjDef.Attr_Key_Lett == aothObjDef.Attr_Key_Lett)
                        {
                            if (rAoneDef.Attr_Txt_Phrs == linkPhrase)
                            {
                                side = "One";
                            }
                            else
                            {
                                side = "Other";
                            }
                        }
                        else
                        {
                            if (aoneObjDef.Attr_Key_Lett == dstObjDef.Attr_Key_Lett)
                            {
                                side = "One";
                            }
                            else
                            {
                                side = "Other";
                            }
                        }
                        linkUnit.MethodName = GeneratorNames.GetRelationshipMethodName(relDef, side, linkPhrase, GeneratorNames.RelLinkMethodType.Linked);
                        linkList.Add(linkUnit);
                    }
                    else
                    {
                        if (aoneObjDef.Attr_Key_Lett == aothObjDef.Attr_Key_Lett)
                        {
                            string side = "";
                            string phrase = "";
                            if (rAoneDef.Attr_Txt_Phrs == linkPhrase)
                            {
                                linkUnit.SetDestination = (rAothDef.Attr_Mult == 1);
                                linkUnit.CondDestination = (rAothDef.Attr_Cond == 1);
                                side = "Other";
                                phrase = rAothDef.Attr_Txt_Phrs;
                            }
                            else
                            {
                                linkUnit.SetDestination = (rAoneDef.Attr_Mult == 1);
                                linkUnit.CondDestination = (rAoneDef.Attr_Cond == 1);
                                side = "One";
                                phrase = rAoneDef.Attr_Txt_Phrs;
                            }
                            linkUnit.MethodName = GeneratorNames.GetRelationshipMethodName(relDef, side, phrase, GeneratorNames.RelLinkMethodType.Linked);
                            if (assrObjDef.Attr_Key_Lett == dstObjDef.Attr_Key_Lett)
                            {
                                linkList.Add(linkUnit);
                            }
                            else
                            {
                                linkUnit.ObjDefOfDestination = assrObjDef;
                                linkList.Add(linkUnit);
                                var nextLinkUnit = new RelatedByLinkUnit() { SetSource = false, SetDestination = false, ObjDefOfSource = assrObjDef, ObjDefOfDestination = dstObjDef, CondDestination = false };
                                nextLinkUnit.MethodName = linkUnit.MethodName;
                                linkList.Add(nextLinkUnit);
                            }
                        }
                        else
                        {
                            string side = "";
                            string phrase = "";
                            CIMClassO_OBJ currentSrcObjDef = null;
                            if (aoneObjDef.Attr_Key_Lett == srcObjDef.Attr_Key_Lett)
                            {
                                linkUnit.SetDestination = (rAothDef.Attr_Mult == 1);
                                linkUnit.CondDestination = (rAothDef.Attr_Cond == 1);
                                side = "Other";
                                phrase = rAothDef.Attr_Txt_Phrs;
                                currentSrcObjDef = aoneObjDef;
                            }
                            else
                            {
                                linkUnit.SetDestination = (rAoneDef.Attr_Mult == 1);
                                linkUnit.CondDestination = (rAoneDef.Attr_Cond == 1);
                                side = "One";
                                phrase = rAoneDef.Attr_Txt_Phrs;
                                currentSrcObjDef = aothObjDef;
                            }
                            linkUnit.MethodName = GeneratorNames.GetRelationshipMethodName(relDef, side, phrase, GeneratorNames.RelLinkMethodType.Linked);
                            if (assrObjDef.Attr_Key_Lett == dstObjDef.Attr_Key_Lett)
                            {
                                linkList.Add(linkUnit);
                            }
                            else
                            {
                                linkUnit.ObjDefOfDestination = assrObjDef;
                                linkList.Add(linkUnit);
                                var nextLinkUnit = new RelatedByLinkUnit() { SetSource = false, SetDestination = false, ObjDefOfSource = assrObjDef, ObjDefOfDestination = dstObjDef, CondDestination = false };
                                nextLinkUnit.MethodName = linkUnit.MethodName;
                                linkList.Add(nextLinkUnit);

                            }
                        }
                    }
                }
                srcObjDef = dstObjDef;
                srcIsSet = linkUnit.SetDestination;
                currentLinkDef = currentLinkDef.LinkedToR604Succeeds();
                linkingVarIndex++;
            }

            // var srcVariableDef = HasDeclaredVariable(srcVarName);
            string currentSrcCode = srcVarName;
            bool currentSrcSet = orgSrcSet;
            bool currentSrcCond = false;
            bool hasDeclaredDstVar = false;

            if (currentSrcSet)
            {
                string dstDomainClassName = GeneratorNames.GetDomainClassName(dstVarObjDef);
                string declCode = "";
                if (dstVariableDef.Set)
                {
                    if (dstVariableDef.Declared == false)
                    {
                        declCode = $"List<{dstDomainClassName}> ";
                        DeclaredVariable(dstVariableDef);
                    }
                    writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = new List<{dstDomainClassName}>();");
                }
                else
                {
                    if (dstVariableDef.Declared == false)
                    {
                        declCode = $"{dstDomainClassName} ";
                        DeclaredVariable(dstVariableDef);
                    }
                    writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = null;");
                }
                hasDeclaredDstVar = true;
                DeclaredVariable(dstVariableDef);
            }
            else
            {

            }
            if (linkList.Count > 1)
            {
                bool hasCondWhile = false;
                int numOfSets = 0;
                for (int i = 0; i < linkList.Count; i++)
                {
                    if (linkList[i].CondDestination)
                    {
                        if (i < linkList.Count - 1)
                        {
                            hasCondWhile = true;
                        }
                    }
                    if (linkList[i].SetDestination)
                    {
                        numOfSets++;
                    }
                }
                if ((hasCondWhile || numOfSets > 0 || !string.IsNullOrEmpty(whereCode)) && hasDeclaredDstVar == false)
                {
                    if (dstVariableDef.Set)
                    {

                        string declCode = "";
                        if (dstVariableDef.Declared == false)
                        {
                            declCode = $"List<{dstVarDomainClassName}> ";
                            DeclaredVariable(dstVariableDef);
                        }
                        writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = new List<{dstVarDomainClassName}>();");
                        hasDeclaredDstVar = true;
                    }
                    else
                    {
                        string declCode = "";
                        if (dstVariableDef.Declared == false)
                        {
                            declCode = $"{dstVarDomainClassName} ";
                            DeclaredVariable(dstVariableDef);
                        }
                        writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = null;");
                        hasDeclaredDstVar = true;
                    }
                }
            }
            if (linkList.Count > 1 && dstVariableDef.Set)
            {
                bool hasCondWhile = false;
                int numOfSets = 0;
                for (int i = 0; i < linkList.Count; i++)
                {
                    if (linkList[i].CondDestination)
                    {
                        if (i < linkList.Count - 1)
                        {
                            hasCondWhile = true;
                        }
                    }
                    if (linkList[i].SetDestination)
                    {
                        numOfSets++;
                    }
                }
                if ((hasCondWhile || numOfSets > 1) && hasDeclaredDstVar == false)
                {
                    string declCode = "";
                    if (dstVariableDef.Declared == false)
                    {
                        declCode = $"List<{dstVarDomainClassName}> ";
                        DeclaredVariable(dstVariableDef);
                    }
                    writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = new List<{dstVarDomainClassName}>();");
                    hasDeclaredDstVar = true;
                }
            }
            if (linkList.Count > 1 && dstVariableDef.Set == false)
            {
                bool hasCondWhile = false;
                int numOfSets = 0;
                for (var i = 0; i < linkList.Count; i++)
                {
                    if (linkList[i].CondDestination)
                    {
                        if (i < linkList.Count - 1)
                        {
                            hasCondWhile = true;
                        }
                    }
                    if (linkList[i].SetDestination)
                    {
                        numOfSets++;
                    }
                }
                if ((hasCondWhile || numOfSets > 1) && hasDeclaredDstVar == false)
                {
                    string declCode = "";
                    if (dstVariableDef.Declared == false)
                    {
                        declCode = $"{dstVarDomainClassName} ";
                        DeclaredVariable(dstVariableDef);
                    }
                    writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = null;");
                    hasDeclaredDstVar = true;
                }
            }

            int logicDepth = 0;
            int linkIndex = 0;
            int loopDepth = 0;
            bool setOfCurrentVal = false;
            // when logic is condition check then true
            var logicSteps = new List<bool>();
            while (linkIndex < linkList.Count)
            {
                string tmpVarName = $"{srcVarName}In{logicDepth}RL{relatedByCodeIndex}";
                string dstDomainClassName = GeneratorNames.GetDomainClassName(dstVarObjDef);
                if (currentSrcSet)
                {
                    if (linkIndex > 0)
                    {
                        if (linkIndex == 1)
                        {
                            if (dstVariableDef.Set && hasDeclaredDstVar == false)
                            {
                                if (dstVariableDef.Declared)
                                {
                                    writer.WriteLine($"{indent}{dstVariableDef.Name}.Clear();");
                                }
                                else
                                {
                                    writer.WriteLine($"{indent}List<{dstDomainClassName}> {dstVariableDef.Name} = new List<{dstDomainClassName}>();");
                                    DeclaredVariable(dstVariableDef);
                                }
                            }
                            else
                            {
                                if (dstVariableDef.Declared == false)
                                {
                                    writer.WriteLine($"{indent}{dstDomainClassName} {dstVariableDef.Name} = null;");
                                    DeclaredVariable(dstVariableDef);
                                }
                            }
                        }
                        writer.WriteLine($"{indent}var {tmpVarName}Set = {currentSrcCode};");
                        currentSrcCode = $"{tmpVarName}Set";
                        setOfCurrentVal = true;
                    }
                    loopDepth++;
                    writer.WriteLine($"{indent}foreach (var {tmpVarName} in {currentSrcCode})");
                    writer.WriteLine($"{indent}" + "{");
                    currentSrcCode = $"{tmpVarName}.{linkList[linkIndex].MethodName}()";
                    currentSrcSet = linkList[linkIndex].SetDestination;
                    if (currentSrcSet)
                    {
                        setOfCurrentVal = true;
                    }
                    else
                    {
                        setOfCurrentVal = false;
                    }
                    logicDepth++;
                    logicSteps.Add(false);
                    indent += baseIndent;
                }
                else
                {
                    if (currentSrcCond)
                    {
                        if (linkIndex > 0)
                        {
                            writer.WriteLine($"{indent}var {tmpVarName} = {currentSrcCode};");
                            currentSrcCode = tmpVarName;
                        }
                        writer.WriteLine($"{indent}if ({currentSrcCode} != null)");
                        writer.WriteLine($"{indent}" + "{");
                        logicDepth++;
                        logicSteps.Add(true);
                        indent += baseIndent;
                    }
                    currentSrcCode += $".{linkList[linkIndex].MethodName}()";
                }
                if (linkIndex == linkList.Count - 1)
                {
                    string condCode = "";
                    if (!string.IsNullOrEmpty(whereCode))
                    {
                        condCode = $".Where(selected =>({whereCode}))";
                    }
                    string castCode = "";
                    string declCode = "";
                    if (dstVariableDef.Declared == false)
                    {
                        if (dstVariableDef.Set)
                        {
                            declCode = $"List<{dstDomainClassName}> ";
                            castCode = $"(List<{dstDomainClassName}>)";
                        }
                        else
                        {
                            declCode = "var ";
                        }
                    }
                    if (dstVariableDef.Set)
                    {
                        if (linkIndex > 0)
                        {
                            if (currentSrcSet)
                            {
                                if (setOfCurrentVal)
                                {
                                    string srcTmpVar = $"{dstVariableDef.Name}In{logicDepth}RL{relatedByCodeIndex}";
                                    writer.WriteLine($"{indent}var {srcTmpVar}Set = {currentSrcCode}{condCode};");
                                    loopDepth++;
                                    writer.WriteLine($"{indent}foreach (var {srcTmpVar} in {srcTmpVar}Set) // test");
                                    writer.WriteLine(indent + "{");
                                    logicDepth++;
                                    logicSteps.Add(false);
                                    indent += baseIndent;
                                    currentSrcCode = srcTmpVar;
                                }
                            }

                            if (dstVariableDef.Declared == false)
                            {
                                writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = {castCode}{currentSrcCode}{condCode};");
                                DeclaredVariable(dstVariableDef);
                            }
                            else
                            {
                                if (linkList[linkIndex].SetDestination == false && linkList[linkIndex].CondDestination)
                                {
                                    string srcTmpVar = $"{dstVariableDef.Name}In{logicDepth}RL{relatedByCodeIndex}";
                                    writer.WriteLine($"{indent}var {srcTmpVar} = {currentSrcCode};");
                                    writer.WriteLine($"{indent}if ({srcTmpVar} != null)");
                                    writer.WriteLine(indent + "{");
                                    logicDepth++;
                                    logicSteps.Add(true);
                                    indent += baseIndent;
                                    currentSrcCode = srcTmpVar;
                                }
                                if (linkList[linkIndex].SetDestination && setOfCurrentVal == false)
                                {
                                    string srcTmpVar = $"{dstVariableDef.Name}In{logicDepth}RL{relatedByCodeIndex}";
                                    writer.WriteLine($"{indent}var {srcTmpVar}Set = {currentSrcCode};");
                                    loopDepth++;
                                    writer.WriteLine($"{indent}foreach (var {srcTmpVar} in {srcTmpVar}Set)");
                                    writer.WriteLine(indent + "{");
                                    logicDepth++;
                                    logicSteps.Add(false);
                                    indent += baseIndent;
                                    currentSrcCode = srcTmpVar;
                                }
                                writer.WriteLine($"{indent}{dstVariableDef.Name}.Add({currentSrcCode});");
                            }
                        }
                        else
                        {
                            if (currentSrcSet)
                            {
                                string tmpSetVarName = $"{dstVariableDef.Name}In{logicDepth}RL{relatedByCodeIndex}";
                                writer.WriteLine($"{indent}var {tmpSetVarName}Set = {currentSrcCode};");
                                loopDepth++;
                                writer.WriteLine($"{indent}foreach (var {tmpSetVarName} in {tmpSetVarName}Set)");
                                writer.WriteLine($"{indent}" + "{");
                                logicDepth++;
                                logicSteps.Add(false);
                                indent += baseIndent;
                                writer.WriteLine($"{indent}{dstVariableDef.Name}.Add({tmpSetVarName});");
                            }
                            else
                            {
                                if (dstVariableDef.Set && orgSrcSet)
                                {
                                    writer.WriteLine($"{indent}{dstVariableDef.Name}.Add({currentSrcCode});");
                                }
                                else
                                {
                                    if (dstVariableDef.Set)
                                    {
                                        castCode = $" as List<{dstDomainClassName}>";
                                        if (dstVariableDef.Declared == false)
                                        {
                                            declCode = "var ";
                                        }
                                    }
                                    writer.WriteLine($"{indent}{declCode}{dstVariableDef.Name} = {currentSrcCode}{castCode};");
                                    DeclaredVariable(dstVariableDef);
                                    if (loopDepth > 0)
                                    {
                                        writer.WriteLine($"{indent}break;");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string setCode = $"{dstVariableDef.Name} = {castCode}{currentSrcCode}";
                        if (linkList[linkIndex].SetDestination)
                        {
                            setCode += condCode;
                        }
                        if (linkIndex > 0)
                        {
                            if (linkList[linkIndex].SetDestination)
                            {
                                writer.WriteLine($"{indent}{declCode}{setCode}.FirstOrDefault();");
                                if (loopDepth > 0)
                                {
                                    writer.WriteLine($"{indent}break;");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(whereCode))
                                {
                                    //string tmpSelectVarName = $"{dstVariableDef.Name}In{logicDepth}RL{relatedByCodeIndex}";
                                    hasUsedSelectedVar = true;
                                    string tmpSelectVarName = "selected";
                                    writer.WriteLine($"{indent}{tmpSelectVarName} = {currentSrcCode};");
                                    hasDeclaredDstVar = true;
                                    writer.WriteLine($"{indent}if {whereCode}");
                                    writer.WriteLine(indent + "{");
                                    logicDepth++;
                                    logicSteps.Add(true);
                                    indent += baseIndent;
                                    writer.WriteLine($"{indent}{dstVariableDef.Name} = ({dstDomainClassName}){tmpSelectVarName};");
                                    if (loopDepth > 0)
                                    {
                                        writer.WriteLine($"{indent}break;");
                                    }
                                }
                                else
                                {
                                    writer.WriteLine($"{indent}{declCode}{setCode};");
                                    if (loopDepth > 0 && dstVariableDef.Set == false)
                                    {
                                        writer.WriteLine($"{indent}break;");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (linkList[linkIndex].SetDestination)
                            {
                                setCode += ".FirstOrDefault()";
                            }
                            writer.WriteLine($"{indent}{declCode}{setCode};");
                            // if (linkList[linkIndex].SetDestination && currentSrcSet)
                            if (loopDepth > 0)
                            {
                                writer.WriteLine($"{indent}break;");
                            }
                        }
                    }
                }
                currentSrcSet = linkList[linkIndex].SetDestination;
                currentSrcCond = linkList[linkIndex].CondDestination;
                linkIndex++;
            }

            bool shoudBreak = false;
            if (cardinality == "any")
            {
                shoudBreak = true;
            }
            for (int i = 0; i < logicDepth; i++)
            {
                indent = indent.Substring(baseIndent.Length);
                writer.WriteLine($"{indent}" + "}");
                if (shoudBreak && dstVariableDef.Set == false && i < logicSteps.Count - 1 && logicSteps[logicSteps.Count - i - 1] == false)
                {
                    bool addBreak = true;
                    if (logicSteps.Count - i - 2 >= 0)
                    {
                        if (logicSteps[logicSteps.Count - i - 2] == true)
                        {
                            addBreak = false;
                        }
                        if (addBreak == false)
                        {
                            int depthIndex = logicSteps.Count - i - 1;
                            while (--depthIndex >= 0)
                            {
                                if (logicSteps[depthIndex] == false)
                                {
                                    addBreak = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (addBreak)
                    {
                        writer.WriteLine($"{indent}if ({dstVariableDef.Name} != null) break;");
                    }
                }
                if (logicSteps[logicSteps.Count - i - 1])
                {
                    shoudBreak = true;
                }
            }

            relatedByCodeIndex++;

            return sb.ToString();
        }

        protected CIMClassO_OBJ GetObjDefOfValDef(CIMClassV_VAL valDef, out string varName, out bool Set)
        {
            varName = null;
            Set = false;
            CIMClassO_OBJ objDef = null;
            var subValDef = valDef.SubClassR801();
            if (subValDef is CIMClassV_IRF)
            {
                var irfDef = (CIMClassV_IRF)subValDef;
                var varDef = irfDef.LinkedToR808();
                var variableDef = HasDeclaredVariable(varDef.Attr_Name);
                objDef = variableDef.GetObjDef();
                varName = variableDef.Name;
                if (varName.ToLower() == "self")
                {
                    varName = this.selfVarNameOnCode;
                }
                Set = variableDef.Set;
            }
            else if (subValDef is CIMClassV_ISR)
            {
                // V_ISR
                var isrDef = (CIMClassV_ISR)subValDef;
                var varDef = isrDef.LinkedToR809();
                var variableDef = HasDeclaredVariable(varDef.Attr_Name);
                objDef = variableDef.GetObjDef();
                varName = variableDef.Name;
                Set = variableDef.Set;
            }
            else
            {

            }
            return objDef;
        }

        protected string GenerateACT_SelectFrom(CIMClassACT_FIO actFioDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            // SELECT MANY FROM INSTANCES OF 
            // SELECT ANY FROM INSTANCES OF
            var varDef = actFioDef.LinkedToR639();
            var objDef = actFioDef.LinkedToR677();

            string domainClassImplClassName = GeneratorNames.GetDomainClassImplName(objDef);
            string domainClassName = GeneratorNames.GetDomainClassName(objDef);
            string candSetVarName = $"candidatesOf{varDef.Attr_Name}";
            string candVarName = $"candidateOf{varDef.Attr_Name}";

            var declaredVarDef = HasDeclaredVariable(varDef.Attr_Name);
            if (declaredVarDef == null)
            {
                declaredVarDef = new VariableDef() { Name = varDef.Attr_Name, Declared = false, VarDef = varDef };
                if (actFioDef.Attr_cardinality == "any")
                {
                    declaredVarDef.Set = false;
                }
                else
                {
                    declaredVarDef.Set = true;
                }
            }

            string declCode = "";


            if (declaredVarDef.Declared == false)
            {
                declCode = "var ";
                // declaredVarDef.Declared = true;
                DeclaredVariable(declaredVarDef);
            }
            if (declaredVarDef.Set)
            {
                writer.WriteLine($"{indent}{declCode}{candSetVarName} = instanceRepository.GetDomainInstances(\"{objDef.Attr_Key_Lett}\");");
                writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null) {candSetVarName} = instanceRepository.ExternalStorageAdaptor.CheckInstanceStatus(DomainName, \"{objDef.Attr_Key_Lett}\", {candSetVarName}, () => {{ return \"\"; }}, () => {{ return {domainClassImplClassName}.CreateInstance(instanceRepository, logger); }}, \"many\").Result;");
                writer.WriteLine($"{indent}{declCode}{varDef.Attr_Name} = new List<{domainClassName}>();");
                writer.WriteLine($"{indent}foreach (var {candVarName} in {candSetVarName})");
                writer.WriteLine($"{indent}" + "{");
                writer.WriteLine($"{indent}{baseIndent}{varDef.Attr_Name}.Add(({domainClassName}){candVarName});");
                writer.WriteLine($"{indent}" + "}");
            }
            else
            {
                string varTempSetName = $"{varDef.Attr_Name}TempSet";
                string tempSetDeclCode = "";
                if (!currentTempSetVarFolder.HasDeclared(varTempSetName))
                {
                    tempSetDeclCode = "var ";
                }
                writer.WriteLine($"{indent}{tempSetDeclCode}{varTempSetName} = instanceRepository.GetDomainInstances(\"{objDef.Attr_Key_Lett}\");");
                writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null) {varTempSetName} = instanceRepository.ExternalStorageAdaptor.CheckInstanceStatus(DomainName, \"{objDef.Attr_Key_Lett}\", {varDef.Attr_Name}TempSet, () => {{ return \"\"; }}, () => {{ return {domainClassImplClassName}.CreateInstance(instanceRepository, logger); }}, \"any\").Result;");
                writer.WriteLine($"{indent}{declCode}{varDef.Attr_Name} = ({domainClassName})({varTempSetName}.FirstOrDefault());");
            }

            return sb.ToString();
        }
        protected string GenerateACT_SelectFromWhere(CIMClassACT_FIW actFiwDef)
        {
            // SELECT ANY FROM INSTANCES OF WHERE
            // SELECT MANY FROM INSTANCES OF WHERE
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var cardinality = actFiwDef.Attr_cardinality;
            var valDef = actFiwDef.LinkedToR610();
            var varDef = actFiwDef.LinkedToR665();
            var objDef = actFiwDef.LinkedToR676();
            string domainClassName = GeneratorNames.GetDomainClassName(objDef);
            string domainClassImplClassName = GeneratorNames.GetDomainClassImplName(objDef);
            string candVarName = $"candidatesOf{varDef.Attr_Name}";

            string valVarName;
            addDomainClassCast = true;
            string valCode = GenerateV_VAL(valDef, out valVarName);
            addDomainClassCast = false;
            string valVarNameForSQL;
            string valCodeForSql = GenerateV_VAL(valDef, out valVarNameForSQL, true);

            var dstVarDef = HasDeclaredVariable(varDef.Attr_Name);
            if (dstVarDef == null)
            {
                dstVarDef = new VariableDef() { Name = varDef.Attr_Name, Declared = false, VarDef = varDef };
                if (actFiwDef.Attr_cardinality == "any")
                {
                    dstVarDef.Set = false;
                }
                else
                {
                    dstVarDef.Set = true;
                }
            }
            string declCode = "";
            if (dstVarDef.Declared == false)
            {
                declCode = "var ";
                //  dstVarDef.Declared = true;
                DeclaredVariable(dstVarDef);
            }

            if (dstVarDef.Set)
            {
                writer.WriteLine($"{indent}{declCode}{candVarName} = instanceRepository.GetDomainInstances(\"{objDef.Attr_Key_Lett}\").Where(selected => ({valCode}));");
                writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null) {candVarName} = instanceRepository.ExternalStorageAdaptor.CheckInstanceStatus(DomainName, \"{objDef.Attr_Key_Lett}\", {candVarName}, () => {{ return $\"{valCodeForSql}\"; }}, () => {{ return {domainClassImplClassName}.CreateInstance(instanceRepository, logger); }}, \"{actFiwDef.Attr_cardinality}\").Result;");
                writer.WriteLine($"{indent}{declCode}{varDef.Attr_Name} = new List<{domainClassName}>();");
                writer.WriteLine($"{indent}foreach (var instance in {candVarName})");
                writer.WriteLine($"{indent}" + "{");
                writer.WriteLine($"{indent}{baseIndent}{varDef.Attr_Name}.Add(({domainClassName})instance);");
                writer.WriteLine($"{indent}" + "}");
            }
            else
            {
                string tempSetDeclCode = "";
                string varTempSetName = $"{varDef.Attr_Name}TempSet";
                if (!currentTempSetVarFolder.HasDeclared(varTempSetName))
                {
                    tempSetDeclCode = "var ";
                }
                writer.WriteLine($"{indent}{tempSetDeclCode}{varTempSetName} = instanceRepository.GetDomainInstances(\"{objDef.Attr_Key_Lett}\").Where(selected => ({valCode}));");
                writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null) {varTempSetName} = instanceRepository.ExternalStorageAdaptor.CheckInstanceStatus(DomainName, \"{objDef.Attr_Key_Lett}\", {varDef.Attr_Name}TempSet, () => {{ return $\"{valCodeForSql}\"; }}, () => {{ return {domainClassImplClassName}.CreateInstance(instanceRepository, logger); }}, \"{actFiwDef.Attr_cardinality}\").Result;");
                writer.WriteLine($"{indent}{declCode}{varDef.Attr_Name} = ({domainClassName})({varTempSetName}.FirstOrDefault());");
            }


            return sb.ToString();
        }
        protected string GenerateACT_UnrelateUsing(CIMClassACT_URU actUruDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            // UNRELATE FROM ACROSS USING
            var varOneDef = actUruDef.LinkedToR622();
            var varOtherDef = actUruDef.LinkedToR623();
            var varAssocDef = actUruDef.LinkedToR624();
            var relDef = actUruDef.LinkedToR656();

            writer.WriteLine($"{indent}// Unrelate {varOneDef.Attr_Name} From {varOtherDef.Attr_Name} Across R{relDef.Attr_Numb} Using {varAssocDef.Attr_Name}");
            string unlinkCode = GenerateLinkCode(relDef, actUruDef.Attr_relationship_phrase, varOneDef, varOtherDef, false, varAssocDef);
            writer.WriteLine($"{indent}{unlinkCode};");

            return sb.ToString();
        }
        protected string GenerateACT_Unrelate(CIMClassACT_UNR actUnrDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            // UNRELATE FROM  ACROSS
            var varOneDef = actUnrDef.LinkedToR620();
            var varOtherDef = actUnrDef.LinkedToR621();
            var relDef = actUnrDef.LinkedToR655();

            writer.WriteLine($"{indent}// Unrelate {varOneDef.Attr_Name} From {varOtherDef.Attr_Name} Across R{relDef.Attr_Numb}");
            string unlinkCode = GenerateLinkCode(relDef, actUnrDef.Attr_relationship_phrase, varOneDef, varOtherDef, false);
            writer.WriteLine($"{indent}{unlinkCode};");

            return sb.ToString();
        }
        protected string GenerateACT_RelateUsing(CIMClassACT_RU actRuDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            // RELATE TO ACROSS USING
            var varOneDef = actRuDef.LinkedToR617();
            var varOtherDef = actRuDef.LinkedToR618();
            var varAssocDef = actRuDef.LinkedToR619();
            var relDef = actRuDef.LinkedToR654();

            string linkCode = GenerateLinkCode(relDef, actRuDef.Attr_relationship_phrase, varOneDef, varOtherDef, true, varAssocDef);

            writer.WriteLine($"{indent}// Relate {varOneDef.Attr_Name} - R{relDef.Attr_Numb} -> {varOtherDef.Attr_Name} USING {varAssocDef.Attr_Name}");
            writer.WriteLine($"{indent}{linkCode};");

            return sb.ToString();
        }
        protected string GenerateACT_Relate(CIMClassACT_REL actRelDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            var varLeftDef = actRelDef.LinkedToR615();
            var varRightDef = actRelDef.LinkedToR616();
            var relDef = actRelDef.LinkedToR653();

            writer.WriteLine($"{indent}// {varLeftDef.Attr_Name} - R{relDef.Attr_Numb} -> {varRightDef.Attr_Name};");
            string code = GenerateLinkCode(relDef, actRelDef.Attr_relationship_phrase, varLeftDef, varRightDef, true);
            writer.WriteLine($"{indent}{code};");

            return sb.ToString();
        }

        protected string GenerateLinkCode(CIMClassR_REL relDef, string phrase, CIMClassV_VAR leftVar, CIMClassV_VAR rightVar, bool link, CIMClassV_VAR assocVar = null)
        {
            string code = "";

            var lVariableDef = HasDeclaredVariable(leftVar.Attr_Name);
            var rVariableDef = HasDeclaredVariable(rightVar.Attr_Name);
            GeneratorNames.RelLinkMethodType methodType = GeneratorNames.RelLinkMethodType.Link;
            if (link == false)
            {
                methodType = GeneratorNames.RelLinkMethodType.Unlink;
            }
            if (phrase.StartsWith("'") && phrase.EndsWith("'"))
            {
                phrase = phrase.Substring(1, phrase.Length - 2);
            }
            var subRelDef = relDef.SubClassR206();
            if (subRelDef is CIMClassR_SIMP)
            {
                var rSimpDef = (CIMClassR_SIMP)subRelDef;
                var rFormDef = rSimpDef.LinkedFromR208();
                if (rFormDef == null)
                {
                    logger.LogError($"R{relDef.Attr_Numb} may not be formalized!");
                }
                var formObjDef = rFormDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                var partVar = lVariableDef;
                var formVar = rVariableDef;
                var rObjDef = rVariableDef.GetObjDef();
                if (formObjDef.Attr_Key_Lett != rObjDef.Attr_Key_Lett)
                {
                    partVar = rVariableDef;
                    formVar = lVariableDef;
                }
                if ((!string.IsNullOrEmpty(phrase)) && rFormDef.Attr_Txt_Phrs != phrase)
                {
                    partVar = rVariableDef;
                    formVar = lVariableDef;
                }
                var rPartDefs = rSimpDef.LinkedFromR207();
                string methodName = "";
                foreach (var partDef in rPartDefs)
                {
                    var partObjDef = partDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                    methodName = GeneratorNames.GetRelationshipMethodName(relDef, "", partDef.Attr_Txt_Phrs, methodType);
                    break;
                }

                code = $"{GetSelfVarNameOnCode(formVar)}.{methodName}({GetSelfVarNameOnCode(partVar)}, changedStates)";
            }
            else if (subRelDef is CIMClassR_COMP)
            {
                var rCompDef = (CIMClassR_COMP)subRelDef;
                var rConeDef = rCompDef.LinkedFromR214();
                var rCothDef = rCompDef.LinkedFromR215();
            }
            else if (subRelDef is CIMClassR_ASSOC)
            {
                var rAssocDef = (CIMClassR_ASSOC)subRelDef;
                var rAoneDef = rAssocDef.LinkedFromR209();
                var aoneObjDef = rAoneDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                var rAothDef = rAssocDef.LinkedFromR210();
                var aothObjDef = rAothDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                var rAssrDef = rAssocDef.LinkedFromR211();
                var assrObjDef = rAssrDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                var lObjDef = lVariableDef.GetObjDef();
                var rObjDef = rVariableDef.GetObjDef();
                var aoneVar = lVariableDef;
                var aothVar = rVariableDef;
                var assocVariableDef = HasDeclaredVariable(assocVar.Attr_Name);
                if (lObjDef.Attr_Key_Lett != aoneObjDef.Attr_Key_Lett)
                {
                    aoneVar = rVariableDef;
                    aothVar = lVariableDef;
                }
                else
                {
                    if ((!string.IsNullOrEmpty(phrase)) && rAothDef.Attr_Txt_Phrs != phrase)
                    {
                        var tmpVar = aoneVar;
                        aoneVar = aothVar;
                        aothVar = tmpVar;
                    }
                }
                string methodName = GeneratorNames.GetRelationshipMethodName(relDef, "", "", methodType);
                code = $"{GetSelfVarNameOnCode(assocVariableDef)}.{methodName}({GetSelfVarNameOnCode(aoneVar)},{GetSelfVarNameOnCode(aothVar)})";
            }
            else if (subRelDef is CIMClassR_SUBSUP)
            {
                var rSubsupDef = (CIMClassR_SUBSUP)subRelDef;
                var rSuperDef = rSubsupDef.LinkedFromR212();
                var lObjDef = lVariableDef.GetObjDef();
                var rObjDef = rVariableDef.GetObjDef();
                var superVar = lVariableDef;
                var subVar = rVariableDef;
                var superObjDef = rSuperDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                if (lObjDef.Attr_Key_Lett != superObjDef.Attr_Key_Lett)
                {
                    superVar = rVariableDef;
                    subVar = lVariableDef;
                }
                var rSubDefs = rSubsupDef.LinkedFromR213();
                string methodName = "";
                var subVarObjDef = subVar.GetObjDef();
                foreach (var rSubDef in rSubDefs)
                {
                    var subObjDef = rSubDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    if (subVarObjDef.Attr_Key_Lett == subObjDef.Attr_Key_Lett)
                    {
                        methodName = GeneratorNames.GetRelationshipMethodName(relDef, "", "", methodType);
                        break;
                    }
                }
                code = $"{GetSelfVarNameOnCode(subVar)}.{methodName}({GetSelfVarNameOnCode(superVar)}, changedStates)";
            }

            return code;
        }

        private string GetSelfVarNameOnCode(VariableDef varDef)
        {
            string varName = varDef.Name;
            if (varName.ToLower() == "self")
            {
                varName = this.selfVarNameOnCode;
            }
            return varName;
        }

        protected string GenerateACT_CTL(CIMClassACT_CTL actCtlDef)
        {
            throw new NotImplementedException();
        }
        protected string GenerateACT_Break(CIMClassACT_BRK actBrkDef)
        {
            return $"{indent}break;";
        }
        protected string GenerateACT_Continue(CIMClassACT_CON actConDef)
        {
            return $"{indent}continue;";
        }
        protected string GenerateACT_ESS(CIMClassE_ESS eEssDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            // Generate event to instance;

            var vParDefs = eEssDef.LinkedFromR700();
            string eventParamsCode = "";
            if (vParDefs.Count() > 0)
            {
                var currentParDef = vParDefs.ElementAt(0);
                while (true)
                {
                    var prevParDef = currentParDef.LinkedFromR816Precedes();
                    if (prevParDef == null)
                    {
                        break;
                    }
                    currentParDef = prevParDef;
                }
                while (currentParDef != null)
                {
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        eventParamsCode += ", ";
                    }
                    var paramValDef = currentParDef.LinkedToR800();
                    string paramVarName;
                    eventParamsCode += $"{currentParDef.Attr_Name}:" + GenerateV_VAL(paramValDef, out paramVarName);
                    currentParDef = currentParDef.LinkedToR816Succeeds();
                }
            }
            var subEEssDef = eEssDef.SubClassR701();
            if (subEEssDef is CIMClassE_CES)
            {
                var eCesDef = (CIMClassE_CES)subEEssDef;
                var csmeDef = eCesDef.LinkedFromR702();
                var vVar = eCesDef.LinkedToR710();
                var smevtDef = csmeDef.LinkedToR706();
                CIMClassO_OBJ smObjDef = null;
                var smDef = smevtDef.LinkedToR502();
                var subSmDef = smDef.SubClassR517();
                if (subSmDef is CIMClassSM_ISM)
                {
                    smObjDef = ((CIMClassSM_ISM)subSmDef).LinkedToR518();
                }
                else if (subSmDef is CIMClassSM_ASM)
                {
                    smObjDef = ((CIMClassSM_ASM)subSmDef).LinkedToR519();
                }
                string domainClassName = GeneratorNames.GetStateMachineClassName(smObjDef);
                string eventClassName = GeneratorNames.GetEventClassName(smObjDef, smevtDef);
                var evtInstVarDef = HasDeclaredVariable(vVar.Attr_Name);
                string declCode = "";
                if (evtInstVarDef.Declared == false)
                {
                    declCode = "var ";
                    DeclaredVariable(evtInstVarDef);
                }
                var subCsmeDef = csmeDef.SubClassR704();
                if (subCsmeDef is CIMClassE_CEI)
                {
                    // CREATE EVENT INSTANCE -evtInstVar- OF -eventName- TO -receiver;
                    var eCeiDef = (CIMClassE_CEI)subCsmeDef;
                    var dstVarDef = eCeiDef.LinkedToR711();
                    var dstVariableDef = HasDeclaredVariable(dstVarDef.Attr_Name);
                    string createParamCode = $"receiver:{dstVariableDef.Name}";
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        createParamCode += ", " + eventParamsCode;
                    }

                    writer.WriteLine($"{indent}{declCode}{vVar.Attr_Name} = {domainClassName}.{eventClassName}.Create({createParamCode}, isSelfEvent:false, sendNow:false);");
                    writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null && instanceRepository.ExternalStorageAdaptor.DoseEventComeFromExternal())");
                    writer.WriteLine($"{indent}" + "{");
                    writer.WriteLine($"{indent}    changedStates.Add(new CEventChangedState() {{ OP = ChangedState.Operation.Create, Target = {dstVariableDef.Name}, Event = {vVar.Attr_Name} }});");
                    writer.WriteLine($"{indent}" + "}");
                }
                else if (subCsmeDef is CIMClassE_CEA)
                {
                    // CREATE EVENT INSTANCE -evtInstVar- OF -ClassEventName- TO -className- ASSIGNER;
                    var eCeaDef = (CIMClassE_CEA)subCsmeDef;
                    string createParamCode = "";
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        createParamCode = $"{eventParamsCode}, ";
                    }
                    writer.WriteLine($"{indent}// This generator doesn't support Class State Machine.");
                    writer.WriteLine($"{indent}// {domainClassName}.{eventClassName}.Create({createParamCode}isSelfEvent:false, sendNow:false);");
                }
                else if (subCsmeDef is CIMClassE_CEC)
                {
                    // CREATE EVENT INSTANCE -evtInstVar- OF -EventName- TO -className- CREATOR;
                    var eCecDef = (CIMClassE_CEC)subCsmeDef;
                    string createParamCode = "receiver:null";
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        createParamCode += ", " + eventParamsCode;
                    }
                    writer.WriteLine($"{indent}{declCode}{vVar.Attr_Name} = {domainClassName}.{eventClassName}.Create({createParamCode}, isSelfEvent:false, sendNow:false, instanceRepository:instanceRepository, logger:logger);");
                    writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null && instanceRepository.ExternalStorageAdaptor.DoseEventComeFromExternal())");
                    writer.WriteLine($"{indent}" + "{");
                    writer.WriteLine($"{indent}    changedStates.Add(new CEventChangedState() {{ OP = ChangedState.Operation.Create, Target = null, Event = {vVar.Attr_Name} }});");
                    writer.WriteLine($"{indent}" + "}");
                }
            }
            else if (subEEssDef is CIMClassE_GES)
            {
                var eGesDef = (CIMClassE_GES)subEEssDef;
                var gsmeDef = eGesDef.LinkedFromR703();
                var smevtDef = gsmeDef.LinkedToR707();
                CIMClassO_OBJ smObjDef = null;
                var smDef = smevtDef.LinkedToR502();
                var subSmDef = smDef.SubClassR517();
                if (subSmDef is CIMClassSM_ISM)
                {
                    smObjDef = ((CIMClassSM_ISM)subSmDef).LinkedToR518();
                }
                else if (subSmDef is CIMClassSM_ASM)
                {
                    smObjDef = ((CIMClassSM_ASM)subSmDef).LinkedToR519();
                }
                string domainClassName = GeneratorNames.GetStateMachineClassName(smObjDef);
                string eventClassName = GeneratorNames.GetEventClassName(smObjDef, smevtDef);
                var subGsmeDef = gsmeDef.SubClassR705();
                if (subGsmeDef is CIMClassE_GEN)
                {
                    // Generate ... to instance
                    var genDef = (CIMClassE_GEN)subGsmeDef;
                    var destVarDef = genDef.LinkedToR712();
                    string pVarName = destVarDef.Attr_Name;
                    string isSelfEventSpec = "isSelfEvent:";
                    bool isSefEvent = false;
                    if (destVarDef.Attr_Name.ToLower() == "self")
                    {
                        pVarName = this.selfVarNameOnCode;
                        isSelfEventSpec += "true";
                        isSefEvent = true;
                    }
                    else
                    {
                        isSelfEventSpec += "false";
                    }
                    string createParamCode = $"receiver:{pVarName}";
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        createParamCode += ", " + eventParamsCode;
                    }
                    if (isSefEvent)
                    {
                        writer.WriteLine($"{indent}{domainClassName}.{eventClassName}.Create({createParamCode}, {isSelfEventSpec}, sendNow:true);");
                    }
                    else
                    {
                        writer.WriteLine($"{indent}if (instanceRepository.ExternalStorageAdaptor != null && instanceRepository.ExternalStorageAdaptor.DoseEventComeFromExternal())");
                        writer.WriteLine($"{indent}" + "{");
                        writer.WriteLine($"{indent}    changedStates.Add(new CEventChangedState() {{ OP = ChangedState.Operation.Create, Target = {pVarName}, Event = {domainClassName}.{eventClassName}.Create({createParamCode}, false, sendNow:false) }});");
                        writer.WriteLine($"{indent}" + "}");
                        writer.WriteLine($"{indent}else");
                        writer.WriteLine($"{indent}" + "{");
                        writer.WriteLine($"{indent}    {domainClassName}.{eventClassName}.Create({createParamCode}, {isSelfEventSpec}, sendNow:true);");
                        writer.WriteLine($"{indent}" + "}");
                    }
                }
                else if (subGsmeDef is CIMClassE_GAR)
                {
                    // Generat ... to -class- ASSIGNER;
                    var garDef = (CIMClassE_GAR)subGsmeDef;
                    string createParamCode = "";
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        createParamCode = $"{eventParamsCode}, ";
                    }
                    writer.WriteLine($"{indent}// This generator doesn't support Class State Machine.");
                    writer.WriteLine($"{indent}// {domainClassName}.{eventClassName}.Create({createParamCode}sendNow:true);");
                }
                else if (subGsmeDef is CIMClassE_GEC)
                {
                    // Genarete ... to -class- Creator
                    var gecDef = (CIMClassE_GEC)subGsmeDef;
                    string createParamCode = "receiver:null";
                    if (!string.IsNullOrEmpty(eventParamsCode))
                    {
                        createParamCode += ", " + eventParamsCode;
                    }
                    writer.WriteLine($"{indent}{domainClassName}.{eventClassName}.Create({createParamCode}, isSelfEvent:false, sendNow:true, instanceRepository:instanceRepository, logger:logger);");
                }
            }

            return sb.ToString();
        }
        protected string GenerateACT_GPR(CIMClassE_GPR eGprDef)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            var valDef = eGprDef.LinkedToR714();
            string varName;
            string code = GenerateV_VAL(valDef, out varName);
            writer.WriteLine($"{indent}{code}.Send();");

            return sb.ToString();
        }
        protected string GenerateACT_IOP(CIMClassACT_IOP actIopDef)
        {
            var valDef = actIopDef.LinkedToR629();
            var vParDefs = actIopDef.LinkedFromR679();
            var sprRoDef = actIopDef.LinkedToR657();
            var sprPoDef = actIopDef.LinkedToR680();
            throw new NotImplementedException();
        }
        protected string GenerateACT_SGN(CIMClassACT_SGN actSgnDef)
        {
            var valDef = actSgnDef.LinkedToR630();
            var vParDefs = actSgnDef.LinkedFromR662();
            var sprRsDef = actSgnDef.LinkedToR660();
            var sprPsDef = actSgnDef.LinkedToR663();
            throw new NotImplementedException();
        }


        // V_VAL generation
        protected string GenerateV_VAL(CIMClassV_VAL valDef, out string varName, bool sqlStyle = false)
        {
            string code = "";
            varName = null;

            var subValDef = valDef.SubClassR801();

            if (subValDef is CIMClassV_FNV)
            {
                code = GenerateVAL_FNV((CIMClassV_FNV)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_PVL)
            {
                // Parameter value
                code = GenerateVAL_PVL((CIMClassV_PVL)subValDef, out varName, sqlStyle);
            }
            else if (subValDef is CIMClassV_SLR)
            {
                code = GenerateVAL_SLR((CIMClassV_SLR)subValDef, out varName, sqlStyle);
            }
            else if (subValDef is CIMClassV_BRV)
            {
                code = GenerateVAL_BRV((CIMClassV_BRV)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_IRF)
            {
                code = GenerateVAL_IRF((CIMClassV_IRF)subValDef, out varName, sqlStyle);
            }
            else if (subValDef is CIMClassV_AVL)
            {
                // Object Attribute
                code = GenerateVAL_AVL((CIMClassV_AVL)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_LIN)
            {
                code = GenerateVAL_LIN((CIMClassV_LIN)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_LST)
            {
                code = GenerateVAL_LST((CIMClassV_LST)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_UNY)
            {
                code = GenerateVAL_UNY((CIMClassV_UNY)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_TRV)
            {
                code = GenerateVAL_TRV((CIMClassV_TRV)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_ISR)
            {
                code = GenerateVAL_ISR((CIMClassV_ISR)subValDef, out varName, sqlStyle);
            }
            else if (subValDef is CIMClassV_EDV)
            {
                code = GenerateVAL_EDV((CIMClassV_EDV)subValDef, out varName, sqlStyle);
            }
            else if (subValDef is CIMClassV_TVL)
            {
                code = GenerateVAL_TVL((CIMClassV_TVL)subValDef, out varName, sqlStyle);
            }
            else if (subValDef is CIMClassV_LRL)
            {
                code = GenerateVAL_LRL((CIMClassV_LRL)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_LBO)
            {
                code = GenerateVAL_LBO((CIMClassV_LBO)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_BIN)
            {
                code = GenerateVAL_BIN((CIMClassV_BIN)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_LEN)
            {
                code = GenerateVAL_LEN((CIMClassV_LEN)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_MVL)
            {
                code = GenerateVAL_MVL((CIMClassV_MVL)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_AER)
            {
                code = GenerateVAL_AER((CIMClassV_AER)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_ALV)
            {
                code = GenerateVAL_ALV((CIMClassV_ALV)subValDef, sqlStyle);
            }
            else if (subValDef is CIMClassV_SCV)
            {
                code = GenerateVAL_SCV((CIMClassV_SCV)subValDef, sqlStyle);
            }
            else
            {
                logger.LogWarning($"Unknown VAL_?:'{subValDef.GetType().Name}'");
                throw new NotImplementedException();
            }

            return code;
        }

        

        protected VariableDef HasDeclaredVariable(string varName)
        {
            VariableDef declared = null;
            foreach (var dictionary in declaredVariables)
            {
                var candidates = dictionary.Keys.Where(k => (k == varName));
                if (candidates.Count() > 0)
                {
                    declared = dictionary[candidates.First()];
                    break;
                }
            }
            return declared;
        }

        protected int blockDepth = -1;

        protected void DeclaredVariable(VariableDef varDef)
        {
            if (varDef.Declared == false)
            {
                varDef.Declared = true;
                varDef.DeclaredDepth = blockDepth;
            }
        }


        protected string GenerateVAL_FNV(CIMClassV_FNV fnvDef, bool sqlStyle = false)
        {
            // Function Invocation
            var syncDef = fnvDef.LinkedToR827();
            var syncParamDefs = fnvDef.LinkedFromR817();
            string paramCode = "";
            foreach (var syncParamDef in syncParamDefs)
            {
                var pValDef = syncParamDef.LinkedToR800();
                string pVarName;
                string pValCode = GenerateV_VAL(pValDef, out pVarName);
                if (!string.IsNullOrEmpty(paramCode))
                {
                    paramCode += ", ";
                }
                paramCode += $"{syncParamDef.Attr_Name}:{pValCode}";
            }
            string result = $"this.{syncDef.Attr_Name}({paramCode})";
            if (sqlStyle)
            {
                result = "{" + result + "}";
            }
            return result;
        }
        protected string GenerateVAL_PVL(CIMClassV_PVL pvlDef, out string varName, bool sqlStyle = false)
        {
            // parameter value for any operations of domain, class, external entitiy and so on
            string code = "";
            var sbparmDef = pvlDef.LinkedToR831();
            if (sbparmDef != null)
            {
                code = sbparmDef.Attr_Name;
            }
            var ssparmDef = pvlDef.LinkedToR832();
            if (ssparmDef != null)
            {
                code = ssparmDef.Attr_Name;
            }
            var otparmDef = pvlDef.LinkedToR833();
            if (otparmDef != null)
            {
                code = otparmDef.Attr_Name;
            }
            var cppDef = pvlDef.LinkedToR843();
            if (cppDef != null)
            {
                code = cppDef.Attr_Name;
            }
            varName = code;

            return code;
        }
        protected string GenerateVAL_SLR(CIMClassV_SLR slrDef, out string varName, bool sqlStyle = false)
        {
            string code = "";

            var attrDef = slrDef.LinkedToR812();
            if (attrDef == null)
            {
                code = "selected";
                var varDef = HasDeclaredVariable(code);

                //                if (varDef == null)
                //                {
                //                    varDef = new VariableDef() { Name = code, Declared = true };
                //                    declaredVariables[declaredVariables.Count-1].Add(code, varDef);
                //                }
            }
            var vtrvDef = slrDef.LinkedToR825();
            varName = code;

            if (sqlStyle)
            {
                code = "";
            }

            return code;
        }
        protected string GenerateVAL_BRV(CIMClassV_BRV brvDef, bool sqlStyle = false)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            // External Entity Operation invocation
            var brgDef = brvDef.LinkedToR828();
            var parmDefs = brvDef.LinkedFromR810();
            string paramCode = "";
            foreach (var parmDef in parmDefs)
            {
                var paramValDef = parmDef.LinkedToR800();
                string varName;
                string paramValCode = GenerateV_VAL(paramValDef, out varName);
                if (!string.IsNullOrEmpty(paramCode))
                {
                    paramCode += ", ";
                }
                paramCode += $"{parmDef.Attr_Name}:{paramValCode}";
            }
            var eeDef = brgDef.LinkedToR19();
            var dtDef = brgDef.LinkedToR20();

            string eeWrapper = GeneratorNames.GetExternalEntityWrappterClassName(eeDef, isAzureDigitalTwins);
            string eeWrapperRefVarName = GeneratorNames.GetExternalEntityWrapperRefVarName(eeDef);
            if (!eeReferDefs.ContainsKey(eeDef.Attr_Key_Lett))
            {
                string eeRefCode = $"var {eeWrapperRefVarName} = ({eeWrapper})instanceRepository.GetExternalEntity(\"{eeDef.Attr_Key_Lett}\")";
                eeReferDefs.Add(eeDef.Attr_Key_Lett, eeRefCode);
            }
            if (!usedExternalEntities.ContainsKey(eeDef.Attr_Key_Lett))
            {
                usedExternalEntities.Add(eeDef.Attr_Key_Lett, eeDef);
            }

            string result = $"{eeWrapperRefVarName}.{brgDef.Attr_Name}({paramCode})";
            if (sqlStyle)
            {
                result = "{" + result + "}";
            }
            return result;
        }
        protected string GenerateVAL_IRF(CIMClassV_IRF irfDef, out string varName, bool sqlStyle = false)
        {
            // instance reference value
            var varDef = irfDef.LinkedToR808();
            string code = varDef.Attr_Name;
            if (varDef.Attr_Name.ToLower() == "self")
            {
                code = this.selfVarNameOnCode;
            }
            else
            {
                code = varDef.Attr_Name;
                var declaredVarDef = HasDeclaredVariable(varDef.Attr_Name);
                //                if (declaredVarDef==null)
                //                {
                //                    var instVarDef = new VariableDef() { Name = varDef.Attr_Name, Declared = false, VarDef = varDef };
                //                    declaredVariables[declaredVariables.Count - 1].Add(varDef.Attr_Name, instVarDef);
                //                    var subVarDef = varDef.SubClassR814();
                //                }
            }
            varName = code;
            if (sqlStyle)
            {
                code = "";
            }
            return code;
        }

        protected bool addDomainClassCast = false;
        protected string GenerateVAL_AVL(CIMClassV_AVL avlDef, bool sqlStyle = false)
        {
            // Class Property Value
            var attrDef = avlDef.LinkedToR806();
            string attrName = GeneratorNames.GetAttrPropertyName(attrDef);
            var valDef = avlDef.LinkedToR807();
            string varName;
            string valCode = GenerateV_VAL(valDef, out varName, sqlStyle);

            string code = $"{valCode}.{attrName}";
            if (addDomainClassCast || varName == "selected")
            {
                var objDef = attrDef.LinkedToR102();
                string domainClassName = GeneratorNames.GetDomainClassName(objDef);
                code = $"(({domainClassName}){valCode}).{attrName}";
            }

            string result = $"{code}";
            if (sqlStyle)
            {
                result = "{" + result + "}";
                if (string.IsNullOrEmpty(valCode))
                {
                    result = attrDef.Attr_Name;
                }
            }
            return result;
        }
        protected string GenerateVAL_LIN(CIMClassV_LIN linDef, bool sqlStyle = false)
        {
            // integer value
            return $"{linDef.Attr_Value}";
        }
        protected string GenerateVAL_LST(CIMClassV_LST lstDef, bool sqlStyle = false)
        {
            // string value
            string result = $"\"{lstDef.Attr_Value}\"";
            if (sqlStyle)
            {
                result = $"'{lstDef.Attr_Value}'";
            }
            return result;
        }
        protected string GenerateVAL_UNY(CIMClassV_UNY unyDef, bool sqlStyle = false)
        {
            // Unery
            string code = "";

            var oprdValDef = unyDef.LinkedToR804();
            string oprdVarName;
            string oprdCode = GenerateV_VAL(oprdValDef, out oprdVarName);
            if (!string.IsNullOrEmpty(oprdVarName))
            {
                var oprdVarDef = HasDeclaredVariable(oprdVarName);
            }

            var oprdDtDef = oprdValDef.LinkedToR820();
            if (unyDef.Attr_Operator == "not_empty")
            {
                if (oprdDtDef.Attr_Name == "inst_ref<Object>")
                {
                    code = $"{oprdCode} != null";
                }
                else
                {
                    code = $"{oprdCode}.Count() > 0";
                }
            }
            else if (unyDef.Attr_Operator == "empty")
            {
                if (oprdDtDef.Attr_Name == "inst_ref<Object>")
                {
                    code = $"{oprdCode} == null";
                }
                else
                {
                    code = $"{oprdCode}.Count() == 0";
                }
            }
            else if (unyDef.Attr_Operator == "cardinality")
            {
                code = $"{oprdCode}.Count()";
            }
            else if (unyDef.Attr_Operator == "-")
            {
                code = $"-{oprdCode}";
            }
            else if (unyDef.Attr_Operator.ToLower() == "not")
            {
                code = $"! ({oprdCode})";
                if (sqlStyle)
                {
                    code = $"NOT ({oprdCode})";
                }
            }
            else
            {
                Console.WriteLine($"Unknown Operator : {unyDef.Attr_Operator}");
            }

            return $"{code}";
        }
        protected string GenerateVAL_TRV(CIMClassV_TRV trvDef, bool sqlStyle = false)
        {
            // Invoke Domain Class Operation
            var valDef = trvDef.LinkedToR830();
            var trfDef = trvDef.LinkedToR829();
            var vslrDef = trvDef.LinkedFromR825();
            var paramDefs = trvDef.LinkedFromR811();
            string paramCode = "";
            foreach (var paramDef in paramDefs)
            {
                var paramValDef = paramDef.LinkedToR800();
                if (!string.IsNullOrEmpty(paramCode))
                {
                    paramCode += ", ";
                }
                string paramVarName;
                string paramValCode = GenerateV_VAL(paramValDef, out paramVarName);
                paramCode += $"{paramDef.Attr_Name}:{paramValCode}";
            }
            string valName = valDef.Attr_Name;
            if (valName.ToLower() == "self")
            {
                valName = selfVarNameOnCode;
            }
            string result = $"{valName}.{trfDef.Attr_Name}({paramCode})";
            if (sqlStyle)
            {
                result = "{" + result + "}";
            }
            return result;
        }
        protected string GenerateVAL_ISR(CIMClassV_ISR isrDef, out string varName, bool sqlStyle = false)
        {
            // TODO: continue
            var varDef = isrDef.LinkedToR809();
            varName = varDef.Attr_Name;
            var hasVarDef = HasDeclaredVariable(varName);
            if (hasVarDef == null)
            {
                hasVarDef = new VariableDef() { Name = varName, Declared = false, VarDef = varDef };
                var subVarDef = varDef.SubClassR814();
            }
            //            isDecl = varDef.Attr_Declared;
            return $"{varDef.Attr_Name}";
        }
        protected string GenerateVAL_EDV(CIMClassV_EDV edvDef, out string varName, bool sqlStyle = false)
        {
            string code = "";
            varName = null;
            var veprDefs = edvDef.LinkedFromR834();
            foreach (var veprDef in veprDefs)
            {
                var evtdiDef = veprDef.LinkedToR846();
                var cppDef = veprDef.LinkedToR847();
                code = evtdiDef.Attr_Name;
                varName = code;
                var hasVarName = HasDeclaredVariable(varName);
                if (hasVarName == null)
                {

                }
            }
            return code;
        }
        protected string GenerateVAL_TVL(CIMClassV_TVL tvlDef, out string varName, bool sqlStyle = false)
        {
            string code = "";
            var varDef = tvlDef.LinkedToR805();
            code = varDef.Attr_Name;
            var hasVarDef = HasDeclaredVariable(varDef.Attr_Name);
            //            if (hasVarDef == null)
            //            {
            //                hasVarDef = new VariableDef() { Name=code, Declared=false, VarDef=varDef};
            //                declaredVariables[declaredVariables.Count-1].Add(code, hasVarDef);
            //                var subVarDef = varDef.SubClassR814();
            //            }
            //          isDecl = varDef.Attr_Declared;
            varName = code;

            return code;
        }
        protected string GenerateVAL_LRL(CIMClassV_LRL lrlDef, bool sqlStyle = false)
        {
            // real value
            //      isDecl =false;
            return $"{lrlDef.Attr_Value}";
        }
        protected string GenerateVAL_LBO(CIMClassV_LBO lboDef, bool sqlStyle = false)
        {
            string result = "";
            if (lboDef.Attr_Value.ToLower() == "true")
            {
                result = "true";
            }
            else
            {
                result = "false";
            }
            if (sqlStyle)
            {
                result = result.ToUpper();
            }

            return result;
        }
        protected string GenerateVAL_BIN(CIMClassV_BIN binDef, bool sqlStyle = false)
        {
            string code = "";

            string op = binDef.Attr_Operator;
            var valLeftDef = binDef.LinkedToR802();
            var valRightDef = binDef.LinkedToR803();

            string leftVarName;
            var leftValCode = GenerateV_VAL(valLeftDef, out leftVarName, sqlStyle);
            string rightVarName;
            var rightValCode = GenerateV_VAL(valRightDef, out rightVarName, sqlStyle);
            if (op == "and")
            {
                op = "&&";
                if (sqlStyle)
                {
                    op = "AND";
                }
            }
            else if (op == "or")
            {
                op = "||";
                if (sqlStyle)
                {
                    op = "OR";
                }
            }
            else if (op == "==")
            {
                if (sqlStyle)
                {
                    op = "=";
                }
            }
            code = $"({leftValCode} {op} {rightValCode})";

            return code;
        }
        protected string GenerateVAL_LEN(CIMClassV_LEN lenDef, bool sqlStyle = false)
        {
            // Enum value
            var enumDef = lenDef.LinkedToR824();
            var edtDef = enumDef.LinkedToR27();
            string dataTypeName = GeneratorNames.GetEnumDataTypeName(edtDef);

            return $"{dataTypeName}.{enumDef.Attr_Name}";
        }
        protected string GenerateVAL_MVL(CIMClassV_MVL mvlDef, bool sqlStyle = false)
        {
            var mbrDef = mvlDef.LinkedToR836();
            var sdtDef = mbrDef.LinkedToR44();
            string complexDataTypeName = GeneratorNames.GetComplexDataTypeName(sdtDef);
            var valDef = mvlDef.LinkedToR837();
            string varName;
            string valCode = GenerateV_VAL(valDef, out varName);
            string result = $"{valCode}.{mbrDef.Attr_Name}";
            if (sqlStyle)
            {
                result = "{" + result + "}";
            }
            return result;
        }
        protected string GenerateVAL_AER(CIMClassV_AER aerDef, bool sqlStyle = false)
        {
            logger.LogWarning($"V_AER({aerDef.ToString()}) is not supprted");
            throw new NotImplementedException();
        }
        protected string GenerateVAL_ALV(CIMClassV_ALV alvDef, bool sqlStyle = false)
        {
            logger.LogWarning($"V_ALV({alvDef.ToString()}) is not supprted");
            throw new NotImplementedException();
        }
        protected string GenerateVAL_SCV(CIMClassV_SCV scvDef, bool sqlStyle = false)
        {
            logger.LogWarning($"V_SCV({scvDef.ToString()}) is not supprted");
            throw new NotImplementedException();
        }

    }

    class VariableDef
    {
        public string Name { get; set; }
        public CIMClassV_VAR VarDef { get; set; } = null;
        public CIMClassV_PVL PvlDef { get; set; } = null;
        public CIMClassSM_EVTDI EvtDiDef { get; set; } = null;
        public bool Set { get; set; } = false;
        public bool Declared { get; set; } = false;
        public int DeclaredDepth { get; set; } = -1;

        public CIMClassS_DT GetDTDef()
        {
            CIMClassS_DT dt = null;
            if (VarDef != null)
            {
                var subVarDef = VarDef.SubClassR814();
                if (subVarDef is CIMClassV_TRN)
                {
                    var vTrnDef = (CIMClassV_TRN)subVarDef;
                    dt = vTrnDef.LinkedToR821();
                }
            }
            else if (PvlDef != null)
            {
                var bParm = PvlDef.LinkedToR831();
                if (bParm != null)
                {
                    dt = bParm.LinkedToR22();
                }
                var sParm = PvlDef.LinkedToR832();
                if (sParm != null)
                {
                    dt = sParm.LinkedToR26();
                }
                var tParm = PvlDef.LinkedToR833();
                if (tParm != null)
                {
                    dt = tParm.LinkedToR118();
                }
            }
            else if (EvtDiDef != null)
            {
                dt = EvtDiDef.LinkedToR524();
            }
            return dt;
        }
        public CIMClassO_OBJ GetObjDef()
        {
            CIMClassO_OBJ obj = null;
            if (VarDef != null)
            {
                var subVarDef = VarDef.SubClassR814();
                if (subVarDef is CIMClassV_INT)
                {
                    obj = ((CIMClassV_INT)subVarDef).LinkedToR818();
                }
                else if (subVarDef is CIMClassV_INS)
                {
                    obj = ((CIMClassV_INS)subVarDef).LinkedToR819();
                }
            }
            return obj;
        }
    }

    class RelatedByLinkUnit
    {
        public bool SetSource { get; set; }
        public bool SetDestination { get; set; }
        public bool CondDestination { get; set; }
        public string MethodName { get; set; }
        public CIMClassO_OBJ ObjDefOfSource { get; set; }
        public CIMClassO_OBJ ObjDefOfDestination { get; set; }
        public CIMClassACT_LNK Target { get; set; }
    }
}
