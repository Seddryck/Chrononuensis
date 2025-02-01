---
title: Contributing to the the development
tags: [development]
---

Before installing and coding on Chrononuensis, ensure your system meets the following requirements:

## Prerequisites

Ensure the following dependencies are installed:

- **.NET SDK**: [Download and install](https://dotnet.microsoft.com/download)
- **Git** (for cloning the repository): [Download Git](https://git-scm.com/downloads)
- A code editor like Visual Studio Code or Visual Studio

Run the following command to check if .NET is installed:

```sh
dotnet --list-sdks
```

If you see a version number higher than 8.0 and higher than 9.0, .NET SDKs are installed correctly.

## Clone the project

Cloning the repository allows you to get the latest source code and contribute to development. Follow the steps below to set up Chrononuensis on your local machine.

Verify Git is installed by running:

```sh
git --version
```

If you see a version number, Git is installed correctly.

### Download source code

To download the source code, open a terminal and run:

```sh
git clone https://github.com/Seddryck/Chrononuensis.git
```

This will create a Chrononuensis folder in your current directory.

Move into the newly cloned repository:

```sh
cd Chrononuensis
```

### Create a Feature Branch

To develop a new feature, create a dedicated branch based on main:

```sh
git checkout -b feature/<your-feature-name>
```

Replace `your-feature-name` with a descriptive name, such as:

```sh
git checkout -b feature/improve-logging
```

## Build the project

Once cloned, build your project to ensure everything is set up correctly:

```sh
dotnet build
```

If the build is successful, you're ready to develop with Chrononuensis!
