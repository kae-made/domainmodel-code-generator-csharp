# Domain Model Code Generator for C#  

[Generator library](./CodeGenerator/Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp/) that generate from [BridgePoint domain model](https://github.com/xtuml/bridgepoint) to C# Application Library running in memory.

## How to use  
1. Create conceptual information model for your conceptual domain according to the style of [eXecutable and Translatable UML modeling](https://xtuml.org/).  
1. Generate application library from the model. Please see [Sample Generator Application](./CodeGenerator/ConsoleAppCsharpGenerator/).  

※ You can use BrdigePoint MicrowaveOven or [LaundromatInHotel](https://github.com/kae-made/artifacts-laundromat-in-hotel-tutorial/tree/main/model/LaundromatInHotel) as a sample model.
※ Generated DTDL schemas of LaundromatInHotel are published at https://github.com/kae-made/artifacts-laundromat-in-hotel-tutorial/tree/main/code/csharp/LaundromatInHotel.

---
## Built in your generator tool.
User can use [Generator library](./CodeGenerator/Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp/) as [NuGet package](https://www.nuget.org/packages/Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp/1.0.0).

---
## Overview of Translation Rule  
- Generate an interface, base implement class for each class
- Generate an state machine class for each class with state model
- For each element of each class


|xtUML|->|C#|
|-|-|-|
|class|->|DomainClass<i>ClassName</i>, DomainClass<i>ClassName</i>Base(, DomainClass<i>ClassName</i>StateMachine)|
|attribute|->|property|
|event|->|DomainClass<i>ClassName</i>StateMachine.<i>EventName</i>|
|operation|->|methods of DomainClass<i>ClassName</i>|

- Relationships are defined in the DomainClass<i>ClassName</i> interface as reference instance, get linked instances, link and unlink method.
- Generate action code of function, transformer, entry action and methematical attribute  
- Generate wrapper class of each External Entity
    - TIM C# implementation can be used.
- Generate adaptor class for other domain implementation
