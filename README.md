Dub
===

Application framework for speedier development of the SaaS application

What's changed
===

### 0.5.0
- Moved all project to the new ASPNET Core projects.

How to build
===
To build this project run 

   &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:BuildSolution /verbosity:d

To package NuGet packages to MyGet use

   &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:PackageNuget /verbosity:d

To package and publish NuGet packages to MyGet use

   &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:PublishNightly /verbosity:d

To package and publish ASP.NET 5 NuGet packages to MyGet

   rm -recurse Dub\artifacts
   &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:PublishAspNet5Nightly /verbosity:d