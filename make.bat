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
SET devargs="%config%" "%name%.sln"
SET versionfile=%name%\version.txt
FOR /F %%G IN (%versionfile%) DO SET version=%%G
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
CALL %0 docs
devenv /rebuild %devargs% /project "%setup%"
IF NOT EXIST "%outpre%" MKDIR "%outpre%"
ECHO *>"%outpre%\.gitignore"
SET cwd=%CD%
CD "%setup%\%outpre%\%config%"
"%setup%.exe"
FOR %%G IN (*.msi) DO MOVE "%%G" "%cwd%\%outpre%\%name%-%version%-%%G"
CD "%cwd%"
SET Sz=%ProgramFiles%\7-zip
IF EXIST "%Sz%" SET PATH=%PATH%;%Sz%
CD "%name%\%outpre%"
SET outname=%name%-%version%
MOVE "%config%" "%outname%"
7z a "%cwd%\%outpre%\%name%-%version%.zip" "%outname%" -mx=9
MOVE "%outname%" "%config%"
CD "%cwd%"
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
