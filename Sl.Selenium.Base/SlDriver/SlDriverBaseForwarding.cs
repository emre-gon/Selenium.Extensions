using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Sl.Selenium
{
    public abstract partial class SlDriver : IWebDriver, IHasInputDevices, IJavaScriptExecutor, IHasCapabilities, IActionExecutor
    {
        #region base props
        public string Url { get => _lazyBaseDriver.Url; set => _lazyBaseDriver.Url = value; }
        public string Title => _lazyBaseDriver.Title;
        public string PageSource => _lazyBaseDriver.PageSource;
        public string CurrentWindowHandle => _lazyBaseDriver.CurrentWindowHandle;
        public ReadOnlyCollection<string> WindowHandles => _lazyBaseDriver.WindowHandles;

        #endregion

        public IWebElement FindElement(By by)
        {
            return _lazyBaseDriver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _lazyBaseDriver.FindElements(by);
        }

        public IOptions Manage()
        {
            return _lazyBaseDriver.Manage();
        }

        public INavigation Navigate()
        {
            return _lazyBaseDriver.Navigate();
        }


        public ITargetLocator SwitchTo()
        {
            return _lazyBaseDriver.SwitchTo();
        }

        public ICapabilities Capabilities
        {
            get
            {
                return _lazyBaseDriver.Capabilities;
            }
        }

        public IKeyboard Keyboard => ((IHasInputDevices)_lazyBaseDriver).Keyboard;

        public IMouse Mouse => ((IHasInputDevices)_lazyBaseDriver).Mouse;

        public bool IsActionExecutor => ((IActionExecutor)_lazyBaseDriver).IsActionExecutor;



        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor)_lazyBaseDriver).ExecuteAsyncScript(script, args);
        }

        public void PerformActions(IList<ActionSequence> actionSequenceList)
        {
            ((IActionExecutor)_lazyBaseDriver).PerformActions(actionSequenceList);
        }

        public void ResetInputState()
        {
            ((IActionExecutor)_lazyBaseDriver).ResetInputState();
        }
    }
}
