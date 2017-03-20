using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Represents command-line options for 'test' command.
    /// </summary>
    public class UploadTestsCommandOptions: IUploadTestsCommandOptions
    {
        public static string OptionsDescription() {
            return  @"
                <app-file>                           - An Android or iOS application to test.
                <api-key>                            - Test Cloud API key.
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

        public static string[] OptionsSyntax()
        {
            return new [] { "<app-file> <api-key> --user <user> --devices <devices> --workspace <workspace> [options]" };
        }

        public UploadTestsCommandOptions(IDictionary<string, ValueObject> options) : base(options) {}

        public override void Validate()
        {
            ValidateRequiredOptions();
        }

        protected virtual void ValidateRequiredOptions()
        {
            if (!File.Exists(this.AppFile))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Cannot find app file: {this.AppFile}",
                    (int)UploadCommandExitCodes.InvalidAppFile);
            }

            if (!Directory.Exists(this.Workspace))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Cannot find workspace directory: {this.Workspace}",
                    (int)UploadCommandExitCodes.InvalidWorkspace);
            }

            if (!string.IsNullOrWhiteSpace(this.DSymDirectory) && !Directory.Exists(this.DSymDirectory))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Cannot find dSYM directory: {this.DSymDirectory}",
                    (int)UploadCommandExitCodes.InvalidDSymDirectory);
            }

            if (string.IsNullOrWhiteSpace(this.Devices))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Missing required option '{DevicesOption}'",
                    (int)UploadCommandExitCodes.InvalidOptions);
            }

            if (string.IsNullOrEmpty(this.User))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Missing required option '{UserOption}'",
                    (int)UploadCommandExitCodes.InvalidOptions);
            }

            if (_parsedTestParameters == null)
            {
                _parsedTestParameters = ParseTestParameters(_options[TestParametersOption]?.ToString()).ToList();
            }
        }
    }
}