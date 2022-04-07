@echo off
if exist tools\Cake\cake.exe goto :build
echo ===================================
echo Installing build tools
echo ===================================
cd tools
call install-tools.bat
cd ..
:build

echo ===================================
echo Executing Cake build script
echo ===================================
tools\Cake\cake.exe build.cake %*
