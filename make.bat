@ECHO OFF

SET cmd=%1
IF "%cmd%"=="" SET cmd=all
GOTO %cmd%

:all
CALL :setvars
devenv /rebuild "Release|Any CPU" "%name%.sln"
GOTO :eof

:setvars
IF NOT DEFINED VS90COMNTOOLS (
	ECHO Visual Studio 9.0 is not installed.
	EXIT 1
)
CALL "%VS90COMNTOOLS%\vsvars32.bat"
SET name=IronAHK
GOTO :eof

:clean
FOR /D %%G IN ("*") DO (
	RMDIR /Q /S "%%G\bin" 2>NUL
	RMDIR /Q /S "%%G\obj" 2>NUL
)
GOTO :eof
