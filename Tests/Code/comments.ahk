#Include %A_ScriptDir%/header.ahk

 ; FileAppend, fail, *
;

x = 0;1;2 ; 3

/* FileAppend, fail, *
x = ;
*/ x =0;1;2

; If x =
If x = 0;1;2 ;
	FileAppend, pass, *
