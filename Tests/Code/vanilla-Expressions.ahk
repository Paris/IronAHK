#Include %A_ScriptDir%/header.ahk

Critical  ; THIS SECTION MUST BE AT THE VERY TOP TO DETECT THIS SORT OF BUG (caused by "iteration_limit = ArgToInt64(1)" vs. "iteration_limit = line->ArgToInt64(1)")
A := False
Loop 4
	A += 1
if a <> 4
	MsgBox, %A%
Critical Off

;#NoEnv  ; << Kept turned off for the purpose of testing environment variables in expressions.
#SingleInstance
SetBatchLines -1

ClipSaved := ClipboardAll  ; Saved for later restoration.

;funcabc(var="", ="")
;funcabc(var="", ="", var2)
;funcabc(=1)
;funcabc(byref=1)
{
}

;funcabc(test = "", test2)
;{
;}

; TEST FUNCTION OVERRIDE (works okay):
;var := WinExist()
;if var <> override built-in function
	;MsgBox %Var%
;WinExist()
;{
	;return "override built-in function"
;}

; test loadtime byref syntax errors:
;Swap(var , var+1)
;Swap(var , "string")
;Swap( var , var )

;xyz = thing with spaces
;Swap(var%xyz%, y)
;xyz = clipboard
;Swap(%xyz%, y)

;xyz(x

; "local vars do not need to be declared in this function":
;fn(x)
;{
	;x = 3
	;local y
;}

; Call to nonexistent function:
;fn1()
;fn2()
;{
 ;msgbox
;}

; Keyword "local" can be used as a variable:
;fn()
;fn()
;{
	;local local
	;local = value
	;msgbox %local%
;}

; Don't declare params:
;fn(x)
;{
	;local x
;}

;; shows that exit seems to work okay:
;var := function_that_exits() . MsgBox()
;function_that_exits()
;{
	;exit
;}

;; Dynamic function calls:
;Var := %var%(x)
;Var := Array%i%(x)

;; function inside function:
;fn1()
;{
	;fn2()
	;{
	;}
;}

; tests ok:
;swap("xxx" . y)
;swap(a_index, x)

; It's a known limitation that this isn't caught as a load-time error unless the global word is present on top.
;fnstatic()
;{
	;;global
	;static a
	;static a
;}

;fnstatic2(y)
;{
	;global x
	;static x
	;static y
;}

;fntest()
;{
	;;local ErrorLevel
	;;global ErrorLevel
	;;local A_Index
	;;global A_Index
	;;global var%i%
;}

;; Test behavior of commands that create arrays (this was a tricky area that has some notes in Discarded Code segments),
;; such as why NEVER to resolve the target var and then try to create array elements at the target, since the target
;; might be another function:
;Var =
;WinGetList(Var)
;MsgBox %Var%`n%Var1%`n%Var2%
;ExitApp
;WinGetList(ByRef List)
;{
	;WinGet, List, List
	;MsgBox %List%`n%List1%`n%List2%
;}

;Test loadtime syntax errors:
;x := %y%fn()
;x := fn%y%()

;var = String with spaces at its end%A_Space%%A_Space%
;if (var = Bogus%Var%) ; Since Bogus%Var% does not exist, it is considered to be "", thus the expression is false.
	;MsgBox %var%

; Test various ways for blank params to occur, in both fn defs and calls, and all seem okay:
;var := add(yy,  ,xx)  ; tested many variants.

;fn(x,, y)
;{
;}

;parent_fn()
;{
	;child_fn()
	;{
	;}
;}

;fn_with_hotstrings()
;{
;::test::test
;}

;Works okay:
;if (0)
	;x = 1
;else MsgBox()
;^#d::MsgBox()

;Brief test of multiple expressions in same line:
;MsgBox, 0, % "title" " appended", % YieldLocalVarContainingXYZ()

; Some more syntax errors (two):
;funcxx(MyParam)
;{
	;; Error #1:
	;local MyParam
	;Global MyParam
;
	;; Error #2 (can't declare both types in same func)
	;local x
	;Global y
;
	;return MyParam
;}


;funcyy(MyParam)
;{
	;; Tested these in both orders with respect to each other:
	;Global
	;Global x
;}


;funczz(A_TickCount, X, Y)
;{
	;msgbox
;}


;; Various ways to improperly use the ByRef keyword:
;funczz(A, B, ByRef, X, Y, Q)
;funczz(ByRef, B, ByRef, X, Y, Q)
;funczz(A B, C D, X, Y, Q)
;{
	;msgbox
;}


;; TESTED OUT OK:
;var = x
;var := TestExit()
;TestExit()
;{
	;exit
;}


; --------------------------------------------
; --------------------------------------------

; MISC TESTS

GroupAdd, FuncGosubGroup, Some Title That Won't Exist,, func_sub
funcgosub()

j = 1
if (FuncGosubArray%j% <> "x")
	MsgBox % FuncGosubArray%j%

funcgosub()
{
	something = func_sub
	Gosub %something%
	gosub func_sub
	GroupActivate FuncGosubGroup  ; Similar effect to "gosub func_sub".
}

func_sub:
i = 1
FuncGosubArray%i% = x
if (A_ThisFunc <> "funcgosub")
	MsgBox % A_ThisFunc
;ListVars
;MsgBox



if 1
{
	test = test
	test2 = test2
	func(s)
	{
		return s
	}
	if (%test%2 <> "test2")
		MsgBox %test%2
}


if "+" . "0"
	MsgBox Should have been false for legacy reasons.
if "-" . "0"
	MsgBox Should have been false for legacy reasons.
if " " . "0"
	MsgBox Should have been false for legacy reasons.
if ("+" . "0") or false
{
}
else
	MsgBox Should have been true for legacy reasons.
if ("-" . "0") or false
{
}
else
	MsgBox Should have been true for legacy reasons.
if (" " . "0") or false
{
}
else
	MsgBox Should have been true for legacy reasons.

x = var
var = 0
if %x%
{
}
else
	MsgBox "if %x%" is supposed to behave like "if x" for backward compatibility (for legacy reasons the same as "return %x%" is the same as "return x"?)
if (%x%)
	MsgBox "if (%x%)" should resolve to "if var", which should be false.

if (A_IsCritical <> "0")
	MsgBox A_IsCritical <> "0"
Critical
if (A_IsCritical <> "16")
	MsgBox A_IsCritical <> "16"
Critical 333
if (A_IsCritical <> "333")
	MsgBox A_IsCritical <> "333"
Critical 222ANY_STRING
if (A_IsCritical <> "222")
	MsgBox A_IsCritical <> "222"
Critical On
if (A_IsCritical <> "16")
	MsgBox A_IsCritical <> "16"
Critical Off
if (A_IsCritical <> "0")
	MsgBox A_IsCritical <> "0"
Critical AnyWordOtherThanOn
if (A_IsCritical <> "0")
	MsgBox A_IsCritical <> "0"
Critical 0
if (A_IsCritical <> "0")
	MsgBox A_IsCritical <> "0"

; SOME TESTS THAT CHECK BACKWARD-COMPATIBILITY:
x := 0xFF
if (x <> "0xFF")
	MsgBox %x%
y := 1, x := 0xFF  ; Forces the assignment to be an expression.
if (x <> "0xFF")
	MsgBox %x%
TestFormat(0xFF, 00123)
TestFormat(x, y, z = 0xFF, a = 04)  ; Default parameters convert to pure numbers, for backward compatibility.
{
	if (x <> "0xFF" || y <> "00123" || z <> "255" || a <> "4")
		MsgBox x <> "0xFF" || y <> "00123" || z <> "255" || a <> "4"
}

b := 1.5
a := b ^ 1
if (b*2 <> 3.0)
	MsgBox Binary number caching issue.

x := true
y := true
z = 0
if x  ; <-- If this layer were to do it, its own else would be unexpectedly encountered.
label1:
{ ; <-- We want this statement's layer to handle the goto.
	++z
	if y
	{
		y := false
		goto, label1
	}
	else
		Sleep -1
}
else
	MsgBox Shouldn't be reached
if z <> 2
	MsgBox Z=%Z% was expected to be 2.

x := true
y := true
z = 5
if x  ; <-- If this layer were to do it, its own else would be unexpectedly encountered.
	label2:
	if y  ; <-- We want this statement's layer to handle the goto.
	{
		y := false
		++z
		goto, label2
	}
	else
		++z
else
	MsgBox Shouldn't be reached
if z <> 7
	MsgBox Z=%Z% was expected to be 7.

x := true
y := true
z = 11
Loop 5
label3:
{
	++z
	if y
	{
		y := false
		goto label3
	}
	var := A_Index
}
if (var <> 5 || z <> 17)
	MsgBox Obscure Loop/Goto problem.


;;;;;;;;;;;;

; MORE MISC TESTS

SetFormat, Integer, Hex
Var:="55"
if (Var <> "55")  ; Formerly a bug.
	MsgBox %Var%
SetFormat, Integer, D

var:=+5
if (var <> "+5")
	MsgBox %var%

Var := "123 "  ; Trailing space only (no leading) because that is a potential bug in one optimization.
if (Var <> "123 ")
	MsgBox %Var%

123 := 4  ; Even though the operator is :=, numerically-named variables are still supported (maybe was a bad idea; in any case, perhaps should be done away with in v2).
if 123 <> 4
	MsgBox problem with numerically-named variables.

var = 1
var += 5.5  ; This is designed to detect a particular bug that might crop up in loadtime optimization of ACT_ADD/SUB/etc.
if (var <> 6.5)
	MsgBox %var%

Var = 2.5
Var += 3  ; Add int to float.
if (var <> 5.5)
	MsgBox %var%

var = 1
var2 = 5abc
var += %Var2%  ; Impure numbers, at least inside vars, are historically allowed for ACT_ADD/SUB/etc.
if (var <> 6)
	MsgBox %var%

var =
var += 5.1, days
var -= , days
if (Var <> 5)
	MsgBox %Var%

Array1 = 0
i = 1
Array%i%++
if (Array%i% <> 1)
	MsgBox % Array%i%
++Array%i%
if (Array%i% <> 2)
	MsgBox % Array%i%
Array%i%+=2
if (Array%i% <> 4)
	MsgBox % Array%i%
Array%i%-=6
if (Array%i% <> -2)
	MsgBox % Array%i%

Var = abc
Var :=  ; Not sure if this was originally intended to work, but tested for backward compat.
if (Var <> "")
	MsgBox %var%

LongVar = 01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789  ; 500 characters.
if (abc%longvar% <> "")
	MsgBox % abc%longvar%
if A_AhkVersion > 1.0.47.07 ; This is checked because the following DYNAMIC CALL will crash versions 1.0.47.06 or older.
	if (abc%longvar%() <> "")  ; Try to call an dynamic function with a name too long.
		MsgBox % abc%longvar%  ; I think the above line ABORTS the expression internally, so it resolves to: if ("")

;;;;;;

; TEST SOME UNUSUAL CONCAT SITUATIONS THAT WERE ONCE BROKEN:

v := ""
v := (v := "a" . "b") . "c" 
if v <> abc
	msgbox "%v%"

v := ""
v := "c" . (v := v . "a" . "b")
if v <> cab
	msgbox "%v%"

;ALREADY WORKED, BUT WOULDN'T CONTINUE TO WORK UNLESS done_and_have_an_output_var WAS USED IN *BOTH* PLACES.
v := ""
v := (v := v . "a") . "b"
if v <> ab
	msgbox "%v%"

var2 = xyz
var := var2 .= "abc"
if (Var <> "xyzabc" or Var2 <> Var)
	MsgBox %Var%`n%Var2%

var3 = xyz  ; Start a new var to force AppendIfRoom() to not find enough space.
var3 .= "abc"
if (Var3 <> "xyzabc")
	MsgBox %var3%
var3 .= var3  ; Just to ensure it's handled gracefully.
if (Var3 <> "xyzabcxyzabc")
	MsgBox %var3%
Var3 := "123" . Var3
if (Var3 <> "123xyzabcxyzabc")
	MsgBox %var3%

var := "xyz"
var3 := &(var.="abc")
if (!Var3)
	MsgBox %Var3%
if (Chr(*Var3) != "x")
	MsgBox % Chr(*Var3)
 

;;;;;;;;

Var = 0123456789
Var2 := Var
if (Var2 <> "0123456789")
	MsgBox "%Var2%" wrongly lost its leading zero.

Var = +123456789
Var2 := Var
if (Var2 <> "+123456789")
	MsgBox "%Var2%" wrongly lost its leading '+'.

Var = .5
Var2 := Var
if (Var2 <> ".5")
	MsgBox "%Var2%" wrongly lost its leading '.'

Var = 123456789012345678901234567890  ; An integer beyond the max size of an integer.
Var2 := Var
if (Var2 <> "123456789012345678901234567890")
	MsgBox "%Var2%" lost some of its digits.

;;;;;;

; SIMILAR TO ABOVE SECTION BUT FOR := vs. =
Var := "0123456789"
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "0123456789")
	MsgBox "%Var2%" wrongly lost its leading zero.

Var := "+123456789"
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "+123456789")
	MsgBox "%Var2%" wrongly lost its leading '+'.

Var := ".5"
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> ".5")
	MsgBox "%Var2%" wrongly lost its leading '.'

Var := "5."  ; Trailing decimal point.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "5.")
	MsgBox "%Var2%" wrongly lost its trailing '.'

Var := "1.0e1"  ; Scientific notation.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "1.0e1" || Var2 == "1")
	MsgBox "%Var2%" wrongly lost its trailing '.'

; The following isn't checked.  See comments in program for Assign(Var...)
;Var := "123456789012345678901234567890"  ; An integer beyond the max size of an integer.
;if (Var > 5)  ; This caches the integer in Var.
	;Sleep -1
;Var2 := Var
;if (Var2 <> "123456789012345678901234567890")
	;MsgBox "%Var2%" lost some of its digits.

;;;;;;;;

; SAME AS ABOVE BUT USES VAR=%VAR% METHOD, AND HAS LEADING & TRAILING SPACES (ALONG WITH UNUSUAL NUMBER FORMATS):

AUTOTRIM OFF

Var := "  0123456789  "
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "  0123456789  ")
	MsgBox "%Var2%"

Var := "  +123456789  "
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "  +123456789  ")
	MsgBox "%Var2%"

Var := "  .5  "
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "  .5  ")
	MsgBox "%Var2%"

Var := "  5.  "  ; Trailing decimal point.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "  5.  ")
	MsgBox "%Var2%"

Var := "  1.0e1  "  ; Scientific notation.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "  1.0e1  " || Var2 == "1")
	MsgBox "%Var2%"

;;;;;;;;


; SAME AS ABOVE EXCEPT FOR LINE BELOW.
AUTOTRIM ON  ; Put it back to default.

Var := "  0123456789  "
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "0123456789")  ; Loses its whitespace because autotrim is on, but keeps it leading zero.
	MsgBox "%Var2%"

Var := "  +123456789  "
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "+123456789")  ; Loses its whitespace because autotrim is on, but keeps it leading plus.
	MsgBox "%Var2%"

Var := "  .5  "
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> ".5")  ; Loses its whitespace because autotrim is on, but keeps it leading period.
	MsgBox "%Var2%"

Var := "  5.  "  ; Trailing decimal point.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "5.")  ; Loses its whitespace because autotrim is on, but keeps it trailing period.
	MsgBox "%Var2%"

Var := "  1.0e1  "  ; Scientific notation.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 = %Var%
if (Var2 <> "1.0e1" || Var2 == "1")  ; Loses its whitespace because autotrim is on, but keeps scientific format.
	MsgBox "%Var2%"

;;;;;

; SIMILAR TO ABOVE SECTION BUT QUOTES OMITTED FROM EACH TOP LINE:
Var := 0123456789
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "0123456789")
	MsgBox "%Var2%" wrongly lost its leading zero.

Var := +123456789
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "+123456789")
	MsgBox "%Var2%" wrongly lost its leading '+'.

Var := .5
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> ".5")
	MsgBox "%Var2%" wrongly lost its leading '.'

Var := 5.  ; Trailing decimal point.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "5.")
	MsgBox "%Var2%" wrongly lost its trailing '.'

Var := 1.0e1  ; Scientific notation.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "1.0e1" && Var2 == "1")
	MsgBox "%Var2%" wrongly lost its trailing '.'

; Slightly different:
Var := 1.0  ; Low precision.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "1.0")
	MsgBox "%Var2%" <> "1.0"

; Slightly different:
Var := 1.12345678  ; High precision.
if (Var > 5)  ; This caches the integer in Var.
	Sleep -1
Var2 := Var
if (Var2 <> "1.12345678")
	MsgBox "%Var2%" <> "1.12345678".  Although no precision was lost internally, some digits of possible use to the script were lost from the string contents of the variable.

; Slightly different:
Var := 123456789012345678901234567890  ; An integer beyond the max size of an integer.
Var2 := Var
if (Var2 <> "123456789012345678901234567890")
	MsgBox "%Var2%" lost some of its digits.


;;;;;;;;;;;;;;;;;;;;;

Var = 123abc
var = %var%  ; Assign to self just to ensure nothing weird happens.
if (Var <> "123abc")
	MsgBox "%Var%"

Var = 123abc
var := var  ; Assign to self just to ensure nothing weird happens.
if (Var <> "123abc")
	MsgBox "%Var%"

Var := " 123 "  ; Spaces around it.
if (Var < 300)  ; Force var to acquire a cached integer.
	sleep -1
Var2 := Var  ; Var-to-var copy.
if (Var2 <>  " 123 ")
	MsgBox "%Var2%"  ; Spaces were wrongly trimmed because := doesn't obey AutoTrim.

Var := " 123 "  ; Spaces around it.
if (Var < 300)  ; Force var to acquire a cached integer.
	sleep -1
Var2 = %Var%  ; Var-to-var copy.
if (Var2 <>  "123")
	MsgBox "%Var2%"  ; Spaces should have been trimmed.

AutoTrim Off
Var := " 123 "  ; Spaces around it.
if (Var < 300)  ; Force var to acquire a cached integer.
	sleep -1
Var2 = %Var%  ; Var-to-var copy.
if (Var2 <>  " 123 ")
	MsgBox "%Var2%"  ; Spaces were wrongly trimmed.
AutoTrim On

Var3 := " 123 "  ; Spaces around it.
if (Var3 < 300)  ; Force var to acquire a cached integer.
	sleep -1
Addr := &Var4  ; Take address to change the internal caching behavior of var4.
Var4 := Var3  ; Var-to-var copy.
if (Var4 <>  " 123 ")
	MsgBox "%Var4%"  ; Spaces were wrongly trimmed.

; Compare numbers or ambiguous-type items to pure strings:
Var = 11
if not (Var . 33 == "1133")
	MsgBox %Var%
if not (11 . 33 == "1133")
	MsgBox %Var%
if not ("11" . 33 == "1133")
	MsgBox %Var%
var = 12
if (var == "12.0")
	MsgBox Strings should be compared as strings not numbers?
var = .0
if (var ? 1 : 0)  ; Once inside a variable, strings that look like zero are considered false.
	MsgBox %var%
if not ("." . "0" ? 1 : 0)
	MsgBox Concat-built strings are currently false only when completely empty.

if IsFunc("Tan") <> 2 || IsFunc("LV_Add") <> 1 || IsFunc("TestForIsFunc") <> 3
	MsgBox IsFunc("Tan") <> 2 || IsFunc("LV_Add") <> 1 || IsFunc("TestForIsFunc") <> 3
TestForIsFunc(x, y, z = 5)
{
}

Var = 1
if Dyn%Var%("test") <> "test"
	MsgBox Problem with dynamic function call.
++Var
if Dyn%Var%("test", "extra/unused param", extra_param := "yes it was evaluated") <> "xyz"
	MsgBox Problem with dynamic function call.
if (extra_param <> "yes it was evaluated")
	MsgBox (extra_param <> "yes it was evaluated")
if Dyn%Var%() <> ""  ; Too few parameters passed, which should be an error, which resolves it to "".
	MsgBox Problem with dynamic function call.
Var = RegExMatch
if %Var%("xxxabc123xyz", "abc.*xyz") <> 4
	MsgBox Problem with dynamic function call.
Var = Dyn3
%Var%("::")  ; Checking for hotstring/hotkey ambiguity.
if (Var <> "new string")
	MsgBox Problem with dynamic function call.
Var = DynByRef
Var := %Var%("xyz")
if (Var <> "")
	MsgBox Problem with dynamic function call.
Var = DynByRef
Var2 := "xyz"
Var := %Var%(Var2)
if (Var <> "xyz")
	MsgBox Problem with dynamic function call.

Loop
{
	if Dyn%A_Index%("test") <> "test"
		MsgBox Problem with dynamic function call.
	break
}

Dyn1(x)
{
	return x
}
Dyn2(x)
{
	return "xyz"
}
Dyn3(x)
{
	global Var
	Var := "new string"
}
DynByRef(ByRef x)
{
	return x
}

; Added to battery in case it changes:
; Evaluation order of a function's parameter list actually IS left-to-right (like multi-statement) but the unexpected behavior below is due to the fact that "i" is left as SYM_VAR until the last minute (when the function is called):
i = 0
check_side_effects(i, i++, 1, 0)
check_side_effects(++i, ++i, 3, 3)
check_side_effects(i--, i++, 3, 2)  ; Demonstrates through side effects that parameters are evaluated left-to-right.
check_side_effects(x, y, x1, y1)  ; When thinking about the unexpected result, consider the possibility that X is byref instead.  In other words, the fact that "i" is left as sym_var is really a means to allow byref to work without complicated code (sym_var might also serve other purposes besides byref)
{
	if (x <> x1 || y <> y1)
	{
		ListLines
		MsgBox Unexpected side effect.
	}
}

; v1.0.47.04:
myNumber  := 1043.22
Var := Round( myNumber, -1 )
DllCall("LoadLibrary", "str", "rmchart.dll")  ; Somehow necessary to trigger the bug.
myNumber  := -1043.22
Var2 := Round( myNumber, -1 )
if (Var <> 1040 || Var2 <> -1040)
	Gosub Error

; v1.0.47.03: bugfixs:
if (-0xe+1 <> -13)
	Gosub Error
if (false)
{
} nocomma()
nocomma()  ; Do nothing because all we want to do is make sure above isn't a loadtime syntax error.
{
}


; v1.0.47.01: Relation operators changed to yield integers vs. floats when inputs are float.
Var := 1.0 < 2.0
if (var <> "1")
	Gosub Error
Var := 3.0 < 2.0
if (var <> "0")
	Gosub Error


; WHILE-LOOPS

var = 0
while var < 10
	++var
if var <> 10
	MsgBox %var%
while A_Index < 10
{
	++var
}
if var <> 19
	MsgBox %var%
x = Var
While %x%
{
	--Var
	if A_Index > 25  ; This must be checked here and not in the loop to catch this likely bug or undesirable behavior.
		break
}

if Var <> 0
	MsgBox %Var%
i = 1
Array1 = 5
While Array%i%
	--Array%i%
if (Array1 <> "0")
	MsgBox %Array1%
while 1
	if (++var) > 5
		break
while (true)
	if (++var) > 10
		break
if Var <> 11
	MsgBox %Var%
while false
	if (++var) > 20
		break
if Var <> 11
	MsgBox %Var%
while 0
	if (++var) > 20
		break
if Var <> 11
	MsgBox %Var%
while true {
	if (++var) > 20
		break
}
if Var <> 21
	MsgBox %Var%
while A_Index < 5 {
	if (++var) > 30
		break
	continue  ; Ensure continue causes A_Index to get incremented.
}
if Var <> 25
	MsgBox %Var%
finished := false
while not finished
	if (++var) > 40
		finished := true
if (Var <> 41 || not finished)
	msgbox (Var <> 41 || not finished)


; CALLBACKS

Var := "x"
if (Var := RegisterCallback(""))
	Gosub Error
if (Var := RegisterCallback("Nonexistent"))
	Gosub Error
if (Var := RegisterCallback("InStr"))  ; Using a BIF is illegal.
	Gosub Error
if (Var := RegisterCallback("CB0", "", 32))  ; too many parameters.
	Gosub Error
if (Var := RegisterCallback("CB0", "", 1))  ; too many parameters but only because not enough formals.
	Gosub Error
if (Var := RegisterCallback("CB1", "", 3))  ; too many parameters to CB1 because not enough formals.
	Gosub Error
if (Var := RegisterCallback("CB0", "", -1))  ; negative parameters is the same as too few.
	Gosub Error
if (Var := RegisterCallback("CB1", "", 0))  ; too few parameters to CB1.
	Gosub Error
if (Var := RegisterCallback("CB_ByRef"))  ; ByRef formals not allowed.
	Gosub Error

if not (Var := RegisterCallback("CB0"))
	Gosub Error
else
{
	Var2 := DllCall(Var)
	if ErrorLevel
		MsgBox ErrorLevel = %ErrorLevel%
	if (Var2 <> Var)
		Gosub Error
}

if not (Var := RegisterCallback("CB0", "", 0, 111))
	Gosub Error
else
{
	Var := DllCall(Var)
	if ErrorLevel
		MsgBox ErrorLevel = %ErrorLevel%
	if (Var <> 111)
		Gosub Error
}

if not (Var := RegisterCallback("CB1"))  ; It will use 1 parameter because 1 is the minimum.
	Gosub Error
else
{
	Var2 := DllCall(Var, uint, 123)
	if ErrorLevel
		MsgBox ErrorLevel = %ErrorLevel%
	if (Var2 <> 123+1+Var)
		Gosub Error
}

if not (Var := RegisterCallback("CB1", "", 1))  ; It will use 1 parameter because 1 is the minimum.
	Gosub Error
else
{
	Var2 := DllCall(Var, uint, 128)
	if ErrorLevel
		MsgBox ErrorLevel = %ErrorLevel%
	if (Var2 <> 128+1+Var)
		Gosub Error
}

if not (Var := RegisterCallback("CB1", "", 2, 11))
	Gosub Error
else
{
	Var2 := DllCall(Var, uint, 123, uint, 22)
	if ErrorLevel
		MsgBox ErrorLevel = %ErrorLevel%
	if (Var2 <> 123+11+22)
		Gosub Error
}

if not (Var := RegisterCallback("CB_CDecl", "CDecl", 2, 11))
	Gosub Error
else
{
	; Don't do the following because it seems to destabilize things:
	;Var := DllCall(Var, uint, 123, uint, 22)
	;if not ErrorLevel
		;MsgBox An ErrorLevel was expected due to absence of CDecl.

	Var2 := DllCall(Var, uint, 123, uint, 22, "CDecl")
	if ErrorLevel
		MsgBox ErrorLevel = %ErrorLevel%
	if (Var2 <> 123+11+22)
		Gosub Error
}

CB0()
{
	; CAN'T, IT ONLY SUPPORTS INTEGER RETURN VALUES: return A_ThisFunc . "-" . A_EventInfo
	return A_EventInfo
}
CB1(Param1, Param2 = 1)
{
	return A_EventInfo + Param1 + Param2
}
CB_CDecl(Param1, Param2 = 1)
{
	return A_EventInfo + Param1 + Param2
}
CB_ByRef(Param1, ByRef Param2)
{
	return 0
}

if not EnumWindowsProcStub:=RegisterCallback("EnumWindowsProc")
	MsgBox Failed
else
{
	out:=
	DllCall("EnumWindows","UInt",EnumWindowsProcStub,"UInt",111)
	
	EnumWindowsProc(winid, otherparam)
	{
		if otherparam <> 111
			Msgbox %otherparam%
		global out
		DetectHiddenWindows, On
		WinGetTitle, text, ahk_id %winid%
		WinGetClass, class, ahk_id %winid%
		out.=winid . "`t" . text . "`t" . class . "`n"
		return 1  ; Tell it to continue the enumeration.
	}
	if strlen(out) < 200
		MsgBox "%out%"
}

