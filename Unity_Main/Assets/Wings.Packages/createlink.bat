@echo off

>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if '%errorlevel%' NEQ '0' (
goto UACPrompt
) else ( goto gotAdmin )
:UACPrompt
echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
echo UAC.ShellExecute "%~s0", "", "", "runas", 1 >> "%temp%\getadmin.vbs"
"%temp%\getadmin.vbs"
exit /B
:gotAdmin
if exist "%temp%\getadmin.vbs" ( del "%temp%\getadmin.vbs" )
pushd "%CD%"
CD /D "%~dp0"

cd ..\..\..\
rd /s /q Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src
rd /s /q Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Plugins
mklink /D Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src %cd%\OtherProj\xlua\Assets\XLua\Src
mklink /D Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Plugins %cd%\OtherProj\xlua\Assets\Plugins
pause