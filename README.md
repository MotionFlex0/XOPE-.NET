# XOPE-.NET
A packet view/editor for Windows with per-application capture/filtering. It also has scripting support using C# and replay functionality. 

# Building

### Requirements

[`vcpkg`](https://vcpkg.io/en/getting-started.html) is used to install the required dependencies.

* bshoshany thread pool
```bat
vcpkg install bshoshany-thread-pool:x64-windows-static bshoshany-thread-pool:x86-windows-static
```

* zlib
```bat
vcpkg install zlib:x64-windows-static zlib:x86-windows-static
```
