# Console Selenium 42 — .NET 10


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

## Executar no Windows com Chrome visível

Na raiz do repositório, execute no PowerShell:

```powershell
.\executar-testes.ps1
```

O script restaura os pacotes, compila e executa os três cenários reais. Caso o DuckDuckGo apresente CAPTCHA, há até três minutos para a pessoa confirmar o desafio no Chrome. Depois disso, a automação continua e todas as asserções originais precisam passar. Essa opção não resolve CAPTCHA automaticamente e não considera a confirmação humana como aprovação do teste.

No modo não interativo, a presença de CAPTCHA encerra o teste com uma mensagem explícita de falha. A validação completa só pode ser afirmada quando os três cenários chegarem ao fim com sucesso.

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

## Validação concluída

Os três cenários foram aprovados na execução local no Windows com Chrome visível em **16/11/2026**. O relatório MSTest `TestResults/selenium.trx` foi conferido: **3 executados, 3 aprovados, 0 falhas e 0 ignorados**. O terminal registrou duração de **39,6 segundos**.

| Cenário | Resultado |
| --- | --- |
| Pesquisa básica por `Selenium WebDriver C#` | Aprovado |
| Clique na terceira sugestão de `C# unit testing` | Aprovado |
| Resultado externo, Voltar e Avançar | Aprovado |

[Detalhes da validação local](docs/validacao-local.md).

A execução local aprovada é distinta das tentativas anteriores no GitHub Actions, que encontraram CAPTCHA ou páginas vazias. Esses resultados anteriores não foram tratados como aprovação. O modo local preserva todas as asserções e permite confirmação humana quando o site a exige.


Desenvolvimento individual, com apoio de IA na implementação e revisão do código.
