// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp
{
    public static class EventDelegate
    {
        public static string GetDelegateMethodName(CIMClassSM_PEVT polymorphicEvtDef)
        {
            string methodName = "";
            var evtDef = polymorphicEvtDef.CIMSuperClassSM_EVT();
            var ismDef = evtDef.LinkedToR502().SubClassR517();
            if (ismDef is CIMClassSM_ISM)
            {
                var objDef = ((CIMClassSM_ISM)ismDef).LinkedToR518();
                methodName = $"DelegateEvent{objDef.Attr_Key_Lett}{evtDef.Attr_Numb}";
            }
            return methodName;
        }

        public static CIMClassO_OBJ[] GetSuperClass(CIMClassO_OBJ objDef)
        {
            var superObjDefs = new List<CIMClassO_OBJ>();
            var oirDefs = objDef.LinkedOneSideR201();
            foreach (var oirDef in oirDefs)
            {
                var rgoDef = oirDef.SubClassR203();
                if (rgoDef is CIMClassR_RGO)
                {
                    var subsupDef = ((CIMClassR_RGO)rgoDef).SubClassR205();
                    if (subsupDef is CIMClassR_SUBSUP)
                    {
                        var superDef = ((CIMClassR_SUBSUP)subsupDef).LinkedFromR212();
                        superObjDefs.Add(superDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104());
                        break;
                    }
                }
            }
            return superObjDefs.ToArray();
        }

        public static CIMClassSM_PEVT[] GetPolymorphicEvetns(CIMClassO_OBJ objDef)
        {
            var pevtDefs = new List<CIMClassSM_PEVT>();

            var ismDef = objDef.LinkedFromR518();
            if (ismDef != null)
            {
                var evtDefs = ismDef.CIMSuperClassSM_SM().LinkedFromR502();
                foreach (var evtDef in evtDefs)
                {
                    var pevtDef = evtDef.SubClassR525();
                    if (pevtDef is CIMClassSM_PEVT)
                    {
                        pevtDefs.Add((CIMClassSM_PEVT)pevtDef);
                    }
                }
            }
            return pevtDefs.ToArray();
        }

        public static CIMClassSM_NLEVT[] GetImplementedEvents(CIMClassO_OBJ objDef)
        {
            var nlevtDefs = new List<CIMClassSM_NLEVT>();

            var ismDef = objDef.LinkedFromR518();
            if (ismDef != null)
            {
                var evtDefs = ismDef.CIMSuperClassSM_SM().LinkedFromR502();
                foreach (var evtDef in evtDefs)
                {
                    var sevtDef = evtDef.SubClassR525();
                    if (sevtDef is CIMClassSM_SEVT)
                    {
                        var nlevtDef = ((CIMClassSM_SEVT)sevtDef).SubClassR526();
                        if (nlevtDef is CIMClassSM_NLEVT)
                        {
                            nlevtDefs.Add((CIMClassSM_NLEVT)nlevtDef);
                        }
                    }
                }
            }

            return nlevtDefs.ToArray();
        }

        public static (CIMClassO_OBJ[] superObjDefs, CIMClassO_OBJ[] subObjDefs) GetSubsupObjs(CIMClassO_OBJ objDef)
        {
            var superObjDefs = new List<CIMClassO_OBJ>();
            var subObjDefs = new List<CIMClassO_OBJ>();

            var oirDefs = objDef.LinkedOneSideR201();
            foreach (var oirDef in oirDefs)
            {
                var subOirDef = oirDef.SubClassR203();
                if (subOirDef is CIMClassR_RGO)
                {
                    var subRgoDef = ((CIMClassR_RGO)subOirDef).SubClassR205();
                    if (subRgoDef is CIMClassR_SUB)
                    {
                        superObjDefs.Add(((CIMClassR_SUB)subRgoDef).LinkedToR213().LinkedFromR212().CIMSuperClassR_RTO().LinkedToR109().LinkedToR104());
                    }
                }
                else if (subOirDef is CIMClassR_RTO)
                {
                    var subRtoDef = ((CIMClassR_RTO)subOirDef).SubClassR204();
                    if (subRtoDef is CIMClassR_SUPER)
                    {
                        var rsubDefs = ((CIMClassR_SUPER)subRtoDef).LinkedToR212().LinkedFromR213();
                        foreach (var rsubDef in rsubDefs)
                        {
                            subObjDefs.Add(rsubDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201());
                        }
                    }
                }
            }

            return (superObjDefs.ToArray(), subObjDefs.ToArray());
        }

        public static CIMClassSM_PEVT[] GetUnimplementedEvents(CIMClassO_OBJ objDef, bool includingOfSelf)
        {
            var uipEvtDefs = new List<CIMClassSM_PEVT>();

            var parents = new List<CIMClassO_OBJ>();

            PickupUnimplementedEvent(objDef, parents, uipEvtDefs);

            if (includingOfSelf)
            {
                var impEvtDefs = GetImplementedEvents(objDef);
                foreach (var impEvtDef in impEvtDefs)
                {
                    var pEvtDef = impEvtDef.LinkedToR527();
                    var resistered = uipEvtDefs.Where(item => item.Attr_SMevt_ID == pEvtDef.Attr_SMevt_ID).FirstOrDefault();
                    if (resistered != null)
                    {
                        uipEvtDefs.Remove(pEvtDef);
                    }
                }
            }

            return uipEvtDefs.ToArray();
        }

        public static void PickupUnimplementedEvent(CIMClassO_OBJ objDef, List<CIMClassO_OBJ> parents, List<CIMClassSM_PEVT> uipEvtDefs)
        {
            var oirDefs = objDef.LinkedOneSideR201();
            PickupUnresisteredPEvents(parents, uipEvtDefs, objDef);
            foreach (var oirDef in oirDefs)
            {
                var rtoDef = oirDef.SubClassR203();
                if (rtoDef is CIMClassR_RTO)
                {
                    var superDef = ((CIMClassR_RTO)rtoDef).SubClassR204();
                    if (superDef is CIMClassR_SUPER)
                    {
                        var subDefs = ((CIMClassR_SUPER)superDef).LinkedToR212().LinkedFromR213();
                        foreach (var subDef in subDefs)
                        {
                            var subObjDef = subDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                            PickupUnresisteredPEvents(parents, uipEvtDefs, subObjDef);
                            parents.Add(subObjDef);
                            PickupUnimplementedEvent(subObjDef, parents, uipEvtDefs);
                            parents.Remove(subObjDef);
                        }
                    }
                }
            }

        }

        private static void PickupUnresisteredPEvents(List<CIMClassO_OBJ> parents, List<CIMClassSM_PEVT> uipEvtDefs, CIMClassO_OBJ subObjDef)
        {
            var impEvtDefs = GetImplementedEvents(subObjDef);
            foreach (var impEvtDef in impEvtDefs)
            {
                var pevtDef = impEvtDef.LinkedToR527();
                var ismDef = pevtDef.CIMSuperClassSM_EVT().LinkedToR502().SubClassR517();
                if (ismDef is CIMClassSM_ISM)
                {
                    var objForPevtDef = ((CIMClassSM_ISM)ismDef).LinkedToR518();
                    bool unimplemented = true;
                    foreach (var parentDef in parents)
                    {
                        if (parentDef.Attr_Key_Lett == objForPevtDef.Attr_Key_Lett)
                        {
                            unimplemented = false;
                            break;
                        }
                    }
                    if (unimplemented)
                    {
                        var existedPevtDefs = uipEvtDefs.Where(item => item.Attr_SMevt_ID == pevtDef.Attr_SMevt_ID).FirstOrDefault();
                        if (existedPevtDefs == null)
                        {
                            uipEvtDefs.Add(pevtDef);
                        }
                    }
                }
            }
        }

        public static CIMClassSM_PEVT[] GetUnimplementedEventsUpper(CIMClassO_OBJ objDef)
        {
            var uipEvtDefs = new List<CIMClassSM_PEVT>();

            PickupPolymorphicEventsUpper(uipEvtDefs, objDef);

            PurgeImpletementedEvents(uipEvtDefs, objDef);

            if (uipEvtDefs.Count > 0)
            {
                RemoveImplementedEventsUper(uipEvtDefs, objDef);
            }

            return uipEvtDefs.ToArray();
        }

        public static void PickupPolymorphicEventsUpper(List<CIMClassSM_PEVT> pEvtDefs, CIMClassO_OBJ objDef)
        {
            var oirDefs = objDef.LinkedOneSideR201();
            foreach (var oirDef in oirDefs)
            {
                var rgoDef = oirDef.SubClassR203();
                if (rgoDef is CIMClassR_RGO)
                {
                    var rsubDef = ((CIMClassR_RGO)rgoDef).SubClassR205();
                    if (rsubDef is CIMClassR_SUB)
                    {
                        var rsuperDef = ((CIMClassR_SUB)rsubDef).LinkedToR213().LinkedFromR212();
                        var superObjDef = rsuperDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                        var ismDef = superObjDef.LinkedFromR518();
                        if (ismDef != null)
                        {
                            var evtDefs = ismDef.CIMSuperClassSM_SM().LinkedFromR502();
                            foreach (var evtDef in evtDefs)
                            {
                                var pEvtDef = evtDef.SubClassR525();
                                if (pEvtDef is CIMClassSM_PEVT)
                                {
                                    pEvtDefs.Add((CIMClassSM_PEVT)pEvtDef);
                                }
                            }
                        }
                        PickupPolymorphicEventsUpper(pEvtDefs, superObjDef);
                    }
                }
            }
        }

        public static void RemoveImplementedEventsUper(List<CIMClassSM_PEVT> pEvtDefs, CIMClassO_OBJ objDef)
        {
            var oirDefs = objDef.LinkedOneSideR201();
            foreach (var oirDef in oirDefs)
            {
                var rgoDef = oirDef.SubClassR203();
                if (rgoDef is CIMClassR_RGO)
                {
                    var rsubDef = ((CIMClassR_RGO)rgoDef).SubClassR205();
                    if (rsubDef is CIMClassR_SUB)
                    {
                        var rsuperDef = ((CIMClassR_SUB)rsubDef).LinkedToR213().LinkedFromR212();
                        var superObjDef = rsuperDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                        PurgeImpletementedEvents(pEvtDefs, superObjDef);
                        RemoveImplementedEventsUper(pEvtDefs, superObjDef);
                    }
                }
            }
        }

        private static void PurgeImpletementedEvents(List<CIMClassSM_PEVT> pEvtDefs, CIMClassO_OBJ objDef)
        {
            var impldEvtDefs = GetImplementedEvents(objDef);
            foreach (var nlEvtDef in impldEvtDefs)
            {
                var pEvtDef = nlEvtDef.LinkedToR527();
                foreach (var rPEvtDef in pEvtDefs)
                {
                    if (pEvtDef.Attr_SMevt_ID == rPEvtDef.Attr_SMevt_ID)
                    {
                        pEvtDefs.Remove(pEvtDef);
                        break;
                    }
                }
            }
        }

        public static CIMClassSM_PEVT[] RemoveGeneratedPEvents(CIMClassSM_PEVT[] unders, CIMClassSM_PEVT[] uppers)
        {
            var uppersList = new List<CIMClassSM_PEVT>();
            foreach(var upper in uppers)
            {
                uppersList.Add(upper);
            }
            foreach(var upper in uppers)
            {
                foreach(var under in unders)
                {
                    if (upper.Attr_SMevt_ID == under.Attr_SMevt_ID)
                    {
                        uppersList.Remove(upper);
                        break;
                    }
                }
            }
            return uppersList.ToArray();
        }

        public static string GetGetSubClassMethodName(CIMClassO_OBJ objDef, CIMClassSM_PEVT pEvtDef)
        {
            string methodName = "";

            var subsupRels = new List<CIMClassR_REL>();

            var found = FindImplementedEvent(objDef, pEvtDef, subsupRels);

            if (found)
            {
                var supsupRel = subsupRels[0];
                methodName = GeneratorNames.GetSubRelClassMethodName(supsupRel);
            }

            return methodName;
        }

        private static bool FindImplementedEvent(CIMClassO_OBJ objDef, CIMClassSM_PEVT pEvtDef, List<CIMClassR_REL> subsupRels)
        {
            bool found = false;
            var oirDefs = objDef.LinkedOneSideR201();
            foreach (var oirDef in oirDefs)
            {
                var rtoDef = oirDef.SubClassR203();
                if (rtoDef is CIMClassR_RTO)
                {
                    var superDef = ((CIMClassR_RTO)rtoDef).SubClassR204();
                    if (superDef is CIMClassR_SUPER)
                    {
                        var subsupDef = ((CIMClassR_SUPER)superDef).LinkedToR212();
                        var relDef = subsupDef.CIMSuperClassR_REL();
                        subsupRels.Add(relDef);
                        var subDefs = subsupDef.LinkedFromR213();
                        foreach (var subDef in subDefs)
                        {
                            var subObjDef = subDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                            found = IsImplementedBySelf(pEvtDef, subObjDef);
                            if (found == false)
                            {
                                found = FindImplementedEvent(subObjDef, pEvtDef, subsupRels);
                            }
                        }
                    }
                }
                if (found)
                {
                    break;
                }
            }
            return found;
        }

        public static bool IsImplementedBySelf(CIMClassSM_PEVT pEvtDef, CIMClassO_OBJ subObjDef)
        {
            bool found = false;
            var ismDef = subObjDef.LinkedFromR518();
            if (ismDef != null)
            {
                var evtDefs = ismDef.CIMSuperClassSM_SM().LinkedFromR502();
                foreach (var evtDef in evtDefs)
                {
                    var sEvtDef = evtDef.SubClassR525();
                    if (sEvtDef is CIMClassSM_SEVT)
                    {
                        var nlEvtDef = ((CIMClassSM_SEVT)sEvtDef).SubClassR526();
                        if (nlEvtDef is CIMClassSM_NLEVT)
                        {
                            var pOfNlEvtDef = ((CIMClassSM_NLEVT)nlEvtDef).LinkedToR527();
                            if (pOfNlEvtDef.Attr_SMevt_ID == pEvtDef.Attr_SMevt_ID)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }

            return found;
        }

        public static string GetCreateEventForDelegated(CIMClassO_OBJ objDef, CIMClassSM_PEVT pEvtDef, string domainEventVarName)
        {
            string stateMachineClassName = GeneratorNames.GetStateMachineClassName(objDef);
            string result = $"{stateMachineClassName}.";
            var evtDefForPEvt = pEvtDef.CIMSuperClassSM_EVT();
            var smDefForPEvt = evtDefForPEvt.LinkedToR502();
            var ismDefForPEvt = smDefForPEvt.SubClassR517();
            if (ismDefForPEvt != null) {
                var objDefForPEvt = ((CIMClassSM_ISM)ismDefForPEvt).LinkedToR518();
                string stateMachineClassNameForPEvt = GeneratorNames.GetStateMachineClassName(objDefForPEvt);
                string eventClassNameForPEvt = GeneratorNames.GetEventClassName(objDefForPEvt, evtDefForPEvt);
                var ismDef = objDef.LinkedFromR518();
                if (ismDef != null)
                {
                    var evtDefs = ismDef.CIMSuperClassSM_SM().LinkedFromR502();
                    foreach (var evtDef in evtDefs)
                    {
                        var sEvtDef = evtDef.SubClassR525();
                        if (sEvtDef is CIMClassSM_SEVT)
                        {
                            var nlEvtDef = ((CIMClassSM_SEVT)sEvtDef).SubClassR526();
                            if (nlEvtDef is CIMClassSM_NLEVT)
                            {
                                var pEvtForNlDef = ((CIMClassSM_NLEVT)nlEvtDef).LinkedToR527();
                                if (pEvtForNlDef.Attr_SMevt_ID == pEvtDef.Attr_SMevt_ID)
                                {
                                    string eventClassName = GeneratorNames.GetEventClassName(objDef, evtDef);
                                    result += $"{eventClassName}.Create(receiver:this";
                                    var evtdiDefs = pEvtDef.CIMSuperClassSM_EVT().LinkedFromR532();
                                    foreach (var evtdiDef in evtdiDefs)
                                    {
                                        result += $", (({stateMachineClassNameForPEvt}.{eventClassNameForPEvt}){domainEventVarName}).{evtdiDef.Attr_Name}";
                                    }
                                    result += $", isSelfEvent:true, sendNow:true)";
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
