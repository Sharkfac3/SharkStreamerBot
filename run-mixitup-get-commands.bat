@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
set "PYTHON_CMD="

where py >nul 2>nul
if %ERRORLEVEL%==0 (
  set "PYTHON_CMD=py -3"
) else (
  where python >nul 2>nul
  if %ERRORLEVEL%==0 (
    set "PYTHON_CMD=python"
  )
)

if not defined PYTHON_CMD (
  echo Could not find Python. Install Python and make sure ^`py^` or ^`python^` is on PATH.
  exit /b 1
)

pushd "%SCRIPT_DIR%" >nul
call %PYTHON_CMD% "%SCRIPT_DIR%Tools\MixItUp\Api\get_commands.py" %*
set "EXIT_CODE=%ERRORLEVEL%"
popd >nul

exit /b %EXIT_CODE%
