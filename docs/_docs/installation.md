---
title: Installation
tags: [quick-start]
---

This guide explains how to install Chrononuensis using **NuGet** and the **.NET CLI**.

## Add Chrononuensis to a .NET Project

You can install Chrononuensis in an existing .NET project using:

### Using .NET CLI

#### Creating a new reference using .NET CLI

If you're using the .NET command-line interface (CLI), open a terminal in your project's root directory and run:

```sh
dotnet add package Chrononuensis
```

This will:

* Add the Chrononuensis package to your project.
* Update the csproj file to include it as a dependency.

#### Updating an existing reference using .NET CLI

Before updating, check the currently installed version by running:

```sh
dotnet list package Chrononuensis
```

This will output something like:

```plaintext
Package Reference      Resolved Version    Latest Version
-------------------    ----------------    --------------
Chrononuensis         1.2.0               1.3.0
```

If a newer version is available, proceed with the update.

update the package by running:

```sh
dotnet add package Chrononuensis --prerelease
```

or specify a version explicitly:

```sh
dotnet add package Chrononuensis --version 1.3.0
```

This updates Chrononuensis in your .csproj file.

### Using NuGet Package Manager

#### Creating a new reference using NuGet Package Manager

Open your .NET project in Visual Studio.

Open the Package Manager Console (Tools > NuGet Package Manager > Package Manager Console).

Run the following command:

```powershell
Install-Package Chrononuensis
```

#### Updating an existing reference using NuGet Package Manager

1. Open Visual Studio.
1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution.
1. Find Chrononuensis in the Installed tab.
1. Click Update to install the latest version.

Alternatively, use the Package Manager Console and run:

```powershell
Update-Package Chrononuensis
```

This will update Chrononuensis to the latest available version.

## Verify Installation

After installing, check that the package was added correctly:

```sh
dotnet list package
```

You should see Chrononuensis in the list of installed packages.
