using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.Selenium
{
    public enum SlDriverBrowserType
    {
        Firefox,
        Chrome,
        IE
    }

    public class DriverDictionary : Dictionary<SlDriverBrowserType, Dictionary<string, SlDriver>>
    {

        public bool IsOpen(SlDriverBrowserType Browser, string ProfileName)
        {
            return GetDriver(Browser, ProfileName) != null;
        }


        public void OpenDriver(SlDriver Driver)
        {
            if(!this.ContainsKey(Driver.BrowserType))
            {
                this[Driver.BrowserType] = new Dictionary<string, SlDriver>();
            }

            var myDict = this[Driver.BrowserType];

            if(myDict.ContainsKey(Driver.ProfileName))
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

            myDict.Remove(ProfileName);
        }

        public void CloseDriver(SlDriver Driver)
        {
            CloseDriver(Driver.BrowserType, Driver.ProfileName);
        }


        public void ClearAllDrivers(SlDriverBrowserType BrowserType)
        {
            if (!this.ContainsKey(BrowserType))
                return;

            this.Remove(BrowserType);
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
