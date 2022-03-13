# Selenium.Extensions


[![Status](https://img.shields.io/badge/status-active-success.svg)]()
[![License](https://img.shields.io/github/license/emre-gon/Selenium.Extensions)](/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Sl.Selenium.Extensions.svg)](https://www.nuget.org/packages/Sl.Selenium.Extensions)

[![NuGet](https://img.shields.io/nuget/v/Sl.ParallelRunner.svg?label=Sl.ParallelRunner)](https://www.nuget.org/packages/Sl.ParallelRunner)

---

## Web Element Extensions

Useful extension methods for getting parent/child elements using Css Query or XPath.

### Example Usage

```cs
using (var driver = FirefoxDriver.Instance("profile_name"))
{
    driver.GoTo("https://example.com");

    var element = driver.GetElementFast("div");

    var childElement = element.GetElementFast("a");

    var parent = childElement.GetParentElement();

    parent.ScrollTopToBottom();
}
```

## SlDriver

WebDriver capable of running with multiple browser profiles.

### Drivers Extending SlDriver: 

Chrome & Firefox: https://github.com/emre-gon/Sl.Selenium.Extensions.Drivers

Undetected Chrome: https://github.com/emre-gon/Selenium.WebDriver.UndetectedChromeDriver



## Sl.ParallelRunner

Useful for consuming queues in a multi threaded manner.