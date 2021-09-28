using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Sl.Selenium.IE
{
    public class SlIEDriver : SlDriver
    {
        private const string Default_Profile_Name = "Default";
        private SlIEDriver()
            : base(SlDriverBrowserType.IE, Default_Profile_Name, false)
        {

        }


        public static SlDriver Instance()
        {
            if (!_openDrivers.IsOpen(SlDriverBrowserType.IE, Default_Profile_Name))
            {
                SlIEDriver cDriver = new SlIEDriver();

                _openDrivers.OpenDriver(cDriver);
            }

            return _openDrivers.GetDriver(SlDriverBrowserType.IE, Default_Profile_Name);
        }



        public override string DriversFolderName()
        {
            return "Drivers";
        }

        public override string DriverName()
        {
            return "IEDriverServer.exe";
        }

        protected override RemoteWebDriver createBaseDriver()
        {
            InternetExplorerDriverService service = InternetExplorerDriverService.CreateDefaultService(DriversFolderPath(), DriverName());


            service.HostName = "127.0.0.1";

            service.SuppressInitialDiagnosticInformation = true;



            var driver = new InternetExplorerDriver(service);

            return driver;


        }



    }
}
