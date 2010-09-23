#Include %A_ScriptDir%/header.ahk

y = 5
x = 0

Loop, %y%
{
	If A_Index =2
		Continue
	x := x + A_Index
	If A_Index =4
		Break
}

If x =8
	FileAppend, pass, *
