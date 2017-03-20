using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocoptNet;
using Microsoft.Xtc.Common.Cli.Commands;

namespace Microsoft.Xtc.TestCloud.Commands
{
    public abstract class IUploadTestsCommandOptions
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

        protected readonly IDictionary<string, ValueObject> _options;
        protected IList<KeyValuePair<string, string>> _parsedTestParameters;

        public IUploadTestsCommandOptions(IDictionary<string, ValueObject> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
            _parsedTestParameters = null;
        }
    
        public virtual string AppFile
        {
            get {  return _options[AppFileOption].ToString(); }
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
            get { return _options[WorkspaceOption]?.ToString(); }
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

        public abstract void Validate();

        protected IEnumerable<KeyValuePair<string, string>> ParseTestParameters(string testParametersString)
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