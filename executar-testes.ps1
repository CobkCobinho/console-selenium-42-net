[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$previousHeadless = $env:HEADLESS
$previousCaptcha = $env:CAPTCHA_MANUAL
Push-Location -LiteralPath $PSScriptRoot
try {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw 'Instale o SDK do .NET 10 antes de executar.'
    }
    $env:HEADLESS = 'false'
    $env:CAPTCHA_MANUAL = 'true'
    Write-Host 'Os tres cenarios serao executados no DuckDuckGo real.'
    Write-Host 'Se aparecer CAPTCHA, confirme manualmente no Chrome. O teste aguardara ate 3 minutos.'
    dotnet restore ConsoleSelenium42.sln
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao restaurar dependencias.' }
    dotnet build ConsoleSelenium42.sln --no-restore --configuration Release
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar a solution.' }
    dotnet test ConsoleSelenium42.sln --no-build --configuration Release --logger 'trx;LogFileName=selenium.trx' --results-directory TestResults
    if ($LASTEXITCODE -ne 0) { throw 'A validacao funcional falhou. Consulte TestResults/selenium.trx.' }
    Write-Host 'Os tres cenarios foram aprovados. Relatorio: TestResults/selenium.trx'
}
finally {
    $env:HEADLESS = $previousHeadless
    $env:CAPTCHA_MANUAL = $previousCaptcha
    Pop-Location
}
