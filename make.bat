@ECHO OFF

IF NOT DEFINED VS90COMNTOOLS (
	ECHO Visual Studio 9.0 is not installed.
	EXIT 1
)
CALL "%VS90COMNTOOLS%\vsvars32.bat"

SET name=IronAHK
devenv /rebuild "Release|Any CPU" "%name%.sln"

SET Sz=%ProgramFiles%\7-Zip\7z.exe
IF NOT EXIST "%Sz%" (
	ECHO Could not find 7-zip.
	EXIT 1
)

SET version=
FOR /F %%G IN (version.txt) DO SET version=%%G

SET cwd=%CD%
SET release=Release
CD "%name%\bin"
MOVE "%release%" "%name%"
SET out=%name%\%name%-%version%.zip
IF EXIST "%out%" DEL "%out%"
"%Sz%" a "%out%"  "%name%" "-x!%out%" -x!*.zip -mx=9
MOVE "%name%" "%release%"
CD "%cwd%"
