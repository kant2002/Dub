Dub
===

Application framework for speedier development of the SaaS application


How to build
===
To build this project run 

   msbuild nuget.proj /t:BuildSolution

To package NuGet packages to MyGet use

   msbuild nuget.proj /t:PackageNuget

To package and publish NuGet packages to MyGet use

   msbuild nuget.proj /t:PublishNightly