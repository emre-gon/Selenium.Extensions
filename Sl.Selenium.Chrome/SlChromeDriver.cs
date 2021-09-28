using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Serilog;
using Sl.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sl.Selenium.Chrome
{
    public class SlChromeDriver : SlDriver
    {
        private SlChromeDriver(string ProfileName, bool Headless)
            : base(SlDriverBrowserType.Chrome, ProfileName, Headless)
        {

        }


        public static SlDriver Instance(String ProfileName, bool Headless = false)
        {
            if (!_openDrivers.IsOpen(SlDriverBrowserType.Chrome, ProfileName))
            {
                SlChromeDriver cDriver = new SlChromeDriver(ProfileName, Headless);

                _openDrivers.OpenDriver(cDriver);
            }

            return _openDrivers.GetDriver(SlDriverBrowserType.Chrome, ProfileName);
        }


        private readonly static string[] ProcessNames = { "chrome" };
        /// <summary>
        /// Use with caution. It will kill all running spiders
        /// </summary>
        public static void KillAllChromeProcesses()
        {
            foreach (var name in ProcessNames)
            {
                foreach (var process in Process.GetProcessesByName(name))
                {
                    Sl.Extensions.Platform.SafeKill(process, Extensions.Platform.SafeKillType.UserProcesses);
                }
            }

            SlDriver.ClearDrivers(SlDriverBrowserType.Firefox);
        }

        public override string DriversFolderName()
        {
            return "Drivers";
        }

        public override string DriverName()
        {
            var os = Sl.Extensions.Platform.CurrentOS();
            if (os == OSPlatform.Windows)
                return @"chromedriver.exe";
            else if (os == OSPlatform.OSX)
                return @"chromedriver_mac";
            else if (os == OSPlatform.Linux)
                return @"chromedriver_linux";
            else
                throw new Exception("Unknown OS");
        }

        protected override RemoteWebDriver createBaseDriver()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(DriversFolderPath(), DriverName());

            
            service.HostName = "127.0.0.1";

            service.SuppressInitialDiagnosticInformation = true;


            ChromeOptions options = new ChromeOptions();


            options.AddArgument("disable-infobars");

            if (this.Headless)
            {
                options.AddArguments("headless");
                //TODO: disable images
            }


            #region cloudflare bypass

            options.AddArgument("--disable-blink-features");
            //options.AddArgument("--incognito");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddExcludedArguments(new List<string>() { "enable-automation" });
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddArguments("disable-infobars");

            options.AddArguments("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");

            #endregion



            string chromeProfilesFolder = null;
            string chromeProfileName = this.ProfileName;
            if (this.ProfileName.Contains("/") || this.ProfileName.Contains("\\"))
            {
                int lastIndex;
                if(this.ProfileName.Contains("/"))
                {
                    lastIndex = this.ProfileName.LastIndexOf("/");
                }
                else
                {
                    lastIndex = this.ProfileName.LastIndexOf("\\");                    
                }

                chromeProfilesFolder = this.ProfileName.Substring(0, lastIndex);

                chromeProfileName = this.ProfileName.Substring(lastIndex + 1);


            }
            else
            {
                var profiles = GetInstalledChromeProfiles();

                foreach (var p in profiles)
                {
                    if (p.FriendlyName == this.ProfileName)
                    {
                        chromeProfilesFolder = p.FullFolderPath;
                        chromeProfileName = p.ActualName;
                        break;
                    }
                    else if (p.ActualName == this.ProfileName)
                    {
                        chromeProfilesFolder = p.FullFolderPath;
                        chromeProfileName = p.ActualName;
                        break;
                    }
                    else if (p.FullFolderPath == this.ProfileName)
                    {
                        chromeProfilesFolder = p.FullFolderPath;
                        chromeProfileName = p.ActualName;
                        break;
                    }
                }

                if(chromeProfilesFolder == null)
                {
                    chromeProfilesFolder = UnixProfilesFolder + this.ProfileName;
                }
            }



            options.AddArgument($@"user-data-dir={UnixProfilesFolder}");
            options.AddArgument($@"profile-directory={chromeProfileName}");


            var driver = new ChromeDriver(service, options);

            return driver;


        }



        private static string UnixProfilesFolder
        {
            get
            {
                if(Sl.Extensions.Platform.CurrentOS() == OSPlatform.Windows)
                {
                    var uProfile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    return $@"{uProfile}\Google\Chrome\User Data\";
                }
                else if (Sl.Extensions.Platform.CurrentOS() == OSPlatform.OSX)
                {
                    var uProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    return $"{uProfile}/Library/Application Support/Google/Chrome/User Data/";
                }
                else if (Sl.Extensions.Platform.CurrentOS() == OSPlatform.Linux)
                {
                    var uProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    return $"{uProfile}/.config/google-chrome/";
                }
                else
                {
                    return "";
                }
            }
        }



        public static IList<ChromeProfile> GetInstalledChromeProfiles()
        {
            Log.Logger.Information("Looking for Chrome Profiles Under: " + UnixProfilesFolder);

            var toBeReturned = new List<ChromeProfile>();
            if (!Directory.Exists(UnixProfilesFolder))
            {
                return toBeReturned;
            }

            var allDirs = Directory.GetDirectories(UnixProfilesFolder);


            foreach(var dir in allDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                var prefFile = dirInfo.GetFiles("Preferences", SearchOption.TopDirectoryOnly)
                    .Where(f => f.Name == "Preferences").FirstOrDefault();

                if(prefFile == null)
                {
                    continue;
                }

                var allText = File.ReadAllText(prefFile.FullName);

                try
                {
                    var model = allText.ParseJson<ChromePreferencesModel>();

                    toBeReturned.Add(new ChromeProfile(model.profile.name, dirInfo.Name, dirInfo.FullName));

                }
                catch(Exception exc)
                {
                    continue;
                }
            }

            return toBeReturned;
        }


        public class ChromePreferencesModel
        {
            public ChromeProfileModel profile { get; set; }
        }

        public class ChromeProfileModel
        {
            public string name { get; set; }
        }
    }


    public class ChromeProfile
    {
        public ChromeProfile(string FriendlyName, string ActualName, string FullFolderPath)
        {
            this.FriendlyName = FriendlyName;
            this.ActualName = ActualName;
            this.FullFolderPath = FullFolderPath;
        }
        public string FriendlyName { get; }

        public string ActualName { get; }
        public string FullFolderPath { get; }
    }
}
