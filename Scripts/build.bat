@echo off
setlocal
pushd "%~dp0.."
dotnet run --project src\ZenlessCompanion.csproj -c Release
set "exitCode=%ERRORLEVEL%"
popd
exit /b %exitCode%
