# FaceDetectionIA

## ⚙️ Quickstart

Prerequisites:
* install [Visual Studio 2015 or later](https://visualstudio.microsoft.com/fr/downloads/)
* install [CMake 3.4 or later](https://cmake.org/download/)
* install [Python 3.6.5](https://www.python.org/downloads/release/python-365/)
* install [OpenVINO](https://www.dropbox.com/s/2svslu5jkdddwj1/w_openvino_toolkit_p_2020.1.033.exe?dl=0)

Configure environment variables:
```bash
cd C:\\Program Files (x86)\\IntelSWTools\\openvino\\bin
setupvars.bat
```

Install OpenVINO dependencies:
```bash
cd C:\\Program Files (x86)\\IntelSWTools\\openvino\\deployment_tools\\model_optimizer\\install_prerequisites
install_prerequisites.bat
```

Install Face Detection dependencies:
* install [Boost 1.72](https://dl.bintray.com/boostorg/release/1.72.0/source/)
* create "OPEN_VINO" environment variable with value: C:\\Users\\your_username\\Documents\\Intel\\OpenVINO

## 👷 Compile and Run

Open FaceDetectionIA.sln with Visual Studio and generate a build.
Run TestFaceDetection project.
