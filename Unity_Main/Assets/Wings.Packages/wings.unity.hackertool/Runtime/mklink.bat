@echo off
cd /d %~dp0
mklink /D src ..\..\..\..\..\OtherProj\Wings.Unity.HackerTool\Wings.Unity.HackerTool\src
echo "Finish"
pause