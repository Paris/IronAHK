#Include %A_ScriptDir%/header.ahk

var := A_LineNumber
if var <> 3
	MsgBox %var%
SplitPath, A_LineFile, filename
if filename <> vanilla-Line Continuation Include.ahk
	MsgBox A_LineFile: %A_LineFile%

Var = 
(
test
test2
)
