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
SET setup=Setup
SET libname=Rusty
SET config=Release
SET outpre=bin
SET site=Site
SET deploy=Deploy
SET devargs="%config%" "%name%.sln"
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
CALL :all
php-cgi -f "%name%\%site%\transform.php"
GOTO :eof

:dist
CALL "%0" docs
"%deploy%\%outpre%\%config%\Setup.exe"
GOTO :eof

:mostlyclean
CALL :clean
php-cgi -f "%name%\%site%\clean.php" > NUL
GOTO :eof

:clean
FOR /D %%G IN ("*") DO (
	RMDIR /Q /S "%%G\%outpre%" 2>NUL
	RMDIR /Q /S "%%G\obj" 2>NUL
)
IF EXIST "%outpre%" RMDIR /Q /S "%outpre%"
GOTO :eof
