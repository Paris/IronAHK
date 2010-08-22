;
n = %A_LineNumber%
If n != 2
	FileAppend, fail, *

path = %A_LineFile%
SplitPath, path, name
If name != line.ahk
	FileAppend, fail, *

x := 1 + A_LineNumber
If x > 2
	FileAppend, pass, *
