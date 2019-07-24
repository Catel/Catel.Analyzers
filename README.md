Catel
=====

Name|Badge
---|---
Chat|[![Join the chat at https://gitter.im/catel/catel](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/catel/catel?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
Downloads|![NuGet downloads](https://img.shields.io/nuget/dt/catel.analyzers.svg)
NuGet stable version|![Version](https://img.shields.io/nuget/v/catel.analyzers.svg)
NuGet unstable version|![Pre-release version](https://img.shields.io/nuget/vpre/catel.analyzers.svg)
MyGet unstable version|![Pre-release version](https://img.shields.io/myget/catel/vpre/catel.analyzers.svg)
Open Collective|[![Backers on Open Collective](https://opencollective.com/Catel/backers/badge.svg)](#backers) [![Sponsors on Open Collective](https://opencollective.com/Catel/sponsors/badge.svg)](#sponsors)

Catel is an application development platform with the focus on MVVM (WPF, UWP, Xamarin.Android, Xamarin.iOS and Xamarin.Forms). 
The goal of Catel is to provide a complete set of modular functionality for Line of Business applications written in any .NET 
technology, from client to server.

This project provides Roslyn code analyzers to provide info on best practices when using Catel.


For documentation, please visit the [documentation portal](https://docs.catelproject.com)

## How to contribute

### Support on Open Collective

Please consider supporting [Catel on Open Collective](https://opencollective.com/catel).

## Building Catel.Analyzers

**Prerequisites** 

Catel requires Visual Studio 2019 to compile successfully. You also need to ensure you have the following features installed:

Note that the `.vsconfig` in the src root should notify about missing components when opening the solution.

- .NET desktop development
- Universal Windows Platform development
- Mobile development with .NET
- .NET Core cross-platform development
- Select the following components from Individual components
  - Windows 10 SDK (10.0.16299.0) 
  - MSVC v141 and v142 (ARM, ARM64 and x64/x86) 
  - C++ ATL v141 and v142 (ARM, ARM64 and x86 & x64)  build tools
  - C++ MFC v141 and v142 (ARM, ARM64 and x86 & x64) build tools
- [Latest Version of .NET Core 3.0 Preview SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)
  - Ensure you enable **Use previews of the .NET Core SDK** under Tools -> Options -> Projects and Solutions -> .NET Core

Note that you should run these commands using powershell in the root of the repository.

### Running a build

`.\build.ps1 -target build`

### Running a build with unit tests

`.\build.ps1 -target buildandtest`

### Running a build with local packages

Note that this assumes a local packages directory at `C:\Source\_packages`, which can be added to the NuGet feeds:

![](doc/nuget_local_packages.png)

`.\build.ps1 -target buildandpackagelocal`

## Contributors

This project exists thanks to all the people who contribute. [[Contribute](CONTRIBUTING.md)].
<a href="graphs/contributors"><img src="https://opencollective.com/Catel/contributors.svg?width=890&button=false" /></a>

## Backers

Thank you to all our backers! 🙏 [[Become a backer](https://opencollective.com/Catel#backer)]

<a href="https://opencollective.com/Catel#backers" target="_blank"><img src="https://opencollective.com/Catel/backers.svg?width=890"></a>

## Sponsors

Support this project by becoming a sponsor. Your logo will show up here with a link to your website. [[Become a sponsor](https://opencollective.com/Catel#sponsor)]

<a href="https://opencollective.com/Catel/sponsor/0/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/0/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/1/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/1/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/2/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/2/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/3/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/3/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/4/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/4/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/5/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/5/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/6/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/6/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/7/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/7/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/8/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/8/avatar.svg"></a>
<a href="https://opencollective.com/Catel/sponsor/9/website" target="_blank"><img src="https://opencollective.com/Catel/sponsor/9/avatar.svg"></a>
