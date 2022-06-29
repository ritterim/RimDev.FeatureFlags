New-Item -ItemType directory -Path "build" -Force | Out-Null

# Release whenever a commit is merged to master
$ENV:UseMasterReleaseStrategy = "true"

# The following variables should be set if unit tests need the Azurite (Azure Storage) Docker container created
# Only do this if running under APPVEYOR
#if (Test-Path 'env:APPVEYOR') {
#  $ENV:RIMDEV_CREATE_TEST_DOCKER_AZURITE = "true"
#}

# The following variables should be set if unit tests need the Elasticsearch Docker container created
#$ENV:RIMDEV_CREATE_TEST_DOCKER_ES = "true"
#$ENV:RIMDEVTESTS__ELASTICSEARCH__BASEURI = "http://localhost"
#$ENV:RIMDEVTESTS__ELASTICSEARCH__PORT = "9206"
#$ENV:RIMDEVTESTS__ELASTICSEARCH__TRANSPORTPORT = "9306"

# The following variables should be set if unit tests need the SQL Docker container created
$ENV:RIMDEV_CREATE_TEST_DOCKER_SQL = "true"
$ENV:RIMDEVTESTS__SQL__HOSTNAME = "localhost"
$ENV:RIMDEVTESTS__SQL__PORT = "11439"
$ENV:RIMDEVTESTS__SQL__PASSWORD = "HbXCXv4qJAWhliA"

try {
  Invoke-WebRequest https://raw.githubusercontent.com/ritterim/build-scripts/master/bootstrap-cake.ps1 -OutFile build\bootstrap-cake.ps1
  Invoke-WebRequest https://raw.githubusercontent.com/ritterim/build-scripts/master/build-net5.cake -OutFile build.cake
}
catch {
  Write-Output $_.Exception.Message
  Write-Output "Error while downloading shared build script, attempting to use previously downloaded scripts..."
}

#.\build\bootstrap-cake.ps1 -Verbose --verbosity=Normal
.\build\bootstrap-cake.ps1 -Verbose --verbosity=Diagnostic

Exit $LastExitCode
