// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class DomainClassActions
    {
        string version;
        string nameSpace;
        CIMClassO_OBJ objDef;
        CIMClassSM_SM smDef;
        bool generateCode;

        public DomainClassActions(string version, string nameSpace, CIMClassO_OBJ objDef, CIMClassSM_SM smDef, bool generateCode)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.objDef = objDef;
            this.smDef = smDef;
            this.generateCode = generateCode;
        }

        public void prototypeAction()
        {
            var stateDefs = smDef.LinkedFromR501();
            foreach (var stateDef in stateDefs)
            {
                var moahDef = stateDef.LinkedOneSideR511();
                var smActDef = moahDef.CIMSuperClassSM_AH().LinkedToR514();
                string smActDescip = smActDef.Attr_Action_Semantics;
                var sabDef = smActDef.LinkedFromR691();
                if (sabDef != null)
                {
                    Console.WriteLine($"Generating '{stateDef.Attr_Numb}.{stateDef.Attr_Name}'...");
                    var actDef = sabDef.CIMSuperClassACT_ACT();
                    var actionGen = new ActDescripGenerator(actDef, "this", "    ", "        ");
                    string code = actionGen.Generate();
                }
            }
        }

        public void prototype()
        {
            string stateMachineClassName = GeneratorNames.GetStateMachineClassName(objDef);

            var stateDefs = smDef.LinkedFromR501();

            foreach(var stateDef in stateDefs)
            {
                string actionMethodName = GeneratorNames.GetStateActionMethodName(stateDef);

                string descrip = "";
                var moahDef = stateDef.LinkedOneSideR511();
                if(moahDef != null)
                {
                    var actDef = moahDef.CIMSuperClassSM_AH().LinkedToR514();
                    if (actDef != null)
                    {
                        descrip = GeneratorNames.DescripToCodeComment("        ", actDef.Attr_Action_Semantics);
                        if (descrip.EndsWith(Environment.NewLine))
                        {
                            descrip=descrip.Substring(0, descrip.Length - Environment.NewLine.Length);
                        }
                    }
                }

                var triggerEvtDef = DomainClassStateMachine.GetTriggerEvent(stateDef);
                var evtDtiDefs = triggerEvtDef.LinkedFromR532();
                string args = "";
                foreach(var evtDtiDef in evtDtiDefs)
                {
                    string argName = evtDtiDef.Attr_Name;
                    var argDtDef = evtDtiDef.LinkedToR524();
                    string argTypeName = DomainDataTypeDefs.GetDataTypeName(argDtDef);
                    if (!string.IsNullOrEmpty(args))
                    {
                        args += ", ";
                    }
                    args += $"{argTypeName} {argName}";
                }


                //string descrip = GeneratorNames.DescripToCodeComment()
                //throw new NotImplementedException();
            }

        }
    }
}
