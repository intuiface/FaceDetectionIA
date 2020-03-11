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
	    def msbuild = tool name: 'msbuild-v15', type: 'msbuild'
		bat "\"${msbuild}\\MSBuild.exe\" /v:m /clp:ErrorsOnly;Summary /p:Configuration=Release /p:Platform=\"Any CPU\" FaceDetectionIA.sln"
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