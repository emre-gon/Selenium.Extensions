using Newtonsoft.Json.Bson;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Extensions
{
    public abstract partial class SlDriver : IWebDriver
    {

        #region static constructor
        protected static DriverDictionary _openDrivers;

        static SlDriver()
        {
            _openDrivers = new DriverDictionary();
        }
        #endregion

        #region base driver
        private RemoteWebDriver _baseDriver;

        private static object createBaseDriverLock = new object();
        private RemoteWebDriver _lazyBaseDriver
        {
            get
            {
                if (_baseDriver == null)
                {
                    lock (createBaseDriverLock)
                    {
                        Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        if (!_openDrivers.IsOpen(this.BrowserType, this.ProfileName))
                        {
                            _openDrivers.OpenDriver(this);
                        }
                        
                        _baseDriver = createBaseDriver();
                        FixDriverCommandExecutionDelay(_baseDriver as RemoteWebDriver);
                    }
                }

                return _baseDriver;
            }
        }

        protected abstract RemoteWebDriver createBaseDriver();


        #endregion

        #region constructor / Factory
        public readonly string ProfileName;
        public readonly bool Headless;
        public readonly SlDriverBrowserType BrowserType;
        protected SlDriver(SlDriverBrowserType BrowserType, string ProfileName, bool Headless)
        {
            this.BrowserType = BrowserType;
            this.ProfileName = ProfileName;
            this.Headless = Headless;
        }
        #endregion



        public abstract string DriverName();
        public abstract string DriversFolderName();

        public string DriversFolderPath()
        {
            string rootPath = Directory.GetCurrentDirectory();


            return $@"{rootPath}/{DriversFolderName()}";
        }

        public string DriverPath()
        {
            return $@"{DriversFolderPath()}\{DriverName()}";
        }

    }
}
