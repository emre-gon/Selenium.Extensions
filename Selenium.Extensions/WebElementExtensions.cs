using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Extensions
{
    public static class WebElementExtensions
    {
        public static void TypeKeys(this IWebElement input, String Text)
        {
            foreach (var c in Text)
            {
                var now = DateTime.Now;
                input.SendKeys(c.ToString());
                Task.Delay(110).Wait();
            }
        }
        public static void TypeKeys(this IWebElement input, String Text, int DelayMs)
        {
            foreach (var c in Text)
            {
                var now = DateTime.Now;
                input.SendKeys(c.ToString());
                Task.Delay(DelayMs).Wait();
            }
        }

        #region get element helpers
        public static IWebElement GetElementFast(this IWebElement Parent, String CssSelector)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    return Parent.FindElement(By.CssSelector(CssSelector));
                }
                catch (NoSuchElementException exc)
                {
                    return null;
                }
                catch (Exception exc)
                {
                    Thread.Sleep(100);
                }
            }
            return null;
        }

        public static IWebElement GetElementFastXP(this IWebElement Parent, String XPSelector)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    return Parent.FindElement(By.XPath(XPSelector));
                }
                catch (NoSuchElementException exc)
                {
                    return null;
                }
                catch (Exception exc)
                {
                    Thread.Sleep(100);
                }
            }
            return null;
        }

        public static IWebElement GetParentElement(this IWebElement Element)
        {
            return Element.FindElement(By.XPath(".."));
        }
        #endregion
        #region get value helpers
        public static String GetTextOf(this IWebElement Parent, String CssSelector)
        {
            return Parent.GetValueOf(CssSelector, "text");
        }

        public static String GetValueOf(this IWebElement Parent, String CssSelector, String Attribute)
        {
            try
            {
                var elem = Parent.GetElementFast(CssSelector);
                if (elem == null)
                    return null;
                else if (Attribute == "text")
                    return elem.Text;
                else
                    return elem.GetAttribute(Attribute);
            }
            catch (NoSuchElementException exc)
            {
                return null;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        #endregion




        public static void ScrollTopToBottom(this IWebElement element)
        {
            element.SendKeys(Keys.Home);
            Task.Delay(110).Wait();
            element.SendKeys(Keys.End);
        }
    }
}
