# CleanerService

A lightweight Windows Background Service built with **.NET Worker Service** that automatically cleans temporary files from your system every 3 days — keeping your machine free of junk files without any manual effort.

---

## Features

- Automatically deletes files and folders inside `C:\Windows\Temp`
- Automatically deletes files and folders inside the user temp folder (`%TEMP%`)
- Skips files that are currently in use or protected by access permissions
- Runs silently in the background as a Windows Service
- Logs every action (deleted files, skipped files, errors) using the built-in .NET logger
- Repeats the cleaning cycle every **3 days** automatically

---

## Project Structure

```
CleanerService/
├── bin/                          # Build output
├── obj/                          # Intermediate build files
├── Properties/                   # Launch settings
├── appsettings.json              # App configuration
├── appsettings.Development.json  # Dev configuration
├── CleanerService.csproj         # Project file
├── CleanerService.sln            # Solution file
├── CleanService.cs               # Core service logic
├── Program.cs                    # Entry point
├── install.bat                   # One-click installer
└── uninstall.bat                 # One-click uninstaller
```

---

## Requirements

- Windows 10 / Windows 11 / Windows Server 2016+
- [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Administrator privileges (required to clean `C:\Windows\Temp`)

---

## Installation

### Option 1 — Using the batch file (Recommended)

- Right-click `install.bat` → **Run as Administrator**

That's it — the service will be installed and started automatically.

---

### Option 2 — Manual installation

Open **Command Prompt as Administrator** and run:

```bash
sc create CleanerService binPath= "C:\full\path\to\CleanerService.exe" start= auto DisplayName= "Cleaner Service"
sc start CleanerService
```

---

## Uninstallation

Right-click `uninstall.bat` → **Run as Administrator**

Or manually:

```bash
sc stop CleanerService
sc delete CleanerService
```

---

## Logs

You can monitor the service activity through **Windows Event Viewer**:

```
Event Viewer → Windows Logs → Application
```

Look for entries under the source `CleanerService`.

---

## Notes

- Files currently in use by other processes are **skipped safely** and logged as warnings
- The service starts automatically on Windows startup after installation

---

## Author

**Mazen Eltelbany**
Computer Science & AI Student — Helwan University, Cairo
GitHub: [github.com/Mazen-Eltelbany](https://github.com/Mazen-Eltelbany)