WindowProcStub := RegisterCallback("WindowProc")
Gui, 2:Add, Edit, HwndMyEditHwnd vMyEdit, Test
Gui 2:Show
Sleep 0

Var := 0
oldwinproc2 := DllCall("GetClassLong", uint, MyEditHwnd, int, -24, uint)  ; -24 is GCL_WNDPROC
oldwinproc := DllCall("SetWindowLong", Uint, MyEditHwnd, int, -4, int, WindowProcStub, uint)  ; -4 is GWL_WNDPROC
GuiControl, 2:, MyEdit, abcd

WindowProc(hwnd, imsg, wparam, lparam)
{
	critical ; avoid interruptions of itself.
	global oldwinproc
	global Var
	++Var
	result := DllCall("CallWindowProcA", uint, oldwinproc, uint, hwnd, uint, imsg, uint, wparam, uint, lparam)  ; WindowProc are callbacks, which are stdcall vs. cdecl.

	static e = 0
	if ErrorLevel
	{
		;++e
		;ToolTip %e%
		MsgBox %ErrorLevel%
	}
	return result
}

sleep 10
if (Var < 1 || var > 500)
	msgbox %var%
Gui 2:Destroy


; NUMPUT AND NUMGET:

SetFormat, integer, hex

Var := "AAAABBBB"
if NumGet(&var) <> 0x41414141
	Gosub Error
if NumGet(var) <> 0x41414141
	Gosub Error
if NumGet(var, 0) <> 0x41414141
	Gosub Error
if NumGet(&var + 4) <> 0x42424242
	Gosub Error
if NumGet(var, 4) <> 0x42424242
	Gosub Error
Var2 := &var
if NumGet(Var2 + 0, 4) <> 0x42424242  ; Use +0 to force contents of var to be used as the address.
	Gosub Error
if NumGet(&var, 4, "Uint") <> 0x42424242
	Gosub Error
if NumGet(var, 4, "Uint") <> 0x42424242
	Gosub Error

Var := "€€€€€€€€"  ; € is 0x80

if NumGet(var) <> 0x80808080
	Gosub Error
if NumGet(var, 4, "Uint") <> 0x80808080
	Gosub Error
if NumGet(var, 4, "int") <> -0x7F7F7F80
	Gosub Error
if NumGet(var, 5, "int") <> 0x808080  ; Final 3 bytes in string together with zero terminator (big- vs. little-endian stuff).
	Gosub Error

if NumGet(var, 4, "Ushort") <> 0x8080
	Gosub Error
if NumGet(var, 4, "short") <> -0x7F80
	Gosub Error
if NumGet(var, 7, "short") <> 0x80  ; Final byte in string together with zero terminator (big- vs. little-endian stuff).
	Gosub Error

if NumGet(var, 4, "Uchar") <> 0x80
	Gosub Error
if NumGet(var, 4, "char") <> -0x80
	Gosub Error
if NumGet(var, 5, "char") <> -0x80
	Gosub Error

if NumGet(var, 0, "int64") <> -0x7F7F7F7F7F7F7F80
	Gosub Error
if NumGet(var, 1, "int64") <> 0x0080808080808080 ; Final 3 bytes in string together with zero terminator (big- vs. little-endian stuff).
	MsgBox % NumGet(var, 1, "int64")

if NumPut(-1, Var) <> &Var + 4  ; Test wrap-around.
	MsgBox % "1: " . NumPut(-1, Var)
if NumGet(var) <> 0xFFFFFFFF
	MsgBox % "2: " . NumGet(var, 4)
if NumGet(var, 0, "int") <> -1
	MsgBox % "3: " . NumGet(var, 0, "int")
if NumPut(-1, Var, 0, "UShort") <> &Var + 2  ; Test wrap-around.
	Gosub Error
if NumGet(var, 0, "UShort") <> 0xFFFF
	Gosub Error
if NumGet(var, 0, "Short") <> -1
	Gosub Error

var_capacity := VarSetCapacity(Var, 16, 65)  ; This inits the entire/actual capacity, not just the requested capacity.
if NumGet(var, var_capacity-4) <> 0x41414141
	MsgBox % NumGet(var, var_capacity-4)
if NumGet(var, var_capacity-3) <> 0x00414141  ; Barely within bounds (zero terminator is considered in-bounds).
	MsgBox % NumGet(var, var_capacity-3)
if NumGet(var, var_capacity-2) <> ""  ; Out-of-bounds should be detected and yield a blank value.
	MsgBox % NumGet(var, var_capacity-2)
if NumGet(0) <> ""  ; Explicit NULL address also should be caught.
	Gosub Error
if NumPut(0, 0) <> ""
	Gosub Error

VarSetCapacity(Var, 16, 0)
NumGetPut(0x41414141, Var, 0, "UInt", 4)
NumGetPut(0x41414141, Var, 4, "UInt", 4)
NumGetPut(0, Var, 4, "UInt", 4)
NumGetPut(0x80808080, Var, 4, "UInt", 4)
NumGetPut(-0x80000000, Var, 4, "Int", 4)
NumGetPut(0x80, Var, 4, "Short", 2)
NumGetPut(0x80, Var, 4, "UShort", 2)
NumGetPut(-0x7F, Var, 6, "Char", 1)
NumGetPut(0x7F, Var, 6, "UChar", 1)
NumGetPut(2.0, Var, 4, "Int", 4)
NumGetPut(2.5, Var, 8, "Double", 8)
NumGetPut(2.5, Var, 4, "Float", 4)
NumGetPut(257, Var, 0, "Char", 1, 1)  ; Deliberate overflow/wrap.
NumGetPut(256, Var, 0, "Char", 1, 0)  ; Deliberate overflow/wrap.
NumGetPut(0x10000, Var, 0, "Short", 2, 0)  ; Deliberate overflow/wrap.
NumGetPut(0x10002, Var, 0, "Short", 2, 2)  ; Deliberate overflow/wrap.

NumGetPut(Num, ByRef Var, Offset, Type, Size, ExpectedGet = "")
{
	if (ExpectedGet = "")
		ExpectedGet := Num
	SetFormat, integer, hex  ; Helps with debugging.
	put_result := NumPut(Num, Var, Offset, Type)
	VarSetCapacity(Var, -1)  ; To help with debugging via ListVars.
	expected := &Var + Offset + Size
	if (put_result <> expected)
		MsgBox % "put_result = " . put_result . ", which isn't the expected value of " . expected
	get_result := NumGet(Var, Offset, Type)
	if (get_result <> ExpectedGet)
		MsgBox % "get_result = """ . get_result . """, which isn't the expected value of " . Num
	SetFormat, integer, D
}

SetFormat, integer, D


; v1.0.46.16: TEST A_THISLABEL (THISFUNC TESTED LATER ON):
if (A_ThisLabel <> "label3")
	MsgBox A_ThisLabel is "%A_ThisLabel%"
Gosub Sub1
Goto JumpOverSubs
Sub1:
if (A_ThisLabel <> "Sub1")
	MsgBox "%A_ThisLabel%"
Gosub Sub2
if (A_ThisLabel <> "Sub1")
	MsgBox "%A_ThisLabel%"
return
Sub2:
if (A_ThisLabel <> "Sub2")
	MsgBox "%A_ThisLabel%"
return
JumpOverSubs:
if (A_ThisLabel <> "JumpOverSubs")
	MsgBox "%A_ThisLabel%"

; v1.0.46.13: SUPPORT FOR QUOTED/LITERAL STRINGS OTHER THAN ""
if StrFunc() <> "test"
	Gosub Error
StrFunc(x = "test")
{
	if (A_ThisFunc <> "StrFunc")
		MsgBox "%A_ThisFunc%"
	Add(2, 2)
	if (A_ThisFunc <> "StrFunc")
		MsgBox "%A_ThisFunc%"
	return x
}

if StrFunc2() <> "test" . """" . "apple" . """" . "ab,c,d)" . "abc`tdef`nxyz"
	Gosub Error
StrFunc2(x = "test""apple""", y = "ab,c,d)", z = "abc`tdef`nxyz")
{
	return x . y . z
}


; OPTIONAL BY-REF PARAMETERS:

Var2 = 2
Var3 = 3
Var4 = 4
if OptByRef(Var2, Var3, Var4) <> 9 || Var3 <> 2 || Var4 <> 5
	Gosub Error
Var3 = 3
Var4 = 4
if OptByRef(Var3, Var4) <> 5 || Var4 <> 3
	Gosub Error
Var3 = 3
if OptByRef(Var3) <> "xyz"
	Gosub Error
Var2 = 2
Var3 = 3
Var4 = 4
if OptByRef(Var2, Var3, Var4) <> 9 || Var3 <> 2 || Var4 <> 5  ; RETEST to stress it and try to break it.
	Gosub Error

; OPTIONAL BYREF PARAMETERS:
OptByRef(x, ByRef r1 = "", ByRef r2 = -1)
{
	if r1
		--r1
	if r2 <> -1
		++r2
	if (r1 == "")
		return "xyz"
	return x + r1 + r2
}

if (bug_1_0_46_09("ab") <> "ab")
	MsgBox bug_1_0_46_09

bug_1_0_46_09(p)
{
	static sCount = 0
	++sCount
	if sCount > 1
	{
		;listvars
		;pause
		return p
	}
	y := 2, x := bug_1_0_46_09(p)
	return x
}

; Same as above test but do it thorugh an intermediary function-call:
if (bug_1_0_46_09_B("ab") <> "ab")
	MsgBox bug_1_0_46_09_B

bug_1_0_46_09_B(p)
{
	static sCount = 0
	++sCount
	if sCount > 1
	{
		;listvars
		;pause
		return p
	}
	y := 2, x := bug_1_0_46_09_intermediary(p)
	return x
}

bug_1_0_46_09_intermediary(p)
{
	return bug_1_0_46_09_B(p)
}

; Ensure two UDFs that both need deref bufs, in the same expression, work ok:
VarSetCapacity(Var3, 1025, 65)
var := buf() . buf()
if (var <> "abc" . Var3 . "xyzabc" . Var3 . "xyz")
	Gosub Error
var2 := "", var := 123, var := buf()  ; Hits a particular optimization.
if (var <> "abc" . Var3 . "xyz" || var2 <> "")
	Gosub Error
buf()
{
	VarSetCapacity(x, 1025, 65)
	return "abc" . x . "xyz"
}


