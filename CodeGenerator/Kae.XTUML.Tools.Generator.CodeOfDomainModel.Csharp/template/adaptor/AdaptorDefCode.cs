using Kae.CIM;
using Kae.CIM.MetaModel.CIMofCIM;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.adaptor.adt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.adaptor
{
    partial class AdaptorDef
    {
        string version;
        string nameSpace;
        string indent ="";
        string baseIndent;
        string projectName;
        string externalStorageImplClassName;
        string externalStorageConnectionStringKey;
        string externalStorageCredentialKey;
        bool useAzureDigitalTwins = false;

        static readonly string folderName = "Adaptor";

        List<CIMClassS_SYNC> syncDefs = new List<CIMClassS_SYNC>();
        List<CIMClassO_OBJ> objDefs = new List<CIMClassO_OBJ>();

        Dictionary<string, OperationSpec> domainOpSpecs = new Dictionary<string, OperationSpec>();
        Dictionary<string, ClassSpec> classSpecs = new Dictionary<string, ClassSpec>();
        Dictionary<string, DataTypeSpec> typeSpecs = new Dictionary<string, DataTypeSpec>();

        static readonly Dictionary<string, ParamSpec.DataType> typeKinds = new Dictionary<string, ParamSpec.DataType>()
        {
            { "int", ParamSpec.DataType.Integer },
            { "string", ParamSpec.DataType.String },
            { "bool", ParamSpec.DataType.Boolean },
            { "DateTime", ParamSpec.DataType.DateTime },
            { "double", ParamSpec.DataType.Real },
            { "void", ParamSpec.DataType.Void }
        };

        public AdaptorDef(string version, string nameSpace, string baseIndent, string projectName, IEnumerable<CIClassDef> ciSyncDefs, IEnumerable<CIClassDef> ciObjDefs, string externalStorageImplClassName, string externalStorageConnectionStringKey, string externalStorageCredentialKey)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.baseIndent = baseIndent;
            this.projectName = projectName;
            this.externalStorageImplClassName = externalStorageImplClassName;
            this.externalStorageConnectionStringKey = externalStorageConnectionStringKey;
            this.externalStorageCredentialKey = externalStorageCredentialKey;
            if (!string.IsNullOrEmpty(externalStorageImplClassName))
            {
                useAzureDigitalTwins = true;
            }
            
            foreach (var ciSyncDef in ciSyncDefs)
            {
                syncDefs.Add((CIMClassS_SYNC)ciSyncDef);
            }
            foreach (var ciObjDef in ciObjDefs)
            {
                objDefs.Add((CIMClassO_OBJ)ciObjDef);
            }
        }

        public static string GetFolderName() { return folderName; }

        public void Prototype()
        {
            Initialize();

            string domainFacadeClassName = GeneratorNames.GetDomainFacadeClassName(nameSpace);
            if ((!string.IsNullOrEmpty(externalStorageImplClassName)) && (!string.IsNullOrEmpty(externalStorageConnectionStringKey)))
            {
                string adtAdaptorImplClassName = AzureDigitalTwinsAdaptorDef.GetAdaptorImplClassName(projectName);
            }

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            string currentIndent = $"{indent}{baseIndent}";

            // domain operation dictionary generation
            bool first = true;
            foreach (var syncSpecKey in domainOpSpecs.Keys)
            {
                var syncSpec = domainOpSpecs[syncSpecKey];
                foreach (var paramSpecKey in syncSpec.Parameters.Keys)
                {
                    if (first == false)
                    {
                        writer.WriteLine("");
                    }
                    writer.Write("");
                }
                int c = domainOpSpecs.Count;
            }

            // class directory generation

            // Invoke Domain Operation
            foreach (var csKey in classSpecs.Keys)
            {
                var classSpec = classSpecs[csKey];
                if (classSpec.Operations.Count > 0)
                {
                    string domainClassName = GeneratorNames.GetDomainClassName(classSpec.ObjDef);
                    string classKeyLett = classSpec.KeyLetter;


                    string instSelectCode = GetSelectedByIdentitiesCode(domainClassName, "selected", "invSpec", classSpec.Properties);

                    foreach (var copKey in classSpec.Operations.Keys)
                    {
                        var cOpSpec = classSpec.Operations[copKey];

                    }
                }
            }

            // Invoke Class Operation

            // Invoke Class Properties Update
            foreach(var ck in classSpecs.Keys)
            {
                var classSpec = classSpecs[ck];
                foreach(var pk in classSpec.Properties.Keys)
                {
                    var propSpec = classSpec.Properties[pk];
                    if (propSpec.Writable)
                    {
                        var attrDef = propSpec.AttrDef;
                        string attrTypeNamef = DomainDataTypeDefs.GetDataTypeName(DomainDataTypeDefs.GetBaseDT(attrDef));
                        string attrPropName = GeneratorNames.GetAttrPropertyName(attrDef);
                    }
                }
            }

            // Invoke Class Properties Get

            // Invoke Class Links Get
            foreach(var ck in classSpecs.Keys)
            {
                var classSpec = classSpecs[ck];
                foreach(var lk in classSpec.Links.Keys)
                {
                    var linkSpec = classSpec.Links[lk];
                    var dstClassKeyLetter = linkSpec.DstKeyLett;
                    string dstDomainClassName = GeneratorNames.GetDomainClassName(linkSpec.DstObjDef);
                    string relName = $"{linkSpec.RelID}{linkSpec.Side}{GeneratorNames.ToProgramAvailableString(linkSpec.Phrase)}{linkSpec.DstKeyLett}";
                    string relInstVarName = $"linkedInstanceOf{relName}";
                    string relInstSetVarName = $"linkedInstancesOf{relName}";
                    string linkedMethodName = GeneratorNames.GetRelationshipMethodName(linkSpec.RelDef, linkSpec.Side, linkSpec.Phrase, GeneratorNames.RelLinkMethodType.Linked);
                    var propSpecs = classSpecs[dstClassKeyLetter].Properties;
                    if (linkSpec.Set)
                    {
                    }
                    else
                    {
                    }
                    foreach (var pk in propSpecs.Keys)
                    {
                        var propSpec = propSpecs[pk];
                        string attrPropName = GeneratorNames.GetAttrPropertyName(propSpec.AttrDef);
                    }
                }
            }

            // Invoke Send Envetn
            string invVarName = "invSpec";
            foreach (var csKey in classSpecs.Keys)
            {
                var classSpec = classSpecs[csKey];
                if (classSpec.Events.Count > 0)
                {
                    string domainClassName = GeneratorNames.GetDomainClassName(classSpec.ObjDef);
                    string classKeyLett = classSpec.KeyLetter;
                    string instVarName = $"instanceOf{classKeyLett}";
                    string instSelectedCode = GetSelectedByIdentitiesCode(domainClassName, "selected", invVarName, classSpec.Properties);
                    foreach (var evtKey in classSpec.Events.Keys)
                    {
                        var evtSpec = classSpec.Events[evtKey];
                        bool isCreationEvent = DomainClassStateMachine.IsCreationEvent(evtSpec.EvtDef);
                        string stateMachineClassName = GeneratorNames.GetStateMachineClassName(classSpec.ObjDef);
                        string evtClassName = GeneratorNames.GetEventClassName(classSpec.ObjDef, evtSpec.EvtDef);
                        string evtCreateMethodName = $"{stateMachineClassName}.{evtClassName}.Create";
                        string paramCode = GetInvokeParamsCode(evtSpec.Parameters, invVarName);
                        if (!string.IsNullOrEmpty(paramCode))
                        {
                            paramCode = $", {paramCode}";
                        }
                        if (evtSpec.CreationEvent)
                        {
                            paramCode += ", domainModel.InstanceRepository, logger:logger";
                        }
                        string evtInvCode = $"{evtCreateMethodName}({instVarName}{paramCode})";
                    }
                }
            }
            // Invoke Domian Operation Spec Get

            // Invoke Class Spec Get
        }


        string GetSelectedByIdentitiesCodeForExternalStorage(string invVarName, IDictionary<string, PropSpec> propSpecs)
        {
            string code = "";
            Dictionary<string, PropSpec> identities = GetIdentityProperties(propSpecs);
            foreach (var idKey in identities.Keys)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    code += " AND ";
                }
                var propSpec = propSpecs[idKey];
                var attrDef = propSpec.AttrDef;
                string attrDataTypeName = DomainDataTypeDefs.GetDataTypeName(DomainDataTypeDefs.GetBaseDT(attrDef));
                string term = "";
                if (attrDataTypeName.ToLower() == "string")
                {
                    term = "'";
                }
                code += $"{attrDef.Attr_Name} = {term}" + "{" + $"{invVarName}[\"{idKey}\"]" + "}" + $"{term}";
            }
            return code;
        }

        string GetSelectedByIdentitiesCode(string domainClassName, string selectedVarName, string invVarName, IDictionary<string, PropSpec> propSpecs)
        {
            string code = "";
            Dictionary<string, PropSpec> identities = GetIdentityProperties(propSpecs);
            foreach (var idKey in identities.Keys)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    code += " && ";
                }
                var propSpec = propSpecs[idKey];
                var attrDef = propSpec.AttrDef;
                string attrDataTypeName = DomainDataTypeDefs.GetDataTypeName(DomainDataTypeDefs.GetBaseDT(attrDef));
                string attrPropName = GeneratorNames.GetAttrPropertyName(attrDef);
                string selectedPart = $"(({domainClassName}){selectedVarName}).{attrPropName}";
                if (attrDataTypeName.ToLower() != "string")
                {
                    selectedPart = $"({selectedPart}).ToString()";
                }
                code += $"{selectedPart} == {invVarName}[\"{idKey}\"]";
            }
            code = $"{selectedVarName} => ({code})";
            return code;
        }

        private Dictionary<string, PropSpec> GetIdentityProperties(IDictionary<string, PropSpec> propSpecs)
        {
            var identities = new Dictionary<string, PropSpec>();
            foreach (var pk in propSpecs.Keys)
            {
                if (propSpecs[pk].Identity == 1)
                {
                    identities.Add(pk, propSpecs[pk]);
                }
            }

            return identities;
        }

        string GetParamSpecCreateCode(ParamSpec paramSpec)
        {
            string isArray = $"{paramSpec.IsArray}".ToLower();
            string code = "new ParamSpec() {" + $"Name = \"{paramSpec.Name}\", TypeKind = ParamSpec.DataType.{paramSpec.TypeKind.ToString()}, IsArray = {isArray}" + "}";

            return code;
        }

        public void Initialize()
        {
            foreach (var syncDef in syncDefs)
            {
                var domOpSpec = new OperationSpec() { Name = syncDef.Attr_Name, Parameters = new Dictionary<string, ParamSpec>() };
                var sparamDefs = syncDef.LinkedFromR24();
                domOpSpec.ReturnTypeSpec = GetTypeSpec(syncDef.LinkedToR25());
                domOpSpec.ReturnType = GetDataType(domOpSpec.ReturnTypeSpec);
                foreach (var sparamDef in sparamDefs)
                {
                    var paramSpec = new ParamSpec() { Name = sparamDef.Attr_Name };
                    paramSpec.TypeSpec = GetTypeSpec(sparamDef.LinkedToR26());
                    paramSpec.TypeKind = GetDataType(paramSpec.TypeSpec);
                    if (paramSpec.TypeKind != ParamSpec.DataType.Unsupported)
                    {
                        domOpSpec.Parameters.Add(paramSpec.Name, paramSpec);
                    }
                }
                domainOpSpecs.Add(domOpSpec.Name, domOpSpec);
            }

            foreach (var objDef in objDefs)
            {
                var classSpec = new ClassSpec()
                {
                    ObjDef = objDef,
                    Name = objDef.Attr_Name,
                    KeyLetter = objDef.Attr_Key_Lett,
                    Operations = new Dictionary<string, OperationSpec>(),
                    Properties = new Dictionary<string, PropSpec>(),
                    Events = new Dictionary<string, EventSpec>(),
                    Links = new Dictionary<string, LinkSpec>()
                };
                var attrDefs = objDef.LinkedFromR102();
                foreach (var attrDef in attrDefs)
                {
                    var propSpec = new PropSpec() { Name = attrDef.Attr_Name, AttrDef = attrDef, Writable = true, Identity = 0 };
                    var attrDtDef = DomainDataTypeDefs.GetBaseDT(attrDef);
                    propSpec.TypeSpec = GetTypeSpec(attrDtDef);
                    propSpec.DataType = GetDataType(propSpec.TypeSpec);
                    if (propSpec.DataType != ParamSpec.DataType.Unsupported)
                    {
                        propSpec.AccessorName = GeneratorNames.GetAttrPropertyName(attrDef);
                        var subAttrDef = attrDef.SubClassR106();
                        if (subAttrDef is CIMClassO_BATTR)
                        {
                            var bAttrDef = (CIMClassO_BATTR)subAttrDef;
                            var subBAttrDef = bAttrDef.SubClassR107();
                            if (subBAttrDef is CIMClassO_DBATTR)
                            {
                                propSpec.Writable = false;
                                propSpec.Mathematical = true;
                            }
                            if (propSpec.TypeSpec.DtDef.Attr_Name == "state<State_Model>")
                            {
                                propSpec.StateMachineState = true;
                                propSpec.Writable = false;
                            }
                        }
                        else if (subAttrDef is CIMClassO_RATTR)
                        {
                            propSpec.Reference = true;
                            propSpec.Writable = false;
                        }
                        classSpec.Properties.Add(propSpec.Name, propSpec);
                    }
                }
                var oidDefs = objDef.LinkedFromR104();
                foreach (var oidDef in oidDefs)
                {
                    var oidaDefs = oidDef.LinkedOtherSideR105();
                    foreach (var oidaDef in oidaDefs)
                    {
                        var oidAttrDef = oidaDef.LinkedOtherSideR105();
                        var propSpec = classSpec.Properties[oidAttrDef.Attr_Name];
                        propSpec.Identity = oidDef.Attr_Oid_ID + 1;
                        propSpec.Writable = false;
                    }
                }

                foreach (var pk in classSpec.Properties.Keys)
                {
                    var propSpec = classSpec.Properties[pk];
                    if (propSpec.Writable)
                    {
                        classSpec.NumOfWritableProperties++;
                    }
                }

                var tfrDefs = objDef.LinkedFromR115();
                foreach (var tfrDef in tfrDefs)
                {
                    var opSpec = new OperationSpec() { Name = tfrDef.Attr_Name, Parameters = new Dictionary<string, ParamSpec>() };
                    var retDtDef = tfrDef.LinkedToR116();
                    opSpec.ReturnTypeSpec = GetTypeSpec(retDtDef);
                    opSpec.ReturnType = GetDataType(opSpec.ReturnTypeSpec);
                    var tparmDefs = tfrDef.LinkedFromR117();
                    foreach (var tparmDef in tparmDefs)
                    {
                        var dtParm = tparmDef.LinkedToR118();
                        var parmSpec = new ParamSpec() { Name = tparmDef.Attr_Name, TypeSpec = GetTypeSpec(dtParm) };
                        parmSpec.TypeKind = GetDataType(parmSpec.TypeSpec);
                        if (parmSpec.TypeKind != ParamSpec.DataType.Unsupported)
                        {
                            opSpec.Parameters.Add(parmSpec.Name, parmSpec);
                        }
                    }
                    classSpec.Operations.Add(opSpec.Name, opSpec);
                }

                foreach (var attrDef in attrDefs)
                {
                    var subAttrDef = attrDef.SubClassR106();
                    if (subAttrDef is CIMClassO_RATTR)
                    {
                        var rAttrDef = (CIMClassO_RATTR)subAttrDef;
                        var orefDefs = rAttrDef.LinkedFromR108();
                        foreach (var orefDef in orefDefs)
                        {
                            var rgoDef = orefDef.LinkedOneSideR111();
                            var rtidaDef = orefDef.LinkedOtherSideR111();
                            var rtoDef = rtidaDef.LinkedOneSideR110();
                            CIMClassR_REL relDef = rtoDef.CIMSuperClassR_OIR().LinkedOneSideR201();

                            var oidaDef = rtidaDef.LinkedOtherSideR110();
                            var dstObjDef = oidaDef.LinkedOneSideR105().LinkedToR104();
                            var subRtoDef = rtoDef.SubClassR204();
                            string relId = $"R{relDef.Attr_Numb}";
                            string relPhrs = "";
                            string side = "";
                            bool isSet = false;
                            bool isCond = false;
                            string methodName = "";
                            if (subRtoDef is CIMClassR_SUPER)
                            {
                                methodName = $"GetSuperClassR{relDef.Attr_Numb}";
                            }
                            else if (subRtoDef is CIMClassR_PART)
                            {
                                var partDef = (CIMClassR_PART)subRtoDef;
                                relPhrs = partDef.Attr_Txt_Phrs;
                                isSet = (partDef.Attr_Mult == 1);
                                isCond = (partDef.Attr_Cond == 1);
                                methodName = GeneratorNames.GetRelationshipMethodName(relDef, "", relPhrs, GeneratorNames.RelLinkMethodType.Linked);
                            }
                            else if (subRtoDef is CIMClassR_AONE)
                            {
                                var aoneDef = (CIMClassR_AONE)subRtoDef;
                                relPhrs = aoneDef.Attr_Txt_Phrs;
                                side = "One";
                                methodName = GeneratorNames.GetRelationshipMethodName(relDef, side, relPhrs, GeneratorNames.RelLinkMethodType.Linked);
                            }
                            else if (subRtoDef is CIMClassR_AOTH)
                            {
                                var aothDef = (CIMClassR_AOTH)subRtoDef;
                                relPhrs = aothDef.Attr_Txt_Phrs;
                                side = "Other";
                                methodName = GeneratorNames.GetRelationshipMethodName(relDef, side, relPhrs, GeneratorNames.RelLinkMethodType.Linked);
                            }
                            string relName = $"{relId}";
                            if (!string.IsNullOrEmpty(relPhrs))
                            {
                                relName = $"{relName}.'{relPhrs}'";
                            }
                            relName = $"{dstObjDef.Attr_Key_Lett}[{relName}]";
                            if (!classSpec.Links.ContainsKey(relName))
                            {
                                var linkSpec = new LinkSpec() { Name = relName, RelID = relId, Phrase = relPhrs, Set = isSet, Condition = isCond, DstObjDef=dstObjDef, DstKeyLett = dstObjDef.Attr_Key_Lett, RelDef = relDef, Side = side, MethodName = methodName };
                                classSpec.Links.Add(relName, linkSpec);
                            }
                        }
                    }
                    foreach (var oidDef in oidDefs)
                    {
                        var oidaDefs = oidDef.LinkedOtherSideR105();
                        foreach (var oidaDef in oidaDefs)
                        {
                            var rtidaDefs = oidaDef.LinkedOneSideR110();
                            foreach (var rtidaDef in rtidaDefs)
                            {
                                var orefDefs = rtidaDef.LinkedOneSideR111();
                                foreach (var orefDef in orefDefs)
                                {
                                    var rgoDef = orefDef.LinkedOneSideR111();
                                    var relDef = rgoDef.CIMSuperClassR_OIR().LinkedOneSideR201();
                                    var dstObjDef = rgoDef.CIMSuperClassR_OIR().LinkedOtherSideR201();
                                    var subRgoDef = rgoDef.SubClassR205();
                                    string relId = $"R{relDef.Attr_Numb}";
                                    string relPhrs = "";
                                    string side = "";
                                    bool isSet = false;
                                    bool isCond = false;
                                    string methodName = "";
                                    if (subRgoDef is CIMClassR_SUB)
                                    {
                                        methodName = $"LinkedR{relDef.Attr_Numb}{dstObjDef.Attr_Key_Lett}";
                                    }
                                    else if (subRgoDef is CIMClassR_FORM)
                                    {
                                        var formDef = (CIMClassR_FORM)subRgoDef;
                                        relPhrs = formDef.Attr_Txt_Phrs;
                                        isSet = (formDef.Attr_Mult == 1);
                                        isCond = (formDef.Attr_Cond == 1);
                                        methodName = GeneratorNames.GetRelationshipMethodName(relDef, "", relPhrs, GeneratorNames.RelLinkMethodType.Linked);
                                    }
                                    else if (subRgoDef is CIMClassR_ASSR)
                                    {
                                        var assrDef = (CIMClassR_ASSR)subRgoDef;
                                        var rtoDef = rtidaDef.LinkedOneSideR110();
                                        var subRtoDef = rtoDef.SubClassR204();
                                        if (subRtoDef is CIMClassR_AONE)
                                        {
                                            var aoneDef = (CIMClassR_AONE)subRtoDef;
                                            relPhrs = aoneDef.Attr_Txt_Phrs;
                                            var aothDef = assrDef.LinkedToR211().LinkedFromR210();
                                            isSet = (aothDef.Attr_Mult == 1);
                                            isCond = (aothDef.Attr_Cond == 1);
                                            side = "One";
                                            methodName = GeneratorNames.GetRelationshipMethodName(relDef,"Other",aothDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                                        }
                                        else if (subRtoDef is CIMClassR_AOTH)
                                        {
                                            var aothDef = (CIMClassR_AOTH)subRtoDef;
                                            relPhrs = aothDef.Attr_Txt_Phrs;
                                            var aoneDef = assrDef.LinkedToR211().LinkedFromR209();
                                            isSet = (aoneDef.Attr_Mult == 1);
                                            isCond = (aoneDef.Attr_Cond == 1);
                                            side = "Other";
                                            methodName = GeneratorNames.GetRelationshipMethodName(relDef, "One", aoneDef.Attr_Txt_Phrs, GeneratorNames.RelLinkMethodType.Linked);
                                        }
                                        var assocDef = assrDef.LinkedToR211();
                                        var objDefOfAoneOfAssocDef = assocDef.LinkedFromR209().CIMSuperClassR_RTO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                                        var objDefOfAothOfAssocDef = assocDef.LinkedFromR210().CIMSuperClassR_RTO().CIMSuperClassR_OIR().LinkedOtherSideR201();
                                        if (objDefOfAoneOfAssocDef.Attr_Key_Lett == objDefOfAothOfAssocDef.Attr_Key_Lett)
                                        {
                                            // methodName += "!";
                                        }
                                    }
                                    string relName = $"{relId}";
                                    if (!string.IsNullOrEmpty(relPhrs))
                                    {
                                        relName = $"{relName}.'{relPhrs}'";
                                    }
                                    relName = $"{dstObjDef.Attr_Key_Lett}[{relName}]";
                                    if (!classSpec.Links.ContainsKey(relName))
                                    {
                                        var linkSpec = new LinkSpec() { Name = relName, RelID = relId, Phrase = relPhrs, Set = isSet, Condition = isCond, DstObjDef=dstObjDef, DstKeyLett = dstObjDef.Attr_Key_Lett, RelDef = relDef, Side = side, MethodName = methodName };
                                        classSpec.Links.Add(relName, linkSpec);
                                    }
                                }
                            }
                        }
                    }

                }
                var ismDef = objDef.LinkedFromR518();
                if (ismDef != null)
                {
                    var smEvtDefs = ismDef.CIMSuperClassSM_SM().LinkedFromR502();
                    foreach (var smEvtDef in smEvtDefs)
                    {
                        var evtSpec = new EventSpec() { EvtDef = smEvtDef, Name = GeneratorNames.GetEventClassName(objDef, smEvtDef), ReturnType = ParamSpec.DataType.Void, ReturnTypeSpec = null, Parameters = new Dictionary<string, ParamSpec>() };
                        var smEvtDiDefs = smEvtDef.LinkedFromR532();
                        foreach (var smEvtDiDef in smEvtDiDefs)
                        {
                            var dtDef = smEvtDiDef.LinkedToR524();
                            var paramSpec = new ParamSpec() { Name = smEvtDiDef.Attr_Name, TypeSpec = GetTypeSpec(dtDef) };
                            paramSpec.TypeKind = GetDataType(paramSpec.TypeSpec);
                            evtSpec.Parameters.Add(paramSpec.Name, paramSpec);
                        }
                        var subSmEvtDef = smEvtDef.SubClassR525();
                        if (subSmEvtDef is CIMClassSM_SEVT)
                        {
                            var subSEvtDef = ((CIMClassSM_SEVT)subSmEvtDef).SubClassR526();
                            if (subSEvtDef is CIMClassSM_LEVT)
                            {
                                var crtxnDef = ((CIMClassSM_LEVT)subSEvtDef).LinkedFromR509();
                                if (crtxnDef != null)
                                {
                                    evtSpec.CreationEvent = true;
                                }
                            }
                        }
                        classSpec.Events.Add(evtSpec.Name, evtSpec);
                    }
                }

                classSpecs.Add(classSpec.KeyLetter, classSpec);
            }

        }

        private static ParamSpec.DataType GetDataType(DataTypeSpec spec)
        {
            ParamSpec.DataType dataType;
            if (spec.EnumType)
            {
                dataType = ParamSpec.DataType.Enum;
            }
            else if (spec.ComplexType)
            {
                dataType = ParamSpec.DataType.Complex;
            }
            else
            {
                if (typeKinds.ContainsKey(spec.NameOnCode))
                {
                    dataType = typeKinds[spec.NameOnCode];
                }
                else
                {
                    dataType = ParamSpec.DataType.Unsupported;
                }
            }
            return dataType;
        }

        DataTypeSpec GetTypeSpec(CIMClassS_DT dtDef)
        {
            DataTypeSpec typeSpec = null;
            if (typeSpecs.ContainsKey(dtDef.Attr_Name))
            {
                typeSpec = typeSpecs[dtDef.Attr_Name];
            }
            else
            {
                typeSpec = new DataTypeSpec() { Name = dtDef.Attr_Name, DtDef = dtDef };
                typeSpecs.Add(dtDef.Attr_Name, typeSpec);
                var subDtDef = dtDef.SubClassR17();
                if (subDtDef is CIMClassS_EDT)
                {
                    typeSpec.EnumType = true;
                }
                else if (subDtDef is CIMClassS_SDT)
                {
                    typeSpec.ComplexType = true;
                }

                typeSpec.NameOnCode = DomainDataTypeDefs.GetDataTypeName(dtDef);
            }
            return typeSpec;
        }

        string GetInvokeParamsCode(IDictionary<string,ParamSpec> invParams, string varName)
        {
            string code = "";
            foreach(var pk in invParams.Keys)
            {
                var invParam = invParams[pk];
                if (!string.IsNullOrEmpty(code))
                {
                    code += ", ";
                }
                code += $"{invParam.Name}:({invParam.TypeSpec.NameOnCode}){varName}.Parameters[\"{invParam.Name}\"]";
            }

            return code;
        }

    }


    class LinkSpec
    {
        public CIMClassR_REL RelDef { get; set; }
        public string Name { get; set; }
        public string RelID { get; set; }
        public string Phrase { get; set; }
        public string Side { get; set; }
        public bool Set { get; set; }
        public bool Condition { get; set; }
        public string DstKeyLett { get; set; }
        public CIMClassO_OBJ DstObjDef { get; set; }
        public string MethodName { get; set; }
    }

    class DataTypeSpec
    {
        public CIMClassS_DT DtDef { get; set; }
        public string Name { get; set; }
        public string NameOnCode { get; set; }
        public bool EnumType { get; set; }
        public bool ComplexType { get; set; }
    }

    class ParamSpec
    {
        public enum DataType
        {
            String,
            Integer,
            Real,
            Boolean,
            DateTime,
            Void,
            Enum,
            Complex,
            Unsupported
        }
        public string Name { get; set; }
        public DataType TypeKind { get; set; }
        public bool IsArray { get; set; }
        public DataTypeSpec TypeSpec { get; set; }
    }

    class OperationSpec
    {
        public string Name { get; set; }
        public IDictionary<string, ParamSpec> Parameters { get; set; }
        public ParamSpec.DataType ReturnType { get; set; }
        public DataTypeSpec ReturnTypeSpec { get; set; }
    }

    class PropSpec
    {
        public string Name { get; set; }
        public string AccessorName { get; set; }
        public int Identity { get; set; }
        public bool Writable { get; set; }
        public ParamSpec.DataType DataType { get; set; }
        public bool Reference { get; set; }
        public bool Mathematical { get; set; }
        public bool StateMachineState { get; set; }
        public CIMClassO_ATTR AttrDef { get; set; }
        public DataTypeSpec TypeSpec { get; set; }
    }

    class EventSpec: OperationSpec
    {
        public bool CreationEvent { get; set; }
        public CIMClassSM_EVT EvtDef { get; set; }
    }

    class ClassSpec
    {
        public CIMClassO_OBJ ObjDef { get; set; }
        public string Name { get; set; }
        public string KeyLetter { get; set; }
        public IDictionary<string, PropSpec> Properties { get; set; }
        public IDictionary<string, OperationSpec> Operations { get; set; }
        public IDictionary<string, LinkSpec> Links { get; set; }
        public IDictionary<string, EventSpec> Events { get; set; }

        public int NumOfWritableProperties { get; set; } = 0;
    }
}
