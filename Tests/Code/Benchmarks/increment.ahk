i = 500000
a = 3

Process, Priority, , R
b = %a%

s0 := A_TickCount

Loop, %i%
{
	a++
	a--
}

s1 := A_TickCount

result := a == b ? "pass" : "fail"
FileAppend, % s1 - s0 . " (" . result . ")", *
