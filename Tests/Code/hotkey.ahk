#Include %A_ScriptDir%/header.ahk

Gosub, ::pass

F12::MsgBox

:*:btw::
(
by the way
)

~*Esc::
ExitApp

::pass::
FileAppend, pass, *
Goto, ~*Esc
