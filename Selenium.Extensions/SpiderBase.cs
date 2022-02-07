using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Extensions
{
    /// <summary>
    /// Selenium WebDriver Runner With Multiple User Profiles
    /// </summary>
    public abstract class SpiderBase : IDisposable
    {
        #region driver/profiles
        public IEnumerable<string> BrowserProfiles { get; }

        public string RootProfileName
        {
            get
            {
                return BrowserProfiles.First();
            }
        }

        #endregion


        protected abstract SlDriver Driver(string Profile);

        protected SlDriver Driver()
        {
            return Driver(this.RootProfileName);
        }

        #region constructor
        public ILogger Logger { get; }
        public SlDriverBrowserType BrowserType { get; }
        public bool Headless { get; }

        protected SpiderBase(SlDriverBrowserType BrowserType, ILogger Logger, string BrowserProfile, bool Headless)
            : this(BrowserType, Logger, new List<string>() { BrowserProfile }, Headless)
        {

        }

        protected SpiderBase(SlDriverBrowserType BrowserType, ILogger Logger, IEnumerable<string> BrowserProfiles, bool Headless)
        {
            if (BrowserProfiles == null || !BrowserProfiles.Any())
                throw new Exception("Enter at least 1 firefox profile");

            this.BrowserType = BrowserType;
            this.BrowserProfiles = BrowserProfiles;
            this.Logger = Logger;
            this.Headless = Headless;
        }
        #endregion

        #region report fields
        public double Progress { get; protected set; }

        public DateTime StartedAt { get; protected set; }

        public double RunningForSeconds
        {
            get
            {
                return (DateTime.Now - StartedAt).TotalSeconds;
            }
        }
        #endregion

        #region events
        public delegate Task ProgressChangedEvent(double NewValue);
        public event ProgressChangedEvent ProgressChanged;


        public delegate Task FinishedEvent();
        public event FinishedEvent Finished;


        protected void RaiseFinished()
        {
            Finished?.Invoke();
        }

        protected void RaiseProgressChanged(double NewValue)
        {
            ProgressChanged?.Invoke(NewValue);
        }
        #endregion

        #region destructor
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SpiderBase()
        {
            Dispose(false);
        }

        private bool isDisposed;
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;


            foreach (var profile in this.BrowserProfiles)
                SlDriver.Dispose(this.BrowserType, profile);

            isDisposed = true;
        }
        #endregion
    }
}
