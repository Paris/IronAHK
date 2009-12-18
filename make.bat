@ECHO OFF

IF NOT DEFINED VS90COMNTOOLS (
	ECHO Visual Studio 9.0 is not installed.
	EXIT 1
)
CALL "%VS90COMNTOOLS%\vsvars32.bat"

SET name=IronAHK
devenv /rebuild "Release|Any CPU" "%name%.sln"
