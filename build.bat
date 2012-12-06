@echo off

::CloudInit.NET

if '%2' NEQ '' goto usage
if '%3' NEQ '' goto usage
if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%~d0%~p0%

SET build.config.settings="%DIR%Scripts\Build\settings.config"
SET current.dir=%DIR%
"%DIR%Tools\nant\nant.exe" %1 /f:.\Scripts\Build\default.build -D:build.config.settings=%build.config.settings% -D:current.dir=%current.dir%

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:usage
echo.
echo Usage: build.bat
echo.
goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish