using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class DomainFacade
    {
        string version;
        string nameSpace;
        string domainFacadeClassName;

        public DomainFacade(string version, string nameSpace, string domainFacadeClassName)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.domainFacadeClassName = domainFacadeClassName;
        }
    }
}
