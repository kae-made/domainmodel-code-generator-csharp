// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
