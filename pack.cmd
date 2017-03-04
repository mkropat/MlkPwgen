@cd %~dp0\MlkPwgen

call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"

msbuild /t:pack /p:Configuration=Release MlkPwgen.csproj

pause