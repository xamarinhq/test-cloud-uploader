using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;

namespace Microsoft.Xtc.TestCloud.Commands
{
    /// <summary>
    /// Represents command-line options for 'test' command.
    /// </summary>
    public class UploadTestsCommandOptions
    {
        public const string AppFileOption = "<app-file>";
        public const string ApiKeyOption = "<api-key>";
        public const string UserOption = "--user";
        public const string WorkspaceOption = "--workspace";
        public const string AppNameOption = "--app-name";
        public const string DevicesOption = "--devices";
        public const string TestParametersOption = "--test-parameters";
        public const string AsyncOption = "--async";
        public const string AsyncJsonOption = "--async-json";
        public const string LocaleOption = "--locale";
        public const string SeriesOption = "--series";
        public const string DSymDirectoryOption = "--dsym-directory";
        public const string DebugOption = "--debug";

        public const string OptionsDescription = @"
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

        public static readonly string[] OptionsSyntax =
        {
            "<app-file> <api-key> --user <user> --devices <devices> --workspace <workspace> [options]"
        };

        private readonly IDictionary<string, ValueObject> _options;
        private IList<KeyValuePair<string, string>> _parsedTestParameters;

        public UploadTestsCommandOptions(IDictionary<string, ValueObject> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
            _parsedTestParameters = null;
        }

        public void Validate()
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

        public string AppFile
        {
            get { return _options[AppFileOption].ToString(); }
        }

        public string ApiKey
        {
            get { return _options[ApiKeyOption].ToString(); }
        }

        public string User
        {
            get { return _options[UserOption]?.ToString(); }
        }

        public string Workspace
        {
            get { return _options[WorkspaceOption]?.ToString() ?? Path.GetDirectoryName(this.AppFile); }
        }

        public string AppName
        {
            get { return _options[AppNameOption]?.ToString(); }
        }

        public string Devices
        {
            get { return _options[DevicesOption]?.ToString(); }
        }

        public IList<KeyValuePair<string, string>> TestParameters
        {
            get 
            {
                if (_parsedTestParameters == null)
                    _parsedTestParameters = ParseTestParameters(_options[TestParametersOption]?.ToString()).ToList();
                 
                return _parsedTestParameters;
            }
        }

        public bool Async
        {
            get { return _options[AsyncOption].IsTrue; }
        }

        public bool AsyncJson
        {
            get { return _options[AsyncJsonOption].IsTrue; }
        }

        public string Locale
        {
            get { return _options[LocaleOption]?.ToString() ?? "en_US"; }
        }

        public string Series
        {
            get { return _options[SeriesOption]?.ToString(); }
        }

        public string DSymDirectory
        {
            get { return _options[DSymDirectoryOption]?.ToString(); }
        }

        public bool Debug
        {
            get { return _options[DebugOption].IsTrue; }
        }

        private IEnumerable<KeyValuePair<string, string>> ParseTestParameters(string testParametersString)
        {
            if (string.IsNullOrWhiteSpace(testParametersString))
                return Enumerable.Empty<KeyValuePair<string, string>>();

            var regex = new Regex("((?<key>.+):(?<value>.+))");
            var pairs = Regex.Split(testParametersString, "\\s*,\\s*");
            
            if (pairs.Any(p => !regex.IsMatch(p)))
            {
                throw new CommandException(
                    UploadTestsCommand.CommandName, 
                    $"Argument {TestParametersOption} is not formatted correctly", 
                    (int)UploadCommandExitCodes.InvalidOptions);
            }
                
            return pairs
                .Select(pair => {
                    var match = regex.Match(pair);
                    return new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value);
                });
        }

        public IList<string> ToArgumentsArray()
        {
            var result = new List<string>();

            foreach (var keyValue in _options)
            {
                if (keyValue.Value != null)
                {
                    result.Add(keyValue.Key);
                    if (keyValue.Value.IsString)
                        result.Add(keyValue.Value.ToString());
                }
            }

            return result;
        }

    }
}