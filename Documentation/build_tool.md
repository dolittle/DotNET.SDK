---
title: Build Tool
description: Describes the Dolittle Build Tool for the .NET SDK
keywords: General, tooling, Build Tool
author: woksin
weight: 2
aliases: 
    - /runtime/dotnet.sdk/tooling/build_tool
    - /runtime/dotnet-sdk/tooling/build_tool
---

## Background
One of our main visions is to enable developers to build Line of Business products with high productivity while also building products that are scalable and easy to maintain. With tooling we can provide developers with functionalities that enables a better development experience by automatically doing some of the work that is tedious and / or error prone. We can also give a better development experience by providing guidance, tips and squiggly lines by, for example, for .Net utilizing the Roslyn compiler to give the developers warnings and suggestions when they are doing something that does not work well when developing products on our platform or to provide with tips and suggestions for improvements when they aren't utilizing the different tools that we're providing for them to write maintainable code. The DotNET Build Tool is one such tool. This tool is rather important not only for its quality of life functions, but first and foremost for automatically generating and maintaining vital information of the *Bounded Context* for the platform. 

The Dolittle platform needs to know several things related to the [*Application* and *Bounded Contexts*]({{< relref bounded_context >}}). The whole functionality of a *Bounded Context* is defined by its [*Artifacts*]({{< relref artifacts >}}). These *Artifacts* are extremely vital and central to our platform, everything is dependent on them; the functionality of the *Bounded Context* itself, interaction with other *Bounded Contexts*, interaction with other *Applications* and several other important aspects. Because these *Artifacts* are so important we cannot rely on human to keep track of this, that's why we have a tool that does this for us. This is just one of the current functionalities of the Build Tool, later we'll explain this in more detail.

