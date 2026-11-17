using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleSelenium.Tests.Infrastructure;

internal static class BrowserFactory
{
    public static IWebDriver Create()
    {
        var options = new ChromeOptions();
        if (!string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "false",
                StringComparison.OrdinalIgnoreCase))
            options.AddArgument("--headless=new");

        options.AddArgument("--window-size=1440,1000");
        options.AddArgument("--disable-dev-shm-usage");
        // Selenium Manager resolve o driver compatível com o Chrome.
        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
        return driver;
    }
}
