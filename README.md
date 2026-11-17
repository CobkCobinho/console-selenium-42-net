# Console Selenium 42 — .NET 10

**Autor:** Luiz Henrique Souza dos Santos. Trabalho realizado individualmente.

Atividade da Lista 42 de Gestão e Qualidade de Software, professor Daniel Henrique Matos de Paiva. A solução automatiza os três cenários solicitados no DuckDuckGo com C#, Selenium WebDriver e MSTest.

## Cenários implementados

| Exercício | Teste | Validação |
| --- | --- | --- |
| 1 — Pesquisa básica | `PesquisaBasica_DeveExibirResultadosDoTermoPesquisado` | Acessa `https://duckduckgo.com`, encontra `name="q"`, digita `Selenium WebDriver C#` com `SendKeys`, envia `Keys.Enter` e valida o termo na URL após o carregamento dos resultados. |
| 2 — Autocompletar | `Autocompletar_DevePesquisarATerceiraSugestao` | Digita `C# unit testing`, aguarda pelo menos três sugestões visíveis com `WebDriverWait`, clica na terceira e compara a busca com o texto selecionado. |
| 3 — Histórico | `Historico_DeveAbrirResultadoVoltarEAvancar` | Pesquisa `Automated Testing`, abre o primeiro resultado orgânico externo, valida a URL, executa `Back()`, valida a restauração dos resultados e também testa `Forward()`. |

## Organização

- `ConsoleSelenium42.sln`: solution com os dois projetos.
- `ConsoleSelenium/`: aplicação console .NET 10 com orientações de execução.
- `ConsoleSelenium.Tests/BuscaDesafioTest.cs`: os três métodos `[TestMethod]` do enunciado.
- `ConsoleSelenium.Tests/Pages/DuckDuckGoPage.cs`: seletores, ações e esperas explícitas.
- `ConsoleSelenium.Tests/Infrastructure/`: criação e encerramento do navegador e evidências de falhas.
- `.github/workflows/tests.yml`: compilação, descoberta dos testes e execução real no Chrome.

O projeto de testes referencia o projeto console e inclui `Selenium.WebDriver`, `Selenium.Support`, MSTest, seu adaptador e o SDK de testes. Cada cenário usa um navegador novo; o encerramento ocorre no `[TestCleanup]`. Não há esperas fixas com `Thread.Sleep`.

## Requisitos

- SDK do .NET 10.
- Google Chrome instalado.
- Acesso à internet para restaurar pacotes, resolver o ChromeDriver com Selenium Manager e acessar o DuckDuckGo e o resultado externo.

## Executar

Na raiz do repositório:

```sh
dotnet restore ConsoleSelenium42.sln
dotnet build ConsoleSelenium42.sln --no-restore
dotnet test ConsoleSelenium42.sln --no-build --logger "trx;LogFileName=selenium.trx" --results-directory TestResults
```

O padrão é Chrome sem janela (`headless`). Para acompanhar visualmente no PowerShell:

```powershell
$env:HEADLESS = "false"
dotnet test ConsoleSelenium42.sln
```

Para executar somente um cenário:

```sh
dotnet test ConsoleSelenium42.sln --filter "FullyQualifiedName~Autocompletar"
```

O console pode ser aberto com `dotnet run --project ConsoleSelenium`. Os testes são executados por `dotnet test`.

## Resultados e limitações

O workflow separa a compilação da execução E2E. O job `selenium` testa o site real, sem substituir sugestões por dados locais e sem ignorar falhas. Os relatórios TRX e, em caso de falha, capturas de tela e HTML ficam no artefato `resultados-selenium` da execução no GitHub Actions.

Como os cenários dependem de um site público, mudanças de layout, indisponibilidade, ausência de sugestões ou desafios antiautomação podem fazer os testes falharem. Nesse caso, consulte as evidências e execute novamente em uma conexão que permita o acesso normal ao site. A automação não contorna CAPTCHA.

O texto da terceira sugestão é capturado durante a execução, pois as sugestões podem mudar. A comparação usa o parâmetro `q` decodificado para preservar espaços e o caractere `#`. O teste de histórico utiliza a mesma aba para que os comandos Voltar e Avançar atuem sobre a navegação testada.

## Entrega

[Repositório público — console-selenium-42-net](https://github.com/CobkCobinho/console-selenium-42-net)

Desenvolvimento individual, com apoio de IA na implementação e revisão do código.
