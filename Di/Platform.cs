using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Di
{
    public static class Platform
    {
        public static readonly bool IsSupported, IsWindows, IsUnix;

        public static readonly string UserConfigDirectory, SystemConfigDirectory, HiddenFilePrefix;

        static Platform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    IsUnix = false;
                    IsWindows = true;
                    IsSupported = true;
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    IsUnix = true;
                    IsWindows = false;
                    IsSupported = true;
                    break;
                default:
                    IsUnix = false;
                    IsWindows = false;
                    IsSupported = false;
                    break;
            }
            if (IsWindows)
            {
                UserConfigDirectory = string.Format("{0}{1}{2}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create), Path.DirectorySeparatorChar, "Di");
                SystemConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System, Environment.SpecialFolderOption.Create);
                HiddenFilePrefix = "";
				throw new NotImplementedException();
            }
            else
            {
                UserConfigDirectory = Environment.GetEnvironmentVariable("HOME");
                SystemConfigDirectory = "/etc";
                HiddenFilePrefix = ".";
            }
        }

        public static string AppendFsPath(this string a, string b)
        {
            return a + Path.DirectorySeparatorChar + b;
        }
    }
}
