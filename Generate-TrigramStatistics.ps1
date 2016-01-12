function Main {
    Add-Type -Path (Join-Path (Get-ScriptDirectory) .\MlkPwgen.FrequencyCounter\bin\Debug\MlkPwgen.FrequencyCounter.dll)

    $wordlist = Get-Wordlist http://www.freescrabbledictionary.com/english-word-list/download/english.txt

    [MlkPwgen.TrigramFrequencyCounter]::Count($wordlist, '.\MlkPwgen\TrigramStatistics.json.gz')
}

function Get-Wordlist {
    param(
        [uri] $url
    )
    
    $filename = $url.Segments[-1]
    if (-not (Test-Path $filename)) {
        Invoke-WebRequest $url -OutFile $filename
    }
    $filename
}

function Get-ScriptDirectory {
    Split-Path $script:MyInvocation.MyCommand.Path
}

Main