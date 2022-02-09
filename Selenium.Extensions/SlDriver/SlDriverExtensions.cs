using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Selenium.Extensions
{
    public abstract partial class SlDriver
    {

        #region dispose

        //close, quit and dispose does the same thing actually

        public void Close()
        {
            if (_baseDriver != null)
            {
                _baseDriver.Close();

                ///toString returns null when browser is totally closed
                if (_baseDriver.ToString() == null)
                {
                    Quit();
                }
            }
        }

        public void Quit()
        {
            if (_baseDriver != null)
            {
                _baseDriver.Quit();
                _baseDriver = null;
                Wait(6);
            }

            _openDrivers.CloseDriver(this);
        }
        public void Dispose()
        {
            Quit();
        }

        public static void Dispose(SlDriverBrowserType browserType, string profile)
        {
            if (_openDrivers.IsOpen(browserType, profile))
            {
                var driver = _openDrivers.GetDriver(browserType, profile);
                driver.Dispose();
            }
        }


        #endregion
        #region refresh
        public void Refresh()
        {
            _baseDriver?.Navigate()?.Refresh();
        }

        public void QuitRefresh()
        {
            string currentUrl = _baseDriver?.Url;
            Quit();
            if (currentUrl != null)
                GoTo(currentUrl);
        }

        #endregion

        #region goto/wait
        private static Random rand = new Random((int)DateTime.Now.Ticks);
        public void RandomWait(int min, int max)
        {
            RandomWaitMiliseconds(min * 1000, max * 1000);
        }
        public void RandomWaitMiliseconds(int min, int max)
        {
            if (min < 0 || max <= 0 || max < min)
                return;

            int randWait;
            if (min != max)
                randWait = rand.Next(min, max);
            else
                randWait = min;

            WaitMiliseconds(randWait);
        }

        public void Wait(int seconds)
        {
            WaitMiliseconds(seconds * 1000);
        }

        public void WaitMiliseconds(int Miliseconds)
        {
            Thread.Sleep(Miliseconds);
        }

        public virtual void GoTo(string URL)
        {
            var isSuccess = false;
            Exception innerException = null;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    this.Url = URL;
                    RandomWait(3, 5);
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    innerException = e;
                    isSuccess = false;
                }

                if (isSuccess)
                    break;
            }

            if (!isSuccess)
                throw new Exception($"Could not go to {URL}. Internet down?", innerException);
        }

        public void WaitUntilVisible(string CssSelector, int MaxWaitSecond = 30)
        {
            new WebDriverWait(this, new TimeSpan(0, 0, MaxWaitSecond))
                    .Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(CssSelector)));
        }

        #endregion
        #region get element/child element
        public IWebElement GetElement(String CssSelector)
        {
            try
            {
                var wait = new WebDriverWait(_lazyBaseDriver, new TimeSpan(0, 0, 10));
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                return wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(CssSelector)));

            }
            catch (Exception exc)
            {
                return null;
            }
        }


        public IWebElement GetElementFast(String CssSelector)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    return FindElement(By.CssSelector(CssSelector));
                }
                catch (NoSuchElementException exc)
                {
                    return null;
                }
                catch (Exception exc)
                {
                    this.WaitMiliseconds(100);
                }
            }
            return null;
        }
        #endregion

        #region get value of element
        public string GetTextOf(String CssSelector)
        {
            return GetValueOf(CssSelector, "text");
        }
        public String GetValueOf(String CssSelector, String Attribute)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    IWebElement elem = _lazyBaseDriver.FindElement(By.CssSelector(CssSelector));
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
                    this.WaitMiliseconds(100);
                }
            }
            return null;
        }
        #endregion

        #region move cursor/click
        public object ExecuteScript(string Script, params object[] args)
        {
            var js = (IJavaScriptExecutor)_lazyBaseDriver;
            return js.ExecuteScript(Script, args);
        }

        public void MoveCursorTo(IWebElement Element)
        {
            Actions builder = new Actions(this);
            builder.MoveToElement(Element).Perform();
        }


        public void ScrollToAndMoveCursorTo(IWebElement Element, int WaitForSeconds = 0)
        {
            ExecuteScript("arguments[0].scrollIntoView(false);", Element);
            if (WaitForSeconds > 0)
                RandomWait(WaitForSeconds, WaitForSeconds + 1);

            Actions builder = new Actions(this);
            builder.MoveToElement(Element).Perform();
        }

        public void ScrollMoveAndClick(IWebElement Element, int WaitForSeconds = 0)
        {
            ScrollToAndMoveCursorTo(Element, WaitForSeconds);
            Element.Click();
        }
        #endregion


        #region mouse wheel on element

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="DeltaY">Positive -> Wheel Down, Negative -> Wheel Up</param>
        public void MouseWheel(IWebElement Element, int DeltaY)
        {
            ScrollToAndMoveCursorTo(Element);

            var script = @"
                var element = arguments[0];
                var deltaY = arguments[1];
                var box = element.getBoundingClientRect();
                var clientX = box.left + (arguments[2] || box.width / 2);
                var clientY = box.top + (arguments[3] || box.height / 2);
                var target = element.ownerDocument.elementFromPoint(clientX, clientY);

                for (var e = target; e; e = e.parentElement) {
                  if (e === element) {
                    target.dispatchEvent(new MouseEvent('mouseover', {view: window, bubbles: true, cancelable: true, clientX: clientX, clientY: clientY}));
                    target.dispatchEvent(new MouseEvent('mousemove', {view: window, bubbles: true, cancelable: true, clientX: clientX, clientY: clientY}));
                    target.dispatchEvent(new WheelEvent('wheel',     {view: window, bubbles: true, cancelable: true, clientX: clientX, clientY: clientY, deltaY: deltaY}));
                    return;
                  }
                }    
                return 'Element is not interactable';
                ";


            int div = Math.Abs(DeltaY) / 100;
            var realDelta = DeltaY < 0 ? -100 : 100;

            for (int i = 0; i < div + 1; i++)
            {
                var scriptResult = ExecuteScript(script, Element, realDelta, 0, 0);
                if (scriptResult != null)
                {
                    throw new WebDriverException(scriptResult.ToString());
                }

                this.WaitMiliseconds(100);
            }
        }



        #endregion

        public void ScrollTopToBottom()
        {
            for (int i = 5000; i > 0; i = i - 300)
            {
                ExecuteScript("window.scrollTo(0, document.body.scrollHeight - " + i + ");");
                RandomWaitMiliseconds(20, 30);
            }
        }
    }
}
