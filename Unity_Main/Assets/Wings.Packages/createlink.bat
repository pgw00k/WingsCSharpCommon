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

del /f /q *.* Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src\Editor\XLua.Editor.asmdef
del /f /q *.* Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src\Editor\XLua.Editor.asmdef.meta

rd /s /q Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src
rd /s /q Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Plugins

mklink /D Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src %cd%\OtherProj\xlua\Assets\XLua\Src
mklink /D Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Plugins %cd%\OtherProj\xlua\Assets\Plugins
mklink /H Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src\Editor\XLua.Editor.asmdef Reference\wings.genocean.unity.xlua\Editor\XLua.Editor.asmdef
mklink /H Unity_Main\Assets\Wings.Packages\wings.genocean.unity.xlua\Runtime\Src\Editor\XLua.Editor.asmdef.meta Reference\wings.genocean.unity.xlua\Editor\XLua.Editor.asmdef.meta

mklink /D Unity_Main\Assets\Wings.Packages\wings.genocean.cs.common\Runtime\Common %cd%\OtherProj\WingsCSharp\WingsCSharp\Common

pause