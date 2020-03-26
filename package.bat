:: Remove useless files
echo deleting useless files
del ".\dist\x64\Release\OpenVINOFaceDetectionServer\*.pdb" /s /f /q
del ".\dist\x64\Release\OpenVINOFaceDetectionServer\*.ipdb" /s /f /q
del ".\dist\x64\Release\OpenVINOFaceDetectionServer\*.iobj" /s /f /q
del ".\dist\x64\Release\FaceDetection\*.pdb" /s /f /q
del ".\dist\x64\Release\TestFaceDetection\*.pdb" /s /f /q
echo delete done
