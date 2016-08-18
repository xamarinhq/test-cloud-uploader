using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocoptNet;

namespace Microsoft.AppHub.TestCloud
{
    /// <summary>
    /// Represents command-line options for upload-tests command.
    /// </summary>
    public class UploadTestsCommandOptions
    {
        public const string AppFileOption = "<app-file>";
        public const string ApiKeyOption = "<api-key>";
        public const string UserOption = "user";
        public const string WorkspaceOption = "workspace";
        public const string AppNameOption = "app-name";
        public const string DevicesOption = "devices";
        public const string TestParametersOption = "test-parameters";
        public const string AsyncOption = "async";
        public const string AsyncJsonOption = "async-json";
        public const string LocaleOption = "locale";
        public const string SeriesOption = "series";
        public const string DSymDirectoryOption = "dsym-directory";
        public const string DebugOption = "debug";

        private static readonly string[] AllOptionIds = 
        {
            UserOption,
            WorkspaceOption,
            AppNameOption,
            DevicesOption,
            TestParametersOption,
            AsyncOption,
            AsyncJsonOption,
            LocaleOption,
            SeriesOption,
            DSymDirectoryOption,
            DebugOption
        };

        public static readonly string OptionsSyntax = $@"    
    --user <user>                     - Email address of the user uploading.
    --workspace <workspace>           - Path to your workspace folder (containing your tests).
    --app-name <app-name>             - App name to create or add test to.
    --devices <devices>               - Device selection id from the Test Cloud upload dialog.
    --async                           - Don't wait for the Test Cloud run to complete.
    --async-json                      - Don't wait for the Test Cloud run to complete and output async results in json format.
    --locale <locale>                 - System language.
    --series <series>                 - Test series.
    --dsym-directory <dsym-directory> - Optional dSYM directory for iOS crash symbolication.
    --debug                           - Prints out more debug information.
";

        private readonly IDictionary<string, ValueObject> _options;

        public UploadTestsCommandOptions(IDictionary<string, ValueObject> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        public void Validate()
        {
            var unrecognizedOptions = GetUnrecognizedOptions(_options).ToArray();

            if (unrecognizedOptions.Any())
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Unrecognized options:");
                foreach (var option in unrecognizedOptions)
                {
                    errorMessage.AppendLine(option);
                }

                throw new ArgumentException(errorMessage.ToString());
            }

            ValidateRequiredOptions();
        }

        protected virtual void ValidateRequiredOptions()
        {
            if (!File.Exists(this.AppFile))
                throw new ArgumentException($"Cannot find app file: {this.AppFile}", AppFileOption);
            
            if (!Directory.Exists(this.Workspace))
                throw new ArgumentException($"Cannot find workspace directory: {this.Workspace}", WorkspaceOption);

            if (!string.IsNullOrWhiteSpace(this.DSymDirectory) && !Directory.Exists(this.DSymDirectory))
                throw new ArgumentException($"Cannot find dSYM directory: {this.DSymDirectory}", DSymDirectoryOption);

            if (string.IsNullOrWhiteSpace(this.Devices))
                throw new ArgumentException($"Missing required option 'devices'", DevicesOption);
        }

        protected virtual IEnumerable<string> GetUnrecognizedOptions(IDictionary<string, ValueObject> allOptions)
        {
            return allOptions.Keys.Where(k => !AllOptionIds.Contains(k));
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
            get { return _options[UserOption].ToString(); }
        }

        public string Workspace
        {
            get { return _options[Workspace].ToString(); }
        }

        public string AppName
        {
            get { return _options[AppName].ToString(); }
        }

        public string Devices
        {
            get { return _options[Devices].ToString(); }
        }

        public string TestParameters
        {
            get { return _options[TestParametersOption].ToString(); }
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
            get { return _options[LocaleOption].ToString(); }
        }

        public string Series
        {
            get { return _options[SeriesOption].ToString(); }
        }

        public string DSymDirectory
        {
            get { return _options[DSymDirectoryOption].ToString(); }
        }

        public bool Debug
        {
            get { return _options[DebugOption].IsTrue; }
        }
    }
}