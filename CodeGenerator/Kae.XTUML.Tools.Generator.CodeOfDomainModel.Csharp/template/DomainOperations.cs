﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 17.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Kae.CIM.MetaModel.CIMofCIM;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DomainOperations : DomainOperationsBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 1 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

  // Copyright (c) Knowledge & Experience. All rights reserved.
  // Licensed under the MIT license. See LICENSE file in the project root for full license information.

            
            #line default
            #line hidden
            this.Write("// ------------------------------------------------------------------------------" +
                    "\r\n// <auto-generated>\r\n//     This file is generated by tool.\r\n//     Runtime Ve" +
                    "rsion : ");
            
            #line 14 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(version));
            
            #line default
            #line hidden
            this.Write(@"
//  
//     Updates this file cause incorrect behavior 
//     and will be lost when the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using Kae.DomainModel.Csharp.Framework;
using Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage;

namespace ");
            
            #line 29 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(nameSpace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public partial class ");
            
            #line 31 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(domainFacadeClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 33 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

    string changedStateClassName = "";
    string changedStateVarName = "";
    GeneratorNames.GetChangedStoreVariable(ref changedStateClassName, ref changedStateVarName);

    foreach(var syncClassDef in syncClassDefs)
    {
        var syncDef = (CIMClassS_SYNC)syncClassDef;
        var retDtDef = syncDef.LinkedToR25();
        string retDTName = DomainDataTypeDefs.GetDataTypeName(retDtDef);
        var sparmDefs = syncDef.LinkedFromR24();
        string actionSemantics = GeneratorNames.DescripToCodeComment("            ", syncDef.Attr_Action_Semantics);
        string methodName = syncDef.Attr_Name;
        string args = "";
        foreach (var sparmDef in sparmDefs)
        {
            var pDtDef = sparmDef.LinkedToR26();
            var pDTName = DomainDataTypeDefs.GetDataTypeName(pDtDef);
            if (!string.IsNullOrEmpty(args))
            {
                args += ", ";
            }
            args += $"{pDTName} {sparmDef.Attr_Name}";
        }
        // if (!string.IsNullOrEmpty(args))
        // {
        //    args += ", ";
        // }
        // args += $"{changedStateClassName} {changedStateVarName}=null";


            
            #line default
            #line hidden
            this.Write("        public ");
            
            #line 64 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(retDTName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 64 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(methodName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 64 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(args));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n            // TODO: Let\'s write action code!\r\n            // Actio" +
                    "n Description on Model as a reference.\r\n\r\n");
            
            #line 69 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(actionSemantics));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n            var changedStates = new List<ChangedState>();\r\n            \r\n");
            
            #line 73 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

        bool genSyncState = true;
        var fnbDef = syncDef.LinkedFromR695();
        if (fnbDef != null)
        {
            Console.WriteLine($"  - Generating {syncDef.Attr_Name}...");
            var actDef = fnbDef.CIMSuperClassACT_ACT();
            var actDescripGen = new ActDescripGenerator(actDef, "'This Statement should not appear!'", "    ", "        ", usedExternalEntities, coloringManager, isAzureDigitalTwins, isAzureIoTHub, logger);
            string code = actDescripGen.Generate();

            
            #line default
            #line hidden
            this.Write("            // Generated from action description\r\n");
            
            #line 84 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(code));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 85 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

        }
        // var retDtDef = syncDef.LinkedToR25();
        if (retDtDef.Attr_Name != "void") genSyncState = false;
        if (genSyncState)
        {

            
            #line default
            #line hidden
            this.Write("            instanceRepository.SyncChangedStates(changedStates);\r\n");
            
            #line 93 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

        }
        if (fnbDef == null)
        {

            
            #line default
            #line hidden
            this.Write("            throw new NotImplementedException();\r\n            // Please delete ab" +
                    "ove throw exception statement after implement this method.\r\n");
            
            #line 100 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

        }

            
            #line default
            #line hidden
            this.Write("        }\r\n");
            
            #line 104 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

    }

            
            #line default
            #line hidden
            this.Write("\r\n        public IList<string> CreateExternalEntities()\r\n        {\r\n            v" +
                    "ar configuration = new List<string>();\r\n");
            
            #line 111 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

            int numOfEE = 0;
            foreach(var eeKey in usedExternalEntities.Keys)
            {
                var eeDef = usedExternalEntities[eeKey];
                string eeKeyLetter = eeDef.Attr_Key_Lett;
                var eeImplInfo = GetExternalEntityConstructorName(eeDef, isAzureDigitalTwins);
                numOfEE++;

            
            #line default
            #line hidden
            this.Write("\r\n            instanceRepository.Add(new ");
            
            #line 121 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeImplInfo.New));
            
            #line default
            #line hidden
            this.Write("());\r\n            var refOf");
            
            #line 122 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write(" = instanceRepository.GetExternalEntity(\"");
            
            #line 122 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write("\");\r\n            configuration.AddRange(refOf");
            
            #line 123 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write(".ConfigurationKeys);\r\n");
            
            #line 124 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

            }
            if (isAzureIoTHub)
            {

            
            #line default
            #line hidden
            this.Write(@"
            instanceRepository.Add(new Kae.DomainModel.Csharp.Framework.ExternalEntities.AzureIoTHub.AzureIoTHubImpl());
            var refOfAIH = instanceRepository.GetExternalEntity(""AIH"");
            configuration.AddRange(refOfAIH.ConfigurationKeys);
");
            
            #line 133 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

            }

            
            #line default
            #line hidden
            this.Write("            return configuration;\r\n        }\r\n        public void Initialize(IDic" +
                    "tionary<string, IDictionary<string, object>> configuration)\r\n        {\r\n");
            
            #line 140 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

            foreach(var eeKey in usedExternalEntities.Keys)
            {
                var eeDef = usedExternalEntities[eeKey];
                string eeKeyLetter = eeDef.Attr_Key_Lett;

            
            #line default
            #line hidden
            this.Write("            var refOf");
            
            #line 146 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write(" = instanceRepository.GetExternalEntity(\"");
            
            #line 146 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write("\");\r\n            refOf");
            
            #line 147 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write(".Initialize(configuration[\"");
            
            #line 147 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eeKey));
            
            #line default
            #line hidden
            this.Write("\"]);\r\n");
            
            #line 148 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

            }
            if (isAzureIoTHub)
            {

            
            #line default
            #line hidden
            this.Write("            var refOfAIH = instanceRepository.GetExternalEntity(\"AIH\");\r\n        " +
                    "    refOfAIH.Initialize(configuration[\"AIH\"]);\r\n");
            
            #line 155 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\DomainOperations.tt"

            }

            
            #line default
            #line hidden
            this.Write("        }\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class DomainOperationsBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
