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
      }
    }

    stage('Archive') {
      steps {
        archiveArtifacts 'FaceDetectionIA.zip'
      }
    }

  }
}