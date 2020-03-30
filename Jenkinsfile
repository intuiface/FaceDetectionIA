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
        bat 'package.bat'
      }
    }

    stage('Sign') {
      steps {
		copyArtifacts filter: '**/Cryptifix-x64-*.zip', fingerprintArtifacts: true, projectName: 'IntuiFace/master', selector: lastSuccessful(), target: 'cryptifix'
		unzip zipFile: "cryptifix\\Cryptifix-x64-*.zip"
		bat "cryptifix\\Cryptifix.exe sign "dist\\x64\\Release\\FaceDetection" --LicenseEdition=FREE --IsAllowedByNonInteractivePlayer=false
      }
    }

    stage('Package') {
      steps {
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