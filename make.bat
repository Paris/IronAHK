@ECHO OFF

SETLOCAL
CALL :setvars
SET cmd=%1
IF "%cmd%"=="" SET cmd=all
GOTO %cmd%

:all
devenv /rebuild %devargs%
GOTO :eof

:setvars
CALL :vs
SET name=IronAHK
SET libname=Rusty
SET config=Release
SET outdir=bin
SET site=Site
SET devargs="%config%" "%name%.sln"
GOTO :eof

:vs
IF DEFINED VSSETVARS GOTO :eof
IF NOT DEFINED VS90COMNTOOLS (
	ECHO Visual Studio 9.0 is not installed.
	EXIT 1
)
CALL "%VS90COMNTOOLS%\vsvars32.bat" >NUL
SET VSSETVARS=1
GOTO :eof

:docs
CALL :all
php-cgi -f "%name%\%site%\transform.php"
GOTO :eof

:mostlyclean
CALL :clean
php-cgi -f "%name%\%site%\clean.php" > NUL
GOTO :eof

:clean
FOR /D %%G IN ("*") DO (
	RMDIR /Q /S "%%G\%outdir%" 2>NUL
	RMDIR /Q /S "%%G\obj" 2>NUL
)
GOTO :eof
