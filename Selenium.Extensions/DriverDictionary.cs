using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Selenium.Extensions
{
    public enum SlDriverBrowserType
    {
        Firefox,
        Chrome,
        IE
    }

    public class DriverDictionary : ConcurrentDictionary<SlDriverBrowserType, ConcurrentDictionary<string, SlDriver>>
    {

        public bool IsOpen(SlDriverBrowserType Browser, string ProfileName)
        {
            return GetDriver(Browser, ProfileName) != null;
        }


        public void OpenDriver(SlDriver Driver)
        {
            if(!this.ContainsKey(Driver.BrowserType))
            {
                this[Driver.BrowserType] = new ConcurrentDictionary<string, SlDriver>();
            }

            var myDict = this[Driver.BrowserType];

            string profileName = Driver.ProfileName;


            if(myDict.ContainsKey(profileName))
            {
                throw new Exception("Driver is already open");
            }

            myDict[Driver.ProfileName] = Driver;
        }

        public void CloseDriver(SlDriverBrowserType BrowserType, string ProfileName)
        {
            if (!this.ContainsKey(BrowserType))
            {
                return;
            }

            var myDict = this[BrowserType];

            if (!myDict.ContainsKey(ProfileName))
            {
                return;
            }

            if (!myDict.TryRemove(ProfileName, out var foo))
                throw new Exception("Could not close driver. Concurrency error occurred.");
        }

        public void CloseDriver(SlDriver Driver)
        {
            CloseDriver(Driver.BrowserType, Driver.ProfileName);
        }


        public void ClearAllDrivers(SlDriverBrowserType BrowserType)
        {
            if (!this.ContainsKey(BrowserType))
                return;

            if (!this.TryRemove(BrowserType, out var foo))
                throw new Exception("Could not clear all drivers. Concurrency error occurred.");
        }

        public SlDriver GetDriver(SlDriverBrowserType Browser, string ProfileName)
        {
            if (!this.ContainsKey(Browser))
                return null;

            var profiles = this[Browser];


            if (profiles.ContainsKey(ProfileName))
            {
                return profiles[ProfileName];
            }
            else
            {
                return null;
            }
        }
    }
}
