using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.Utility.Logging;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.ciclass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.adaptor.adt
{
    public partial class AzureDigitalTwinsAdaptorDef
    {
        string version;
        string nameSpace;
        string projectName;
        string dtdlNamespace;
        string dtdlVersion;
        Logger logger;

        List<CIMClassO_OBJ> objDefs = new List<CIMClassO_OBJ>();
        List<CIMClassR_REL> relDefs = new List<CIMClassR_REL>();

        public AzureDigitalTwinsAdaptorDef(string version, string nameSpace, string projectName, string dtdlNamespace, string dtdlVersion, IEnumerable<CIClassDef> objDefs, IEnumerable<CIClassDef> relDefs, Logger logger)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.projectName = projectName;

            this.dtdlNamespace = dtdlNamespace;
            this.dtdlVersion = dtdlVersion;

            this.logger = logger;

            foreach(var objDef in objDefs)
            {
                this.objDefs.Add((CIMClassO_OBJ)objDef);
            }

            foreach(var relDef in relDefs)
            {
                this.relDefs.Add((CIMClassR_REL)relDef);
            }
        }

        public void prototype()
        {
            string classImplName = GetAdaptorImplClassName(projectName);
            int numOfObjDefs = objDefs.Count();
            int indexOfObjDefs = 0;
            foreach(var objDef in objDefs)
            {
                string twinModelId = GetTwinModelId(objDef);
                string objKeyLett = objDef.Attr_Key_Lett;
                if (++indexOfObjDefs < numOfObjDefs) { }
            }

            var dtdlRelDefs = new List<(string RelName, string RelId, string DTDLRelName, string SourceTwinModelId, string TargetTwinModelId, string FormClassKeyLetter, string PartClassKeyLetter)>();
            foreach(var relDef in relDefs)
            {
                var dtdlRelDefSet = GetRelationshipId(relDef);
                foreach(var dtdlRelDef in dtdlRelDefSet)
                {
                    dtdlRelDefs.Add(dtdlRelDef);
                }
            }
            int numOfRelDefs = dtdlRelDefs.Count();
            int indexOfRelDefs = 0;
            foreach( var dtdlRelDef in dtdlRelDefs)
            {
                string relName = dtdlRelDef.RelName;
                string dtdlRelId = dtdlRelDef.RelId;
                string dtdlRelName = dtdlRelDef.DTDLRelName;
                string sourceTwinModelId = dtdlRelDef.SourceTwinModelId;
                string targetTwinModelId = dtdlRelDef.TargetTwinModelId;
                string partClassKey = dtdlRelDef.PartClassKeyLetter;
                string formClassKey = dtdlRelDef.FormClassKeyLetter;
                if (++indexOfRelDefs < numOfRelDefs) { }
            }
        }

        public static string GetAdaptorImplClassName(string projectName)
        {
            return $"{projectName}AzureDigitalTwinsAdaptor";
        }

        public string GetTwinModelId(CIMClassO_OBJ objDef)
        {
            return $"{dtdlNamespace}:{projectName}:{objDef.Attr_Key_Lett};{dtdlVersion}";
        }

        public IEnumerable<(string RelName, string RelId, string DTDLRelName, string SourceTwinModelId, string TargetTwinModelId, string FormClassKeyLetter, string PartClassKeyLetter)> GetRelationshipId(CIMClassR_REL relDef)
        {
            var results = new List<(string RelName, string RelId, string DTDLRelName, string SourceTwinModelId, string TargetTwinModelId, string FormClassKeyLetter, string PartClassKeyLetter)>();

            var subRelDef = relDef.SubClassR206();
            if (subRelDef is CIMClassR_SIMP)
            {
                var rsimpDef = (CIMClassR_SIMP)subRelDef;
                var rformDef = rsimpDef.LinkedFromR208();
                CIMClassO_OBJ destObjDef = null;
                var orefDefs = rformDef.CIMSuperClassR_RGO().LinkedOtherSideR111();
                foreach (var orefDef in orefDefs)
                {
                    destObjDef = orefDef.LinkedOtherSideR111().LinkedOneSideR110().LinkedToR109().LinkedToR104();
                    break;
                }
                if (destObjDef != null)
                {
                    var ownerObjDef = rformDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    string relName = $"R{relDef.Attr_Numb}";
                    string dtdlRelName = $"{relName}_{destObjDef.Attr_Key_Lett}";
                    string relId = $"{dtdlNamespace}:{projectName}:{dtdlRelName};{dtdlVersion}";
                    string sourceTwinModelId = GetTwinModelId(ownerObjDef);
                    string targetTwinModelId = GetTwinModelId(destObjDef);
                    results.Add((relName, relId, dtdlRelName, sourceTwinModelId, targetTwinModelId, ownerObjDef.Attr_Key_Lett, destObjDef.Attr_Key_Lett));
                }
                else
                {
                    logger.LogError($"R{relDef.Attr_Numb}'s partitipant class doesn't exist.");
                }
            }
            else if (subRelDef is CIMClassR_ASSOC)
            {
                var rassocDef = (CIMClassR_ASSOC)subRelDef;
                var rassrDef = rassocDef.LinkedFromR211();
                var ownerObjDef = rassrDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                var raoneDef = rassocDef.LinkedFromR209();
                var aoneObjDef = raoneDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                string sourceTwinModelId = GetTwinModelId(ownerObjDef);
                string targetTwinModelId = GetTwinModelId(aoneObjDef);
                string relName = $"R{relDef.Attr_Numb}_{GeneratorNames.ToProgramAvailableString(raoneDef.Attr_Txt_Phrs)}";
                string dtdlRelName = $"{relName}_{aoneObjDef.Attr_Key_Lett}";
                string relId = $"{dtdlNamespace}:{projectName}:{dtdlRelName};{dtdlVersion}";
                results.Add((relName, relId, dtdlRelName, sourceTwinModelId, targetTwinModelId, ownerObjDef.Attr_Key_Lett, aoneObjDef.Attr_Key_Lett));
                var raothDef = rassocDef.LinkedFromR210();
                var aothObjDef = raothDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                targetTwinModelId = GetTwinModelId(aothObjDef);
                relName = $"R{relDef.Attr_Numb}_{GeneratorNames.ToProgramAvailableString(raothDef.Attr_Txt_Phrs)}";
                dtdlRelName = $"{relName}_{aothObjDef.Attr_Key_Lett}";
                relId = $"{dtdlNamespace}:{projectName}:{dtdlRelName};{dtdlVersion}";
                results.Add((relName, relId, dtdlRelName, sourceTwinModelId, targetTwinModelId, ownerObjDef.Attr_Key_Lett, aothObjDef.Attr_Key_Lett));
            }
            else if (subRelDef is CIMClassR_SUBSUP)
            {
                var rsubsupDef = (CIMClassR_SUBSUP)subRelDef;
                var rsuperDef = rsubsupDef.LinkedFromR212();
                var targetObjDef = rsuperDef.CIMSuperClassR_RTO().LinkedToR109().LinkedToR104();
                string targetTwinModelId = GetTwinModelId(targetObjDef);
                var rsubDefs = rsubsupDef.LinkedFromR213();
                foreach (var rsubDef in rsubDefs)
                {
                    var ownerObjDef = rsubDef.CIMSuperClassR_RGO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                    string relName = $"R{relDef.Attr_Numb}_{ownerObjDef.Attr_Key_Lett}";
                    string dtdlRelName = $"R{relDef.Attr_Numb}From{ownerObjDef.Attr_Key_Lett}";
                    string relId = $"{dtdlNamespace}:{projectName}:{dtdlRelName};{dtdlVersion}";
                    string sourceTwinModelId = GetTwinModelId(ownerObjDef);
                    results.Add((relName, relId, dtdlRelName, sourceTwinModelId, targetTwinModelId, ownerObjDef.Attr_Key_Lett, targetObjDef.Attr_Key_Lett));
                }
            }
            else if (subRelDef is CIMClassR_COMP)
            {
                logger.LogWarning($"R_COMP is not supprted for R{relDef.Attr_Numb}");
            }
            else
            {
                logger.LogError($"Unknown sub class of R_REL - '{subRelDef.GetType().FullName}'");
            }

            return results;
        }
    }
}
