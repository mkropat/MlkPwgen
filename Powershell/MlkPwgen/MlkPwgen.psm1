function New-Password {
    param(
        [int] $Length = 10,
        [switch] $Lower,
        [switch] $Upper,
        [switch] $Digits,
        [switch] $Symbols,
        [string[]] $RequiredSets,
        [scriptblock] $Predicate
    )
    
    if ($Lower) {
        $RequiredSets += [MlkPwgen.Sets]::Lower
    }
    if ($Upper) {
        $RequiredSets += [MlkPwgen.Sets]::Upper
    }
    if ($Digits) {
        $RequiredSets += [MlkPwgen.Sets]::Digits
    }
    if ($Symbols) {
        $RequiredSets += [MlkPwgen.Sets]::Symbols
    }

    if (-not $RequiredSets) {
        $RequiredSets = @(
            [MlkPwgen.Sets]::Lower,
            [MlkPwgen.Sets]::Upper,
            [MlkPwgen.Sets]::Digits
        )
    }

    if ($Predicate) {
        [MlkPwgen.PasswordGenerator]::GenerateComplex($Length, $RequiredSets, {
            param([string]$password)
            $password | foreach $Predicate
        })
    }
    else {
        [MlkPwgen.PasswordGenerator]::GenerateComplex($Length, $RequiredSets)
    }
}

Export-ModuleMember -Function 'New-Password'

function New-PronounceablePassword {
    param(
        [int] $Length = 12,
        [switch] $Digits,
        [switch] $Symbols,
        [string[]] $RequiredSets = @(),
        [scriptblock] $Predicate = $null
    )

    if ($Digits) {
        $RequiredSets += [MlkPwgen.Sets]::Digits
    }
    if ($Symbols) {
        $RequiredSets += [MlkPwgen.Sets]::Symbols
    }

    if ($Predicate) {
        [MlkPwgen.PronounceableGenerator]::Generate($Length, $RequiredSets, {
            param([string]$password)
            $password | foreach $Predicate
        })
    }
    else {
        [MlkPwgen.PronounceableGenerator]::Generate($Length, $RequiredSets)
    }
}

Export-ModuleMember -Function 'New-PronounceablePassword'
