# Farewell

![Status](https://img.shields.io/badge/status-Alpha-purple)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-blue)
![DotNet](https://img.shields.io/badge/.NET-10.0-purple)
![License](https://img.shields.io/badge/license-MIT%20with%20Attribution-green)

## About
**Farewell** - a lightweight, host-agnostic abstraction framework for .NET applications that brings clean architecture principles beyond ASP.NET boundaries. 
Provides pure abstractions over common infrastructure concerns - storage, mediation, validation, domain configuration, allowing Domain and Application layers to remain completely free 
of third-party dependencies, while Infrastructure layer binds everything together with concrete implementations.
Works equally well for Web APIs, console tools, desktop and mobile applications, giving developers a consistent and clean foundation to build on without reinventing the wheel every time.

## How that should work
Default Clean Architecture template:
1. **Domain** - referensing ONLY abstract layer - Farewell.Abstractions.
2. **Application** - referensing Domain(and Farewell.Abstractions).
3. **Infrastructure** - binding and configurating everything, referensing Application and implementations.
4. **Presentation** - referencing Infrastructure and connecting Application layer capabilities to running environment.

It can be used pretty anywhere - console, desktop, server applications, no limitations - application logic is isolated and abstracted from implementation packages.

## Disclaimer
This is a **research project** aimed at the **theoretical** feasibility of providing a **convenient and universal environment** for implementing projects with business logic. 
The main goal is to isolate this logic from the multiple abstract layers of various libraries or a specific execution environment for this logic (hello, ASP). 
**This isn't a new abstraction standard**; it's a separate environment. Here I'm not trying to cover all the capabilities of all libraries with a completely universal abstraction layer - that's just impossible. 
This environment has its own rules of operation and a separate approach to its use and receiving what you want.

## License
```
MIT License with Attribution Requirement

Copyright (c) 2026 starplaysyt
https://github.com/starplaysyt/Farewell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, subject to the following conditions:

1. Attribution is required.
   Any redistributions of this software, in source or binary forms, must
   include a prominent attribution to the original author and repository URL
   (for example, in documentation, “About” dialogs, or credits).

2. The above copyright notice and this permission notice shall be included in
   all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
```

