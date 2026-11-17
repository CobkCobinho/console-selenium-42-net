using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests.Pages;

internal sealed class DuckDuckGoPage
{
    public const string BaseUrl = "https://duckduckgo.com";
    private static readonly By SearchInput = By.Name("q");
    private static readonly By Suggestions = By.CssSelector(
        "[role='listbox'] [role='option'], .search-suggestions-list li, " +
        ".sbox-suggestions li, [role='option']");
    private static readonly By OrganicLinks = By.CssSelector(
        "[data-testid='result'] a[data-testid='result-title-a'], " +
        "#links .result:not(.result--ad) a.result__a");

    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public DuckDuckGoPage(IWebDriver driver)
    {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
    }

    public void Open() => driver.Navigate().GoToUrl(BaseUrl);

    public void Search(string term)
    {
        var input = GetSearchInput();
        input.Clear();
        input.SendKeys(term);
        input.SendKeys(Keys.Enter);
        WaitForResults(term);
    }

    public string SelectThirdSuggestion(string term)
    {
        var input = GetSearchInput();
        input.Clear();
        input.SendKeys(term);
        return wait.Until(browser =>
        {
            var suggestions = browser.FindElements(Suggestions)
                .Where(element => element.Displayed && element.Enabled &&
                    !string.IsNullOrWhiteSpace(element.Text)).ToList();
            if (suggestions.Count < 3) return null;
            var third = suggestions[2];
            var selectedText = third.Text.Trim();
            third.Click();
            return selectedText;
        })!;
    }

    public void WaitForResults(string term)
    {
        wait.Until(browser => IsSearchPage(browser.Url, term));
        // A alteração da URL sozinha não comprova que os resultados carregaram.
        wait.Until(browser => browser.FindElements(OrganicLinks).Any(link => link.Displayed));
    }

    public Uri OpenFirstOrganicResult()
    {
        var link = wait.Until(browser => browser.FindElements(OrganicLinks)
            .FirstOrDefault(element => element.Displayed && element.Enabled &&
                Uri.TryCreate(element.GetAttribute("href"), UriKind.Absolute, out var uri) &&
                uri.Scheme is "https" or "http" && !IsDuckDuckGo(uri)));
        var href = link!.GetAttribute("href")
            ?? throw new InvalidOperationException("O resultado orgânico não possui href.");
        var destination = new Uri(href);
        // Mantém a mesma aba para exercitar o histórico.
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].removeAttribute('target');", link);
        link.Click();
        WaitForExternalPage();
        return destination;
    }

    public void WaitForExternalPage()
    {
        wait.Until(browser => Uri.TryCreate(browser.Url, UriKind.Absolute, out var uri) &&
            uri.Scheme is "https" or "http" && !IsDuckDuckGo(uri));
        wait.Until(browser => (string?)((IJavaScriptExecutor)browser)
            .ExecuteScript("return document.readyState") == "complete");
    }

    public static bool IsSearchPage(string url, string term) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) && IsDuckDuckGo(uri) &&
        string.Equals(GetQuery(url), term, StringComparison.OrdinalIgnoreCase);

    public static bool IsDuckDuckGo(Uri uri) =>
        uri.Host.Equals("duckduckgo.com", StringComparison.OrdinalIgnoreCase) ||
        uri.Host.EndsWith(".duckduckgo.com", StringComparison.OrdinalIgnoreCase);

    public static string? GetQuery(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        foreach (var parameter in uri.Query.TrimStart('?').Split('&'))
        {
            var pair = parameter.Split('=', 2);
            if (pair.Length == 2 && pair[0] == "q")
                return Uri.UnescapeDataString(pair[1].Replace("+", " "));
        }
        return null;
    }

    private IWebElement GetSearchInput() => wait.Until(browser =>
        browser.FindElements(SearchInput).FirstOrDefault(element => element.Displayed && element.Enabled))!;
}
