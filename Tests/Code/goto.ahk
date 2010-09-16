#Include %A_ScriptDir%/header.ahk

Gosub, labelA
x++

labelB:
x++
;Goto, LABELC
Goto, labelc
x++

labelc:
If x = 3
	FileAppend, pass, *
Return

labelA:
x = 1
