// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp.template
{
    partial class ProjectFile
    {
        string version;
        string nameSpace;
        string targetFramework;
        IList<Library> usingLibraries;

        public ProjectFile(string version, string nameSpace, string targetFramework, IList<Library> libraries)
        {
            this.version = version;
            this.nameSpace = nameSpace;
            this.targetFramework = targetFramework;
            this.usingLibraries = libraries;
        }

        public void prototype()
        {
            if (usingLibraries.Count > 0)
            {
                foreach (var library in usingLibraries)
                {
                    string pkgRef = $"<PackageReference Include=\"{library.Name}\" Version=\"{library.Version}\" />";
                }
            }
        }

        public class Library
        {
            public string Name { get; set; }
            public string Version { get; set; }
        }
    }
}
