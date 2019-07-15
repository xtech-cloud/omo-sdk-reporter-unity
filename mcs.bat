@echo off

set libpath=.\_bin\
::----------------------------------
set name=uniReporter
set depref=
set libref=.\unitysln\Reporter\Assets\3rd\uniSimpleJSON.dll;.\unitysln\Reporter\Assets\3rd\uniJsonPack.dll
::----------------------------------
MD %libpath%
set outfile=%libpath%\%name%.dll
set srcpath=.\unitysln\Reporter\Assets\XTC\OMO\NET\Scripts
call "%UNITY_ROOT%\Editor\Data\Mono\bin\smcs" -target:library -r:"%UNITY_ROOT%\Editor\Data\Managed\UnityEngine.dll";"%UNITY_ROOT%\Editor\Data\UnityExtensions\Unity\GUISystem\UnityEngine.UI.dll" -out:%outfile% -recurse:%srcpath%\*.cs -reference:%depref%;%libref%
echo FINISH
pause
exit
