using ConsoleSelenium.Tests.Infrastructure;
using ConsoleSelenium.Tests.Pages;

namespace ConsoleSelenium.Tests;

[TestClass]
[DoNotParallelize]
[TestCategory("E2E")]
public sealed class BuscaDesafioTest : BrowserTestBase
{
    [TestMethod]
    public void PesquisaBasica_DeveExibirResultadosDoTermoPesquisado()
    {
        const string term = "Selenium WebDriver C#";
        var page = new DuckDuckGoPage(Driver);
        page.Open();
        page.Search(term);

        Assert.IsTrue(DuckDuckGoPage.IsSearchPage(Driver.Url, term),
            $"A URL deve conter o termo completo, incluindo C#. URL: {Driver.Url}");
    }

    [TestMethod]
    public void Autocompletar_DevePesquisarATerceiraSugestao()
    {
        var page = new DuckDuckGoPage(Driver);
        page.Open();
        var selectedSuggestion = page.SelectThirdSuggestion("C# unit testing");
        TestContext.WriteLine($"Terceira sugestão selecionada: {selectedSuggestion}");
        page.WaitForResults(selectedSuggestion);

        Assert.AreEqual(selectedSuggestion.ToLowerInvariant(),
            DuckDuckGoPage.GetQuery(Driver.Url)?.ToLowerInvariant(),
            "A busca deve corresponder ao texto da terceira sugestão clicada.");
    }

    [TestMethod]
    public void Historico_DeveAbrirResultadoVoltarEAvancar()
    {
        const string term = "Automated Testing";
        var page = new DuckDuckGoPage(Driver);
        page.Open();
        page.Search(term);
        var resultsUrl = Driver.Url;
        var destination = page.OpenFirstOrganicResult();
        var externalUrl = Driver.Url;

        Assert.IsFalse(DuckDuckGoPage.IsDuckDuckGo(new Uri(externalUrl)),
            $"O primeiro resultado deve levar a uma página externa. Destino: {destination}");

        Driver.Navigate().Back();
        page.WaitForResults(term);
        Assert.AreEqual(resultsUrl, Driver.Url, "Voltar deve restaurar a página de resultados.");

        // O objetivo também menciona Avançar, além dos passos obrigatórios de Voltar.
        Driver.Navigate().Forward();
        page.WaitForExternalPage();
        Assert.AreEqual(externalUrl, Driver.Url, "Avançar deve restaurar a página externa.");
    }
}
