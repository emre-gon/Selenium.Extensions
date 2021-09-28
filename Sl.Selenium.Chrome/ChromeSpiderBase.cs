using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.Selenium.Chrome
{
    public abstract class ChromeSpiderBase : SpiderBase
    {
        protected ChromeSpiderBase(ILogger Logger, string BrowserProfile, bool Headless)
            : base(SlDriverBrowserType.Chrome, Logger, new List<string>() { BrowserProfile }, Headless)
        {

        }

        protected ChromeSpiderBase(ILogger Logger, IEnumerable<string> BrowserProfiles, bool Headless)
        : base(SlDriverBrowserType.Chrome, Logger, BrowserProfiles, Headless)
        {
        }


        protected override SlDriver Driver(string Profile)
        {
            return SlChromeDriver.Instance(Profile, this.Headless);
        }

    }
}
