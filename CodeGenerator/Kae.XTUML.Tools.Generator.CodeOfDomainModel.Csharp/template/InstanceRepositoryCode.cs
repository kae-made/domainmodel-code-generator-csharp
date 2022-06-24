using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class InstanceRepository
    {
        string version;
        string nameSpace;

        public InstanceRepository(string version, string nameSpace)
        {
            this.version = version;
            this.nameSpace = nameSpace;
        }
    }
}
