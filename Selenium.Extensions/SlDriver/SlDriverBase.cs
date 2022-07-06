using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Selenium.Extensions
{
    public abstract partial class SlDriver 
    {
        protected static void ClearDrivers(SlDriverBrowserType slDriverBrowserType)
        {
            _openDrivers.ClearAllDrivers(slDriverBrowserType);
        }

        #region mime type download
        private static HashSet<string> _allowedMimeTypesForDownload = new HashSet<string>();
        public static void AllowMimeTypeForDownload(string MimeType)
        {
            _allowedMimeTypesForDownload.Add(MimeType);
        }

        public static IEnumerable<string> GetAllowedMimeTypesForDownload()
        {
            return _allowedMimeTypesForDownload.OrderBy(f => f).ToList();
        }

        public static void DisallowMimeTypeForDownload(string MimeType)
        {
            _allowedMimeTypesForDownload.Remove(MimeType);
        }
        #endregion

    }
}
