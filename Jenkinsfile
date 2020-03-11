pipeline {
  agent {
    node {
      label 'sapin3'
    }

  }
  stages {
    stage('Build') {
      agent {
        node {
          label 'sapin3'
        }
      }
      steps {
		bat "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\BuildTools\\MSBuild\\15.0\\Bin\\MSBuild.exe\" /v:m /clp:ErrorsOnly;Summary /p:Configuration=Release /p:Platform=\"Any CPU\" FaceDetectionIA.sln"
      }
    }
	
	stage('Package') {
      agent {
        node {
          label 'sapin3'
        }
      }
      steps {
		bat "package.bat"
      }
    }

  }
}