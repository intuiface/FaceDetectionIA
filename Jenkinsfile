pipeline {
  agent {
    node {
      label 'sapin3'
    }

  }
  stages {
    stage('Build') {
      steps {
        bat "\"${tool 'msbuild-v15'}\\MSBuild.exe\" /v:m /clp:ErrorsOnly;Summary /p:Configuration=Release /p:Platform=\"Any CPU\" FaceDetectionIA.sln"
      }
    }

    stage('Package') {
      steps {
        bat 'package.bat'
		zip archive: true, dir: "dist\\x64\\Release\\FaceDetection", glob: '', zipFile: "FaceDetectionServer.zip"
		zip archive: true, dir: "dist\\x64\\Release\\FaceDetectionIA", glob: '', zipFile: "FaceDetectionIA.zip"
      }
    }

    stage('Archive') {
      steps {
        archiveArtifacts 'FaceDetectionServer.zip'
        archiveArtifacts 'FaceDetectionIA.zip'
      }
    }

  }
}