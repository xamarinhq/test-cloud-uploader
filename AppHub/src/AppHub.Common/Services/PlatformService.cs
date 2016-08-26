using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.AppHub.Common
{
    public class PlatformService: IPlatformService
    {
        private static readonly Lazy<OSPlatform> _platformId = new Lazy<OSPlatform>(DetectPlatform);
        private static readonly Lazy<PlatformService> _instance = new Lazy<PlatformService>(() => new PlatformService());

        public static PlatformService Instance
        {
            get { return _instance.Value; }
        }

#if NETSTANDARD1_5 || NETSTANDARD1_6
        private static OSPlatform DetectPlatform()
        {
            var platforms = new[] { OSPlatform.Linux, OSPlatform.OSX, OSPlatform.Windows };
            
            return platforms.First(p => RuntimeInformation.IsOSPlatform(p));
        }
#else
        private const int UNameBufferSize = 8192;

        [DllImport("libc")]
        static extern int uname(IntPtr buffer);

        private static OSPlatform DetectPlatform()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32Windows
               || Environment.OSVersion.Platform == PlatformID.Win32S
               || Environment.OSVersion.Platform == PlatformID.Win32NT
               || Environment.OSVersion.Platform == PlatformID.WinCE)
	        {
                return OSPlatform.Windows;
            }

            IntPtr buffer = IntPtr.Zero;

            try
            {
                buffer = Marshal.AllocHGlobal(UNameBufferSize);

                if (uname(buffer) == 0)
                {
                    if (Marshal.PtrToStringAnsi(buffer).Contains("Darwin"))
                    {
                        return OSPlatform.OSX;
                    }
                    else
                    {
                        return OSPlatform.Linux;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Cannot detect current platform");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
#endif

        public OSPlatform CurrentPlatform
        {
            get { return _platformId.Value; }
        }
    }
}