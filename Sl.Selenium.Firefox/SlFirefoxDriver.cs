using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sl.Selenium.Firefox
{
    public class SlFirefoxDriver : SlDriver
    {
        private SlFirefoxDriver(string ProfileName, bool Headless)
            : base(SlDriverBrowserType.Firefox, ProfileName, Headless)
        {

        }



        public static SlDriver Instance(String ProfileName, bool Headless = false)
        {
            if (!_openDrivers.IsOpen(SlDriverBrowserType.Firefox, ProfileName))
            {
                SlFirefoxDriver ffDriver = new SlFirefoxDriver(ProfileName, Headless);

                _openDrivers.OpenDriver(ffDriver);
            }

            return _openDrivers.GetDriver(SlDriverBrowserType.Firefox, ProfileName);
        }



        #region os service
        public bool Use64bitDrivers = true;
        public override string DriversFolderName()
        {
            return Use64bitDrivers ? "DriversX64" : "DriversX86";
        }

        public override string DriverName()
        {
            var os = Sl.Extensions.Platform.CurrentOS();
            if (os == OSPlatform.Windows)
                return @"geckodriver.exe";
            else if (os == OSPlatform.OSX)
                return @"geckodriver_osx";
            else if (os == OSPlatform.Linux)
                return @"geckodriver_linux";
            else
                throw new Exception("Unknown OS");
        }
        #endregion


        private readonly static string[] ProcessNames = { "geckodriver", "firefox" };
        /// <summary>
        /// Use with caution. It will kill all running spiders
        /// </summary>
        public static void KillAllFirefoxProcesses()
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



        protected override RemoteWebDriver createBaseDriver()
        {
            FirefoxDriverService service = FirefoxDriverService
                .CreateDefaultService(DriversFolderPath(), DriverName());

            service.Host = "127.0.0.1";

            service.SuppressInitialDiagnosticInformation = true;

            FirefoxOptions options = new FirefoxOptions()
            {
                Profile = GetFirefoxProfile(this.ProfileName),
            };


            options.Profile.SetPreference("dom.webdriver.enabled", false);
            options.Profile.SetPreference("dom.navigator.webdriver", false);
            options.Profile.SetPreference("useAutomationExtension", false);

            if (this.Headless)
            {
                options.AddArguments("--headless");
                //TODO: disable images
            }

            try
            {
                var driver = new FirefoxDriver(service, options, new TimeSpan(0, 1, 0));
                return driver;
            }
            catch (Exception exc)
            {
                throw new DriverCreationException("Error creating driver. See inner exception for details: ", exc);
            }
        }



        private static FirefoxProfile GetProfileRaw(string ProfileName)
        {
            if (Sl.Extensions.Platform.CurrentOS() == OSPlatform.Windows)
            {
                return new FirefoxProfileManager().GetProfile(ProfileName);
            }

            var profileDirs = Directory.GetDirectories(UnixProfilesFolder);

            foreach (var dir in profileDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                if (!dirInfo.Name.Contains("."))
                {
                    //firefox profile folders should contain a dot
                    continue;
                }


                string name = string.Join(".", dirInfo.Name.Split('.').Skip(1));

                if (name == ProfileName)
                {
                    if(Sl.Extensions.Platform.CurrentOS() == OSPlatform.Linux)
                    {
                        var lockFile = dirInfo.GetFiles("lock").FirstOrDefault();

                        if(lockFile != null && lockFile.Exists)
                        {
                            lockFile.Delete();
                            Log.Logger.Information("Lock file deleted");
                        }
                    }
                    


                    Log.Logger.Information("Profile Folder: " + name);
                    return new FirefoxProfile(dir);
                }
            }

            return new FirefoxProfileManager().GetProfile(ProfileName);
        }


        public static FirefoxProfile GetFirefoxProfile(string ProfileName)
        {
            var profile = GetProfileRaw(ProfileName);

            if(profile == null)
            {
                profile = new FirefoxProfile();
            }

            if (GetAllowedMimeTypesForDownload().Any())
            {

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


                profile.SetPreference("browser.download.folderList", 2);
                //profile.SetPreference("browser.download.manager.showWhenStarting", false);
                profile.SetPreference("browser.download.dir", desktopPath);


                var str = string.Join(",", GetAllowedMimeTypesForDownload());
                profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", str);
            }

            profile.SetPreference("dom.webdriver.enabled", false);

            profile.SetPreference("devtools.jsonview.enabled", false);
            profile.SetPreference("useAutomationExtension", false);


            return profile;
        }

        private static string UnixProfilesFolder
        {
            get
            {
                var uProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (Sl.Extensions.Platform.CurrentOS() == OSPlatform.OSX)
                {
                    return $"{uProfile}/Library/Application Support/Firefox/Profiles/";
                }
                else if (Sl.Extensions.Platform.CurrentOS() == OSPlatform.Linux)
                {
                    return $"{uProfile}/.mozilla/firefox/";
                }
                else
                {
                    return "";
                }
            }
        }

        public static IList<string> GetInstalledFirefoxProfiles()
        {

            if (Sl.Extensions.Platform.CurrentOS() == OSPlatform.Windows)
            {
                var pManager = new FirefoxProfileManager();
                return pManager.ExistingProfiles;
            }
            else
            {
                Log.Logger.Information("Looking for Firefox Profiles Under: " + UnixProfilesFolder);
                var profileDirs = Directory.GetDirectories(UnixProfilesFolder);

                
                var toBeReturned = new List<string>();
                foreach (var dir in profileDirs)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);

                    if(!dirInfo.Name.Contains("."))
                    {
                        //firefox profile folders should contain a dot
                        continue;
                    }

                    string name = string.Join(".", dirInfo.Name.Split('.').Skip(1));
                    toBeReturned.Add(name);
                }

                return toBeReturned;
            }
        }
    }
}
