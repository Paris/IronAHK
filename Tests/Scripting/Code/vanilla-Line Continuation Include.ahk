var := A_LineNumber
if var <> 1
	MsgBox %var%
if A_LineFile <> C:\A-Source\AutoHotkey\Test\TEST SUITES\Line Continuation Include.ahk
	MsgBox A_LineFile: %A_LineFile%

Var = 
(
test
test2
)
