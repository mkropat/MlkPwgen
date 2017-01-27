function Main {
    $dllPath = Get-ChildItem -Recurse -Include 'MlkPwgen.FrequencyCounter.dll' | select -First 1 -ExpandProperty FullName
    Add-Type -Path $dllPath

    $wordlist = Get-Wordlist http://www.freescrabbledictionary.com/english-word-list/download/english.txt

    [MlkPwgen.FrequencyCounter.TrigramFrequencyCounter]::Count($wordlist, '.\MlkPwgen\TrigramStatistics.json.gz')
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