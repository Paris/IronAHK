@ECHO OFF

FOR /D %%G IN ("*") DO (
	RMDIR /Q /S "%%G\bin" 2>NUL
	RMDIR /Q /S "%%G\obj" 2>NUL
)
