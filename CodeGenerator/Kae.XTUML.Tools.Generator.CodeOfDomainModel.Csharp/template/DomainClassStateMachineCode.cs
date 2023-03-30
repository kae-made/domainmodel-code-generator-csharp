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
    partial class DomainClassStateMachine
    {
        string version;
        string nameSpace;
        CIMClassO_OBJ objDef;
        CIMClassSM_SM smDef;
            
        public DomainClassStateMachine(string version, string nameSpace, CIMClassO_OBJ objDef, CIMClassSM_SM smDef)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.objDef = objDef;
            this.smDef = smDef;
        }

        public void prototype()
        {
            string stateMachineClassName = GeneratorNames.GetStateMachineClassName(objDef);

            var stateDefs = smDef.LinkedFromR501();
            var evtDefs = smDef.LinkedFromR502();

            int index = 0;

            bool hasCreationEvent = false;
            evtDefs = evtDefs.OrderBy(e => e.Attr_Numb);
            foreach (var evtDef in evtDefs)
            {
                var evtName = GeneratorNames.GetEventEnumLabelName(objDef, evtDef);
                var comment = $"// {evtDef.Attr_Mning}";

                var subEvtDef = evtDef.SubClassR525();
                if (subEvtDef is CIMClassSM_SEVT)
                {
                    var sevtDef = (CIMClassSM_SEVT)subEvtDef;
                    var subSevtDef = sevtDef.SubClassR526();
                    if (subSevtDef is CIMClassSM_NLEVT)
                    {
                        var nlevtDef = (CIMClassSM_NLEVT)subSevtDef;
                        var pevtDef = nlevtDef.LinkedToR527();

                    }
                    else if (subSevtDef is CIMClassSM_LEVT)
                    {
                        var levtDef = (CIMClassSM_LEVT)subSevtDef;
                        var crtxnDef = levtDef.LinkedFromR509();
                        if (crtxnDef != null)
                        {
                            var creationStateDef = crtxnDef.CIMSuperClassSM_TXN().LinkedToR506();
                            string creationStateName = GeneratorNames.GetStateEnumLabelName(creationStateDef);
                            hasCreationEvent = true;
                        }
                    }
                }
                else if (subEvtDef is CIMClassSM_PEVT)
                {
                    string getSubClassMethodNameOfpm = "";
                    var nlevtDefs = ((CIMClassSM_PEVT)subEvtDef).LinkedFromR527();
                    foreach (var nlevtDef in nlevtDefs)
                    {
                        var pmEvtDef = nlevtDef.CIMSuperClassSM_SEVT().CIMSuperClassSM_EVT();
                        var ismDef = pmEvtDef.LinkedToR502().SubClassR517();
                        if (ismDef != null)
                        {
                            var objDefOfpm = ((CIMClassSM_ISM)ismDef).LinkedToR518();
                            var oirDefs = objDefOfpm.LinkedOneSideR201();
                            foreach (var oirDef in oirDefs)
                            {
                                var rgoDef = oirDef.SubClassR203();
                                if (rgoDef is CIMClassR_RGO)
                                {
                                    var subRelDef = ((CIMClassR_RGO)rgoDef).SubClassR205();
                                    if (subRelDef is CIMClassR_SUB)
                                    {
                                        var subsupRelDef = ((CIMClassR_SUB)subRelDef).LinkedToR213();
                                        var superObjDef = subsupRelDef.LinkedFromR212().CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                                        if (superObjDef.Attr_Key_Lett == objDefOfpm.Attr_Key_Lett)
                                        {
                                            var relOfpm = subsupRelDef.CIMSuperClassR_REL();
                                            getSubClassMethodNameOfpm = GeneratorNames.GetSubRelClassMethodName(relOfpm);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (nlevtDefs.Count() > 0)
                    {

                    }
                    string prefixIf = "if";
                    foreach (var nlevtDef in nlevtDefs)
                    {
                        string domainClassNameOfpm = "";
                        string stateMachineClassNameOfpm = "";
                        string eventClassNameOfpm = "";
                        var pmEvtDef = nlevtDef.CIMSuperClassSM_SEVT().CIMSuperClassSM_EVT();
                        var ismDef = pmEvtDef.LinkedToR502().SubClassR517();
                        if (ismDef != null)
                        {
                            var objDefOfpm = ((CIMClassSM_ISM)ismDef).LinkedToR518();
                            domainClassNameOfpm = GeneratorNames.GetDomainClassName(objDefOfpm);
                            stateMachineClassNameOfpm = GeneratorNames.GetStateMachineClassName(objDefOfpm);
                            eventClassNameOfpm = GeneratorNames.GetEventClassName(objDefOfpm, pmEvtDef);
                        }
                        prefixIf = $"else {prefixIf}";
                    }
                }
            }

            stateDefs = stateDefs.OrderBy(s => s.Attr_Numb);
            foreach (var stateDef in stateDefs)
            {
                var stateName = GeneratorNames.GetStateEnumLabelName(stateDef);
                index++;
                if (index < stateDefs.Count())
                {

                }
            }

            var definedEventArgIFs = new List<string>();
            foreach (var evtDef in evtDefs)
            {
                bool definedEvent = false;
                string argIFName = GetEventArgsInterfaceName(evtDef, ref definedEvent);
                if (definedEvent)
                {
                    if (!definedEventArgIFs.Contains(argIFName))
                    {
                        definedEventArgIFs.Add(argIFName);
                    }
                }
            }

            index = 0;
            foreach (var evtDef in evtDefs)
            {
                var evtName = GeneratorNames.GetEventEnumLabelName(objDef, evtDef);
                var comment = $"// {evtDef.Attr_Mning}";

                var subEvtDef = evtDef.SubClassR525();
                if (subEvtDef is CIMClassSM_SEVT)
                {
                    var sevtDef = (CIMClassSM_SEVT)subEvtDef;
                    var subSevtDef = sevtDef.SubClassR526();
                    if (subSevtDef is CIMClassSM_NLEVT)
                    {
                        var nlevtDef = (CIMClassSM_NLEVT)subSevtDef;
                        var pevtDef = nlevtDef.LinkedToR527();

                    }
                    else if (subSevtDef is CIMClassSM_LEVT)
                    {
                        var levtDef = (CIMClassSM_LEVT)subSevtDef;
                        var crtxnDef = levtDef.LinkedFromR509();
                        if (crtxnDef != null)
                        {
                            var creationStateDef = crtxnDef.CIMSuperClassSM_TXN().LinkedToR506();
                            string creationStateName = GeneratorNames.GetStateEnumLabelName(creationStateDef);
                        }
                    }
                }
                else if (subEvtDef is CIMClassSM_PEVT)
                {

                }

                var evtdiDefs = evtDef.LinkedFromR532();
                foreach (var evtdiDef in evtdiDefs)
                {
                    var evtdiName = evtdiDef.Attr_Name;
                    var dtDef = evtdiDef.LinkedToR524();
                    var dataTypeName = DomainDataTypeDefs.GetDataTypeName(dtDef);
                }
            }

            string domainClassName = GeneratorNames.GetDomainClassName(objDef);
            if (hasCreationEvent)
            {

            }
            else
            {

            }

            foreach (var stateDef in stateDefs)
            {
                var semeEvents = new List<CIMClassSM_EVT>();
                var semeDefs = stateDef.LinkedOtherSideR503();

                string stateName = GeneratorNames.GetStateEnumLabelName(stateDef);

                foreach (var semeDef in semeDefs)
                {
                    var semeEvtDef = semeDef.LinkedOtherSideR503().CIMSuperClassSM_EVT();
                    var semeEvtName = GeneratorNames.GetEventEnumLabelName(objDef, semeEvtDef);
                    var subSemeDef = semeDef.SubClassR504();
                    string nextState = "";
                    if (subSemeDef is CIMClassSM_NSTXN)
                    {
                        var nextStateDef = ((CIMClassSM_NSTXN)subSemeDef).CIMSuperClassSM_TXN().LinkedToR506();
                        nextState = GeneratorNames.GetStateEnumLabelName(nextStateDef);
                    }
                    else if (subSemeDef is CIMClassSM_EIGN)
                    {
                        nextState = "ITransition.Transition.Ignore";
                    }
                    else if (subSemeDef is CIMClassSM_CH)
                    {
                        nextState = "ITransition.Transition.CantHappen";
                    }
                }
                var txnDefs = stateDef.LinkedFromR506();
                foreach (var txnDef in txnDefs)
                {
                    var subTxnDef = txnDef.SubClassR507();
                    if (subTxnDef is CIMClassSM_NETXN)
                    {
                        var netxnDef = (CIMClassSM_NETXN)subTxnDef;
                        var levtDef = netxnDef.LinkedToR508();
                    }
                    else if (subTxnDef is CIMClassSM_CRTXN)
                    {
                        var crtxnDef = (CIMClassSM_CRTXN)subTxnDef;
                        var crtxnEvtDef = crtxnDef.LinkedToR509().CIMSuperClassSM_SEVT().CIMSuperClassSM_EVT();
                    }
                    else if (subTxnDef is CIMClassSM_NSTXN)
                    {
                        var nstxDef = (CIMClassSM_NSTXN)subTxnDef;

                    }
                }
            }

            var stateTransferTableDef = new List<StateTransfersEntry>();
            foreach (var stateDef in stateDefs)
            {
                var semeDefs = stateDef.LinkedOtherSideR503();
                var nextTransitions = new List<NexTransition>();
                foreach(var semeDef in semeDefs)
                {
                    var triggerEvtDef = semeDef.LinkedOtherSideR503().CIMSuperClassSM_EVT();
                    var subSemeDef = semeDef.SubClassR504();
                    var nextTransition = new NexTransition() { EvtDef = triggerEvtDef, EventNumber=triggerEvtDef.Attr_Numb, NextStateDef = null, IsCantHappen = false, IsIgnore = false };
                    if (subSemeDef is CIMClassSM_NETXN)
                    {
                        var netxnDef = (CIMClassSM_NETXN)subSemeDef;
                        var nextStateDef = netxnDef.CIMSuperClassSM_TXN().LinkedToR506();
                        nextTransition.NextStateDef = nextStateDef;
                    }
                    else if (subSemeDef is CIMClassSM_EIGN)
                    {
                        nextTransition.IsIgnore= true;
                    }
                    else if (subSemeDef is CIMClassSM_CH)
                    {
                        nextTransition.IsCantHappen = true;
                    }
                    nextTransitions.Add(nextTransition);
                }
                var nextTnss = nextTransitions.OrderBy(nxt => (nxt.EventNumber));
                var sttEntry = new StateTransfersEntry() { StateDef = stateDef, StateNumber = stateDef.Attr_Numb, NexTransitions = nextTnss };
                stateTransferTableDef.Add(sttEntry);
            }

            int sttColumns = stateTransferTableDef[0].NexTransitions.Count();
            int sttRows = stateTransferTableDef.Count;
            var sttDef = (List<StateTransfersEntry>)stateTransferTableDef.OrderBy(entry => entry.StateNumber);
            index = 0;
            foreach(var sttEntry in sttDef)
            {
                string sttRowDef = "";
                foreach(var nt in sttEntry.NexTransitions)
                {
                    if (!string.IsNullOrEmpty(sttRowDef))
                    {
                        sttRowDef += ", ";
                    }
                    string nextState = "";
                    if (nt.NextStateDef != null)
                    {
                        nextState = $"States.{GeneratorNames.ToProgramAvailableString(nt.NextStateDef.Attr_Name)}";
                    }
                    else
                    {
                        if (nt.IsIgnore)
                        {
                            nextState = "ITransition.Transition.Ignore";
                        }
                        else if (nt.IsCantHappen)
                        {
                            nextState = "ITransition.Transition.CantHappen";
                        }
                    }
                    sttRowDef += "{" + $"(int){nextState}" + "}";
                }
                if (++index < stateTransferTableDef.Count())
                {

                }
            }

            var logSttTxn0 = new logging.Logging("logger", "", objDef, "this", logging.Logging.Mode.StatTransition, "entering");
            var logSttTxnE0Gen = logSttTxn0.TransformText();
            var logSttTxn1 = new logging.Logging("logger", "", objDef, "this", logging.Logging.Mode.StatTransition, "entered");
            var logSttTxnE1Gen = logSttTxn1.TransformText();

            foreach (var stateDef in stateDefs)
            {
                var stateMethodName = GeneratorNames.GetStateActionMethodName(stateDef);
                var stateLabelName = GeneratorNames.GetStateEnumLabelName(stateDef);

                bool ifDefined = false;
                string argsIfName = "";
                var triggerEvtDef = GetEventArgsInterfaceNameForTheState(stateDef,ref argsIfName, ref ifDefined);
                string actionArgs = "";
                if (ifDefined)
                {
                    var evtdiDefs = triggerEvtDef.LinkedFromR532();
                    foreach (var evtdiDef in evtdiDefs)
                    {
                        var argName = evtdiDef.Attr_Name;
                        var argDtDef = evtdiDef.LinkedToR524();
                        var argTypeName = DomainDataTypeDefs.GetDataTypeName(argDtDef);
                        if (!string.IsNullOrEmpty(actionArgs))
                        {
                            actionArgs += ", ";
                        }
                        actionArgs += $"(({argsIfName})eventData).{argName}";
                    }
                }

            }
        }

        class NexTransition
        {
            public int EventNumber { get; set; }
            public CIMClassSM_EVT EvtDef { get; set; }
            public CIMClassSM_STATE NextStateDef { get; set; }
            public bool IsIgnore { get; set; }
            public bool IsCantHappen { get; set; }
        }

        class StateTransfersEntry
        {
            public CIMClassSM_STATE StateDef { get; set; }
            public int StateNumber { get; set; }
            public IEnumerable<NexTransition> NexTransitions { get; set; }
        }

        public static string GetEventArgsInterfaceName(CIMClassSM_EVT evtDef,ref bool defined)
        {
            defined = false;
            string args = "";
            var evtdiDefs = evtDef.LinkedFromR532();
            foreach(var evtdiDef in evtdiDefs)
            {
                string evtdiName = evtdiDef.Attr_Name;
                string frag = evtdiName.Substring(0, 1).ToUpper() + evtdiName.Substring(1);
                args += frag;
                defined = true;
            }
            return $"IEventArgs{args}Def";
        }

        public static CIMClassSM_EVT GetEventArgsInterfaceNameForTheState(CIMClassSM_STATE stateDef,ref string argsIfName, ref bool defined)
        {
            defined = false;
            argsIfName = "";
            CIMClassSM_EVT triggerEvtDef = GetTriggerEvent(stateDef);
            if (triggerEvtDef != null)
            {
                argsIfName = GetEventArgsInterfaceName(triggerEvtDef, ref defined);
            }
            return triggerEvtDef;
        }

        public static CIMClassSM_EVT GetTriggerEvent(CIMClassSM_STATE stateDef)
        {
            CIMClassSM_EVT triggerEvtDef = null;
            var txnDefs = stateDef.LinkedFromR506();
            foreach (var txnDef in txnDefs)
            {
                var subTxnDef = txnDef.SubClassR507();
                if (subTxnDef is CIMClassSM_NETXN)
                {
                    var netxnDef = (CIMClassSM_NETXN)subTxnDef;
                    /// ???
                }
                else if (subTxnDef is CIMClassSM_CRTXN)
                {
                    var crtxnDef = (CIMClassSM_CRTXN)subTxnDef;
                    triggerEvtDef = crtxnDef.LinkedToR509().CIMSuperClassSM_SEVT().CIMSuperClassSM_EVT();
                }
                else if (subTxnDef is CIMClassSM_NSTXN)
                {
                    var nstxnDef = (CIMClassSM_NSTXN)subTxnDef;
                    triggerEvtDef = nstxnDef.CIMSuperClassSM_SEME().LinkedOtherSideR503().CIMSuperClassSM_EVT();
                }
                if (triggerEvtDef != null)
                    break;
            }

            return triggerEvtDef;
        }

        public static bool IsCreationEvent(CIMClassSM_EVT evtDef)
        {
            bool result = false;
            var subEvtDef = evtDef.SubClassR525();
            if (subEvtDef is CIMClassSM_SEVT)
            {
                var subSevtDef = ((CIMClassSM_SEVT)(subEvtDef)).SubClassR526();
                if (subSevtDef is CIMClassSM_LEVT)
                {
                    var crtxnDef = ((CIMClassSM_LEVT)subSevtDef).LinkedFromR509();
                    if (crtxnDef != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
