using System;
using System.Diagnostics;

namespace Microsoft.AppHub.Cli
{
    public static class ProgramUtilities
    {
        private static readonly Lazy<string> _currentExecutableName = new Lazy<string>(
            () => Process.GetCurrentProcess().ProcessName);
        
        public static string CurrentExecutableName
        {
            get 
            { 
                return _currentExecutableName.Value; 
            }
        }
    }
}