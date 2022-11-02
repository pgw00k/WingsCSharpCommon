@echo off

set src=%~dp0
set tgt=%1
set /p tgt="Target(%tgt%):"

set obj[0].name=wings.genocean.cs.common
set obj[1].name=wings.genocean.unity.common
set obj[2].name=wings.genocean.unity.frame
set obj[3].name=wings.genocean.character.control
set obj[4].name=wings.genocean.unity.uiextension

set objLength=5
set objIndex=0

:loopStart
if %objIndex% equ %objLength% goto end

set objCurrent.name=0

for /f "usebackq delims==, tokens=1-3" %%i in (`set obj[%objIndex%]`) do (
    set objCurrent.name=%%j
)

mklink /D %tgt%\%objCurrent.name% %src%\%objCurrent.name%

set /a objIndex=%objIndex% + 1

goto loopStart

:end

pause