#Include %A_ScriptDir%/header.ahk

x = 1
y=2
z = % x + y

If x != 1
	FileAppend, fail, *

If y!=2
	FileAppend, fail, *

if z!=	3
	FileAppend, fail, *

FileAppend, pass, *
