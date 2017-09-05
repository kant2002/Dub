Dub
===

Application framework for speedier development of the SaaS application

What's changed
===

### 1.0.0
- Moved all projects to the .NET Core RTM.

### 0.9.0
- Moved all project to the new ASP.NET Core RC2 projects.

### 0.5.0
- Moved all project to the new ASPNET Core projects.

How to build
===
To build this project run 

   &"${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild" nuget.proj /t:BuildSolution /verbosity:d

To package and publish .NET Core NuGet packages to MyGet

    rm -recurse artifacts
    $time = (Get-Date).ToString("yyyyMMdd")
    dotnet pack Dub\Dub.sln --version-suffix dev-$time-1 --output ..\..\artifacts
    dotnet msbuild nuget.proj /t:PublishNightly /verbosity:d
