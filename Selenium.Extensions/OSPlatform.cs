using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium.Extensions
{
    public enum OperatingSystemType
    {
        Windows,
        OSX,
        Linux,
        Unknown
    }

    public static class Platform
    {
        private static OperatingSystemType OS;
        static Platform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                OS = OperatingSystemType.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                OS = OperatingSystemType.OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                OS = OperatingSystemType.Linux;
            else
                OS = OperatingSystemType.Unknown;
        }

        public static OperatingSystemType CurrentOS
        {
            get
            {
                return OS;
            }
        }
    }
}
