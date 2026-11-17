using OpenQA.Selenium;

namespace ConsoleSelenium.Tests.Infrastructure;

public abstract class BrowserTestBase
{
    protected IWebDriver Driver { get; private set; } = null!;
    public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void OpenBrowser() => Driver = BrowserFactory.Create();

    [TestCleanup]
    public void CloseBrowser()
    {
        if (Driver is null) return;
        try
        {
            if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
                SaveFailureEvidence();
        }
        finally
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }

    private void SaveFailureEvidence()
    {
        try
        {
            var directory = TestContext.TestResultsDirectory ?? Path.GetTempPath();
            Directory.CreateDirectory(directory);
            var prefix = Path.Combine(directory, $"{TestContext.TestName}-{Guid.NewGuid():N}");
            var screenshot = prefix + ".png";
            ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(screenshot);
            TestContext.AddResultFile(screenshot);
            var html = prefix + ".html";
            File.WriteAllText(html, Driver.PageSource);
            TestContext.AddResultFile(html);
            TestContext.WriteLine($"URL no momento da falha: {Driver.Url}");
        }
        catch (Exception error)
        {
            // Não substitui a falha original por uma falha de coleta.
            TestContext.WriteLine($"Não foi possível coletar evidência: {error.Message}");
        }
    }
}
