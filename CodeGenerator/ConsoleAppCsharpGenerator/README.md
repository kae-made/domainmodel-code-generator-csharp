# How to Use  
## Setup
1. Clone this site and https://github.com/kae-made/artifacts-laundromat-in-hotel-tutorial.  
1. Install BridgePoint according to https://xtuml.org/download/.  
1. Open ./CodeGenerator/CodeGenerator.sln by Visual Studio 2022.  
1. Set ConsoleAppCsharpGenerator as startup project.   
1. Edit command line args in debug launch profile of ConsoleAppCsharpGenerator project depending on your environment.  
6. Build & Run!

## Command line arguments.
```shell
ConsoleAppCsharpGenerator --metamodel BridgePoint\tools\mc\schema\sql\xtumlmc_schema.sql --base-datatype BridgePoint\tools\mc\schema\Globals.xtuml --domainmodel workspace\domain-model\models\domain-model --project generating-project-name --dotnetver net5.0 --gen-folder csharp\gen
```
Command line options  
- --metamodel  <i>OOA of OOA definition sql file</i>  
absolute file path of xtumlmc_schema.sql which is stored into tools\mc\schema\sql of your BridgePoint installation folder.
- --base-type <i>core data type definition xtuml file</i>  
absolute file path of Globals.xtuml which is stored into tools\mc\schema\sql of your BridgePoint installation folder.  
- --domain-model <i>your-domain-model</i>  
absolute folder path of your domain model under BridgePoint's workspace.  
- --project <i>project-name</i>  
generating C# project name.  
- --dotnetver <i>.NET runtime version</i>  
.NET runtime version which is used in generating C# project.  
- --gen-folder <i>folder-path</i>  
generated files are stored into the <i>folder-path</i>.
