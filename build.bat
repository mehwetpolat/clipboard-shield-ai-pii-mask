@echo off
setlocal
echo ===================================================
echo [PII-Mask] Windows Masaustu Uygulamasi Derleniyor...
echo ===================================================

if not exist release mkdir release

taskkill /F /IM PII-Mask.exe >nul 2>&1

set CSC_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    set CSC_PATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
)

if not exist "%CSC_PATH%" (
    echo [HATA] .NET C# Derleyicisi csc.exe bulunamadi.
    exit /b 1
)

set ICON_PARAM=
if exist assets\app.ico (
    set ICON_PARAM=/win32icon:assets\app.ico
)

echo Derleyici: %CSC_PATH%
"%CSC_PATH%" /target:winexe %ICON_PARAM% /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:release\PII-Mask.exe src\DesktopApp.cs src\Engine\*.cs src\Validators\*.cs

if %ERRORLEVEL% equ 0 (
    echo.
    echo [BASARILI] release\PII-Mask.exe basariyla derlendi!
) else (
    echo.
    echo [HATA] Derleme basarisiz oldu.
)
