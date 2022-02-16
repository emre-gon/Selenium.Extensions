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


        protected static void FixDriverCommandExecutionDelay(RemoteWebDriver _baseDriver)
        {            
            PropertyInfo commandExecutorProperty = typeof(RemoteWebDriver).GetProperty("CommandExecutor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
            ICommandExecutor commandExecutor = (ICommandExecutor)commandExecutorProperty.GetValue(_baseDriver);

            FieldInfo remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);

            if (remoteServerUriField == null)
            {
                FieldInfo internalExecutorField = commandExecutor.GetType().GetField("internalExecutor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                commandExecutor = (ICommandExecutor)internalExecutorField.GetValue(commandExecutor);
                remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
            }

            if (remoteServerUriField != null)
            {
                string remoteServerUri = remoteServerUriField.GetValue(commandExecutor).ToString();

                string localhostUriPrefix = "http://localhost";

                if (remoteServerUri.StartsWith(localhostUriPrefix))
                {
                    remoteServerUri = remoteServerUri.Replace(localhostUriPrefix, "http://127.0.0.1");

                    remoteServerUriField.SetValue(commandExecutor, new Uri(remoteServerUri));
                }
            }
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
