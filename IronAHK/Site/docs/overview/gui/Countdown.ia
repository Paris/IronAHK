#SingleInstance ignore

Gui, Add, GroupBox, w255 r3 Section, Select
Gui, Add, Text, xs+10 ys+25 Section, Time:
Gui, Add, Edit, vTimeValue ys-2 Limit3 gValidate
Gui, Add, UpDown, vTime Range1-60, 10
Gui, Add, Radio, vSecs, Seconds
Gui, Add, Radio, vMins xp+65 yp Checked, Minutes
Gui, Font, bold
Gui, Add, Button, vRun ys-4 w65 Default gRun, Start
Gui, Font
Gui, Add, GroupBox, xm w255 r4 Section, Progress
Gui, Font, s24 bold, Calibri
Gui, Add, Text, vCount xs+10 ys+20 w235 Center, 00:00
Gui, Add, Progress, vProgress wp r0.5 -Smooth
Gui, Font
GuiControl, Focus, Time
Gui, Show, , Countdown Timer
Return

Validate:
GuiControlGet, time, , TimeValue
If time is not integer
	state = disable
Else
	state = enable
GuiControl, %state%, Run
Return

Run:
If running
{
	running := false
	GuiControl, , Count, 00:00
	GuiControl, , Progress, 0
	GuiControl, , Run, Start
}
Else
{
	running := true
	GuiControlGet, time, , Time
	GuiControlGet, mins, , Mins
	If mins = 1
		time *= 60
	total := remainder := time
	GuiControl, , Run, Stop
	GuiControl, Focus, Time
	SetTimer, Tick, 1000
	Goto, Tick
}
Return

Tick:
Critical
If (!running or remainder < 0)
{
	SetTimer, Tick, Off
	Return
}
SetFormat, Float, 02.0
s := Mod(remainder, 60) + 0.0, m := (remainder - s) / 60 + 0.0
GuiControl, , Count, %m%:%s%
completed := Ceil(remainder / total * 100)
GuiControl, , Progress, %completed%
remainder--
Return

GuiEscape:
GuiClose:
ExitApp