; UNDOCUMENTED EVALUATION ORDER IS LEFT-TO-RIGHT WHEN NOT OTHERWISE GOVERNED BY PRECEDENCE TABLE:
Var = 1
if (++Var + 2*Var) <> 6
	Gosub Error
Var = 1
if (2*Var + ++Var) <> 4
	Gosub Error
Var = 44
prec_func(Var:=33, Var)
prec_func(x, y)
{
	if (x <> 33 || y <> 33)
		Gosub Error
}


; BUGS FIXED IN V1.0.46.02:

StringLen, Var, ALLUSERSPROFILE
if (Var < 1 || var <> strlen(ALLUSERSPROFILE))
	Gosub Error

StringReplace, Var, ALLUSERSPROFILE, \All Users
if substr(Var, 4) <> "Documents and Settings"
	Gosub Error

; RELATED BEHAVIORS:
number_of_processors += 2
if number_of_processors <> 2  ; Environment variables are treated as blank in these cases (due to obscurity/rarity).
	Gosub Error

number_of_processors =
var:=2, number_of_processors += 4  ; Deep in expressions, env vars aren't supported as assignment targets.  And since math performed on a blank value yields a blank, we get a blank.
envget, num, number_of_processors
if (var <> 2 || number_of_processors <> num)
	Gosub Error

ALLUSERSPROFILE =
ALLUSERSPROFILE .= "xyz"
if (ALLUSERSPROFILE <> "xyz")
	Gosub Error



; THINGS DELIBERATELY NOT SUPPORTED (due to obscurity/complexity) AND WHICH THUS HAVE INCONSISTENT OR COUNTERINTUITIVE BEHAVIOR:

Var2 = 1
Var := ++Var2 + ++Var2  ; This is a quirk of the fact that pre-op produces an lvalue.  See comments in code for why.
if Var <> 6
	Gosub Error

; THIS FIRST IS JUST FOR ILLUSTRATION.  Rightmost = is a comparison.
Var:="", Var1:="", Var2:=2, Var3:=3, Var4:=""
Var := 1, Var1 = (Var2 = Var3)
if (Var <> 1 || Var1 <> 0 || Var2 <> 2 || Var3 <> 3)
	Gosub Error

Var:="", Var1:="", Var2:=2, Var3:=3, Var4:=4, Var5=5
Var := 1, Var1 = (Var2 = Var3, Var4 = Var5)  ; YIELDS DELIBERATELY INCONSISTENT RESULT BECAUSE NOT WORTH SUPPORTING.
if (Var <> 1 || Var1 <> 0 || Var2 <> 2 || Var3 <> 3 || Var4 <> 5 || Var5 <> 5)
	Gosub Error

Var:="", Var1:="", Var2:=2, Var3:=3, Var4:=4, Var5=5
Var := 1, Var1 = fn1((Var2 = Var3, Var4 = Var5))  ; Multi-statement not supported when nested in a function call.
if (Var <> 1 || Var1 <> "" || Var2 <> 2 || Var3 <> 3 || Var4 <> 4 || Var5 <> 5)
	Gosub Error


; TEST AUTO-RECOGNITION OF = AS BEING := after a comma (as well as cascading such as x=y=z)

Var:="", Var1:="", Var2:="", Var3:="", Var4:=""
Var := 1, Var1 = Var2 = Var3 = Var4 = Add(5,2)
if (Var <> 1 || Var1 <> 7 || Var2 <> 7 || Var3 <> 7 || Var4 <> 7)
	Gosub Error
Var:="", Var1:="", Var2:="", Var3:="", Var4:=""
Var := 1, Var1 = Var2 = Var3 = (Var4 = Add(5,2))
if (Var <> 1 || Var1 <> 0 || Var2 <> 0 || Var3 <> 0 || Var4 <> "")
	Gosub Error
Var:="", Var1:="", Var2:="", Var3:="", Var4:=7
Var := 1, Var1 = Var2 = Var3 = (Var4 = Add(5,2))  ; Parens are used to make last term a comparison.
if (Var <> 1 || Var1 <> 1 || Var2 <> 1 || Var3 <> 1 || Var4 <> 7)
	Gosub Error
Var:="", Var1:="", Var2:=3, Var3:=7, Var4:=7
Var := 1, Var1 = Var2 += Var3 = Var4
if (Var <> 1 || Var1 <> 4 || Var2 <> 4 || Var3 <> 7 || Var4 <> 7)
	Gosub Error
Var:="", Var1:="", Var2:=3, Var3:=7, Var4:=7
Var := 1, Var1 = Var2 += Var3 *= Var4
if (Var <> 1 || Var1 <> 52 || Var2 <> 52 || Var3 <> 49 || Var4 <> 7)
	Gosub Error
Var:="", Var1:="", Var2:=2, Var3:=7, Var4:=7
Var := 1, Var1 = Var2 = Var3 ? 4 : 5  ; Even more ambiguous due to ternary on right, but for consistency all are assign.
if (Var <> 1 || Var1 <> 4 || Var2 <> 4 || Var3 <> 7)
	Gosub Error

Var:= "", Var2:=""
Var := 1, Var2 = 2 ? 5 : 6  ; DELIBERATELY (FOR CONSISTENCY) RECOGNIZE "Var2=2" as an assignment rather than a comparison 
if (Var <> 1 || Var2 <> 5)
	Gosub Error
Var:= "", Var2:=1, fn1:="", fn2:=""
Var := 1, (Var2 = 2) ? fn1() : fn2()  ; Same but use parens to force a comparison, which forces the ternary to be stand-alone.
if (Var <> 1 || Var2 <> 1 || fn1 || !fn2)
	Gosub Error
Var:= "", Var2:=1, fn1:="", fn2:=""
Var := 1, 2 = Var2 ? fn1() : fn2()  ; Same but use "2=Var" to force a comparsion vs. assignment.
if (Var <> 1 || Var2 <> 1 || fn1 || !fn2)
	Gosub Error
Var:= "", Var2:=1, fn1:="", fn2:=""
Var := 1, Var2 == 2 ? fn1() : fn2()  ; Same but use == vs = to force comparison.
if (Var <> 1 || Var2 <> 1 || fn1 || !fn2)
	Gosub Error

Var := Add(2, (2, 3))  ; Statement-separators nested inside a function call not currently supported (see comments in code).
if (Var <> "")
	Gosub Error

Var1:="", Var2:=""
Var := Add(2, (Var1:=3, Var2:=4))  ; Similar to above but more elaborate.
if (Var <> "" || Var1 <> 3, Var2 <> 4)  ; i.e. the assignments get done but then the expression fails.
	Gosub Error

; Ensure '=' isn't misinterpreted as an assignment when the preceding commas is a function-comma vs. statement=separator.
Var1 = 2
Var2 = 3
if Add(2, var1 = var2) <> 2 || Var1 <> 2
	Gosub Error

Var2:=""
Var := 1, Var2 = 2
if (Var <> 1 || Var2 <> 2)
	Gosub Error

Var2:=""
Var := 1, Var2 = 2 ? 5 : 6  ; DELIBERATELY (FOR CONSISTENCY) RECOGNIZE "Var2=2" as an assignment rather than a comparison that is used as the condition for the ternary.
if (Var <> 1 || Var2 <> 5)
	Gosub Error

Var2:=77, fn1 := false, fn2 := false
Var := 1, (Var2 = 2) ? fn1() : fn2()  ; Same but use parens to force a comparison, which forces the ternary to be stand-alone.
if (Var <> 1 || Var2 <> 77 || fn1 || !fn2)
	Gosub Error

Var2:=77, fn1 := false, fn2 := false
Var := 1, Var2 == 2 ? fn1() : fn2()  ; Same but == instead of parens to resolve ambiguity.
if (Var <> 1 || Var2 <> 77 || fn1 || !fn2)
	Gosub Error

Var2:=77, fn1 := false, fn2 := false
Var := 1, 2 = Var2 ? fn1() : fn2()  ; Same reverse the comparsion order to resolve ambiguity.
if (Var <> 1 || Var2 <> 77 || fn1 || !fn2)
	Gosub Error


; TEST INTIALIZERS FOR STATIC VARS IN FUNCTIONS

static_var()
static_var()  ; Called again to test subsequent calls.
static_var()
{
	static first_call:=true, x = 1, y = 2.2, z := "", b, a:="xyz", c = """apple""", d = "`n`t"
	if (first_call)
	{
		if (x <> 1 || y <> 2.2 || z <> "" || b <> "" || a<>"xyz" || c <> """apple""" || d <> "`n`t")
			MsgBox Static initializer problem.
		a:="changed"  ; Used by next section.
		first_call := false
		return
	}
	; Otherwise, this is the 2nd call:
	if (a <> "changed")
		MsgBox Static initializer was re-init'd when it's only supposed to be init'd once?
}


; TEST INITIALIZERS INSIDE DECLARATION LISTS

Var1 := "x", Var2 := "x"  ; Init to detect problems with globals vs. locals.
VarGlobal = "g"

TestInit(1, 2)
if (Var1 <> "x" || Var2 <> "x" || VarGlobal <> 11 || VarGlobal2 <> "Global")
	Gosub Error

TestInit(x, y)
{
	local Var1 := 5, Var2 = add(1, 2), Var3:=(5+4)/2, Var4=3+Var1//Var2
		,var5=false ? 2 : Add(1,Add(1,1))
		,var6:=VarGlobal:=add(5, 6)  ; Declares var6 as local but VarGlobal is a global.
	if (Var1 <> 5 || Var2 <> 3 || Var3 <> 4.5 || Var4 <> 4 || var5 <> 3 || var6 <> 11)
		Gosub Error

	Local L1=1  , L2=2
	if (L1 <> "1")  ; Formerly a bug.
		MsgBox Problem.
	static S1=1  , S2=2
	if (S1 <> "1")  ; Formerly a bug.
		MsgBox Problem.
	static S3 = "xyz  "   , S4 = 5
	local L3 = "abc  "   , L4 = "xxx"   ; Formerly wrongly displayed a syntax error.
	if (S3 <> "xyz  " || L3 <> "abc  ")
		MsgBox Problem.

	Var1 := -1, Var2 := -2  ; Should also be local, not global (caller will check them).
	VarGlobal2 := "Global"  ; Assigns to global.
	local VarGlobal2:="Local"
	if (VarGlobal2 <> "Local")
		MsgBox %VarGlobal2%  ; And now caller will check the global of the same name.
	if (true)
		local xx = 1, yy = 2
	else
		local aa = 1, bb = 2
	if (false)
		local xxx = 1, yyy = 2
	else
		local aaa = 1, bbb = 2
	if (true)
		xyz := 1, abc := 2
	else
		xyz := 3, abc := 4
	if (   xx <> 1 || yy <> 2 || aa <> "" || bb <> ""
		|| aaa <> 1 || bbb <> 2 || xxx <> "" || yyy <> ""
		|| xyz <> 1 || abc <> 2   )
		MsgBox Declaration or multi-statement comma problem.
}


; DETECT UNWANTED CHANGES TO COMMAND RECOGNITION AND PARSING:

Var = 1
Var2 = xyz
Var := Path, Var2.=Var
if not instr(var, "system32") || substr(var2, 1, 3) <> "xyz" || strlen(var2) < 8
	Gosub Error

Var := 0, Var2 := -1
Var := Var + 1, Var2 := Var
if (Var <> 1 || Var2 <> 1)
	Gosub Error

Var = 5
Var :=  ; Supported in v1.
if (Var <> "")
	Gosub Error

Var := ""
Var += 1+1
if Var <> 2
	Gosub Error
Var := ""
Var += 1+1, y := 3
if (Var <> "" || y <> 3)  ; Var is blank because the comma turns the entire thing into a pure expression (i.e. not ACT_ADD).
	Gosub Error

Var = 6
Var /= 2
if (Var <> "3")  ; Legacy EnvDiv behavior.
	Gosub Error
Var = 6
Var /= 2.0
if not instr(Var, "3.00")
	Gosub Error
Var = 6.0
Var /= 2
if not instr(Var, "3.00")
	Gosub Error
Var = 6
Var /= (4+4)//4  ; EnvDiv on an integer denominator.
if (Var <> "3")
	Gosub Error
Var = 6
Var /= (4+4)/4  ; EnvDiv on a floating pt denominator.
if not instr(Var, "3.00")
	Gosub Error
Var = 6
Var /= (4+4)//4, Var2=33  ; EnvDiv on an integer denominator, but it's NOT EnvDiv due to comma-separated statements.
if not instr(Var, "3.00") or var2<>33
	Gosub Error

Var := ""
Var += 2
if Var <> 2
	Gosub Error
Var := ""
Var += 2, Var2=44
if (Var <> "" || var2 <> 44)
	Gosub Error

Var2:=1, Var3:=2, Var4:=3
Var := Var2 == Var3 = Var4  ; Seen as Var = ((Var2 == Var3) = Var4) [two comparisons on right]
if (Var <> 0)
	Gosub Error
Var2:=2, Var3:=2, Var4:=1
Var := Var2 == Var3 = Var4  ; Seen as Var = ((Var2 == Var3) = Var4) [two comparisons on right]
if (Var <> 1)
	Gosub Error

Var := "", Var1 := "", Var2 = 3
Var := Var1 := (Var2 ? 2 : 3)
if (Var <> 2 || Var1 <> 2 || Var2 <> 3)
	Gosub Error

; Stand-alone expressions should allow a variety comma-separated statements:
fn1 := false, fn2 := false
fn1(), fn2()
if (!fn1 || !fn2)
	Gosub Error

fn1 := false, fn2 := false, Var := 0
true ? fn1() : 3, Var := 3
if (!fn1 || fn2 || Var <> 3)
	Gosub Error

Var := 44,, Var2 := 55  ; Works better due to v1.0.46.01 revisions.  OLDER BEHAVIOR: Double-comma, causes the 44 to be removed prematurely from the stack, preventing Var from being set to it.
if (Var <> 44 || Var2 <> 55)
	Gosub Error

fn1 := false, fn2 := false, Var := 0
Var := 4, false ? fn1() : fn2()  ; Converse of above; ensure precedence works right for it.
if (fn1 || !fn2 || Var <> 4)
	Gosub Error

fn1 := false, fn2 := false
Var:=3, Var2:=2
Var++, Var2--, fn1()
--Var, ++Var2
--Var, ++Var2
Var*=2, Var2/=2
if (!fn1 || fn2 || Var <> 4 || Var2 <> 1.5)
	Gosub Error

msgbox? = 2
if msgbox? <> 2
	MsgBox %msgbox?%

