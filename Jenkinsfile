pipeline {
  agent {
    node {
      label 'sapin3'
    }

  }
  stages {
    stage('Build') {
      steps {
        bat "\"${tool 'msbuild-v15'}\\MSBuild.exe\" /v:m /clp:ErrorsOnly;Summary /p:Configuration=Release /p:Platform=\"Any CPU\" FaceDetection.sln"
      }
    }

    stage('Package') {
      steps {
        bat 'package.bat'
		zip archive: true, dir: "dist\\x64\\Release\\OpenVINOFaceDetectionServer", glob: '', zipFile: "OpenVINOFaceDetectionServer.zip"
		zip archive: true, dir: "dist\\x64\\Release\\FaceDetection", glob: '', zipFile: "FaceDetection.zip"
      }
    }

    stage('Archive') {
      steps {
        archiveArtifacts 'OpenVINOFaceDetectionServer.zip'
        archiveArtifacts 'FaceDetection.zip'
      }
    }

  }
}