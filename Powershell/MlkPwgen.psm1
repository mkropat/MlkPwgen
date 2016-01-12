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
        $RequiredSets += [MlkPwgen.Classes]::Lower
    }
    if ($Upper) {
        $RequiredSets += [MlkPwgen.Classes]::Upper
    }
    if ($Digits) {
        $RequiredSets += [MlkPwgen.Classes]::Digits
    }
    if ($Symbols) {
        $RequiredSets += [MlkPwgen.Classes]::Symbols
    }

    if (-not $RequiredSets) {
        $RequiredSets = @(
            [MlkPwgen.Classes]::Lower,
            [MlkPwgen.Classes]::Upper,
            [MlkPwgen.Classes]::Digits
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
        [int] $Length = 10,
        [switch] $Digits,
        [switch] $Symbols,
        [string[]] $RequiredSets,
        [scriptblock] $Predicate
    )

    if ($Digits) {
        $RequiredSets += [MlkPwgen.Classes]::Digits
    }
    if ($Symbols) {
        $RequiredSets += [MlkPwgen.Classes]::Symbols
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