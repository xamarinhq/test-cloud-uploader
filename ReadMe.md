# AppHub Command Line Interface (CLI)

Unified command line interface for App Hub.

## Usage
```
./app help
Usage: app <command> [options]
Available commands:
  help    Prints help for a command
  run     Runs external command extension
```

## Project structure
The solution consits of the following projects:

1. AppHub.Common
   
   Contains interfaces and default implementations for common services that can be used
   by entire AppHub. This library has minimal dependencies and can be also used by non-CLI projects.

2. AppHub.Common.Cli

   Contains interfaces and utility classes for implementing CLI commands. This 
   project depends on Docopt.NET for parsing command line arguments.

3. AppHub.Cli

   The actual `app` executable that works as a driver for command-line interface.

## Extensibility
The extensibility model is very similar to `dotnet` command from .NET Core. The `app` driver
has a few built-in commands, but it can also execute extension implemented as separate executables. 

If you want to add new AppHub command, you have two options:
1. Implement the `IDescriptionCommand` and `ICommand` interfaces, and register them in AppHub.Cli.
   Typically, you would:
   1. Create a new project for your team (e.g. AppHub.TestCloud)
   2. Create your implementation of `ICommandDescription` and `ICommand` interfaces.
   3. Reference your project from AppHub.Cli.
   4. Register your command descripion in AppHub.Cli/Program.cs, method `CreateCommandsRegistry`.

2. Create a separate executable named `app-commandName`, and make sure that it is either
   deployed togeter with `app`, or it's location is included in the PATH variable.

Option #1 should be used for most of AppHub internal commands. Option #2 allows users
to create their own extensions, or to create commands that cannot be easily implemented
using .NET Core.

## Build status

| Branch  | Windows  | OS X      |
|:-------:|:--------:|:--------:|
| master (stable) | [![Build Status](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20master/badge/icon)](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20master/) | [![Build Status](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/badge/icon)](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/) | 
| develop | [![Build Status](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20develop/badge/icon)](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20develop/)     | [![Build Status](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20develop/badge/icon)](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20develop/)     | 