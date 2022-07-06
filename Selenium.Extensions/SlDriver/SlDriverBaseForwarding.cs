using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Selenium.Extensions
{
    public abstract partial class SlDriver : IWebDriver, ISearchContext, IDisposable, IJavaScriptExecutor, IFindsElement, ITakesScreenshot, ISupportsPrint, IActionExecutor, IAllowsFileDetection, IHasCapabilities, IHasCommandExecutor, IHasSessionId, ICustomDriverCommandExecutor
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



        public bool IsActionExecutor => _lazyBaseDriver.IsActionExecutor;

        public IFileDetector FileDetector { get => _lazyBaseDriver.FileDetector; set => _lazyBaseDriver.FileDetector = value; }

        public SessionId SessionId => _lazyBaseDriver.SessionId;

        public ICommandExecutor CommandExecutor => _lazyBaseDriver.CommandExecutor;

        public object ExecuteScript(PinnedScript script, params object[] args)
        {
            return _lazyBaseDriver.ExecuteScript(script, args);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return _lazyBaseDriver.ExecuteAsyncScript(script, args);
        }

        public void PerformActions(IList<ActionSequence> actionSequenceList)
        {
            _lazyBaseDriver.PerformActions(actionSequenceList);
        }

        public void ResetInputState()
        {
            _lazyBaseDriver.ResetInputState();
        }

        public Screenshot GetScreenshot()
        {
            return _lazyBaseDriver.GetScreenshot();
        }

        public IWebElement FindElement(string mechanism, string value)
        {
            return _lazyBaseDriver.FindElement(mechanism, value);
        }

        public ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value)
        {
            return _lazyBaseDriver.FindElements(mechanism, value);
        }

        public PrintDocument Print(PrintOptions options)
        {
            return _lazyBaseDriver.Print(options);
        }

        public object ExecuteCustomDriverCommand(string driverCommandToExecute, Dictionary<string, object> parameters)
        {
            return _lazyBaseDriver.ExecuteCustomDriverCommand(driverCommandToExecute, parameters);
        }

        public void RegisterCustomDriverCommands(IReadOnlyDictionary<string, CommandInfo> commands)
        {
            _lazyBaseDriver.RegisterCustomDriverCommands(commands);
        }

        public bool RegisterCustomDriverCommand(string commandName, CommandInfo commandInfo)
        {
            return _lazyBaseDriver.RegisterCustomDriverCommand(commandName, commandInfo);
        }
    }
}
