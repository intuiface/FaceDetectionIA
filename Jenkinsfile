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
		bat "\"${tool 'msbuild-v15'}\\MSBuild.exe\" /v:m /clp:ErrorsOnly;Summary /p:Configuration=Release /p:Platform=\"Any CPU\" FaceDetectionIA.sln"
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

	stage('Archive') {
      agent {
        node {
          label 'sapin3'
        }
      }
      steps {
		archiveArtifacts "FaceDetectionIA.zip"
      }
    }
  }
}