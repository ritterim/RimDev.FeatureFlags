@echo off

dotnet build --configuration Release
dotnet test --configuration Release --no-build
