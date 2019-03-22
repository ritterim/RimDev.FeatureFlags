@echo Off
pushd %~dp0
setlocal enabledelayedexpansion

rmdir /s /q "artifacts"

dotnet build --configuration Release
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build --output ../../artifacts
