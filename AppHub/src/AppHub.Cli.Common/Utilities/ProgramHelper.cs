using System;
using System.IO;
using System.Diagnostics;

namespace Microsoft.AppHub.Cli
{
    public static class ProgramHelper
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