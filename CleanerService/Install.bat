@echo off
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Please run as Administrator!
    pause
    exit /b 1
)

echo ================================
echo    CleanerService Installer
echo ================================
echo.

dotnet --version >nul 2>&1
if %errorLevel% neq 0 (
    echo .NET SDK is not installed!
    echo Please install it from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

sc query CleanerService >nul 2>&1
if %errorLevel% equ 0 (
    echo Found old service. Removing...
    sc stop CleanerService >nul 2>&1
    timeout /t 3 >nul
    sc delete CleanerService >nul 2>&1
    timeout /t 2 >nul
    echo Old service removed!
    echo.
)

echo [1/3] Publishing project...
dotnet publish "%~dp0CleanerService.csproj" -c Release -o "%~dp0publish"
if %errorLevel% neq 0 (
    echo Publish failed!
    pause
    exit /b 1
)
echo Publish done!
echo.

echo [2/3] Installing service...
sc create CleanerService binPath= "%~dp0publish\CleanerService.exe" start= auto DisplayName= "Cleaner Service"
if %errorLevel% neq 0 (
    echo Failed to create service!
    pause
    exit /b 1
)

sc description CleanerService "Cleans Windows temp folders every 3 days automatically"

sc failure CleanerService reset= 86400 actions= restart/5000/restart/5000/restart/5000
echo Service installed!
echo.

echo [3/3] Starting service...
sc start CleanerService
timeout /t 5 >nul

sc query CleanerService | find "RUNNING" >nul 2>&1
if %errorLevel% equ 0 (
    echo ================================
    echo   Service is RUNNING!
    echo ================================
) else (
    echo ================================
    echo   Service installed but not running.
    echo   Check Event Viewer for details.
    echo ================================
)

pause