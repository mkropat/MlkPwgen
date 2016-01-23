# MlkPwgen

*Secure random password generator for .NET and PowerShell*

Benefits at a glance:

- Uses a [cryptographically secure PRNG](https://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider)
- Can guarantee that the password contains digits, special characters, or any other set
- Can generate pronounceable, easier-to-remember passwords
- Packaged as a .NET library and as a PowerShell module

## PowerShell

### Installation

With [PowerShell 5](https://www.powershellgallery.com/Content/Images/arrow.png), installation is as simple as:

    Install-Module MlkPwgen

### Usage

Generate a handful of passwords:

```powershell
PS > 1..5 | foreach { New-Password }
xVs7tYANfs
FGQ4hF29Oe
QHffH4QRUE
ai1AaBqSMe
Dd7cnAG8a8
```

Generate letters only:

```powershell
PS > New-Password -Lower -Upper
HccNubILPl
```

Digits only:

```powershell
PS > New-Password -Digits -Length 6
470114
```

All together now, with symbols:

```powershell
PS > New-Password -Lower -Upper -Digits -Symbols
y3iF(g(xUw
```

Generate pronounceable passwords:

```powershell
PS > 1..5 | foreach { New-PronounceablePassword }
NaternNeam
LumLictles
StZattlate
InfeHascal
Tighampers
```

Pronounceable passwords can have digits and symbols too:

```powershell
PS > New-PronounceablePassword -Digits -Symbols
^Norompog2
```

## .NET

The library is [available from NuGet](https://www.nuget.org/packages/MlkPwgen/):

```
Install-Package MlkPwgen
```

Import the namespace:

```csharp
using MlkPwgen;
```

Then calling the library is as simple as:

```csharp
Console.WriteLine(PasswordGenerator.Generate());
```

Check out the [API
Documentation](http://www.codetinkerer.com/MlkPwgen/.net/html/4af4c04a-d525-8e81-133e-679872b3984f.htm)
for full details.

## Pronounceable Password Algorithm

Credit for the algorithm used to generate pronounceable password [goes to Tom Van Vleck](http://www.multicians.org/thvv/gpw-js.html). I've made a few changes along the way:

- The algorithm has been modified to take into account word endings
- The data tables have been generated from a new source
- I added the ability to mix in random character sets (such as digits and symbols) into the generated password
