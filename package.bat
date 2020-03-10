:: Remove useless files
echo deleting useless files
del ".\dist\x64\Release\*.pdb" /s /f /q
del ".\dist\x64\Release\*.ipdb" /s /f /q
del ".\dist\x64\Release\*.iobj" /s /f /q
del ".\dist\x64\Release\TestFaceDetection.*" /s /f /q
echo delete done

:: Zip files
echo archiving Face Detection IA
7z a -tzip FaceDetectionIA.zip -r .\dist\x64\Release\*
echo archive done
