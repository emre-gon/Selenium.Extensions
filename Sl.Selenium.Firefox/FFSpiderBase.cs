using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.Selenium.Firefox
{
    public abstract class FFSpiderBase : SpiderBase
    {
        protected FFSpiderBase(ILogger Logger, string BrowserProfile, bool Headless)
            : base(SlDriverBrowserType.Firefox, Logger, new List<string>() { BrowserProfile }, Headless)
        {

        }

        protected FFSpiderBase(ILogger Logger, IEnumerable<string> BrowserProfiles, bool Headless)
        : base(SlDriverBrowserType.Firefox, Logger, BrowserProfiles, Headless)
        {
        }


        protected override SlDriver Driver(string Profile)
        {
            return SlFirefoxDriver.Instance(Profile, this.Headless);
        }

    }
}