The .Net Build Tool can be found [here](https://github.com/dolittle/dotnet.sdk/tree/master/Source/Build). It's basically a .NetCore application that is executed each time a build is performed in the .csproj that has a reference to the *Build Tool* "entrypoint" defined [here](https://github.com/dolittle/DotNET.SDK/tree/master/Source/Build.MSBuild).

## Setting it up
Setting up the *Dolittle* for a Web-based project should be pretty straight forward. The *Core* of the *Bounded Context* could have a .csproj like this for setting up a AspNet-based Web application with Swagger and MongoDB implementation of Event store and Read models:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
  </PropertyGroup>

  <PropertyGroup>
    <!-- The relative path from this .csproj file to the bounded-context.json configuration file. It is defaulted to ../bounded-context.json -->
    <DolittleBoundedContextConfigPath>../bounded-context.json</DolittleBoundedContextConfigPath>
    <!-- Whether or not to use modules or not when genreating bounded context topology structure -->
    <DolittleUseModules>True</DolittleUseModules>
    <!--  A | separated Key/Value pair map of namespace segments to strip -->
    <DolittleNamespaceSegmentsToStrip>Web=Features|Events=External</DolittleNamespaceSegmentsToStrip>
    <!-- Whether or not the build tool should generate proxies -->
    <DolittleGenerateProxies>True</DolittleGenerateProxies>
    <!-- The relative path to put proxies if generated-->
    <DolittleProxiesBasePath>../InteractionLayer/Features</DolittleProxiesBasePath>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include="wwwroot\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Autofac" Version="4.8.1" />
    <PackageReference Include="Autofac.Extensions.DependencyInjection" Version="4.2.0" />
    
    <PackageReference Include="Dolittle.AspNetCore" Version="2.0.0-alpha2*" />
    <PackageReference Include="Dolittle.Build" Version="2.0.0-alpha2*" />   
    <PackageReference Include="Dolittle.Concepts.Serialization.Json" Version="2.0.0-alpha2.*" />
    <PackageReference Include="Dolittle.DependencyInversion.Autofac" Version="2.0.0-alpha2*" />
    <PackageReference Include="Dolittle.ReadModels.MongoDB" Version="2.0.0-alpha2*" />
    <PackageReference Include="Dolittle.ResourceTypes.Configuration" Version="2.0.0-alpha2*" /> 
    <PackageReference Include="Dolittle.Runtime.Events.MongoDB" Version="2.0.0-alpha2*" />   
    <PackageReference Include="Dolittle.SDK" Version="2.0.0-alpha2*" />
    <PackageReference Include="Dolittle.Serialization.Json" Version="2.0.0-alpha2.*" />
    <PackageReference Include="Dolittle.Tenancy.Configuration" Version="2.0.0-alpha2*" />
    
    <PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.5" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="2.2.0" />
    
    <PackageReference Include="Swashbuckle.AspNetCore" Version="2.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Concepts\Concepts.csproj" />
    <ProjectReference Include="..\Domain\Domain.csproj" />
    <ProjectReference Include="..\Events\Events.csproj" />
    <ProjectReference Include="..\Read\Read.csproj" />
  </ItemGroup>
</Project>
```
The important parts is:
```xml  
  <PropertyGroup>
    <!-- The relative path from this .csproj file to the bounded-context.json configuration file -->
    <DolittleBoundedContextConfigPath>../bounded-context.json</DolittleBoundedContextConfigPath>
    <!-- Whether or not to use modules or not when genreating bounded context topology structure -->
    <DolittleUseModules>False</DolittleUseModules>
    <!--  A | separated Key/Value pair map of namespace segments to strip -->
    <DolittleNamespaceSegmentsToStrip></DolittleNamespaceSegmentsToStrip>
    <!-- Whether or not the build tool should generate proxies -->
    <DolittleGenerateProxies>True</DolittleGenerateProxies>
    <!-- The relative path to put proxies if generated-->
    <DolittleProxiesBasePath>../Web/Features</DolittleProxiesBasePath>
  </PropertyGroup>
```
and
```xml
  <PackageReference Include="Dolittle.Build" Version="2.0.0-alpha2*" />
```

Having the Dolittle.Build reference will trigger the *Build Tool* given the Properties defined above as configuration arguments to the tool. We'll talk more about that later

## The Bounded Context configuration
When you first set up a *Bounded Context* project you need to provide the platform with a few vital pieces of information. The *Build Tool* is expecting a bounded-context.json file describing the configuration in the root folder of the *Bounded Context's* source code (at the same level as the solution file). The bounded-context.json configuration needs the following information:
{{% notice warning %}}
May be subject to change
{{% /notice%}}
bounded-context.json:

```json
{
  "application": "0d577eb8-a70b-4e38-aca8-f85b3166bdc2",
  "boundedContext": "f660966d-3a74-44e6-8268-a9aefbae6115",
  "boundedContextName": "Shop",
  "resources": {
    "readModels": {
      "production": "MongoDB",
      "development": "MongoDB"
    },
    "eventStore": {
      "production": "MongoDB",
      "development": "MongoDB"
    }
  },
  "core": {
    "language": "csharp"
  }
}
```
* application - The GUID of the *Application* that this *Bounded Context* belongs to
* boundedContext - The GUID of the *Bounded Context*
* boundedContextName - The name of the *Bounded Context*
* resources - A configuration telling the Dolittle Runtime which implementations of the Read model and Event Store database to use for which environment
* core - The core configuration
* core.language - The core language used for the *Bounded Context* 

## Topology
One important aspect of *Bounded Contexts* is its topology. The platform needs to have some metadata for *Artifacts*, which *Feature* it belongs to is one such. To be able to map out a *Bounded Context's* *Features* we need to first define its *Topology*. When the *Build Tool* is referenced in a .csproj it will take the assembly and referenced assemblies and it will start discovering *Artifacts*. After it has discovered all the *Artifacts* of the *Bounded Context* it will try define the topology of the *Bounded Structure*. Based on "DolittleUseModules" and "DolittleNamespaceSegmentsToStrip" options in the configuration the *Build Tool* will look at the type paths of the *Artifact* CLR types and create a topology structure based on this type path.

{{% notice warning %}}
For the *Build Tool* to work you actually also have to reference to Dolittle.DependencyInversion.Autofac in order for the assembly discovery and the IoC container mechanisms to work. You should get a runtime error in the *Build Tool* dll if this is not in place.
{{% /notice%}}
When the *Build Tool* has ran its course it will output a topology that would look something like this:
```json
{
  "topology": {
    "modules": [
      {
        "module": "8d5a724b-84eb-4085-a766-8d28e681743e",
        "name": "Carts",
        "features": [
          {
            "feature": "80f5e1a2-a2bc-4403-b7ec-8bd90920cf2a",
            "name": "Shopping",
            "subFeatures": []
          }
        ]
      },
      {
        "module": "c020195d-5675-4c17-9cc5-1a7539ce4680",
        "name": "SomeModule",
        "features": [
          {
            "feature": "728459c2-fab1-40c1-9ead-7122a1a890ea",
            "name": "SomeFeature",
            "subFeatures": [
              {
                "feature": "716259c2-fab1-40c1-9ead-7122a1a890ea",
                "name": "SomeSubFeature",
                "subFeatures": []
              },
              {
                "feature": "824459c2-fab1-40c1-9ead-7122a1a890ea",
                "name": "SomeOtherSubFeature",
                "subFeatures": []
              },
            ]
          }
        ]
      },
      {
        "module": "9291da5e-a5ad-4dc7-9037-5c97fad04046",
        "name": "Catalog",
        "features": [
          {
            "feature": "05b89f06-19c3-4502-b349-873ef7761a21",
            "name": "Listing",
            "subFeatures": []
          }
        ]
      }
    ],
    "features": []
  }
}
```
The *Topology* json object will sit in its own file, topology.json, inside a .dolittle folder somewhere in the root of the source code for the *Bounded Context*.
{{% notice note %}}
If your *Bounded Context* has a Web-interaction layer, then the .doltitle folder would be sitting in that folder.
{{% /notice %}}

#### Structuring; Modules and Features
We currently support two ways of structuring a *Bounded Context*; one is with *Modules* (the topology definition above is the result of building a *Bounded Context* with topology defined with modules), the other way is with *Features* only.
{{% notice note %}}
There are not practical implications of using *Modules* over *Features*, or vica versa, from the platform's perspective. Currently it's only a matter of how you want to structure the *Bounded Context* internally.
{{% /notice %}}
The only thing that will happen is that the *Build Tool* will enforce the namespace naming convention of *Artifact* types, based on whether or not you use modules or not, so that you are consistent with the structuring. For example if you have defined the bounded context to use modules, the *Build Tool* will fail over if you define this artifact:
```csharp
namespace Domain.TheModule
{
    public class TheCommand : ICommand
    { }
}
```
The *Build Tool* will tell you that a particular *Artifact* cannot fit inside the topology. 
{{% notice note %}}
The reason for this is that every *Artifact* has to belong to a single *Feature*, a *Module* is not a *Feature* it is only a structure that groups *Features*. 
{{% /notice %}}
To correct this you would either have to set useModules to false (then a *Feature* called TheModule would appear in the topology), or you could solve it by simply adding another segment to the namespace, for example:
```csharp
namespace Domain.TheModule.TheFeature
{
    public class TheCommand : ICommand
    { }
}
```
If this was the only *Artifact* in the *Bounded Context* the topology would look like this:
```json
 {
  "topology": {
    "modules": [
      {
        "module": "<Generated GUID>",
        "name": "TheModule",
        "features": [
          {
            "feature": "<Generated GUID>",
            "name": "TheFeature",
            "subFeatures": []
          }
        ]
      }
    ],
    "features": []
  }
}
```
{{% notice note %}}
Note that the "Domain" part of the namespace is completely ignored. This is because the *Build Tool* is by convention ignoring the first segment of the namespace. This is because we think that the first part of the namespace is reserved to indicate the domain area of the type, i.e. "Domain", "Events", "Events.OtherBoundedContext", "Read", "Web", "Policy", etc...
{{% /notice %}}

##### "DolittleNamespaceSegmentsToStrip"
DolittleNamespaceSegmentsToStrip can be useful when you want a namespace to have a specific prefix, or if you have a namespace that has a namespace segment which is '.' separated, like for example "Events.Shop".

If you didn't provide any DolittleNamespaceSegmentsToStrip Key-value pairs, DolittleUseModules is 'True' and you had this *Artifact*:
```csharp
namespace Events.OtherBoundedContext.TheModule
{
    [Artifact("<The Artifact's ArtifactId>")]
    public class IAmAnEventFromAnotherBoundedContext : IEvent
    { }
}
```
the *Build Tool* would not throw any errors and the topology would look like this:
```json
 {
  "topology": {
    "modules": [
      {
        "module": "<Generated GUID>",
        "name": "OtherBoundedContext",
        "features": [
          {
            "feature": "<Generated GUID>",
            "name": "TheModule",
            "subFeatures": []
          }
        ]
      }
    ],
    "features": []
  }
}
```
which is obviously not right, we would want the *Build Tool* to fail because we have not given the *Artifact* a real *Feature*. To fix this we could in the .csproj containing the configuration of the build tool add a namespaceSegmentsToStrip with the following definition:
```xml
<DolittleNamespaceSegmentsToStrip>Events=OtherBoundedContext</DolittleNamespaceSegmentsToStrip>
```
then the *Build Tool* would fail saying that the *Artifact* IAmAnEventFromAnotherBoundedContext does not fit in the topology.

## Artifacts
After the *Topology* of the Bounded Context has been created, the *Build Tool* will start the process of building the artifacts.json file. The artifacts.json file contains arguably the most vital and fragile information meant for the platform, the collection of all the *Artifacts* of the *Bounded Context*. These *Artifacts* are language-agnostic meant for the *Runtime*, they all contain the three following pieces of information:
{{% notice warning %}}
The definition of an *Artifact* may be subject to change.
{{% /notice %}}

* artifact - An ID that is unique throughout the *Application*
* generation - The generation of the *Artifact* represented as an integer
* type - A human readable string that will uniquely identify the *Artifact* in the *Bounded Context* and should give an idea of where it is located.

Given a list of all CLR types and the bounded context configuration, the *Build Tool* will create the artifacts.json file which contains a single object called "artifacts". "artifacts" will essentially be a dictionary where the *Key* is a *Feature* Id and the *Value* is a dictionary where the *Key* is the *Artifact* type as a string and the *Value* is a list of *Artifacts*. That object will look something like this:
{{% notice warning %}}
The set of *Artifact* types may be subject to change.
{{% /notice %}}

```json
{
  "artifacts": {
    "<The Feature's Id>": {
      "commands": [
        {
          "artifact": "<The Artifact's Id>",
          "generation": 1,
          "type": "<A human readable string that will uniquely identify, and be used to locate, the Artifact within the Bounded Context>"
        }
      ],
      "events": ["<List of Event-Artifacts>"],
      "eventSources": ["<List of EventSource-Artifacts>"],
      "readModels": ["<List of ReadModel-Artifacts>"],
      "queries": ["<List of Query-Artifacts>"]
    }
  }
}
```
And here is an example of a artifacts.json configuration after the *Build Tools* has outputted the configuration files.
artifacts.json:
```json
{
  "artifacts": {
    "80f5e1a2-a2bc-4403-b7ec-8bd90920cf2a": {
      "commands": [
        {
          "artifact": "8f75772f-6282-4854-86aa-4cbcbf47867a",
          "generation": 1,
          "type": "Domain.Carts.Shopping.AddItemToCart, Domain"
        }
      ],
      "events": [
        {
          "artifact": "ae6e7f74-7991-46bd-881b-941c6d87fbb8",
          "generation": 1,
          "type": "Events.Carts.Shopping.ItemAddedToCart, Events"
        }
      ],
      "eventSources": [
        {
          "artifact": "b25d8657-dffe-40e6-ba45-71dbfe09d98f",
          "generation": 1,
          "type": "Domain.Carts.Shopping.Cart, Domain"
        }
      ],
      "readModels": [],
      "queries": []
    },
    "728459c2-fab1-40c1-9ead-7122a1a890ea": {
      "commands": [],
      "events": [
        {
          "artifact": "64808183-4bf1-4b87-8144-84f85da5676f",
          "generation": 1,
          "type": "Events.SomeModule.SomeFeature.StockChanged, Events.Warehouse"
        }
      ],
      "eventSources": [],
      "readModels": [],
      "queries": []
    },
    "05b89f06-19c3-4502-b349-873ef7761a21": {
      "commands": [],
      "events": [],
      "eventSources": [],
      "readModels": [
        {
          "artifact": "a3041218-0d14-4fae-a349-32d791c6149b",
          "generation": 1,
          "type": "Read.Catalog.Listing.Product, Read"
        }
      ],
      "queries": [
        {
          "artifact": "46ee11e1-041c-4fab-92fd-8a683a6d1696",
          "generation": 1,
          "type": "Read.Catalog.Listing.ListingByCategory, Read"
        }
      ]
    }
  }
}
```
The process for generating this configuration goes as follows:
{{< mermaid align="left" >}}
    graph TD
    A[Create new Artifacts configuration]
    B{Are there more Artifacts?}
    B1[Pick next Artifact]
    B2{Are there any Artifacts<br/> not matching to a Feature?}
    C[Find matching Feature]
    D{Found matching Feature?}
    D1[Create and add Artifact to configuration<br/> if it's not already there]
    D2[Add Artifact to a list of non-matching Artifacts]
    E[Log the errors and fail the Build-process]
    F{Artifacts are valid?}
    F1[Finished the Artifacts process]
    F2{Duplicate Ids?}
    G[Log warnings]
    
    A -->|List of Artifacts<br/>New Topology<br/>Existing Artifacts configuration|B
    B --> |No| B2
    B2 --> |Yes| E
    B2 --> |No| F
    F --> |Yes| F1
    F --> |No| F2
    F2 --> |Yes| E
    F2 --> |No| G
    G --> F1

    B -->|Yes| B1
    B1 -->|The Artifact| C
    C --> D
    D --> |Yes| D1
    D1 --> B
    D -->|No| D2
    D2 --> B


{{< /mermaid >}}
#### Artifacts Validation

The validation process after the new configuration is created will inform the user if anything has gone wrong. The validation process consists of the following steps:

* It will go through every *Artifact* in the configuration and check if there is any duplication of *Artifact* Id. If so, the *Build Tool* will tell you that an *Artifact* with that *Artifact* Id already exists. This error is incredibly important to discover because it will jeopardize the whole system if this error were to occur. Since the *Artifact* Id must uniquely identify each and every one of the *Artifacts* in an *Application* the platform would identify *Artifact* A and *Artifact* B as the same if it had the same Id.

* It will go through every *Artifact* in the configuration and check whether or not the *Feature* Id it sits under exists in the topology. If so, the *Build Tool* will tell you that there are *Artifacts* under a *Feature* that doesn't exist. This is an indicator that you have built the application before and that there are left-over artifacts from a *Feature* that you have removed.

* It will go through every *Artifact* in the configuration and check if there are any *Artifacts* that cannot be mapped up to an actual CLR type. If so, the *Build Tool* will tell you that there are *Artifacts* than cannot be found in current *Bounded Context* topology structure and that you probably have to write a [migrator for that *Artifact*.]**(LINK TO ARTIFACT MIGRATION)**. This is an indicator that you had an *Artifact* that now has been changed.

## Proxy generation

When the *Bounded Context* is supposed to provide a web-interaction layer it probably has to deal with the *Command*, *ReadModel* and *Query* *Artifacts*. Since we already have discovered all the CLR types of the *Artifacts*, we can automatically create proxies for these *Artifacts* and output them as Javascript classes to be used in the web-interaction layer.

If the *Bounded Context* is configured with "generateProxies": true, the *Build Tool* will use the discovered *Artifacts* to find all *Commands*, *ReadModels* and *Queries* and create proxies for them based on their public, settable properties and the default values of each property's type. The generated proxies will have a path that corresponds the the Module / Feature hierarchy that's associated with the *Artifact*. You can provide the relative path where the proxies will be outputted to by setting the "proxiesBasePath" variable in the *Bounded Context* configuration. 

## .csproj Configuration Properties
* ```<DolittleUseModules>```: A boolean, True/False, indicating whether or not to generate using a Module or a Feature topology structure. Default value = True
* ```<DolittleBoundedContextConfigPath>```: A string path pointing to the *Bounded Context's* bounded-context.json file. Default value = ../bounded-context.json
* ```<DolittleNamespaceSegmentsToStrip```: A '|' separated key-value list separated by '=' where Key represents the first segment of a namespace that you want to strip the namespace of, and Value is the segment that you actually want to strip from the namespace generation / topology creation. Default value = " "
* ```<DolittleGenerateProxies>```: A boolean, True/False, indicating whether or not to generate query, read model and command proxies for the interaction layer. Default value = False
* ```<DolittleProxiesBasePath>```: a string path pointing to the location where proxies should be generated if generated. Default value = " "
