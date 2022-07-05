using Kae.CIM.MetaModel.CIMofCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template.logging
{
    partial class Logging
    {
        string logVarName;
        string indent;
        CIMClassO_OBJ subjectObjDef;
        string subjectVarName;
        Mode mode;
        string verb;
        public CIMClassO_OBJ oneObjDef { get; set; }
        public string oneVarName { get; set; }
        public CIMClassO_OBJ otherObjDef { get; set; }
        public string otherVarName { get; set; }

        public Logging(string logVarName, string indent, CIMClassO_OBJ objDef, string subjectVarName, Mode mode, string verb)
        {
            this.logVarName = logVarName;
            this.indent = indent;
            this.subjectObjDef = objDef;
            this.subjectVarName = subjectVarName;
            this.mode = mode;
            this.verb = verb;
        }

        public void prototype()
        {
            string logVarCheck = $"if ({logVarName} != null) ";
            string logThisIdProps = DomainClassBase.GetIdentityPropertiesArgsInFormattedString(subjectObjDef, subjectVarName);
            if (mode== Mode.InstanceLifeCycle)
            {

            }
            else if (mode== Mode.LinkLifeCycle)
            {
                string oneVarIdProps = "";
                string otherVarIdProps = "";
                if (!string.IsNullOrEmpty(oneVarName))
                {
                    oneVarIdProps = DomainClassBase.GetIdentityPropertiesArgsInFormattedString(oneObjDef, oneVarName);
                }
                if (!string.IsNullOrEmpty(otherVarName))
                {
                    otherVarIdProps = DomainClassBase.GetIdentityPropertiesArgsInFormattedString(otherObjDef, otherVarName);
                }
                string objectLog = "";
                if (string.IsNullOrEmpty(otherVarIdProps))
                {
                    objectLog = $"{oneObjDef.Attr_Key_Lett}({oneVarIdProps})";
                }
                else
                {
                    objectLog = $"One({oneObjDef.Attr_Key_Lett}({oneVarIdProps})),Other({otherObjDef.Attr_Key_Lett}({otherVarIdProps}))";
                }
            }
            else if (mode == Mode.StatTransition)
            {

            }
        }

        public enum Mode
        {
            InstanceLifeCycle,
            LinkLifeCycle,
            StatTransition
        }
    }
}
