@ECHO OFF

CALL :setvars
SET cmd=%1
IF "%cmd%"=="" SET cmd=all
GOTO %cmd%

:all
devenv /rebuild "%config%|Any CPU" "%name%.sln"
GOTO :eof

:setvars
CALL :vs
SET name=IronAHK
SET libname=Rusty
SET config=Release
SET outdir=bin
SET site=Site
GOTO :eof

:vs
IF DEFINED VSSETVARS GOTO :eof
IF NOT DEFINED VS90COMNTOOLS (
	ECHO Visual Studio 9.0 is not installed.
	EXIT 1
)
CALL "%VS90COMNTOOLS%\vsvars32.bat"
SET VSSETVARS=1
GOTO :eof

:docs
SET dir=%name%\%outdir%\%config%
SET id=%name%.%libname%
IF NOT EXIST "%dir%" MD "%dir%"
csc "/doc:%dir%\%id%.xml" /reference:System.Windows.Forms.dll,System.Drawing.dll "/out:%dir%\%id%.dll" /target:library "/recurse:%libname%\*.cs" /unsafe /warn:0
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
