using Sl.Selenium.Chrome;
using Sl.Selenium.IE;
using System;

namespace Sl.Selenium.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            

            using(var driver = SlIEDriver.Instance())
            {

                driver.GoTo("https://www.investing.com/currencies/usd-try-historical-data");

                driver.Wait(100);


            }
        }
    }
}
