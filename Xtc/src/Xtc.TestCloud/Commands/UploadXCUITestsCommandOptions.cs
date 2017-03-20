using Microsoft.Xtc.TestCloud.ObjectModel;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.Utilities;
using System.Collections.Generic;
using DocoptNet;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Represents command-line options for 'xcuitest' command.
    /// </summary>
    public class UploadXCUITestsCommandOptions: UploadTestsCommandOptions
    {
        public const string AppFileOptionFlag = "--app-file";

        new public static string[] OptionsSyntax() 
        {
            return new [] { "<api-key> --user <user> --devices <devices> --workspace <workspace> [options]" };
        }
        new public static string OptionsDescription() {
            return  @"
                <api-key>                            - Test Cloud API key.
                --app-file <app-file>                - iOS application to test
                --user <user>                        - Email address of the user.
                --workspace <workspace>              - Path to the workspace folder (containing your tests).
                --app-name <app-name>                - App name to create or add tests to.
                --devices <devices>                  - Device selection id from the Test Cloud upload dialog.
                --async                              - Don't wait for the Test Cloud run to complete.
                --async-json                         - Don't wait for the Test Cloud run to complete and output async results in json format.
                --locale <locale>                    - System language (en_US by default)
                --series <series>                    - Test series name.
                --dsym-directory <dsym-directory>    - Optional dSYM directory for iOS crash symbolication.
                --test-parameters <cspairs>          - Comma-separated test parameters (e.g. user:nat@xamarin.com,password:xamarin)
                --debug                              - Prints out more debug information.
            ";
        }

        public UploadXCUITestsCommandOptions(IDictionary<string, ValueObject> options) : base(options) {}

        public override void Validate()
        {
            ValidateRequiredOptions();    
        }

        private string _appFile;

        public override string AppFile
        {
            get 
            {  
                if (_options[AppFileOptionFlag] != null) 
                {
                    return _options[AppFileOptionFlag].ToString();
                } 
                if (_appFile == null) 
                {
                    var appBundle = XCUITestWorkspace.FindAUT(this.Workspace);
                    if (appBundle == null)
                    {
                        throw new CommandException(
                            UploadXCUITestsCommand.CommandName, 
                            $"App file not specified and not found in workspace",
                            (int)UploadCommandExitCodes.MissingAppFile);
                    }
                    _appFile= FileHelper.ArchiveAppBundle(appBundle);
                }
                return _appFile;
            }
        }
    }
}