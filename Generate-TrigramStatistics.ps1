function Main {
    $dllPath = Get-ChildItem -Recurse -Include 'MlkPwgen.FrequencyCounter.dll' | Select-Object -First 1 -ExpandProperty FullName
    Add-Type -Path $dllPath

    $wordlist = Resolve-Path (Get-Wordlist http://www.freescrabbledictionary.com/english-word-list/download/english.txt)
    $statsOutput = Join-Path $PWD '.\MlkPwgen\TrigramStatistics.json.gz'

    [MlkPwgen.FrequencyCounter.TrigramFrequencyCounter]::Count($wordlist, $statsOutput)
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
