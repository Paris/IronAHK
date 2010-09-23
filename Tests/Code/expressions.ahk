#Include %A_ScriptDir%/header.ahk

if (a == 1 && v != 2)
	FileAppend, fail, *

x := "a" . (1 ? "" : "b")
v := 1 + %x%()

If v = 2
	FileAppend, pass, *

a() {
	return 1
}
