# AppHub Command Line Interface (CLI)

Unified command line interface for App Hub.

## Installation

### Windows

1. Download ```app.win10-x64.zip``` from the [last stable build](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/).
   
   After downloading, right-click on the Zip file, select "Unlock", and click the "OK" button.
2. Unzip the file.

### OS X
1. Download ```app.osx.10.10-x64.tar.gz``` from the [last stable build](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/).
2. Unpack the archive file:

   ```tar -xvzf app.osx.10.10-x64.tar.gz```

3. Add "Execute" permission to the "app".

   ```chmod u+x apphub/app```

## Usage
```
./app help
Usage: app <command> [options]
Available commands:
  help            Print help for a command
  run             Run external command extension
  upload-tests    Upload tests to the Test Cloud
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

4. AppHub.TestCloud

   Implementation of command that can upload tests to the Test Cloud.

## Build status

| Branch  | Windows  | OS X      |
|:-------:|:--------:|:--------:|
| master (stable) | [![Build Status](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20master/badge/icon)](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20master/) | [![Build Status](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/badge/icon)](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20master/) | 
| develop | [![Build Status](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20develop/badge/icon)](http://xtc-jenkins.xamdev.com/view/Uploader/job/Uploader%20develop/)     | [![Build Status](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20develop/badge/icon)](http://calabash-ci.macminicolo.net:8080/view/Uploader/job/Uploader%20develop/)     | 