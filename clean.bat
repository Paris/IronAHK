@ECHO OFF

FOR /D %%G IN ("*") DO (
	RMDIR /Q /S %%G\bin
	RMDIR /Q /S %%G\obj
)
