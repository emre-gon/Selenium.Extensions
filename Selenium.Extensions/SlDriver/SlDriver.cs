using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Selenium.Extensions
{
    public abstract partial class SlDriver 
    {

        #region static constructor
        protected static DriverDictionary _openDrivers;

        static SlDriver()
        {
            _openDrivers = new DriverDictionary();
        }
        #endregion

        #region base driver
        protected WebDriver _baseDriver;

        private static object createBaseDriverLock = new object();
        protected WebDriver _lazyBaseDriver
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

                        DownloadDriver();
                        _baseDriver = getBaseDriver();
                    }
                }

                return _baseDriver;
            }
        }

        protected abstract WebDriver getBaseDriver();


        #endregion

        #region constructor / Factory
        public readonly string ProfileName;
        public readonly bool Headless;

        public readonly ISet<string> DriverArguments;
        public readonly SlDriverBrowserType BrowserType;
        protected SlDriver(SlDriverBrowserType BrowserType, ISet<string> DriverArguments, string ProfileName, bool Headless)
        {
            this.BrowserType = BrowserType;
            this.ProfileName = ProfileName;
            this.Headless = Headless;
            this.DriverArguments = DriverArguments;
        }
        #endregion



        public abstract string DriverName();
        public abstract string DriversFolderName();

        public void DownloadDriver()
        {            
            if (!File.Exists(DriverPath()))
            {
                DownloadLatestDriver();
            }
        }

        protected abstract void DownloadLatestDriver();


        public string DriversFolderPath()
        {
            string rootPath = Directory.GetCurrentDirectory();


            return $@"{rootPath}/{DriversFolderName()}";
        }

        public string DriverPath()
        {
            return $@"{DriversFolderPath()}/{DriverName()}";
        }

    }
}
