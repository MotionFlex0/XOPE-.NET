# XOPE-.NET
A packet view/editor for Windows with per-application capture/filtering. It also has scripting support using C# and replay functionality. 

# Building

Built using VS2022 with .NET6 and c++20.

### Requirements

* [`vcpkg`](https://vcpkg.io/en/getting-started.html) is used to install the required c++ dependencies.

    * bshoshany thread pool
        ```bat
        vcpkg install bshoshany-thread-pool:x64-windows-static bshoshany-thread-pool:x86-windows-static
        ```

    * zlib
        ```bat
        vcpkg install zlib:x64-windows-static zlib:x86-windows-static
        ```

* [`NuGet`](https://www.nuget.org/) is used to installed required c# dependencies.

    _They should be automatically restored when building the project but if not, there are two ways to install required dependencies._


    * Right-click the `XOPE .NET` solution inside of the Visual Studio Solution explorer and click `Restore NuGet Packages`.

        _**-or-**_

    * Using the NuGet CLI enter the command
        ```bat
        nuget restore YourSolution.sln
        ```

* Make sure the _XOPE UI_ project is set as startup project