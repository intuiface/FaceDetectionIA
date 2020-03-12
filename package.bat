:: Remove useless files
echo deleting useless files
del ".\dist\x64\Release\*.pdb" /s /f /q
del ".\dist\x64\Release\*.ipdb" /s /f /q
del ".\dist\x64\Release\*.iobj" /s /f /q
del ".\dist\x64\Release\TestFaceDetection.*" /s /f /q
echo delete done
