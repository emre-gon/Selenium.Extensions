using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Selenium.Extensions
{
    public abstract class GenSlDriver<T> : SlDriver where T : RemoteWebDriver
    {
        protected GenSlDriver(ISet<string> DriverArguments, string ProfileName, bool Headless) : base(GetBrowserType(typeof(T)), DriverArguments, ProfileName, Headless)
        {
        }


        public T BaseDriver
        {
            get
            {
                return _lazyBaseDriver as T;
            }
        }

        protected override RemoteWebDriver getBaseDriver()
        {
            return CreateBaseDriver();
        }

        protected abstract T CreateBaseDriver();

        private static SlDriverBrowserType GetBrowserType(Type T)
        {
            if(T == typeof(ChromeDriver))
            {
                return SlDriverBrowserType.Chrome;
            }
            else if(T == typeof(FirefoxDriver))
            {
                return SlDriverBrowserType.Firefox;
            }
            else if (T == typeof(InternetExplorerDriver))
            {
                return SlDriverBrowserType.IE;
            }

            throw new Exception("Unsupported browser type: " + typeof(T));
        }

    }
}
