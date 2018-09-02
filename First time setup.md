# Setup

Run genicons.cmd to create the necessary icon files for all projects, then build the project inside Visual Studio (see below for required tooling).

icogen.exe and vpatch.exe are embedded in the repository, they can originally be found in these repositories: [IconGenerator](https://github.com/MarcStan/IconGenerator) and [VersionPatcher](https://github.com/MarcStan/VersionPatcher)

## Prerequisites

Visual Studio 2017 with C#, UWP and Xamarin workload.

For iOS you need a mac build agent.

To get the Android app up and running, be sure to have installed the required tools:

In Visual Studio go to Tools -> Android -> Android SDK Manager

Here's the list of installed tools on the build server and my machine:

* Platform 8.1 - Oreo
    * Android SDK Platform 27
    
* Platform 8.0 - Oreo
    * Android SDK Platform 26

Under tools also install:

* Android SDK Tools
    * Android SDK Tools (26.1.1)
* Android SDK Platform-Tools (28.0.0)
* Android SDK Build Tools
    * Android SDK Build-Tools 27.0.3
* Extras
    * Google USB Driver (11)
* SDK Patch Applier v4

Optionally (if needed) also install the HAXM and Android Emulator.