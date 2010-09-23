#Include %A_ScriptDir%/header.ahk

x := a() . b() . c("a()")

if x =l!a()z
	FileAppend, pass, *

a() {
	return "l"
}

b(x)
{
;	return x
	return "!" . x
}

c(x, y = "z")
{ return x . y
}
