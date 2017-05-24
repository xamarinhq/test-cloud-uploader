# XTC Command Line Interface (CLI) 

Unified command line interface for XTC.

# Notice 

This is a temporary product used to upload Espresso, Appium and XCUITest tests to Xamarin Test Cloud. This project will be deleted and removed when Visual Studio Mobile Center launches publicly, and will be replaced by the [Mobile Center CLI](https://github.com/Microsoft/mobile-center-cli).

This project will not accept contributions outside of Microsoft due to the temporary nature of the product.

## Installation

### Windows

1. Download ```xtc.win7-x64.zip``` from the [last stable build](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/lastSuccessfulBuild/artifact/publish/Release/xtc.win7-x64.zip).
   
   After downloading, right-click on the Zip file, select "Unblock", and click the "OK" button.
2. Unzip the file.

### OS X
1. Download ```xtc.osx.10.10-x64.tar.gz``` from the [last stable build](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/lastSuccessfulBuild/artifact/publish/Release/xtc.osx.10.10-x64.tar.gz).

2. Unpack the archive file to any directory:

   ```tar -xvzf xtc.osx.10.10-x64.tar.gz```

   This will create a `xtc` directory.

3. Add the `xtc` directory created above to the PATH environment variable.

## Usage
```
xtc help
Usage: xtc <command> [options]
Available commands:
  help            Print help for a command
  run             Run external command extension
  test            Upload tests to the Test Cloud
```

## Project structure
The solution consits of the following projects:

1. Xtc.Common
   
   Contains interfaces and default implementations for common services that can be used
   by entire xtc. This library has minimal dependencies and can be also used by non-CLI projects.

2. Xtc.Common.Cli

   Contains interfaces and utility classes for implementing CLI commands. This 
   project depends on Docopt.NET for parsing command line arguments.

3. Xtc.Cli

   The actual `xtc` executable that works as a driver for command-line interface.

4. Xtc.TestCloud

   Implementation of command that can upload tests to the Test Cloud.

## Build status

| Branch  | Windows  | OS X      |
|:-------:|:--------:|:--------:|
| master (stable) | [![Build Status](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20master/badge/icon)](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20master/) | [![Build Status](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/badge/icon)](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/) | 
| develop | [![Build Status](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20develop/badge/icon)](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20develop/)     | [![Build Status](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20develop/badge/icon)](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20develop/)     | 
