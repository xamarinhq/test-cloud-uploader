# XTC Command Line Interface (CLI): Test Cloud commands

This project contains Test Cloud commands for the common CLI.

## Commands
### test 
```
Command 'test': Upload tests to the Test Cloud.

Usage:
    xtc test <app-file> <api-key> [options]

Options:
    --user <user>                        - Email address of the user.
    --workspace <workspace>              - Path to the workspace folder (containing your tests).
    --app-name <app-name>                - App name to create or add test to.
    --devices <devices>                  - Device selection id from the Test Cloud upload dialog.
    --async                              - Don't wait for the Test Cloud run to complete.
    --async-json                         - Don't wait for the Test Cloud run to complete and output async results in json format.
    --locale <locale>                    - System language (en_US by default)
    --series <series>                    - Test series name.
    --dsym-directory <dsym-directory>    - Optional dSYM directory for iOS crash symbolication.
    --test-parameters <cspairs>          - Comma-separated test parameters (e.g. user:nat@xamarin.com,password:xamarin)
    --debug                              - Prints out more debug information.
```

## Supported test frameworks
The test command supports the following test frameworks:

- [UI Test](Docs/UITest.md)
- [Calabash](Docs/Calabash.md)
- [Appium (Java)](Docs/Appium.md)

## Implementation details
### Test Cloud URL 
By default, the tool uploads tests to  ```https://testcloud.xamarin.com/ci```. For testing
purposes, you can change it by setting environment variable ```XTC_ENDPOINT``` to your own URL.

### Uploading files
All files are uploaded to Test Cloud in a single multi-part HTTP request.

To save both bandwidth and storage space, the Test Cloud and the upload command detect whether a file
was already uploaded. To do so, the upload tool computes SHA-256 hashes of each file and asks
the server whether file with the same hash already exists on Test Cloud server (again, using 
single multi-part request). For files that were already uploaded, the client only sends
their hashes and names. 

### Workspace directory validation
The upload command tries to validate the workspace folder and ensure it contains tests for the chosen framework. The
verification is very simple - it checks whether the workspace folder contains certain files and directories.
It does not ensure that the test can be run on the actual device, but it should prevent a user to accidentally
upload invalid folder.
You can find more details about tested files and directories in documentation for each test framework.

### Project folders structure
```
Xtc.TestCloud/
|-- Docs/                       Documentation files.
|
|-- Commands/                   Implementations of the Xtc ICommand / ICommandExecutor interfaces.
|
|-- ObjectModel/                Classes that represent data sent or received from Test Cloud REST API.
|   | 
|   |-- MultipartContent/       Classes for building multi-part HTTP content in a format recognized by Test Cloud REST API.
|
|-- Services/                   Implementation of proxy that communicates with the Test Cloud. 
|
|-- Utilities/                  Other helper classes for computing hashes, computing relative paths, etc.
```