Clipboard := 5 + 2  ; This could be broken in the future if := ever becomes an internal/expr operator vs. command (since internally it doesn't support clipboard as an lvalue)
if Clipboard <> 7
	MsgBox %Clipboard%

Random, Var, x:=3, y:=4, z:=5  ; First two commas are param separators, but last one is a statement separator.
if (x <> 3 || y <> 4 || z <> 5)
	MsgBox Problem

IfWinActive ? 2  ; This is here just to catch unwanted parser changes.
{
}
else {  ; Should report "else with no if" if the above wasn't interpreted as a command.
}
IfWinActive . 2
{
}
else {
}
IfWinActive < 2
{
}
else {
}
IfWinActive : 2
{
}
else {
}
IfWinActive --  ; Post-decrement operator; but this should be seen as a command not a post-dec.
{
}
else {
}
IfWinActive--  ; Same as above (this behavior is for bk-compat and code simplicity)
{
}
else {
}
IfWinActive, := x  ; Deliberate use of comma to force it to be a command.
{
}
else {
}

IfWinActive = 3
IfWinActive .= 2  ; This is here just to catch unwanted parser changes.
if (IfWinActive <> "32")
	MsgBox %IfWinActive%

if msgbox? <> 2
	MsgBox %msgbox?%

Var = limit ? x:=1 : y:=1  ; This is almost an expression, and perhaps should be so in V2.
if (Var <> "limit ? x:=1 : y:=1")
	MsgBox %Var%



; COMMA AS A STATEMENT SEPARATOR

y := 0, x := 0
fn(1), x:=4  ; Bugfix in v1.0.46.11.
if (y <> 5 || x <> 4)
	MsgBox Problem.
fn(z)
{
	global y = 5
}

Var := 3, Var2 := 4
if (Var <> 3 || Var2 <> 4)
	MsgBox %Var%
Var := 3+2, Var2 := 4+5
if (Var <> 5 || Var2 <> 9)
	MsgBox %Var%

Var := 3+2 ? 7 : 8, Var2 := 4+5-9 ? 1 : 2
if (Var <> 7 || Var2 <> 2)
	MsgBox %Var%

Var := Add((5, 3), (2, 2))  ; Current implementation doesn't allow multi-statement to be nested inside function calls.
if (Var <> "")
	Gosub Error

Var := 3+2 ? add(3,4) : add(4,4), Var2 := add(4,5)-9 ? add(0,1) : add(1,1)
if (Var <> 7 || Var2 <> 2)
	MsgBox %Var%

Var:=1, 2   ; Due to precedence, seen as (Var:=1), 2
if Var <> 1
	MsgBox Problem

Var:="", Var1:="", Var2:="", Var3:=""
Var1:= Var2:=1, Var3:=2
if (Var1 <> 1 || Var2 <> 1 || Var3 <> 2)
	MsgBox Problem.

Var:="", Var1:="", Var2:="", Var3:=""
Var1:= (Var2:=2, Var3:=3)
if (Var1 <> 2 || Var2 <> 2 || Var3 <> 3)
	MsgBox Problem.

Var:="", Var1:="", Var2:="", Var3:=""
Var := 5, Var1:= (Var2:=1, Var3:=2)
if (Var <> 5 || Var1 <> 1 || Var2 <> 1 || Var3 <> 2)
	MsgBox %Var2%

Var := 1, Var2 := 2
var += 4, var2+= 5
if (Var <> 5 || Var2 != 7)
	MsgBox %Var%

Var := 1, Var2 := 2
var -= 4, var2-= 6
if (Var <> -3 || Var2 != -4)
	MsgBox %Var%

Var = 1
Var += 3+2 ? add(3,4) : add(4,4), Var2 := add(4,5)-9 ? add(0,1) : add(1,1)
if (Var <> 8 || Var2 <> 2)
	MsgBox %Var%


; ENSURE BASIC INCREMENTS/DECREMENTS WORK (BOTH POST AND PRE) SINCE THEY'RE PRONE TO GET BROKEN by changes.
Var = 1
++Var
if Var <> 2
	MsgBox %Var%
--Var
if Var <> 1
	MsgBox %Var%
Var++
if Var <> 2
	MsgBox %Var%
Var--
if Var <> 1
	MsgBox %Var%

x = 4
var = 3
x := var := 3//0  ; Divide by zero and other errors should store blank values even in internal assignments.
if (var <> "" or x <> "")
	MsgBox %Var%


; PRE- AND POST-INCREMENT/DECREMENT IN EXPRESSIONS:

Var2 = 2
Var := ++Var2
if (Var <> 3 || Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := --Var2
if (Var <> 1 || Var2 <> 1)
	MsgBox %Var%
Var2 = 2
Var := Var2++
if (Var <> 2 || Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := Var2--
if (Var <> 2 || Var2 <> 1)
	MsgBox %Var%

Var2 = 2
Var := ++Var2 // 3
if (Var <> 1 || Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := Var2++ // 3
if (Var <> 0 || Var2 <> 3)
	MsgBox %Var%
Var2 = 3
Var := 2 // Var2--
if (Var <> 0 || Var2 <> 2)
	MsgBox %Var%
Var2 = 2
Var := 2 // --Var2
if (Var <> 2 || Var2 <> 1)
	MsgBox %Var%
Var2 = 2
Var := 5 + --Var2++  ; Invalid because the ++ is applied first but it produces a non-lvalue, so the -- fails.
if (Var2 <> 3 || Var <> "")
	MsgBox %Var%
Var2 = 2
Var := 5 + Var2++++  ; Fails because the first ++ creates a non-lvalue, so the second ++ fails.
if (Var2 <> 3 || Var <> "")
	MsgBox %Var%
Var2 = 2
Var := 5 + ++++Var2
if (Var2 <> 4 || Var <> 9)
	MsgBox %Var%
Var2 = 2
Var := 5 + ++--Var2
if (Var2 <> 2 || Var <> 7)
	MsgBox %Var%
Var2 = 2
Var := 5 + !--Var2
if (Var2 <> 1 || Var <> 5)
	MsgBox %Var%
Var2 = 1
Var := 5 + !--Var2
if (Var2 <> 0 || Var <> 6)
	MsgBox %Var%
Var2 = 0
Var := 5 + !Var2++
if (Var2 <> 1 || Var <> 6)
	MsgBox %Var%
Var2 = -1
Var := 5 + !Var2++
if (Var2 <> 0 || Var <> 5)
	MsgBox %Var%
Var2 = -1
Var := 5 + (-Var2)++  ; Not valid be the ++ is applied to a non-lvalue.
if (Var2 <> -1 || Var <> "")
	MsgBox %Var%
Var2 = 2
Var := 5 + -++Var2
if (Var2 <> 3 || Var <> 2)
	MsgBox %Var%
Var2 = 2
Var := 5 + ++Var2**2   ; Valid because ++ has higher precedence than **.
if (Var2 <> 3 || Var <> 14)
	MsgBox %Var%
Var2 = 2
Var := 5 + Var2++**2
if (Var2 <> 3 || Var <> 9)
	MsgBox %Var%
Var2 = 2
Var := 5 + ++Var2:=4  ; := is applied before ++ due to special exception.
if (Var2 <> 5 || Var <> 10)
	MsgBox %Var%
Var2 = 2
Var := 5 + ++Var2:=4.5  ; Same but with floating point.
if (Var2 <> 5.5 || Var <> 10.5)
	MsgBox %Var%

Var2 = 2
Var := 2 +++Var2 + 2 ; Doesn't work because it's probably seen as (2++) +Var + 2
if (Var <> "" or Var2 <> 2)
	MsgBox %Var%
Var2 = 2
Var := 2 + ++Var2 + 2
if (Var <> 7 or Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := 2 +(++Var2) + 2
if (Var <> 7 or Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := 2 + ++(Var2) + 2
if (Var <> 7 or Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := 5 + (Var2:=3)++
if (Var <> 8 or Var2 <> 4)
	MsgBox %Var%
Var2 = 1
Var := 5 - --(Var2:=3)
if (Var <> 3 or Var2 <> 2)
	MsgBox %Var%
Var := 5++  ; Just to ensure graceful handling of some of the basics.
if (Var <> "")
	MsgBox %Var%
Var := --"xx"  ; Just to ensure graceful handling of some of the basics.
if (Var <> "")
	MsgBox %Var%
Var := 33+=4  ; Just to ensure graceful handling of some of the basics.
if (Var <> "")
	MsgBox %Var%

Var2 = 2
if Add(Var2++, 3) <> 5 || Var2 <> 3
	MsgBox %Var2%
Var2 = 2
if Add(--Var2, 3) <> 4 || Var2 <> 1
	MsgBox %Var2%

Var2 = 2
Var := 2 + ++Var2-- - 3  ; Doesn't work because -- has high prec and fails to produce an lvalue for ++.
if (Var <> "" or Var2 <> 1)
	MsgBox %Var%

Var2 = 2
Var := Var2++ + 2
if (Var <> 4 or Var2 <> 3)
	MsgBox %Var%
Var2 = 2
Var := (Var2)-- - 2
if (Var <> 0 or Var2 <> 1)
	MsgBox %Var%
Var2 = 2
Var := (Var2)--- 2  ; Same but not space between -- and -.
if (Var <> 0 or Var2 <> 1)
	MsgBox %Var%
Var2 = 2
Var := Var2-- * 2
if (Var <> 4 or Var2 <> 1)
	MsgBox %Var%
Var2 = 2
Var := Var2-- & 3
if (Var <> 2 or Var2 <> 1)
	MsgBox %Var%

Var2 = 2
Var := Var2-- 3  ; Auto-concat
if (Var <> "23" or Var2 <> 1)
	MsgBox %Var%

Var2 = 2
Var := Var2-- "x"  ; Auto-concat
if (Var <> "2x" or Var2 <> 1)
	MsgBox %Var%

++Var := 5 ; silly due to overwrite, so assign the 5 then do the pre-inc
if Var <> 6
	MsgBox %Var%
Var = 4
++Var //= 2 ; valid, but too obscure and inconsist to treat it differently than the others.
if Var <> 3
	MsgBox %Var%
Var++ := 5
if Var <> 6
	MsgBox %Var%
Var = 4
Var++ //= 2
if Var <> 3
	MsgBox %Var%

x = 5
Var2 := &++x
Var3 := &(++x)
if (Var2 <> &x || Var3 <> &x)
	MsgBox %Var2%
x = 5
Var2 := &x++   ; Not an lvalue, so can't take its address
Var3 := &(x++)
if (Var2 <> "" || Var3 <> "")
	MsgBox %Var2%


; ASSIGNMENT OPERATORS THAT ARE INTERNALLY STAND-ALONE EXPRESSIONS SINCE THEY HAVE NO COMMAND COUNTERPART:

var = 11
var //= 5.0
if var <> 2
	MsgBox %Var%

var = xyz
var .= "abc"
if (Var <> "xyzabc")
	MsgBox %Var%

clipboard := 55, Var:=555
if (clipboard <> 55 || Var <> 555)
	Gosub Error
clipboard = 1
clipboard += 55+2, Var:=555
if (clipboard <> 58 || Var <> 555)
	Gosub Error
clipboard = 1
Var2:=clipboard++, Var:=555
if (clipboard <> 2 || Var2 <> 1 || Var <> 555)
	Gosub Error
clipboard = 1
Var2:=++clipboard, Var:=555
if (clipboard <> 2 || Var2 <> 2 || Var <> 555)
	Gosub Error
clipboard := 66, Var=666  ; = vs. :=  (i.e. testing cascade conversion of = to :=)
if (clipboard <> 66 || Var <> 666)
	Gosub Error
Var:=777, clipboard := 77
if (clipboard <> 77 || Var <> 777)
	Gosub Error
Var:=888, clipboard = 88  ; = vs. :=  (i.e. testing cascade conversion of = to :=)
if (clipboard <> 88 || Var <> 888)
	Gosub Error
Var:=999, clipboard = Var2 = 99  ; Same but cascade.
if (clipboard <> 99 || Var2 <> 99 || Var <> 999)
	Gosub Error
Var:=111, Var2 = clipboard = 11  ; Same but reverse cascade.
if (clipboard <> 11 || Var2 <> 11 || Var <> 111)
	Gosub Error
clipboard = xx
Var2=yy
Var:=222, Var3 = Var2.=clipboard .= 22  ; Same but reverse cascade.
if (clipboard <> "xx22" || Var2 <> "yyxx22" || Var3 <> "yyxx22" || Var <> 222)
	Gosub Error
Clipboard = abc
Var:=333, Clipboard := Clipboard
if (clipboard <> "abc" || || Var <> 333)
	Gosub Error

clipboard = 2
clipboard <<= 1
if clipboard <> 4
	MsgBox "%clipboard%"
clipboard = xyz  ; UPDATE: Now it's supported. OLDER: Test graceful failure of clipboard since it isn't supported.
clipboard .= "abc"
if (clipboard <> "xyzabc")
	MsgBox "%clipboard%"
clipboard = xyz  ; But other uses of clipboard should succeed.
Var = abc
Var.=clipboard
if (Var <> "abcxyz")
	MsgBox %Var%

Var = 3
Var &= 5
if Var <> 1
	MsgBox %Var%

Var = 3
Var |= 5
if Var <> 7
	MsgBox %Var%

Var = 3
Var ^= 5
if Var <> 6
	MsgBox %Var%

Var = 3
Var >>= 1
if Var <> 1
	MsgBox %Var%

Var = 3
Var <<= 1
if Var <> 6
	MsgBox %Var%

var2 = 5
true ? var:=1 : var2:=2
if (var <> 1 || var2 <> 5)
	MsgBox %Var%
var = 5
false ? var:=1 : var2:=2
if (var <> 5 || var2 <> 2)
	MsgBox %Var%
false ? Var:=add(2,3) : Var:=add(1,2)
if (var <> 3)
	MsgBox %Var%
var2 = 5
3 > 2 ? var:=1 : var2:=2
if (var <> 1 || var2 <> 5)
	MsgBox %Var%
Var = 5
var3:=0 ? var:=1 : var2:=2
if (var3 <> 2 || var <> 5 || var2 <> 2)
	MsgBox %Var%


; ASSIGNMENT OPERATORS AND THEIR PRECEDENCE EXCEPTIONS

var := false or false or true
if (var <> true)
	MsgBox %Var%

var := 1 < 2 and 3 < 2
if (var <> false)
	MsgBox %Var%

var := var2 := 2
if (Var <> 2 or Var2 <> 2)
	MsgBox %Var%`n%Var2%

var = 6
var2 = 5
x := (var := var2) := 2  ; This is a weird one because var gets written to twice.
if (Var <> 2 or Var2 <> 5 or x <> 2)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 += 2
if (Var <> 5 or Var2 <> 5)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 -= 2
if (Var <> 1 or Var2 <> 1)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 *= 2
if (Var <> 6 or Var2 <> 6)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 /= 2
if (Var <> 1.5 or Var2 <> 1.5)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 //= 2
if (Var <> 1 or Var2 <> 1)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 |= 4
if (Var <> 7 or Var2 <> 7)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 &= 4
if (Var <> 0 or Var2 <> 0)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 ^= 5
if (Var <> 6 or Var2 <> 6)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 >>= 1
if (Var <> 1 or Var2 <> 1)
	MsgBox %Var%`n%Var2%

var2 = 3
var := var2 <<= 1
if (Var <> 6 or Var2 <> 6)
	MsgBox %Var%`n%Var2%

var2 = xyz
var := var2 .= "abc"
if (Var <> "xyzabc" or Var2 <> Var)
	MsgBox %Var%`n%Var2%

; MORE ADVANCED
var := var2 := 1 + 3 ? 5//2 : 8
if (var <> var2 || var2 <> 2)
	MsgBox Problem.

var2 = 2
var := var2 += 1 + 3 ? 5//2 : 8
if (var <> var2 || var2 <> 4)
	MsgBox Problem.

var2 = 3
var := 5 + var2+=5
if var <> 13
	MsgBox %Var%



; ASSIGNMENT'S PRECEDENCE EXCEPTIONS (E.G. IN CASES WHERE PRECEDENCE WOULD CAUSE A SYNTAX ERROR DUE TO INVALID LVALUE):
Var3 = 11
Var2 = 2
Var := 1 ? Var3:=4 : Var2:=3
if (Var <> 4 || Var3 <> 4 || Var2 <> 2)
	MsgBox Problem.

Var:=1, ++Var*=5  ; See comments in code for why.
if Var <> 6
	Gosub Error

var2 := false
var := 0 or var2 := 1
if (var <> true)
	MsgBox %Var%

var2 := 7
var := 5 > var2 := 2
if (var <> true || var2 <> 2)
	MsgBox %Var%

var2 := true
var := (not var2 := false)
if (var <> true || var2 <> false)
	MsgBox %Var%

var2 := true
var := (!var2 := false)
if (var <> true || var2 <> false)
	MsgBox %Var%

var2 := true  ; Same as above but without parens below.
var := !var2 := false
if (var <> true || var2 <> false)
	MsgBox %Var%

Var := Var2 := true ? 2 : 3  ; Should cascade.
if (Var <> 2 || Var2 <> 2)
	MsgBox %Var%

Var := (Var2 := true) ? 2 : 3  ; Override precedence.
if (Var <> 2 || Var2 <> 1)
	MsgBox %Var%

x = 1
a1 := &x
a2 := &(x:=2)
if (a1 <> a2 || x <> 2)
	MsgBox Problem
x = 11
byref(x//=2+2+2//2, 2)  ; Ensure assignment operators can be passed byref (i.e. not seen as loadtime error).
x = 11
byref(x .= 2+2+2//2, 115)
x = 2
byref(x |= 4, 6)
x = 2
byref(x &= 6, 2)
x = 2
byref(x >>= 1, 1)
x = 2
byref(x<<=1, 4)
x = 3
byref(true ? x : x, 3)
byref(x:=1, 1)
byref((x := 2), 2)
byref(x := " ", " ")  ; This was a bug-fix.
byref(byref var, expected)
{
	if (var <> expected)
		MsgBox ByRef(): %Var% <> %Expected%
}


; TERNARY OPERATOR

Var := 1 ? 2 : 3
if Var <> 2
	Gosub Error
Var := true ? 2 : 3
if Var <> 2
	Gosub Error
Var := 0 ? 2 : 3
if Var <> 3
	Gosub Error

Var := 2 < 3 ? 2 : 3
if Var <> 2
	Gosub Error
Var := 2 > 3 ? 2 : 3
if Var <> 3
	Gosub Error

Var := 2 < 3 ? 2+1 : 3+1
if Var <> 3
	Gosub Error
Var := 2 > 3 ? 2+1 : 3+1
if Var <> 4
	Gosub Error

; SYNTAX ERRORS THAT ARE CAUGHT BY LOADTIME VALIDATION:
;Var := 1 ? 2 : 3 : 4  ; Else with no THEN.
;if (Var <> "")
	;Gosub Error
;Var := 1 (: 2 :) ? 3  ; Syntax error.
;if (Var <> "")
	;Gosub Error
;Var := 1 ? (:) :  ; Syntax error.
;if (Var <> "")
	;Gosub Error
;Var := 1 ? ,3 : 4  ; Syntax error (these types of tests are mostly just to find crash points).
;if (Var <> "")
	;Gosub Error
;Var := (1 ? 2) : 3  ; Unsupported use of parentheses to override association of ? with it's :.
;if (Var <> "")
	;Gosub Error
;Var := (0 ? 2) : 3
;if (Var <> "")
	;Gosub Error

; SYNTAX ERRORS THAT ARE NOT CAUGHT BY LOADTIME VALIDATION:
Var := 0 ? 2
if (Var <> "")
	Gosub Error
Var := (0 ? 2) + 1
if (Var <> "")
	Gosub Error
Var := 0 ? 2 ? 3 : 4  ; Postfix: 02?3?4:
if (Var <> "")  ; Syntax error
	Gosub Error  ; ...but then it unexpectedly encounters another ? because the 3 wasn't cascaded?
Var := (1 ? 2 :) 3  ; Weird paren problem.
if (Var <> "23")  ; Though it actually does work somewhat.
	Gosub Error
Var := (0 ? 2 :) 3  ; But this one doesn't work.
if (Var <> "")
	Gosub Error
Var := 1 (? 2 :) 3  ; Syntax error.
if (Var <> "")
	Gosub Error
Var := 1 ? 3 : ,4  ; Syntax error (these types of tests are mostly just to find crash points).
if (Var <> 3)  ; It actually winds up being 3, but seems too rare to add extra code to check for.
	Gosub Error
Var := 1 ? 3 : 4,  ; Trailing comma.  Actually works in spite of this in v1.0.46.01.  OLDER BEHAVIOR: Syntax error (these types of tests are mostly just to find crash points).
if (Var <> 3)
	Gosub Error
Var := 1, true ? 3 : 4,  ; See comment above. OLDER: Syntax error (these types of tests are mostly just to find crash points).
if (Var <> 1)
	Gosub Error

; TERNARY'S RESULT IS THE INPUT FOR ANOTHER TERNARY:
Var := (0 ? 0 : 1) ? 3 : 4
if (Var <> 3)
	Gosub Error
Var := (1 ? 0 : 1) ? 3 : 4
if (Var <> 4)
	Gosub Error
Var := (0 ? 1 : 0) ? 3 : 4
if (Var <> 4)
	Gosub Error
Var := (1 ? 1 : 0) ? 3 : 4
if (Var <> 3)
	Gosub Error
Var := 1 ? 1 : 0 ? 3 : 4  ; Same as above but without the parens: right-to-left associativity/precedence changes the meaning.
if (Var <> 1)
	Gosub Error
Var := 0 ? 1 : 0 ? 3 : 4  ; Converse
if (Var <> 4)
	Gosub Error
Var := b = 1 ? "x" : (b = 2 ? "y" : "z")  ; An example from PhiLho.
if (Var <> "z")
	Gosub Error
b = 2
Var := b = 1 ? "x" : (b = 2 ? "y" : "z")  ; Variant.
if (Var <> "y")
	Gosub Error
b = 1
Var := b = 1 ? "x" : (b = 2 ? "y" : "z")  ; Variant.
if (Var <> "x")
	Gosub Error
b = 5
Var := b = 1 ? "x" : b = 2 ? "y" : "z"  ; Same but no parens.
if (Var <> "z")
	Gosub Error

; INTERACTION BETWEEN TERNARY AND "AND/OR" (SHORT-CIRCUIT AND CASCADE):
fn1 := false
fn2 := false
Var := 0 and fn1() ? 1 + fn1() : 0 + fn2() ? 3 + fn1() : 4 + fn2(2)
if (Var <> 6)
	Gosub Error
if (fn1 || !fn2)
	Gosub Error

; Converse of above:
fn1 := false
fn2 := false
Var := 1 and fn2(1) ? 2 + fn2(2) : 0 + fn1() ? 3 + fn1() : 4 + fn1(2)
if (Var <> 4)
	Gosub Error
if (fn1 || !fn2)
	Gosub Error

; Same, but test OR vs. AND:
fn1 := false
fn2 := false
Var := 1 or fn1(1) ? 2 + fn2(2) : 0 + fn1() ? 3 + fn1() : 4 + fn1(2)
if (Var <> 4)
	Gosub Error
if (fn1 || !fn2)
	Gosub Error

; Converse
fn1 := false
fn2 := false
Var := 0 or fn2(1) ? 2 + fn2(2) : 0 + fn1() ? 3 + fn1() : 4 + fn1(2)
if (Var <> 4)
	Gosub Error
if (fn1 || !fn2)
	Gosub Error

; CONVERSE OF SECTION ABOVE: MAKE A TERNARY THE LEFT/RIGHT SIDE OF AN AND/OR.
fn1 := false
fn2 := false
Var := (1 ? 0 : 2) and fn1() and fn2()
if (Var <> false)
	Gosub Error
if (fn1 || fn2)
	Gosub Error
fn1 := false
fn2 := false
Var := (1 ? 1 : 0) and fn1() and fn2()
if (Var <> false)
	Gosub Error
if (!fn1 || fn2)
	Gosub Error
fn1 := false
fn2 := false
Var := (1 ? 1 : 0) and fn1(1) and fn2(2)
if (Var <> true)
	Gosub Error
if (!fn1 || !fn2)
	Gosub Error
fn1 := false
fn2 := false
Var := (0 ? 0 : 1) or fn1(1) or fn2(2)
if (Var <> true)
	Gosub Error
if (fn1 || fn2)
	Gosub Error
fn1 := false
fn2 := false
Var := (0 ? 0 : 0) or fn1(0) or fn2(0)
if (Var <> false)
	Gosub Error
if (!fn1 || !fn2)
	Gosub Error
fn1 := false
fn2 := false
Var := (0 ? 0 : 0) or (fn1(0) ? fn2() : 3) or fn2(0)
if (Var <> true)
	Gosub Error
if (!fn1 || fn2)
	Gosub Error

; PROPER PRECEDENCE IN UNUSUAL CASES THAT LACK PARENS:
Var := 1 ? 1 ? 2 : 3 : 4  ; Should produce postfix 112?3:?4: rather than something like 112?3?4::
if (Var <> 2)
	Gosub Error
Var := 0 ? 1 ? 2 : 3 : 4  ; Converse
if (Var <> 4)
	Gosub Error
Var := 0 ? 1 ? 2 : 3 : 4+3/3 ? 5 : 6  ; Same, but final item a little more complicated.
if (Var <> 5)
	Gosub Error
Var := 0 ? (1 ? 2 : 3) : 4  ; Same, but using parens to make the order explicit.
if (Var <> 4)
	Gosub Error

var2 = 5
var := (true ? var1 : var2) := 3  ; Override special precedence rules.
if (var1 <> 3 || var2 <> 5 || var <> 3)
	MsgBox %var%
var1 = 6
var2 = 5
var := true ? var1 : var2 := 3  ; Same but without the override.
if (var1 <> 6 || var2 <> 5 || var <> 6)
	MsgBox %var%


; SAME AS ABOVE but make sure they short-circuit properly (i.e. according to position in INFIX not in POSTFIX).
fn1 := false
fn2 := false
Var := 1 + fn1() ? 1 ? 2 : 3 + fn2() : 4 + fn2()  ; Should produce postfix 112?3:?4: vs. something like 112?3?4::
if (Var <> 2)
	Gosub Error
if (!fn1 || fn2)
	Gosub Error
fn1 := false
fn2 := false
Var := 0 + fn1() ? 1 + fn2() ? 2 + fn2() : 3 + fn2() : 4 + fn1()
if (Var <> 4)
	Gosub Error
if (!fn1 || fn2)
	Gosub Error

; MORE FUNCTION CALLS AND SHORT-CIRCUIT:
fn1 := false
fn2 := false
Var := 2 < 3 ? fn1(4) : fn2(5)
if Var <> 4
	Gosub Error
if (!fn1 or fn2)  ; Check for the proper short-circuit side-effects.
	Gosub Error

fn1 := false
fn2 := false
Var := 2 > 3 ? fn1(4) : fn2(5)
if Var <> 5
	Gosub Error
if (fn1 or !fn2)  ; Check for the proper short-circuit side-effects.
	Gosub Error

fn1 := false
fn2 := false
Var := 2 < 3 ? 2+fn1() : 3+fn2()
if Var <> 2
	Gosub Error
if (!fn1 or fn2)  ; Check for the proper short-circuit side-effects.
	Gosub Error

fn1 := false
fn2 := false
Var := 2 > 3 ? 2+fn1() : 3+fn2()
if Var <> 3
	Gosub Error
if (fn1 or !fn2)  ; Check for the proper short-circuit side-effects.
	Gosub Error

; Same but with ?: nested in each other:
fn1 := false
fn2 := false
Var := 2 > 3 ? 2+fn1() + (1 ? fn1() : 3) : 3+fn2(2) + 0 ? 2 + fn2(4) : fn2(1)
if Var <> 6
	Gosub Error
if (fn1 or !fn2)  ; Check for the proper short-circuit side-effects.
	Gosub Error

fn1(x = 0)
{
	global fn1
	fn1 := true
	return x
}

fn2(x = 0)
{
	global fn2
	fn2 := true
	return x
}

; ABOVE IS TERNARY OPERATOR SECTION
; --------------------------------------------
; --------------------------------------------


; Bugfix for v1.0.40.06:
if (-0x8000000000000000 + 0x7FFFFFFFFFFFFFFF) <> -1
	MsgBox -0x8000000000000000
if (-0x7FFFFFFFFFFFFFFF + 0x7FFFFFFFFFFFFFFE) <> -1
	MsgBox -0x7FFFFFFFFFFFFFFF
; In conj. with above, test array whose name is numeric:
i = 1
121 = 5
if (1 + -12%i%) != -4
	MsgBox Numeric-named-array problem.
123%i% = 333
if 1231 <> 333
	Msgbox %1231%
Array%i% = 123
if (Array%i% != 123)
	Msgbox % """" . Array%i% . """"
if Array%i% <> 123
	Msgbox % """" . Array%i% . """"
5 = 6
if 5 <> 6
	MsgBox %5%
0 = 1
if 0  ; This should be an expression and shouldn't treat 0 as a variable.
	msgbox
0 =  ; Reset for proper detection below.
0 := 1  ; For bk-compat.
if 0 <> 1
	msgbox %0%


Var = 123
if Var <> +123  ; Right side seen as number not string.
	MsgBox %Var%
if Var <> 0123
	MsgBox %Var%
if (Var <> +123)
	MsgBox %Var%
if (Var <> 0123)
	MsgBox %Var%

; Bugfix for v1.0.35.04:
a=2
b=3
c=4
if a*b*c <> 24
	MsgBox problem.
a=16
b=4
c=2
if a/b/c <> 2
	MsgBox problem.

a=2
b=4
c=0
if (a<b>c)<>1  ; Parens needed due to differing precedence.
	MsgBox problem.

; Ensure no parens needed if first item is function call:
if add(3,4) <> 7
	MsgBox problem.

Var := Concat("123")
if Var <> 1234.40000011
	MsgBox %Var%
Var := Add(1, 2, 3)
if Var <> 6
	MsgBox %Var%

TestArrayCmd()
if gArrayX0 <> 4
	MsgBox %gArrayX0%
if gArrayY < 4
	MsgBox Only %gArrayY% windows on entire system?
TestArrayCmd()
{
	global gArrayX0, gArrayY
	var = abcd
	StringSplit, gArrayX, var
	WinGet, gArrayY, List
}

; Bug-fix for v1.0.31.04 (incorrect load-time syntax error):
var = test(
StringGetPos,pos,var,(,R
if pos <> 4
	MsgBox %pos%

; Ensure params containing double colons aren't seen as hotkeys are hotstrings.
; Keep the following line as bare as possible because it's only purpose is to catch invalid load-time errors:
YieldParam("::")

; Ensure function-calls work okay when on same line as an ELSE or a hotkey (see bottom of script for single-line hotkey):
Var =
if (false)
{
}
else SideEffect()
if Var <> SideEffect
	MsgBox %Var%

; bug fix for v1.0.31.01:
Var := Add(2*3,3)
if var <> 9
	MsgBox %var%

Var := Add(2*3,Add(2*3,3))
if var <> 15
	MsgBox %var%

Var := Add(Add(2*3,(1+2)*3),Add(2*3,(1+2)*3))
if var <> 30
	MsgBox %var%

Array1 = xyz
TestArray()
TestArray()
{
	i = 1
	Var := Array%i%
	if Var <> xyz
		MsgBox %Var%
	Array%i% = abc
	Var := Array%i%
	if Var <> abc
		MsgBox %Var%
}

TestArray2()
if Thing55 <> 123
	MsgBox %Thing55%
TestArray2()
{
	global
	local newarray1
	newarray1 = aaa
	i = 1
	Var := newarray%i%
	if Var <> aaa ; Even when assume-global is in effect, existing locals eclipse globals (only for dynamic vars).
		MsgBox %Var%
	NewArray%i% = bbb  ; Even this obeys the eclipse rule.
	Var := newarray%i%
	if Var <> bbb ; Even when assume-global is in effect, existing locals eclipse globals (only for dynamic vars).
		MsgBox %Var%
	i = 55
	Thing%i% = 123  ; Since neither a global nor a local exists, this should fall back to our assume-mode, which is global.
}


SetDefaults()
if var <> 33
	MsgBox %var%
SetDefaults()
{
	global
	Var := 33  ; Assigns 33 to a global variable, first creating the variable if necessary.
	local x, y, z  ; Local variables must be declared in this mode.
	; ...
}

Sleep 0  ; Seems to avoid split-second timing issues (relating to the display of MsgBox's in any line higher above, which causes the below to report a problem.
WinGet, ID, ID, A
Var := WinActive("A")
if ID <> %Var%
	MsgBox %var%

Var := WinExist("bogus window")
if Var <> 0
	MsgBox %var%
if WinExist("bogus window")  ; Test that its yielded "0" is not interpreted at true due to being a string.
	MsgBox problem

if WinExist("ahk_id" ID)  ; set the last found window
{
	if !WinExist()
		MsgBox problem
	if WinExist() <> ID
		MsgBox %id%
	SetTitleMatchMode 2
	if WinExist("pad") and not WinExist()   ; Short circuit.
		MsgBox problem
	if WinExist("pad", "dfiaodfjaodifjpiejwoeij")
		MsgBox problem
}

Clipboard = 0123456789
Var := ClipboardAll
StringLen, VarLen, Var
if (VarLen <> StrLen(Var) || VarLen < 4)
	MsgBox StrLen(Var) of binary clipboard doesn't match StringLen cmd.
Var2 := Var  ; Assign binary clipboard to another variable.
if (Var <> Var2 || StrLen(Var2) < 4)
	MsgBox Assign binary clipboard variable.
Var2:=1, Var3 := Var  ; Binary-clip vars supported deep inside expressions.
if (Var2 <> 1 || Var <> Var3 || StrLen(Var3) < 4)
	MsgBox Assign binary clipboard variable.
Var := "x"
Var2:=1, Var := ClipboardAll  ; But ClipboardAll isn't supported deep inside expressions.
if (Var2 <> 1 || Var <> "")
	MsgBox ClipboardAll isn't supported deep inside expressions?

ErrorLevel = 2
TestErrorLevel()
if ErrorLevel <> 1
	MsgBox ErrorLevel
TestErrorLevel()
{
	ErrorLevel = 2
	WinWait, bogus,, 0.01
}
TestErrorLevel2()
if ErrorLevel <> 3
	MsgBox ErrorLevel
TestErrorLevel2()
{
	ErrorLevel = 3
}

; Does nothing because "MyMsgBox("test")" is considered the exclude-title:
IfWinNotExist, bogus,, MyMsgBox("test")
{
}

; INCLUDE
;#include C:\A-Source\AutoHotkey\Test\TEST SUITES\function_includes.ahk
;var := incl_add(1,2)
;if Var <> 3
	;MsgBox %var%
;var := incl_mult(3,2)
;if Var <> 6
	;MsgBox %var%

; INCLUDE FUNCTION SPLIT ACROSS 2 FILES
var := func_span()
if var <> 1
	msgbox %var%
func_span()
{
	global
	local x
	x = 1
	;#include C:\A-Source\AutoHotkey\Test\TEST SUITES\function_tail.ahk
	return x
}

var = xx
var := donothing()
if var <>
	MsgBox %var%
donothing()
{
}

; Test multi-declaration:
Var := Declare(1,2)
if Var <> 3i1j-1
	MsgBox %Var%
if z <> test
	MsgBox %z%
Var := Declare(2,3)
if Var <> 5i2j-2
	MsgBox %Var%
if z <> test
	MsgBox %z%
Declare(a, b)
{
	global
	local x, y
	static i, j,  ; extra comma at end currently not caught, unlike other misplace commas here, but seems harmless.
	x := a
	y := b

	++i
	--j
	z = test

	return x + y . "i" i "j" j
}

; Test assume-static mode:
my_global = global_contents
AssumeStatic() ; Call #1.
AssumeStatic() ; Call #2.
if (StaticArray11 <> "")  ; Ensure the function didn't actually/wrongly set a global instead of a static.
	MsgBox StaticArray11 <> ""
AssumeStatic()
{
	static
	global my_global
	if (my_global <> "global_contents")
		MsgBox my_global <> "global_contents"
	local my_local
	if (my_local <> "")
		MsgBox my_local <> ""
	my_local = something non-blank to ensure it gets erased when we return to our caller.
	static i = 10  ; Soemthing other than zero to catch any weirdness related to blanks being treated as zero.
	++i
	if i = 11  ; This is the first time we were called.
		StaticArray%i% := "static value"
	else  ; This is the 2nd or later time we were called.
	{
		if (StaticArray11 <> "static value")
			MsgBox StaticArray11 <> "static value"
		j := i-1
		if (StaticArray%j% <> "static value")
			MsgBox StaticArray%i% <> "static value"
	}
}

; GUI ByRef var can be used if it points to a global var:
CreateGUI(MyEdit)
CreateGUI(ByRef ParamMyEdit)
{
	gui, add, edit, vParamMyEdit, Leave this here
	;gui, add, edit, vMyLocal, Test   ; Syntax error since it's not local.
	;gui, add, button, default, &OK
	Gui Submit
	if ParamMyEdit <> Leave this here
		MsgBox %ParamMyEdit%
	global MyEdit
	if MyEdit <> Leave this here
		MsgBox %MyEdit%
	Gui Destroy
}

; RECURSION TEST:
RecurseBasic(1)
RecurseBasic(aVar)
{
	if aVar > 10
		return
	++aVar
	aVarPrev = %aVar%
	RecurseBasic(aVar)
	if aVar <> %aVarPrev%
		MsgBox %aVar% <> %aVarPrev%
}

; RECURSION TEST:
Var := RecurseFactorial(5)
if Var != 120
	MsgBox %Var%
RecurseFactorial(aStart)
{
	if aStart > 1
		return aStart * RecurseFactorial(aStart - 1)
	else
		return aStart
}

; RECURSION TEST:
testfn(Var)
if Var <> for main caller
	MsgBox %Var%
testfn(ByRef Var)
{
	; tested ok: MsgBox Press Win-space to interrupt this thread.
	static calls
	++calls
	if calls = 1
	{
		var = for main caller
	}
	if calls > 2
		return
	if (calls == 2)
	{
		if x <>  ; Since it's an undefined local, it should be blank at this stage.
			MsgBox %x%
		Var = vv ; This will indirectly assign vv to our x and our caller's X due to a documented ByRef limitation.
		if x <> vv
			MsgBox x=%x%
	}
	x = xx
	testfn(x)
	if x <> xx
		MsgBox new x: %x% (should be xx)
}

; RECURSION TEST:
; (this one was for a bug fixed in 1.0.42.07)
trace =
r := Fibonacci_(9, trace)
if r <> 34
	MsgBox Fibonacci/recurse problem.
Fibonacci_(_n, ByRef @trace)
{
	If (_n = 1 or _n = 2)
		Return 1
	@trace = %@trace%%_n%/
	Return Fibonacci_(_n - 1, @trace) + Fibonacci_(_n - 2, @trace)
}



start_time := A_TickCount
Var = 1
Sleep %Var%00
elapsed := A_TickCount - start_time
if elapsed not between 80 and 120
	MsgBox % elapsed

Var = 1
Array1 = 1
Array2 = 2
result := GeneralTest(Var)
if result <> return value8
	MsgBox %result%
if Var <> 1  ; It shouldn't have altered it because it was passed byval.
	MsgBox %Var%
if ErrorLevel <> xx
	MsgBox %ErrorLevel%
Var = clipboard
%Var% = yy  ; Should set the global even if assume-local is in effect.
if Clipboard <> yy
	MsgBox %Clipboard%
GeneralTest(X)
{
	Y = 2
	Swap(X,Y)
	if (x <> 2 or Y <> 1)
		MsgBox %X%`n%Y%
	Var = ErrorLevel
	%Var% = xx  ; Should set the global even if assume-local is in effect.
	if ErrorLevel <> xx
		MsgBox %ErrorLevel%
	Var = clipboard
	%Var% = yy  ; Should set the global even if assume-local is in effect.
	if Clipboard <> yy
		MsgBox %Clipboard%
	i = 0
	Loop, 2
	{
		++i
		if Array%i% <> %i%
			MsgBox % Array%i%
		StringTrimLeft, element, Array%i%, 0
		if element <> %i%
			MsgBox %element%
	}
	; works okay (causes runtime error):
	;Var = A_Index
	;%Var% = xx  ; should be an err.
	Loop  ; Test nested return value
	{
		if (true)
			return "return value" . (3+1)*2
	}
}

; VARIABLES CONTAINING QUESTION MARKS (THIS BEHAVIOR MIGHT CHANGE IN FUTURE VERSIONS):
Var? = 2
?Var = 2
if (Var? <> 2 || ?Var <> 2)
	MsgBox Problem.
? = 2  ; Currently allowed.
Var := ? = 2  ; But ? isn't recognized in expressions due to ambiguity (and code size reduction).
if (Var <> "")
	MsgBox Problem

; CONSECUTIVE UNARY OPERATORS
Var = 2
Var := !!Var  ; Do it as a separate step because older AHK versions did weird things with !! that made it sometimes invalid and sometimes valid depending on the operators that followed it.
if Var <> 1
	MsgBox Problem.
Var = 2
Var := not not Var
if Var <> 1
	MsgBox Problem.
Var = 2
Var := not -Var
if Var <> 0
	MsgBox Problem.
Var = 0
Var := not -Var
if Var <> 1
	MsgBox Problem.
Var = 2
Var := not !Var
if Var <> 1
	MsgBox Problem.
Var = 0
Var := not !Var
if Var <> 0
	MsgBox Problem.
Var = 2
Var := !-Var
if Var <> 0
	MsgBox Problem.
Var = 0
Var := !-Var
if Var <> 1
	MsgBox Problem.
Var = 2
Var := -!Var
if Var <> 0
	MsgBox Problem.
Var = 0
Var := -!Var
if Var <> -1
	MsgBox Problem.
Var := !&Var
if Var <> 0
	MsgBox Problem.
Var := -&Var
if Var >= 0
	MsgBox Problem.
Var := ~&Var
if not Var
	MsgBox Problem.
Var = A
Var2 := &Var
Var := -*Var2
If Var <> -65  ; This one worked even in older AHK versions because then, * had a higher precedence than the other unaries.
	MsgBox Problem.
Var = A
Var2 := &Var
Var := !*Var2
If Var <> 0
	MsgBox Problem.
Var =
Var2 := &Var
Var := !*Var2
If Var <> 1
	MsgBox Problem.
Var = A
Var2 := &Var
Var := ~*Var2
If Var <> 0xFFFFFFBE
	MsgBox Problem.
Var =
Var2 := &Var
Var := ~*Var2
If Var <> 0xFFFFFFFF
	MsgBox Problem.

; SHORT-CIRCUIT:
if (2<1 and 2<1 and 2<1 and 2<1)
	MsgBox problem

if !(2>1 or 2>1 or 2>1 or 2>1)
	MsgBox problem

if (2>1 and 2<1 and 2<1 and 2<1)
	MsgBox problem

if !(2<1 or 2>1 or 2>1 or 2>1)
	MsgBox problem

if (2<1 and 2<1 and 2>1 and 2<1)
	MsgBox problem

if !(2>1 or 2>1 or 2<1 or 2>1)
	MsgBox problem

if (2>1 and 2>1 and 2>1 and 2<1)
	MsgBox problem

if !(2<1 or 2<1 or 2<1 or 2>1)
	MsgBox problem

if (2>1 and 2>1 and 2<1 and 2>1)
	MsgBox problem

if !(2<1 or 2<1 or 2>1 or 2<1)
	MsgBox problem


; HYBRID AND/OR:
if (2<1 and 2<1 or 2<1 and 2<1)
	MsgBox problem

if !((2>1 or 2>1) and (2>1 or 2>1))
	MsgBox problem

if (2>1 and 2<1 or 2<1 and 2>1)
	MsgBox problem

if !((2<1 or 2>1) and (2>1 or 2<1))
	MsgBox problem

if !(not 2<1 and not 2<1 or 2<1 and 2<1)
	MsgBox problem

if ((not 2>1 or not 2>1) and (2>1 or 2>1))
	MsgBox problem

if !(2<1 and 2<1 or not 2<1 and not 2<1)
	MsgBox problem

if ((2>1 or 2>1) and (not 2>1 or not 2>1))
	MsgBox problem

if !(0 and "" or "0" and "str")  ; A literal "0", unlike 0, should be considered true.
	MsgBox problem

if (("0" or "0") and ("" or 0))
	MsgBox problem


if (2<1 and NeverCalled("44",2))  ; It will display an error if it doesn't properly short-circuit.

if !(2>1 or NeverCalled("44",2))  ; It will display an error if it doesn't properly short-circuit.
	MsgBox problem
if !(2<1 or 2>1 or NeverCalled("44",2))  ; It will display an error if it doesn't properly short-circuit.
	MsgBox problem
if !(2<1 and NeverCalled("44",2) or 2<1 or 2>1 or NeverCalled("44",2))  ; It will display an error if it doesn't properly short-circuit.
	MsgBox problem
if (2>1 and 2<1 and NeverCalled("44",2))  ; It will display an error if it doesn't properly short-circuit.
	MsgBox problem
if ((2>1 or NeverCalled("44",2)) and 2>1 and 2<1 and NeverCalled("44",2))  ; It will display an error if it doesn't properly short-circuit.
	MsgBox problem

Var =
if (Add(1,2) > 3 or SideEffect()) and NeverCalled(1,5)  ; NeverCalled() will report a problem if it really is wrongly called.
	MsgBox problem
if Var <> SideEffect
	MsgBox problem

Var =
if !((Add(1,2) > 2 and SideEffect()) or true or NeverCalled(1,5)) ; NeverCalled() will report a problem if it's called.
	MsgBox problem
if Var <> SideEffect
	MsgBox problem

; ---

Var = 12.0
if (Var = "12") = 1
	MsgBox problem

Var := "xyz" . "abc"
if Var <> xyzabc
	MsgBox %Var%

Var := . "xyz"  ; requires an explicit "", so this is a syntax error.
if Var <>
	MsgBox %Var%

;Var := "xyz" .  ; caught as a load-time error since no trailing space after the dot.
;if Var <>
;	MsgBox %Var%

Var =
Var := Var . "xyz"
if Var <> xyz
	MsgBox %Var%

Var = abc
Var := Var . "xyz"
if Var <> abcxyz
	MsgBox %Var%

Var =
Var := "xyz" . Var
if Var <> xyz
	MsgBox %Var%

Var =
Var := Var . "xyz" . Var
if Var <> xyz
	MsgBox %Var%
Var := Var . "xyz" . Var
if Var <> xyzxyzxyz
	MsgBox %Var%

Var := "Test" . 5+2
if Var <> Test7
	MsgBox %Var%

Var := ("1" 5) < 2  ; string compare
if Var <> 1
	MsgBox %Var%

Var := ("1" . 5) < 2  ; string compare
if Var <> 1
	MsgBox %Var%

Var := (1 . "5") < 2  ; string compare
if Var <> 1
	MsgBox %Var%

Var := (3 5)+2
if Var <> 37
	MsgBox %Var%

Var := (3 . 5)+2
if Var <> 37
	MsgBox %Var%

Var := 3 < 1 2   ; true because concat is higher prec, and it builds the number 12
if Var <> 1
	MsgBox %Var%

Var := 3 < 1 . 2   ; true because concat is higher prec, and it builds the number 12
if Var <> 1
	MsgBox %Var%

Var := 1 . 2 + 3   ; concat is lower prec in this case
if Var <> 15
	MsgBox %Var%

Var = zz
Var := Add(5, 2) . YieldParam("xxx") . Concat("abc" . 123, YieldParam("yyy"), Var)
if Var <> 7xxxabc123yyyzz
	MsgBox %Var%

i = 1
Array1 = Test
Var := "Result: " Array%i% " xxx" Array%i% Add(3,2) . Array%i%
if Var <> Result: Test xxxTest5Test
	MsgBox %Var%`nResult: Test xxxTest5Test

i = 1
Array1 = Test
Var = zz
Var := Add(5, 2) . Array%i% . YieldParam("xxx") . Concat("abc" . 123, YieldParam("yyy"), Var) . Array%i%
if Var <> 7Testxxxabc123yyyzzTest
	MsgBox 7Testxxxabc123yyyzzTest`n%Var%

; Same as above but without an explicit "." operator:
i = 1
Array1 = Test
Var = zz
Var := Add(5, 2) Array%i% YieldParam("xxx") Concat("abc" 123, YieldParam("yyy"), Var) Array%i%
if Var <> 7Testxxxabc123yyyzzTest
	MsgBox 7Testxxxabc123yyyzzTest`n%Var%

; Simple test is commented out, but it's followed by a more programmatic test:
;MsgBox, 0, % Add(1, 2), Text
Var = 0
Loop, % Add(1,2)
	Var++
if Var <> 3
	MsgBox %Var% 

Var = 1
Var += Add(Var, 2)
if Var <> 4
	MsgBox %Var% 

start_time := A_TickCount
WinWait, bogus111,, Add(0.05, 0.1)
elapsed := A_TickCount - start_time
if elapsed not between 100 and 200
	Msgbox %elapsed%

; This check ensures that the Transform hack/patch didn't break legacy usages of Transform:
Var = 2
Var2 = 2
Transform, Var, Pow, %Var%, -%Var2%
if Var <> 0.25
	MsgBox %Var% 

Transform, Var, Pow, Add(1,2), Add(1,1)
if Var <> 9
	MsgBox %Var%

Var =
SideEffect()
if Var <> SideEffect
	MsgBox %Var%

Var := Yield123()
if Var <> 123
	MsgBox %Var%

Var := YieldABC()
if Var <> ABC
	MsgBox %Var%

GlobalVar = Test
Var := YieldGlobalVar()
if Var <> %GlobalVar%
	MsgBox %Var%

; Copy var onto itself:
GlobalVar = test
GlobalVar := YieldGlobalVar()
if GlobalVar <> Test
	MsgBox %GlobalVar%

Var := YieldLocalVarContainingXYZ()
if Var <> XYZ
	MsgBox %Var%

var := YieldProgramFiles()
IfNotInString, var, Program Files
	MsgBox %Var%

var := YieldEnvPath()
IfNotInString, var, \System32
	MsgBox %Var%
Var := 1, Path := "xx"
if Path <> xx
	MsgBox "%Path%"
Path := ""
Var := 1, Path = "xx"  ; Same but uses = vs. :=.
if Path <> xx
	MsgBox "%Path%"
Path := ""
Path .= "xyz"
if (Path <> "xyz")
	msgbox "%path%"
Path := ""
if not &(++Path)
	MsgBox It should always be possible to take the address of a pre-op, even if it hits against an env var or blank var.
Path := ""
Var := 1, path = homedrive = "xyz"
if (var <> 1 || path <> "xyz" || homedrive <> "xyz")
	Gosub Error
Path := ""  ; Reset in case any other fetches of env. path later on.
HomeDrive := ""

Var := 1, SomeEmptyVar = "xyz"  ; Testing SomeEmptyVar to ensure no environment var logic messes it up.
if (Var <> 1 || SomeEmptyVar <> "xyz")
	Gosub Error
SomeEmptyVar := ""
Var := 1, SomeEmptyVar := "xyz"  ; Same but with := instead of =.
if (Var <> 1 || SomeEmptyVar <> "xyz")
	Gosub Error
SomeEmptyVar := ""
if (not &SomeEmptyVar) || (not &(++SomeEmptyVar))
	MsgBox It should always be possible to take the address of a pre-op, even if it hits against an env var or blank var.
SomeEmptyVar := ""
SomeEmptyVar .= "xyz"
if (SomeEmptyVar <> "xyz")
	Gosub Error
SomeEmptyVar := ""

a = 2
b = 3
x = a
y = b
Var:=%x%+%y%  ; Test with minimum of spacing to ensure no weirdness during parsing of double-derefs.
if Var <> 5
	Gosub Error

clipboard = xyz
if strlen(clipboard) <> 3 || strlen(A_YYYY) <> 4 || strlen(HOMEDRIVE) <> 2
	gosub Error

var := "x" ProgramFiles  ; test concat of env. vars
var2 = x%ProgramFiles%
if var <> %var2%
	MsgBox %Var%`n%var2%

Var := YieldParam("LMN")
if Var <> LMN
	MsgBox %Var%

Var := YieldParam("L,K") ; Test comma embedded in quoted/literal string.
if Var <> L,K
	MsgBox %Var%

Var := YieldParam("L((K,,)M)(") ; Test parens embedded in quoted/literal string.
if Var <> L((K,,)M)(
	MsgBox %Var%

Var := YieldParam(Add(2,YieldParam(3))) ; Test nested functions.
if Var <> 5
	MsgBox %Var%

Var := Add(Add(2,3)+1,Add(5,1)+2) ; Test nested functions.
if Var <> 14
	MsgBox %Var%

Var := Concat("ABC", "123", "")
if Var <> ABC123
	MsgBox %Var%

Var := Concat(BlankUnknownVar, "123", "")
if Var <> 123
	MsgBox %Var%

Var := Concat("ABC", 123, "")
if Var <> ABC123
	MsgBox %Var%

Var := Concat(A_Space, "123", A_Tab)
if (Var <> " 123`t")
	MsgBox "%Var%"

Var := Concat("ABC", 123, "XYZ")
if Var <> ABC123XYZ
	MsgBox %Var%

X = 1
Y = 2
Swap(X,Y)
if (X <> 2 or Y <> 1)
	MsgBox problem
Swap(Y,X)
if (X <> 1 or Y <> 2)
	MsgBox problem
Z = 3
Swap3(X,Y,Z)
if (X <> 3 or Y <> 2 or Z <> 1)
	MsgBox problem

i = 1
j = bogus
Array1 = 123
Swap(Array%i%, Array%j%)  ; This should create ArrayBogus since it doesn't yet exist.
if (Array%i% != "" or Array%j% != 123)
	MsgBox problem

Var := Concat("ABC", "", "XYZ")
if Var <> ABCXYZ
	MsgBox %Var%

Var =
Var := Concat("ABC", Var, "")
if Var <> ABC
	MsgBox %Var%

Var =  ; Must reset.
Var := Concat(Var, 888, Var)
if Var <> 888
	MsgBox %Var%

Var = 123
Var := Concat("ABC", Var, "")
if Var <> ABC123
	MsgBox %Var%

Var = 123  ; Must reset.
Var := Concat(Var, "ABC", Var)
if Var <> 123ABC123
	MsgBox %Var%

Var := MultiplyByTen(5)
if Var <> 50
	MsgBox %Var%

Var := Add(3, 9)
if Var <> 12
	MsgBox %Var%

Var := Add(3, 9) + Add(2,8)
if Var <> 22
	MsgBox %Var%

Var := Add(3, 9) + Add(2, Add(2,4)) + Add(Add(1,2), Add(3,4))
if Var <> 30
	MsgBox %Var%

Var := Add(3,2) + Add(3,-1) * Add(1,2)
if Var <> 11
	MsgBox %Var%

Var := Add((3+4),Add(5,(2*2))) + Add((3-5),(-1*2)) * Add(1,2)
if Var <> 4
	MsgBox %Var%

Var := -Add((3+4),~Add(0xFFFFFFFE,1)) + !Add(-1,1)
if Var <> -6
	MsgBox %Var%

Var := Add4(3, 2,4 ,5)
if Var <> 14
	MsgBox %Var%

Var := PrependTest("abc")
if Var <> Testabc
	MsgBox %Var%

FileRead, FileContents, %A_ScriptFullPath% ; For use below.

Var := ReturnFileContentsSimple()
if (Var <> FileContents)
	MsgBox %var%
Var:="", Var3 := Var := ReturnFileContentsSimple()  ; Doing it this way tests a particular optimization section in the code.
if (Var <> FileContents || Var3 <> FileContents)
	MsgBox %var%
Var:="", Var3 := Var := SubStr(FileContents, 1)  ; Tests another optimization.
if (Var <> FileContents || Var3 <> FileContents)
	MsgBox %var%
Var:="", Var3 := Var := SubStr(FileContents, 1, 1000)  ; Tests another optimization.
if (Var <> SubStr(FileContents, 1, 1000) || Var3 <> SubStr(FileContents, 1, 1000))
	MsgBox %var%
Var:="abc", Var3 := Var := Var . SubStr(FileContents, 1, 1000)  ; Tests yet another optimization.
if (Var <> "abc" . SubStr(FileContents, 1, 1000) || Var3 <> "abc" . SubStr(FileContents, 1, 1000))
	MsgBox %var%

Var := ReturnFileContents("", 1)
if (Var <> FileContents)
	MsgBox %var%

Var := ReturnFileContents("`nAPPENDED TEXT", 1)
if (Var <> FileContents "`nAPPENDED TEXT")
	MsgBox %var%

Var := ReturnFileContents("`nAPPENDED TEXT", 10)
if (Var <> FileContents FileContents FileContents FileContents FileContents FileContents FileContents FileContents FileContents FileContents "`nAPPENDED TEXT")
	MsgBox %var%

Var := ReturnFileContents(ReturnFileContents("`nAPPENDED TEXT", 5), 5)
if (Var <> FileContents FileContents FileContents FileContents FileContents FileContents FileContents FileContents FileContents FileContents "`nAPPENDED TEXT")
	MsgBox %var%

Var := ReturnFileContents(ReturnFileContents("`nAPPENDED TEXT", 30), 5)
FileContents35 =
Loop, 35
	FileContents35 = %FileContents35%%FileContents%
if (Var <> FileContents35 "`nAPPENDED TEXT")
	MsgBox %var%

Var := WriteLocalArray(20000)
if Var <> 20000
	MsgBox %Var%

Var := WriteGlobalArray(20000)
if Var <> 20000
	MsgBox %Var%
if ZGlobal20000 <> 20000
	MsgBox %ZGlobal20000%

;ExitApp


; --------------------------------------------

MyMsgBox(aStr)
{
	MsgBox %aStr%.
}

MsgBox()
{
	MsgBox Msgbox() was called.
}

NeverCalled(x,y)
{
	MsgBox This function should never be called.
}

SideEffect()
{
	global Var
	Var = SideEffect
	goto xyz
	if (1)
	{
		goto xyz  ; Ignored.
	}
	return
}

Yield123()
{
	return 123
	msgbox BAD: never reached
}

YieldABC()
{
	return "ABC"
	msgbox BAD: never reached
}

YieldGlobalVar()
{
	global GlobalVar
	return GlobalVar
	msgbox BAD: never reached
}

YieldLocalVarContainingXYZ()
{
	Var = xyz
	return Var
	msgbox BAD: never reached
}

YieldEnvPath()
{
	return Path  ; Returns an evironment variable because any blank var (like Path) should auto-fetch env.
}

YieldProgramFiles()
{
	return ProgramFiles
}

YieldParam(aValueToReturn)
{
	return aValueToReturn
}

Concat(a1, a2 = 4.4, a3 = 11, a4 = "")
{
	return a1 a2 a3
}

MultiplyByTen(x)
{
	return x * 10
	msgbox BAD: never reached
}

Add(x, y, z = false)
; Keep this comment here to test that it doesn't disrupt distinguishing between a function call and a definition.
{
	if (A_ThisFunc <> "Add")
		MsgBox "%A_ThisFunc%"
	return x + y + z
	msgbox BAD: never reached
}

Add4(a,b,c,d)
{
	return Add(Add(a,b), Add(c,d))
}

Swap3(ByRef a, ByRef b, ByRef c)  ; Test cascading/passing ByRef to multiple layers of functions.
{
	; Though the order could be reversed more simply with a SINGLE call, namely Swap(a,c), do it this
	; way to exercise/test it more.
	Swap(a, b)
	Swap(a, c)
	Swap(b, c)
}

Swap(ByRef a, ByRef b)
{
	c = %a%
	a = %b%
	b = %c%
}

PrependTest(aStr)
{
	var = test
	return var aStr
}

ReturnFileContentsSimple()
{
	FileRead, var, %A_ScriptFullPath%
	return var
}

ReturnFileContents(aTextToAppend, aCount)
{
	FileRead, var, %A_ScriptFullPath%
	appended =
	;msgbox % "aCount = " aCount
	Loop %aCount%
	{
		appended = %appended%%var%
	}
	if aTextToAppend
		return appended aTextToAppend
	else
		return appended
}


WriteLocalArray(aCount)
{
	; ARRAY/VAR DATA INTEGRITY TEST:
	SetBatchLines -1  ; Just to be sure.
	Loop, %aCount%
		Local%A_Index% := A_Index
	Loop
	{
		if (A_Index > aCount)
			return A_Index - 1  ; Just for testing.
		if Local%A_Index% <> %A_Index%
			MsgBox Data corruption: Array item #%A_Index% contains something other than its index.
	}
}

WriteGlobalArray(aCount)
{
	Global
	; ARRAY/VAR DATA INTEGRITY TEST:
	SetBatchLines -1  ; Just to be sure.
	Loop, %aCount%
		ZGlobal%A_Index% := A_Index
	Loop
	{
		if (A_Index > aCount)
			return A_Index - 1  ; Just for testing.
		if ZGlobal%A_Index% <> %A_Index%
			MsgBox Data corruption: Array item #%A_Index% contains something other than its index.
	}
}


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

Var =
A := &Var
if not (!*A)
	MsgBox problem

Var = 1
A := &Var
if !*A
	MsgBox problem

Var := &Var
if not Var
	MsgBox problem
VarAddress := DllCall("CharNext", str, Var, UInt)
; When the first character in pSource isn't a binary zero, undo CharNext's increment.
if Var <>  ; Must explicitly compare to blank to avoid seeing "0" as false.
  VarAddress -= 1
if Var <> %VarAddress%
	MsgBox problem.
Var := &123
if Var <>
	MsgBox %var%

VarSetCapacity(Var, 15, 65)   ; 65 == 'A'
StringLeft, Var, Var, 15
if Var <> AAAAAAAAAAAAAAA
	MsgBox %Var%
VarSetCapacity(Var, 15, 0)
if Var <>
	MsgBox %Var%

Var = 123
VarAddress := &Var
Var2 := """" . *VarAddress . """"
if (Var2 <> """" . Asc("1") . """")
	msgbox %Var2%

Var = 123456789
VarAddress := &Var
Count = 0
Loop
{
	if (!*VarAddress)
		break
	if (*VarAddress <> Asc(A_Index))
		MsgBox % *VarAddress
	++Count
	++VarAddress
}
if Count <> 9
	MsgBox Count was expected to be 9.

VarSetCapacity(Var, 16, 0)
; The function "Verify" will also verify different values of pOffset and pSize for each call:
Verify(0xFFFFFFFF, Var)
Verify(0xF800F800, Var)
Verify(0x00F800F8, Var)
Verify(0, Var)
Verify(0xF8F8F8F8, Var)
Verify(44444, Var)
Verify(-44444, Var, true)
Verify(1, Var, true)
Verify(-1, Var, true)
Verify(-1, Var, true, 8)  ; True/false doesn't matter for 64-bit ints.
Verify(-1, Var, false, 8) ;
Verify(0xF8FF8FF8FFFF8FFF, Var, true, 8)
Verify(0xF800F800F800F800, Var, true, 8)
Verify(0x00F800F800F800F8, Var, true, 8)
Verify(0xFFFFFFFFFFFFFFFF, Var, true, 8)
Verify(0xFFFF, Var, true, 2)
Verify(0xFF, Var, true, 1)
Verify(0, Var, true, 2)
Verify(0, Var, false, 1)
Verify(0xF8F8, Var, false, 2)
Verify(0xF8, Var, true, 1)

Verify(pInteger, ByRef pVar, pIsSigned = false, pSize = 0)
{
	if pSize
		size := pSize
	else
		size := 4
	offset = 0
	InsertInteger(pInteger, pVar, offset, size)
	result := ExtractInteger(pVar, offset, pIsSigned, size)
	if (result <> pInteger)
		MsgBox Extracted %result% but inserted %pInteger%.
	offset = 4
	InsertInteger(pInteger, pVar, offset, size)
	result := ExtractInteger(pVar, offset, pIsSigned, size)
	if (result <> pInteger)
		MsgBox Extracted %result% but inserted %pInteger%.
	if not pSize  ; Caller wanted extra test of 8-byte integers
	{
		size := 8
		offset = 0
		InsertInteger(pInteger, pVar, offset, size)
		result := ExtractInteger(pVar, offset, pIsSigned, size)
		if (result <> pInteger)
			MsgBox Extracted %result% but inserted %pInteger%.
	}
}



ExtractInteger(ByRef pSource, pOffset = 0, pIsSigned = false, pSize = 4)
; pSource is a string (buffer) whose memory area contains a raw/binary integer at pOffset.
; The caller should pass true for pSigned to interpret the result as signed vs. unsigned.
; pSize is the size of PSource's integer in bytes (e.g. 4 bytes for a DWORD or Int).
; pSource must be ByRef to avoid corruption during the formal-to-actual copying process
; (since pSource might contain valid data beyond its first binary zero).
{
	Loop %pSize%  ; Build the integer by adding up its bytes.
		result += *(&pSource + pOffset + A_Index-1) << 8*(A_Index-1)
	if (!pIsSigned OR pSize > 4 OR result < 0x80000000)
		return result  ; Signed vs. unsigned doesn't matter in these cases.
	; Otherwise, convert the value (now known to be 32-bit) to its signed counterpart:
	return -(0xFFFFFFFF - result + 1)
}



InsertInteger(pInteger, ByRef pDest, pOffset = 0, pSize = 4)
; To preserve any existing contents in pDest, only pSize number of bytes starting at pOffset
; are altered in it. The caller must ensure that pDest has sufficient capacity.
{
	Loop %pSize%  ; Copy each byte in the integer into the structure as raw binary data.
		DllCall("RtlFillMemory", UInt, &pDest + pOffset + A_Index-1, UInt, 1, UChar, pInteger >> 8*(A_Index-1) & 0xFF)
}



;; BENCHMARK
;i = 1
;start_time := A_TickCount
;
;Loop, 10000
	;a += i
;elapsed := A_TickCount - start_time
;MsgBox %elapsed%
;ExitApp


; This undesired load-time syntax errors have been fixed:
;WinMove, x == "test with comma, test"
;WinMove, x == """test with comma"","

; This one is still a known issue load-time issue:
;WinMove, x == " ; "

Var := ~1 "1"   ; Should wind up being ~11 since auto-concat is highest prec.  Update: highest prec no longer exists.
if (Var <> "42949672941")
	MsgBox %Var%

Var := -5 "3"
if Var <> -53
	MsgBox %Var%

; Crashes in original version of 1.0.25:
i= 
var:=i<(10-11)
if !var
	MsgBox %var%

Var:=""<(10-11)
if !var
	MsgBox %var%

var := EmptyVar + 5   ; Empty string involved in a math operation resolves to blank.
if var <>
	MsgBox %var%

var = abc
var:="test" var   ; Bug-fix to allow no whitespace around :=
if var <> testabc
	MsgBox %var%

var := 5 + EmptyVar
if var <>
	MsgBox %var%

var := 
if var <> 
	MsgBox %var%

var := ()
if var <> 
	MsgBox %var%

var := +
if var <> 
	MsgBox %var%

var := -
if var <> 
	MsgBox %var%

var := -()
if var <> 
	MsgBox %var%

var := -3
if var <> -3
	MsgBox %var%

var := +3
if var <> 3
	MsgBox %var%

var := (-)
if var <> 
	MsgBox %var%

var := (-)(+)+-*/()
if var <> 
	MsgBox %var%

x = something non-numeric
var := x   ; Works only because ACT_ASSIGNEXP has special handling for simple assignment.
if var <> %x%
	MsgBox %var%

x = 3 + 4/2
var := x  ; By design, this won't evaluate the expression because vars containing non-numeric items are always strings.
if var <> %x%
	MsgBox %var%

; No longer tested since * is now a unary-deref operator:
;var := ((-*3))
;if var <>
	;MsgBox %var%
;
;var := /*3
;if var <>
	;MsgBox %var%
	
var := 5 + 10
if var <> 15
	MsgBox %var%

var := (5 + 10)
if var <> 15
	MsgBox %var%

var := -(5 + 10)
if var <> -15
	MsgBox %var%

var := -(-5 + -10)
if var <> 15
	MsgBox %var%

var := +2 - +4
if var <> -2
	MsgBox %var%

var := +(2 - +4)
if var <> -2
	MsgBox %var%

var := +(2 - +-4)  ; Probably most correct to consider +-4 to be 4, since the + traditionally has no effect, as in +y when y contains a negative.
if var <> 6
	MsgBox %var%

var := 2 + 5*2
if var <> 12
	MsgBox %var%

var := (2 + 5) * 0x2
if var <> 14
	MsgBox %var%

var := 2 + 6/2
if var <> 5
	MsgBox %var%

var := 4//3
if var <> 1
	MsgBox %var%

var := 4//0
if (var <> "")
	MsgBox %var%

var := 4.0//0.0
if (var <> "")
	MsgBox %var%

var := -5//3
if var <> -1
	MsgBox %var%

var := -6//3
if var <> -2
	MsgBox %var%

var := 4.0//-3
if var <> -2
	MsgBox %var%

var := -4//3.0
if var <> -2
	MsgBox %var%

var := (2 + 6) / 2
if var <> 4
	MsgBox %var%

var := (((2 + 6) / 2))
if var <> 4
	MsgBox %var%

var := ((((9) + -(3)) / 2))
if var <> 3
	MsgBox %var%

var := 1 + -(-2 + 3)
if var <> 0
	MsgBox %var%

var := 1 < 2
if (var <> true)
	MsgBox %var%

var := 1 > 2
if var <> 0
	MsgBox %var%

var := 1 <> 2
if var <> 1
	MsgBox %var%

var := 1 >= 2
if var <> 0
	MsgBox %var%

var := 1 >= 2 + 10/7
if var <> 0
	MsgBox %var%

var := 3 >= 2 + 7/7
if var <> 1
	MsgBox %var%

;
; POWER: TEST ONES THAT OMIT PARENS AROUND THE FIRST NEGATIVE (since POWER is higher precedence than unary minus):
;
var := -2**2
if var <> -4
	MsgBox %var%

var := (-2)**2
if var <> 4
	MsgBox %var%

Var = -2
if (Var**2 <> 4)
	MsgBox Problem

var := 2**5 + -3**3  ; Power takes precedence over unary minus.
if var <> 5
	MsgBox %var%

var := 2**5 + 3**+3
if var <> 59
	MsgBox %var%

var := 2**5 + -(-3**3)
if var <> 59
	MsgBox %var%

var := 2**5 + -(-3**3)
if var <> 59
	MsgBox %var%

var := -2**-2  ; REMEMBER THAT ** IS HIGHER PRECEDENCE THAN UNARY MINUS.
if var <> -0.25
	MsgBox %var%

var := -2**+2
if var <> -4
	MsgBox %var%

var := -2**-2
if var <> -0.25
	MsgBox %var%


; POWER: TEST OTHER NON-SIMPLE CASES:
var := 2**3**2  ; Unlike other languages, left-to-right eval for this (to keep code simple).
if var <> 64
	MsgBox %var%

var := 2**(3**2)
if var <> 512
	MsgBox %var%

var := 0**-2 = false  ; Result of 0 to a negative power should be empty string, so this expression should be false.
if var <> 0
	MsgBox %var%

var := 2**-(2 + 1)
if var <> 0.125
	MsgBox %var%

var := (-0.5)**(-2.5+0.5)
if var <> 4
	MsgBox %var%


; POWER: STANDARD/EASY CASES (called as function to also test "Transform, Pow".

pow(0,0,0)
pow(0,1,0)
pow(-3,2,9)
pow(-1,0,1)
pow(-1,1,-1)
pow(-1,-1,-1)
pow(-2,2,4)
pow(-2,4,16)
pow(-2,-2,0.25)
pow(-4,2.0,16)
pow(-4,-2.0,0.0625)
pow(-0.5,2.0,0.25)
pow(2,-2,0.25)
pow(4,-0.5,0.5)
pow(4,0.5,2)

; Undefined ones that should yield empty string:
pow(0,-1,"")
pow(-4,0.5,"")
pow(-4,-0.5,"")
pow(-0.5,-2.5,"")
pow(0,-2,"")

pow(base, exp, result)
{
	result2 := base**exp
	if (result2 <> result)
		MsgBox %base%**%exp% is %result2% instead of the expected %result%.
	Transform, result2, pow, base, exp
	if (result2 <> result)
		MsgBox "Transform Pow" of %base% & %exp% is %result2% instead of the expected %result%.
}

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


Var := 2/0
if var <> 
	MsgBox %var%

var := 2/0.0
if var <> 
	MsgBox %var%

var := 3 + 2.0
IfNotInString, Var, 5.000000
	msgbox %var%

var := 3.2 + 2.3
IfNotInString, Var, 5.500000
	msgbox %var%

var := 3.0 + 2 + 0xB/2
IfNotInString, Var, 10.500000
	msgbox %var%

var := 3.0 + 2 + 11/2.0
IfNotInString, Var, 10.500000
	msgbox %var%

var := 15 * 0.0
IfNotInString, Var, 0.000000
	msgbox %var%

tick_start = %A_TickCount%
Sleep, 50
elapsed := A_TickCount - tick_start
if elapsed not between 46 and 51  ; New dual-core CPU vs. AthlonXP needs a lower low-limit (formerly 49).
	MsgBox %elapsed% <> 50

Array1 = test
i = 1

Var := Array%i% = blank_var
if var <> 0
	MsgBox %var%

compare = test
Var := Array%i% = compare
if var <> 1
	MsgBox %var%

string = test
var := -string  ; Math operations on strings should fail.
if var <>
	MsgBox %var%
var := +string  ; However unary plus is ignored. Obsolete: But the result of an expression can't be a string, so this fails too.
if var <> %string%
	MsgBox %var%

Array_1_3 = 15
i = 1
j = 3
test = 6
var := 3 + Array_%i%_%j%/test
if var <> 5.5
	MsgBox %var%
var := (3+Array_%i%_%j%)/test
if var <> 3
	MsgBox %var%

x = test
var := x
if var <> %x%
	MsgBox %var%

x = 3
var := x
if var <> %x%
	MsgBox %var%

x = 3
var := -x
if var <> -%x%
	MsgBox %var%

x = i
i = test
var := %x%
if var <> %i%
	MsgBox %var%

Array1 = test
i = 1
var := Array%i%
if var <> test
	MsgBox %var%

x = 100
start_time := A_TickCount
sleep x
elapsed := A_TickCount - start_time
elapsed_wrong := elapsed - x
if elapsed_wrong not between -10 and 10  ; New dual-core CPU (vs. Athlon XP) needs greater tolerance (formerly was -2 to 2)
	MsgBox %elapsed% <> 100

Array1 = 100
i = 1
start_time := A_TickCount
sleep Array%i%
elapsed := A_TickCount - start_time
elapsed_wrong := elapsed - x
if elapsed_wrong not between -10 and 10
	MsgBox %elapsed% <> 100

var := """ and string containing ""literal quoted string"" inside and """" and """
if var <> " and string containing "literal quoted string" inside and "" and "
	MsgBox problem

var := "%thing%"
if var <> `%thing`%
	msgbox thing

; Load-time errors as expected:
;a := 5 % 3
;a := 5 `% 3

a = y
var := a="y"
if var <> 1
	MsgBox %var%

a = string with spaces
var := a <> "string with spaces"
if var <> 0
	MsgBox %var%

test = MyString
1 = test
var := %1%
if var <> %test%
	msgbox %var%

1x = 55   ; Test var whose name starts with a number.
var := 2 + 1x + 3
if var <> 60
	MsgBox %var%

var := -"0"
if Var <>
	MsgBox %Var%

var := !"0"
if Var <> 0
	MsgBox %Var%

var := !0
if Var <> 1
	MsgBox %Var%

var := 0
if Var <> 0
	MsgBox %Var%

var := !0
if Var <> 1
	MsgBox %Var%

var := 1
if Var <> 1
	MsgBox %Var%

var := !2
if Var <> 0
	MsgBox %Var%

var := 3 > 5
if var <> 0
	MsgBox %Var%

var := 1 > 2&&1 > 3
if var <> 0
	MsgBox %Var%

var := 1 > 2||1 > 3
if var <> 0
	MsgBox %Var%

var := 1 > 2 && not 1 > 3
if var <> 0
	MsgBox %Var%

var := 1 > 2||not 1 > 3
if var <> 1
	MsgBox %Var%


var := 1 < 2 and 1 < 3
if var <> 1
	MsgBox %Var%

var := 1 < 2 or 1 < 3
if var <> 1
	MsgBox %Var%

var := 1 < 2 and not 1 < 3
if var <> 0
	MsgBox %Var%

var := 1 < 2 or not 1 < 3
if var <> 1
	MsgBox %Var%


var := 1 < 2 and 1 > 3
if var <> 0
	MsgBox %Var%

var := 1 < 2 or 1 > 3
if var <> 1
	MsgBox %Var%

var := 1 < 2 and not 1 > 3
if var <> 1
	MsgBox %Var%

var := 1 < 2 or not 1 > 3
if var <> 1
	MsgBox %Var%


var := not 1 < 2 and (1 > 3)
if var <> 0
	MsgBox %Var%

var := not 1 < 2 or (1 > 3)
if var <> 0
	MsgBox %Var%

var := not 1 < 2 and !(1 > 3)
if var <> 0
	MsgBox %Var%

var := not 1 < 2 or !(1 > 3)
if var <> 1
	MsgBox %Var%


var := not (1 and 3)
if var <> 0
	MsgBox %Var%

var := not (1 and 0)
if var <> 1
	MsgBox %Var%

var := !(1 or 0)
if var <> 0
	MsgBox %Var%

var := !(0 or 0)
if var <> 1
	MsgBox %Var%

x = -3
var := !x
if var <> 0
	MsgBox %Var%

var := !+3   ; Works only because unary plus is ignored.
if var <> 0
	MsgBox %Var%

Var := !(-3)
if var <> 0
	MsgBox %Var%

Var := -(!3)
if var <> 0
	MsgBox %Var%

Var := -(!0)
if var <> -1
	MsgBox %Var%
; BUT FOR SIMPLICITY OF CODE, things like !-3 and -!3 are not supported.

if (3 > 5)
	MsgBox Problem
if (not 3 < 5)
	MsgBox Problem
if (not 3 >= 3)
	MsgBox Problem

var = 0
if var
	MsgBox Problem
var = 1
if not var
	MsgBox Problem
var =
if var
	MsgBox Problem
var = abc
if !var
	MsgBox Problem
if !var and 3 > 4
	MsgBox Problem

var = 5
if (var > 5) or 2 > 3
	MsgBox Problem

if var*3 > 15
	MsgBox Problem

if not var*3 = 15
	MsgBox Problem

if (5 != 5)
	MsgBox Problem

if ~0 <> 0xFFFFFFFF  ; 32-bit unsigned.
	MsgBox Problem

if ~1 <> 0xFFFFFFFE  ; 32-bit unsigned.
	MsgBox Problem

if ~0xFFFFFFFF <> 0
	MsgBox Problem

if ~0x0000FFFFFFFFFFFF <> -281474976710656  ; Can't compare to this because it's too large for ATOI64: 0xFFFF000000000000
	MsgBox Problem

; 0xFFFFFFFFFFFFFFFF is seen (perhaps arbitrarily) as the maximum positive signed 64-bit int, namely 0x7FFFFFFFFFFFFFFF:
if ~0xFFFFFFFFFFFFFFFF <> -9223372036854775808 ; -9223372036854775808 is one less than the smallest 64-bit signed int.
	MsgBox Problem

if ~-1 <> 0
	MsgBox Problem

if ~-2 <> 1
	MsgBox Problem

if !-2 <> 0
	MsgBox Problem

if !!-2 <> 1
	MsgBox Problem

if -!2 <> 0
	MsgBox Problem

if -(-2) <> 2
	MsgBox Problem

if --2 = 3
	MsgBox Problem

if 0xF0F0 | 0xF <> 0xF0FF
	MsgBox Problem

if 0xFFFFFFFFF0F0 | 0xFFFF000F <> 0xFFFFFFFFF0FF
	MsgBox Problem

if 0xF0F0 & 0x00FF <> 0xF0
	MsgBox Problem

if 0xFFFF0000F0F0 & 0xF0F0F0F000FF <> 0xF0F0000000F0
	MsgBox Problem

if 0xF0F0 ^ 0x00FF <> 0xF00F
	MsgBox Problem

if 0xFFFF0000F0F0 ^ 0xF0F0F0F000FF <> 0x0F0FF0F0F00F
	MsgBox Problem

if ~0x0 & 0xFFFFFFFFFFFF ^ 0xF0F0 <> 0xFFFF0F0F
	MsgBox Problem

if !0.0 <> 1
	MsgBox Problem

if !1.1 <> 0
	MsgBox Problem

if !0.4 <> 0
	MsgBox Problem

if (1 << 5) <> 32
	MsgBox Problem

if (1 >> 5) <> 0
	MsgBox Problem

if (0xFFFF0000FFFF >> 16) <> 0xFFFF0000
	MsgBox Problem

if (0xFFFF0000FFFF >> 32) <> 0xFFFF
	MsgBox Problem

if (0xFFFF << 32) <> 0xFFFF00000000
	MsgBox Problem

if (0xFFFF << 48) <> -281474976710656 ; 64-bit results are always signed in AHK.  Can't compare directly to 0xFFFF000000000000 since ATOI64 can't handle that number.
	MsgBox Problem

if (-1 >> 1) <> -1  ; Shifting right retains the sign bit.
	MsgBox Problem

if (-3 << 62) <> 0x4000000000000000  ; Shifting left (unlike right) doesn't retain the sign bit.
	MsgBox Problem

if !5**2 <> 0
	MsgBox Problem

if !0**2 <> 1
	MsgBox Problem

if (!0)**2 <> 1
	MsgBox Problem

if !35**0 <> 0
	MsgBox Problem


AutoTrim, off

var = String with spaces at its end%A_Space%%A_Space%
if (var <> "String with spaces at its end  ")
	MsgBox %var%

var = Normal String
if (var <> "Normal String")
	MsgBox %var%
if (not var == "Normal String")  ; Case sens.
	MsgBox %var%
if (var == "normal string")  ; Case sens.
	MsgBox %var%

var := "  Normal String  "
if (var = "Normal String")
	MsgBox %var%
if (var != "  Normal String  ")
	MsgBox %var%

var = 33
if (not var)
	MsgBox %var%

var = 0
if(var)  ; Also tests having no space before paren.
	MsgBox %var%
if(-var)
	MsgBox %var%

Var = string
if !var
	MsgBox %var%

Var := "string"
if !var
	MsgBox %var%
if var <> string  ; Legacy
	MsgBox %var%

if 3 + 3 <> 6
	MsgBox Problem

if 0
	MsgBox Problem
if not 1
	MsgBox Problem
if not !0
	MsgBox Problem
if !(not !33)
	MsgBox Problem

if (3 + 3 "string") <> "6string"
	MsgBox Problem
if !(3 + 3 "string") != 0  ; Update: it's no longer "bad": The bitwise-not of a bad expression evaluates to true.
	MsgBox Problem

var := 3 + 3 "string"
if var <> 6string
	MsgBox %var%

var := +"string"
if var <> string
	MsgBox %var%

var := "string"
if var <> string
	MsgBox %var%

var := 3/0
if var <>
	MsgBox %var%

var := !"string"  ; Unlike unary minus, a string literal is true.  Thus !true is false.
if var <> 0
	MsgBox %var%

var = string
var := -var
if var <>
	MsgBox %var%

var := "abc" and "def"
if var <> 1
	MsgBox %var%

x = abc
y = def
var := x and y
if var <> 1
	MsgBox %var%

var := x==x and y==y
if var <> 1
	MsgBox %var%

if !x or !y
	MsgBox Problem

if 0 or 0
	MsgBox Problem

x =
y =
if x or y
	MsgBox Problem


if 0 or 0 or 0
	MsgBox Problem
if !(0 or 0 or 1)
	MsgBox Problem
if !(0 or 1 or 0)
	MsgBox Problem
if !(0 or 1 or 1)
	MsgBox Problem
if !(1 or 0 or 0)
	MsgBox Problem
if !(1 or 0 or 1)
	MsgBox Problem
if !(1 or 1 or 0)
	MsgBox Problem
if !(1 or 1 or 1)
	MsgBox Problem

if 0 and 0 and 0
	MsgBox Problem
if 0 and 0 and 1
	MsgBox Problem
if 0 and 1 and 0
	MsgBox Problem
if 0 and 1 and 1
	MsgBox Problem
if 1 and 0 and 0
	MsgBox Problem
if 1 and 0 and 1
	MsgBox Problem
if 1 and 1 and 0
	MsgBox Problem
if !(1 and 1 and 1)
	MsgBox Problem

if !(1 or 0 and 1)
	MsgBox Problem
if !(0 or 1 and 1)
	MsgBox Problem


; TEST PRECEDENCE of all adjacent precedence levels:
if !(1 or 1 and 0)  ; OR below AND
	MsgBox Precedence problem.
if (not 1 and 0)  ; AND below NOT
	MsgBox Precedence problem.
if !(not 1 = 0)  ; NOT below equal/not-equal
	MsgBox Precedence problem.
if (0 = 3 < 4)  ; EQUAL/NOT-Equal below Greater-than/Less-than
	MsgBox Precedence problem.
if (3 < 1 | 1)  ; Relational below Bitwise OR
	MsgBox Precedence problem.
if !(1 | 1 & 0)  ; Bit-or below Bit-and (no test is done for OR below XOR, since that is a rarity)
	MsgBox Precedence problem.
if (1 & 4 >> 1) = 1  ; Bit-and below Bit-shift
	MsgBox Precedence problem.
if (2 >> 1 + 1) != 0  ; Bit-shift below add/subtract
	MsgBox Precedence problem.
if (2 + 3 * 4 + 6/6) != 15  ; Add/subtract below Mult/Div
	MsgBox Precedence problem.
if (2 * -2**2) != -8 ; Power the highest precedence of all
	MsgBox Precedence problem.

var := "my string"
stringleft, var, var, 1+1
if var <> my
	MsgBox %var%

x = 25
ylong = 44
Array1 = 5
i = 1
var := 1+88/ylong+x/Array%i%**2
if var <> 4
	MsgBox %var%
var := 1+88/ylong+(x/Array%i%)**2
if var <> 28
	MsgBox %var%

SetWinDelay 0
var = default
Run, calc
WinWaitActive, Calculator
WinMove,,, 100 + 50, 44/4, 200-50, %var%   ; Min size allowed by OS is usually 112
WinGetPos, X, Y, W, H
if X <> 150
	MsgBox Position problem.
if Y <> 11
	MsgBox Position problem.
if W <> 150
	MsgBox Size problem.
WinMove,,, default, default, 2**7, default  ; Looks like calc doesn't allow it's height to be changed, only its width.
WinGetPos, X, Y, W, H2
if X <> 150
	MsgBox Position problem.
if Y <> 11
	MsgBox Position problem.
if W <> 128
	MsgBox Size problem.
if H <> %H2%
	MsgBox Size problem.
WinMove, default, default
WinGetPos, X, Y, W, H2
if X <> 150
	MsgBox Position problem.
if Y <> 11
	MsgBox Position problem.
if W <> 128
	MsgBox Size problem.
if H <> %H2%
	MsgBox Size problem.
WinMove, 33, 44
WinGetPos, X, Y, W, H2
if X <> 33
	MsgBox Position problem.
if Y <> 44
	MsgBox Position problem.
if W <> 128
	MsgBox Size problem.
if H <> %H2%
	MsgBox Size problem.
WinMove, 500 - 33/11, (3 < 4) + (5 > 6) + 25/10
WinGetPos, X, Y
if X <> 497
	MsgBox Position problem.
if Y <> 3
	MsgBox Position problem.
WinMove, "str" == "STR", "str"",""" = "STR"","""  ; Test that quoted-commas are seen as arg-delimiters.
WinGetPos, X, Y
if X <> 0
	MsgBox Position problem.
if Y <> 1
	MsgBox Position problem.
WinClose


; The following is here so that a warning will be given if a comma ever becomes an operator but forget that
; comma has special meaning (date math) for += and -=:
date := 20050101
date += 31, days
if date <> 20050201000000
	MsgBox Date problem with += ... "%date%"
date := 20050101
date += 15+16, days  ; EnvAdd (but not sub) supports expressions with date-math.
if date <> 20050201000000
	MsgBox Date problem with += ... "%date%"
date -= 20050101, days
if date <> 31
	MsgBox Date problem with -= ... "%date%"
date = 20050201000000
Var = 20050101
VAr2 = days
date -= Var, %Var2%  ; This was a bug once.
if date <> 31
	MsgBox Date problem with -= ... "%date%"
Var = Days
date := 20050101
date += 31, %Var%
if date <> 20050201000000
	MsgBox Date problem with += ... "%date%"

x := 3
x += 4 + 3/3
if x <> 8
	MsgBox %x%

x = 1
x += "str" == "str"
if x <> 2
	Msgbox %x%

; Ensure commas aren't seen as arg-delimiters (in this case delimiting days/hours) when they're inside double quotes:
x = 1
x += "str," == "str,"
if x <> 2
	Msgbox %x%

x = 1
x += "str"",""" == "str"","""
if x <> 2
	Msgbox %x%

x = 3
x *= 5/2
if x <> 7.5
	MsgBox %x%


; Ensure load-time tricks to accelerate perf. don't mess anything simple up:
var = 1
if var
	Sleep, 0
else
	MsgBox %var%

var = 0
if var
	MsgBox %var%

var =
if var
	MsgBox %var%

var = 1
Array1 = 15
i = 1
var += Array%i%
if var <> 16
	MsgBox %var%

var := "string"
if var <> string
	MsgBox %var%

var := 10*10
if var != 100
	MsgBox %var%

if 1 and!0<>1
	Msgbox Problem

if (1)and(0) <> 0
	MsgBox Problem

var = string
var := "test " var
if var <> test string
	MsgBox %var%

var = xxx
var := var " test " var
if var <> xxx test xxx
	MsgBox %var%

var = xxx
var := var 123 var
if var <> xxx123xxx   ; Spaces are only transcribed when inside "".
	MsgBox %var%

var = xxx
var := var " " 123 " " var " "
if var <> xxx 123 xxx%A_Space%   ; Spaces are only transcribed when inside "".
	MsgBox %var%

Var := "abc" + 123
if var <>
	MsgBox %var%

Array1 = xyz
i = 1
var := "abc" Array%i% 123
if var <> abcxyz123
	MsgBox %var%

var := Array%i% + 123
if var <>
	MsgBox %var%

Array1 = 5
var := Array%i% + 123
if var <> 128
	MsgBox %var%

var = xxx
var := "abc"var
if var <> abcxxx
	msgbox %var%

var := (!"0"123)   ; !"0", since quoted "0" is "true", is !true concatenated with 123
if (var <> "0123")
	msgbox %var%

var := 0 "" 1 "" 2 "" "A"
if (var <> "012A")
	msgbox %var%

var := !0 ""
if (var <> 1)
	msgbox %var%

var := 1 "" 0
if (var <> "10")
	msgbox %var%

var := 0 "" 1 "" 2 "" "A"
if (var <> "012A")
	msgbox "%var%"

var := "" "" ""
if (var <> "")
	msgbox "%var%"

var := 0 ""1 ""2 """A"
if (var <> "012""A")
	msgbox "%var%"


SoundGet, MasterOrig  ; Save orig
SetFormat, Float, 0  ; Sound that floating point vol levels are rounded off automatically.
SoundSet, 100
SoundSet -50
SoundGet, Master
if Master <> 50
	MsgBox %Master%
Var = 10
SoundSet, -%Var%
SoundGet, Master
if Master <> 40
	MsgBox %Master%
SoundSet, +%Var%
SoundGet, Master
if Master <> 50
	MsgBox %Master%
SoundSet, Var+Var/5+15
SoundGet, Master
if Master <> 27
	MsgBox %Master%
SoundSet, +Var+Var/5+15
SoundGet, Master
if Master <> 54
	MsgBox %Master%
SetFormat, Float, 0.6

SoundSet, %MasterOrig%  ; Restore

if not "0"
	MsgBox Problem

; Write to the array:
loop, 5
	Random, array%a_index%, 1, 100  ; Put a random number into each element.
; Read from the array:
loop
{
	element := array%a_index%
	if not element  ; Blank item encountered, so this is the end of the array.
		break
	if A_Index > 5 or element < 1 or element > 100
		MsgBox %element%
}

if ("BlueRed" != "Blue" "RED")  ; Case insens.
	MsgBox problem

if ("BlueRed" == "Blue" "RED")  ; Case sens.
	MsgBox problem

Var = Red
if ("BlueRedRed" <> ("Blue" Var) "Red")
	MsgBox problem


Var = xxx
Dyn = Var
if (%Dyn% <> "xxx")
	MsgBox % %Dyn%


var := "x" . %xyxyx%%ziziz%
if var <> x
	MsgBox "%Var%"

var := "x" . %xyxyx%_%ziziz%
if var <> x
	MsgBox "%Var%"

var := %xyxyx% . %ziziz%
if var <>
	MsgBox "%Var%"

var := "x" %xyxyx% %ziziz% "x"
if var <> xx
	MsgBox "%Var%"

var := "x" . %xyxyx% . %ziziz% . "x"
if var <> xx
	MsgBox "%Var%"

Var := "x" . Array%BlankVar%
if var <> x
	MsgBox "%Var%"

Var := 100/4 . Array%BlankBogusVar%
if Var <> 25
	MsgBox %Var%

Var := "x" . 100/(3 + Array%BlankVar%)
if Var <> x
	MsgBox %Var%

Var := %BlankVar% 123
if Var <> 123
	MsgBox %Var%

Var := 123 %BlankVar%
if Var <> 123
	MsgBox %Var%

DynVar1 = A_ScriptFullPath
Var := 123 %DynVar1%
if Var <> 123
	MsgBox %Var%

DynVar1 = ProgramFiles
Var := %DynVar1% 123
if Var <> 123
	MsgBox %Var%

NewArray = yy
Var := "x" . NewArray%BlankVar%
if Var <> xyy
	MsgBox %Var%

Var := 100/(3 + Array%BlankBogusVar%)
if Var
	MsgBox %Var%

Var := 100/(4 . Array%BlankBogusVar%)
if Var <> 25
	MsgBox %Var%

Var := 100/4 . Array%BlankBogusVar%
if Var <> 25
	MsgBox %Var%

var = 1
if (var = %bogus%)
	MsgBox %var%

var := !(1 = Array%bogus%)
if var <> 1
	Msgbox %var%

bogus =
var := !(1 = Array%bogus%)
if var <> 1
	Msgbox %var%

Array = 1
var := 3 + (Array%bogus%)/2
if var <> 3.5
	MsgBox %var%

var := 3 + (Array%bogus%)
if var <> 4
	MsgBox %var%

bogus = 1
Array1 = 2
var := 3 + (Array%bogus%)
if var <> 5
	MsgBox %var%

test = blue
var = test
var := %var%
if var <> blue
	MsgBox %var%

var = test
var := (%var%)
if var <> blue
	MsgBox %var%

test = 
Array = x
var := %test%Array
if var <> x
	msgbox %var%

Array = x
var := %NoWay%Array
if var <> x
	msgbox %var%

test = 1
1Array = ccc
var := %test%Array
if var <> ccc
	msgbox %var%

var := ((%test%Array))
if var <> ccc
	msgbox %var%

Array1 = 11
i = 1
var := (Array%i%)
if var <> 11
	msgbox %var%

test = xxx
var := Array%i% test
if var <> 11xxx
	msgbox %var%

var := Array%i% "yyy"
if var <> 11yyy
	msgbox %var%

var := "a" . 123 + 100
if Var <> a223
	msgbox %var%

var := (1+2)(3+1)/2   ; Division has higher prec than concat.
if Var <> 32
	msgbox %var%

var := "x" 1+2  ; Update: it works now. legacy concat has highest prec., so this is "x1"+2, which is blank.
if Var <> x3
	msgbox %var%

var := "x" (1+2)
if Var <> x3
	msgbox %var%

var := "x" !1  ; No auto-concat for this, see code for comments.
if Var <>
	msgbox %var%

x = Test
var := x . !1
if Var <> Test0
	msgbox %var%

var := Array%i% 123 + 100  ; 112ay1 ... 112ay1 123
if var <> 11223
	msgbox %var%

1 = temp
i = 1

Var := "abc" %i%
if Var <> abctemp
	MsgBox %var%

Array1 = test
Var := Array%i% "abc"
if Var <> testabc
	MsgBox %var%

Var := "abc" Array%i%
if Var <> abctest
	MsgBox %var%

Var := i Array%i%
if (Var <> "1test")
	MsgBox %var%

Var := Array%i% i
if (Var <> "test1")
	MsgBox %var%

var := -"test" = 3
if var <> 0
	MsgBox %Var%

var := ~"test" = 3
if var <> 0
	MsgBox %Var%

var := "abc" + 3 = 5
if var <> 0
	MsgBox %var%

var := 3/0 = false
if var <> 0
	MsgBox %var%

AutoTrim on

Var := "  12345"
if (Var <> "  12345")
	MsgBox "%var%"

i = 1
Array1 = test
Var := Array%i% "  12345"
if (Var <> "test  12345")
	MsgBox "%var%"

var := true
if var <> 1
	MsgBox %var%

var := false
if var <> 0
	MsgBox %var%

var = abc
if var  ; Just to be sure it works for simplest case.
	Sleep 0
else
	MsgBox %var%

var = abc
if !var
	MsgBox %var%

var = 123abc
if !var
	MsgBox %var%

var = 000abc
if !var
	MsgBox %var%

var =
if var
	MsgBox %var%

var = 0.0
if var
	MsgBox %var%

var = 0x0
if var
	MsgBox %var%

var = 0.0e0
if var
	MsgBox %var%

var = 0.0e+1
if var
	MsgBox %var%

var = 0.0e-1
if var
	MsgBox %var%

var := " "
if !var
	MsgBox %var%

var =
if (var <> "")
	MsgBox %var%

var = 123
if (var = "")
	MsgBox %var%

; Misc. test:
str = 1,2,,3,4
stringsplit, out, str, `,
if out3 <>
	Msgbox %out3%


Clipboard := ClipSaved  ; Restore for user.
MsgBox Done
ExitApp

; -------------------------

xyz:
msgbox This should never appear because any goto from inside an fn body to outside of it should be ignored.
return

#space::MsgBox Hotkey to serve as an interrupting thread. (var = %var%).


; THE FOLLOWING LINES ARE NEVER EXECUTED BUT KEPT HERE TO CATCH INVALID SYNTAX ERROR MESSAGES BROUGHT ON BY FUTURE
; CHANGES TO HOW FUNCTIONS VS. LABELS VS. NORMAL COMMANDS ARE DIFFERENTIATED FROM EACH OTHER BY THE LOAD-TIME ROUTINES:

MsgBox,xyz()   ; Previously a load-time error due to bad recognition of functions in too many places.
hotkey, ^!z, Label(bho)   ; Same
Label(bho):
Label():
Label(:
Label):

;::myhotstring::repl(parentheses)  ; <<< Must still check for colons.

;#!+space::MsgBox()   ; Single-line hotkey to call function (there is special code to handle this).

Error:
ListLines
MsgBox Problem. Var = "%Var%"
if (A_ThisLabel <> "Error")
	MsgBox "%A_ThisLabel%"
return
