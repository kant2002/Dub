Dub
===

Application framework for speedier development of the SaaS application

What's changed
===

### 0.9.0
- Moved all project to the new ASP.NET Core RC2 projects.

### 0.5.0
- Moved all project to the new ASPNET Core projects.

How to build
===
To build this project run 

   &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:BuildSolution /verbosity:d

To package and publish .NET Core RC2 NuGet packages to MyGet

    rm -recurse Dub\artifacts
    dotnet pack --output artifacts Dub\Dub.Web.Core\
	dotnet pack --output artifacts Dub\Dub.Web.Dto\
	dotnet pack --output artifacts Dub\Dub.Web.Identity\
	dotnet pack --output artifacts Dub\Dub.Web.Mvc\
    &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:PublishNightly /verbosity:d
