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
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class InstanceRepository : InstanceRepositoryBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 1 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"

  // Copyright (c) Knowledge & Experience. All rights reserved.
  // Licensed under the MIT license. See LICENSE file in the project root for full license information.

            
            #line default
            #line hidden
            this.Write("// ------------------------------------------------------------------------------" +
                    "\r\n// <auto-generated>\r\n//     This file is generated by tool.\r\n//     Runtime Ve" +
                    "rsion : ");
            
            #line 13 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"
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
using Kae.Utility.Logging;
using Kae.DomainModel.Csharp.Framework;

namespace ");
            
            #line 25 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(nameSpace));
            
            #line default
            #line hidden
            this.Write(@"
{
    public class InstanceRepositoryInMemory : InstanceRepository
    {
        private Logger logger;

        public InstanceRepositoryInMemory(Logger logger)
        {
            this.logger = logger;
        }

        public override void LoadState(string domainName, IDictionary<string, IList<IDictionary<string, object>>> instances)
        {
            // current edition doesn't support multi domain feature.
            foreach (var className in instances.Keys)
            {
                foreach (var states in instances[className])
                {
                    DomainClassDef newInstance = null;
                    switch (className)
                    {
");
            
            #line 46 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"

    foreach(var objDef in objDefs)
    {
        string domainClassBaseName = GeneratorNames.GetDomainClassImplName(objDef);
        string domainClassName = objDef.Attr_Key_Lett;

            
            #line default
            #line hidden
            this.Write("                        case \"");
            
            #line 52 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(domainClassName));
            
            #line default
            #line hidden
            this.Write("\":\r\n                            newInstance = ");
            
            #line 53 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(domainClassBaseName));
            
            #line default
            #line hidden
            this.Write(".CreateInstance(this, logger);\r\n                            break;\r\n");
            
            #line 55 "C:\Users\kae-m\source\repos\xtMULMetaModelProjects\Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp\template\InstanceRepository.tt"

    }

            
            #line default
            #line hidden
            this.Write("                        default:\r\n                            if (logger != null)" +
                    " logger.LogError($\"{className} is not right domain class.\");\r\n                  " +
                    "          break;\r\n                    }\r\n                    if (newInstance != " +
                    "null)\r\n                    {\r\n                        newInstance.Restore(states" +
                    ");\r\n                    }\r\n                }\r\n            }\r\n        }\r\n\r\n      " +
                    "  public override void UpdateCInstance(CInstanceChagedState instanceState)\r\n    " +
                    "    {\r\n            NotifyClassStateChanged(instanceState.OP, instanceState.Targe" +
                    "t, instanceState.ChangedProperties);\r\n        }\r\n\r\n        public override void " +
                    "UpdateState(DomainClassDef instance, IDictionary<string, object> chnaged)\r\n     " +
                    "   {\r\n            NotifyClassStateChanged(ChangedState.Operation.Update,instance" +
                    ", chnaged);\r\n        }\r\n\r\n        public override void UpdateCLink(CLinkChangedS" +
                    "tate linkState)\r\n        {\r\n            NotifyRelationshipStateChanged(linkState" +
                    ".OP, linkState.Target.RelationshipID, linkState.Target.Phrase, linkState.Target." +
                    "Source, linkState.Target.Destination);\r\n        }\r\n\r\n        public override IEn" +
                    "umerable<T> SelectInstances<T>(string className, IDictionary<string, object> con" +
                    "ditionPropertyValues, Func<T, IDictionary<string, object>, bool> compare)\r\n     " +
                    "   {\r\n            var resultSet = new List<T>();\r\n            var candidates = d" +
                    "omainInstances[className].Where(i => { return compare((T)i, conditionPropertyVal" +
                    "ues); });\r\n            foreach (var ci in candidates)\r\n            {\r\n          " +
                    "      resultSet.Add((T)ci);\r\n            }\r\n            return resultSet;\r\n     " +
                    "   }\r\n\r\n    }\r\n\r\n}\r\n");
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
    public class InstanceRepositoryBase
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
