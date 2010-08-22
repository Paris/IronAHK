@ECHO OFF

SETLOCAL
CALL :setvars
SET cmd=%1
IF "%cmd%"=="" SET cmd=all
GOTO %cmd%

:all
devenv /rebuild "%config%" "%name%.sln"
GOTO :eof

:setvars
CALL :vs
SET name=IronAHK
SET config=Release
SET outpre=bin
SET sitedocs=%name%\Site\docs\commands
GOTO :eof

:vs
IF DEFINED VSSETVARS GOTO :eof
IF NOT DEFINED VS100COMNTOOLS (
	ECHO Visual Studio 2010 is not installed.
	EXIT 1
)
CALL "%VS100COMNTOOLS%\vsvars32.bat" >NUL
SET VSSETVARS=1
GOTO :eof

:docs
CALL :dist docs
GOTO :eof

:dist
CALL :all
"Deploy\%outpre%\%config%\Setup.exe" %1
GOTO :eof

:clean
FOR /D %%G IN ("*") DO (
	RMDIR /Q /S "%%G\%outpre%" 2>NUL
	RMDIR /Q /S "%%G\obj" 2>NUL
)
IF EXIST "%outpre%" RMDIR /Q /S "%outpre%"
FOR /F %%d in ('DIR /AD /B "%sitedocs%"') DO RMDIR /Q /S "%sitedocs%\%%d"
GOTO :eof
