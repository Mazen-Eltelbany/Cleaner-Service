@echo off
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Please run as Administrator!
    pause
    exit /b 1
)

echo ================================
echo   CleanerService Uninstaller
echo ================================
echo.

sc query CleanerService >nul 2>&1
if %errorLevel% neq 0 (
    echo Service is not installed!
    pause
    exit /b 1
)

echo Stopping service...
sc stop CleanerService >nul 2>&1
timeout /t 3 >nul

echo Deleting service...
sc delete CleanerService >nul 2>&1
timeout /t 2 >nul

sc query CleanerService >nul 2>&1
if %errorLevel% neq 0 (
    echo ================================
    echo   Service uninstalled!
    echo ================================
) else (
    echo ================================
    echo   Something went wrong.
    echo   Try rebooting and run again.
    echo ================================
)

pause