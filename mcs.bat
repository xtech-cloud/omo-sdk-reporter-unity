@echo off

set libpath=.\_bin\
::----------------------------------
set name=uniReporter
set depref=
set libref=.\unitysln\Reporter\Assets\3rd\uniSimpleJSON-v1.0.dll;.\unitysln\Reporter\Assets\3rd\uniJsonPack-v1.0.dll
::----------------------------------
MD %libpath%
set outfile=%libpath%\%name%.dll
set srcpath=.\unitysln\Reporter\Assets\OMO\SDK\NET\Scripts
call "%UNITY_ROOT%\Editor\Data\Mono\bin\smcs" -target:library -r:"%UNITY_ROOT%\Editor\Data\Managed\UnityEngine.dll";"%UNITY_ROOT%\Editor\Data\UnityExtensions\Unity\GUISystem\UnityEngine.UI.dll" -out:%outfile% -recurse:%srcpath%\*.cs -reference:%depref%;%libref%
echo FINISH
pause
exit
