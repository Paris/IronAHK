#Include %A_ScriptDir%/header.ahk

SetWorkingDir %A_ScriptDir%
#SingleInstance

; Set default here, otherwise timer test will reveal more timer issues probably due to reverting to
; 10ms batchlines every time the timer's thread launches:
SetBatchLines -1

goto, start
; ALREADY TESTED:
;ensure comma delimiter works without whitespace, and with tabs

;#h::SetNumlockState, alwaysoff
;#Y::SetNumlockState, on

; gives error as expected: mouseclickdrag, left, , , , , 5

errorlevel = 1
msgbox, errorlevel = %errorlevel%`n`nNow press win-f12 to reset it`, but upon returning here it should be restored to its old value.
msgbox, errorlevel = %errorlevel%
exitapp

; TO TEST RESPONSIVENESS DURING LONG READS.  IT WAS OK:
;filereadline, linevar, C:\Big File.txt, 9999999
;msgbox, %linevar%
;exitapp

;NOTE: WinKill works ok, at least as often as the aut3 version.  Tested it on a hung MSIE window.

; ok
sleep, 1000
Send, {ASC 65}  ; Same as Send, {altdown}{Numpad6}{Numpad5}{altup}
Send, {altdown}{Numpad6}{Numpad5}{altup}
exitapp

FileSelectFile, filename, 24, d:\   ; I've tested all the modes, 5:28 PM Saturday, November 08, 2003
if errorlevel = 0
	msgbox, %filename%
else
	msgbox, the dialog was cancelled
exitapp


return

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
start:
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; This is 10000 characters long (reduces the overhead of having to keep expanding the variable so often).
; It is used here and for the StringReplace test:
String10K =0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789


x := Username  ; This was a a bug once, but only with the := operator.
if not x
	MsgBox Possible problem with environment variables.

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; SORT COMMAND

; SORTING VIA CALLBACK FUNCTION
TestSort(Source, Expected, Options = "", RunUDF = true)
{
	if RunUDF
		Source2 := Source
	Sort, Source, %Options%
	if (Source <> Expected)
	{
		ListLines
		MsgBox Sort #1 problem "%Source%"`nWas expected to be: "%Expected%".
	}
	ChkLen(Source)

	if not RunUDF
		return
	Sort, Source2, , %Options% fStringSort
	if (Source2 <> Expected)
	{
		ListLines
		MsgBox Sort #2 problem "%Source2%"`nWas expected to be: "%Expected%".
	}
	ChkLen(Source2)
}

TestSortNumUDF(Source, Expected, Options = "")
{
	Sort, Source, f NumSort %Options%
	if (Source <> Expected)
		MsgBox Sort problem "%Source%"`nWas expected to be: "%Expected%".
	ChkLen(Source)
}

TestSortOffset(Source, Expected, Options = "")
{
	Sort, Source, f ReverseDir %Options%
	if (Source <> Expected)
		MsgBox Sort problem "%Source%"`nWas expected to be: "%Expected%".
	ChkLen(Source)
}


StringSort(a1, a2)
{
	return a1 > a2 ? 1 : a1 < a2 ? -1 : 0
}

NumSort(a1, a2)
{
	return a1 - a2
}

ReverseDir(a1, a2, offset)
{
	return offset
}

; SORT PROBLEMS FIXED IN v1.0.47.05:
TestSort("x`nx", "x", "U")
TestSort("x`nx`n", "x`n", "U")
TestSort("x`nx", "x", "UZ")
TestSort("x`nx`n", "`nx", "UZ")
TestSort("x`n`nx", "x`n", "UR", false)      ; Pass false for these because "R" option not supported by UDF.
TestSort("x`n`nx`n", "x`n`n", "UR", false)  ;
TestSort("x`n`nx", "x`n", "UZR", false)     ;
TestSort("x`n`nx`n", "x`n", "UZR", false)   ;

; Same as above but with CRLF vs. LF:
TestSort("x`r`nx", "x", "U")
TestSort("x`r`nx`r`n", "x`r`n", "U")
TestSort("x`r`nx", "x", "UZ")
TestSort("x`r`nx`r`n", "`r`nx", "UZ")
TestSort("x`r`n`r`nx", "x`r`n", "UR", false)         ; Pass false for these because "R" option not supported by UDF.
TestSort("x`r`n`r`nx`r`n", "x`r`n`r`n", "UR", false) ;
TestSort("x`r`n`r`nx", "x`r`n", "UZR", false)        ;
TestSort("x`r`n`r`nx`r`n", "x`r`n", "UZR", false)    ;

; Same as above set but with custom delimiter:
TestSort("x|x", "x", "U D|")
TestSort("x|x|", "x|", "U D|")
TestSort("x|x", "x", "UZ D|")
TestSort("x|x|", "|x", "UZ D|")
TestSort("x||x", "x|", "UR D|", false)    ; Pass false for these because "R" option not supported by UDF.
TestSort("x||x|", "x||", "UR D|", false)  ;
TestSort("x||x", "x|", "UZR D|", false)   ;
TestSort("x||x|", "x|", "UZR D|", false)  ;

; Original bug report example:
TestSort("5|6|5|6|5", "5|6", "D| U")

TestSort("test", "test")
TestSort("test`n", "test`n")
TestSort("test`n", "`ntest", "Z")
TestSort("def`r`nabc`r`nmno`r`nfgh`r`n", "abc`r`ndef`r`nfgh`r`nmno`r`n")
TestSort("def`nabc`nmno`nFGH`nco-op`ncoop`ncop`ncon`n", "abc`nco-op`ncon`ncoop`ncop`ndef`nFGH`nmno`n") ; It should use string sort, not MS's word sort.
TestSort("abc`nco-op`ncon`ncoop`ncop`ndef`nFGH`nmno`n", "mno`nFGH`ndef`ncop`ncoop`ncon`nco-op`nabc`n", "R", false)
TestSort("mno`nFGH`ndef`ncop`ncoop`ncon`nco-op`nabc`n", "`nabc`nco-op`ncon`ncoop`ncop`ndef`nFGH`nmno", "Z")
TestSort("`nabc`nco-op`ncon`ncoop`ncop`ndef`nFGH`nmno", "`nFGH`nabc`nco-op`ncon`ncoop`ncop`ndef`nmno", "C", false)

; Locale-insensitive sorting:
TestSort("def`näbd`nmno`nÄBC`n", "ÄBC`näbd`ndef`nmno`n", "CL", false)
TestSort("def`näbc`nmno`nÄBD`n", "äbc`nÄBD`ndef`nmno`n", "CL", false)
TestSort("def`nabc`nmno`nFGH`nco-op`ncoop`ncop`ncon", "abc`ncon`ncoop`nco-op`ncop`ndef`nFGH`nmno", "CL", false) ; It should use MS's word sort.
TestSort("daf`nbcd`nmao`nFGH`n", "FGH`ndaf`nmao`nbcd`n", "CP2", false)

; TEST THE SPECIAL `r`n workaround (CASE #1, last item is moved by the sort):
TestSort("daf`r`nbcd`r`nmao`r`nFGH", "bcd`r`ndaf`r`nFGH`r`nmao")

; TEST THE SPECIAL `r`n workaround (CASE #2, last item is *not* moved by the sort):
TestSort("daf`r`nbcd`r`nmao`r`nZZZ", "bcd`r`ndaf`r`nmao`r`nZZZ")

; TEST THE SPECIAL `r`n workaround (CASE #3, last item is terminated by a delimiter):
TestSort("daf`r`nbcd`r`nmao`r`nFGH`r`n", "bcd`r`ndaf`r`nFGH`r`nmao`r`n")
TestSort("daf5`nbcd`nmao1`nFGH`n", "bcd`nFGH`nmao1`ndaf5`n", "CP4", false)

; NUMERIC SORTING:
; Sort sees them as strings, but UDF sees them as *numeric* strings.  Hence don't test UDF:
TestSort("5,3,7,9,1,13,999,-4", "-4,1,3,5,7,9,13,999", "D,N", false)
TestSort("-4,1,3,5,7,9,13,999", "999,13,9,7,5,3,1,-4", "D,NR", false)
TestSort("999,13,9,7,5,3,1,-4", "-4,1,13,3,5,7,9,999", "D,", false)
TestSort("999`r`n13`r`n9`r`n7`r`n5`r`n3`r`n1`r`n-4", "-4`r`n1`r`n13`r`n3`r`n5`r`n7`r`n9`r`n999", "", false) ; Same as previous but uses CRLF.

TestSortNumUDF("5,3,7,9,1,13,999,-4", "-4,1,3,5,7,9,13,999", "D,")
TestSortOffset("0,5,15,-5,-3,555,3", "3,555,-3,-5,15,5,0", "D,")

; TEST DUPE-REMOVAL
TestSort("fgh`r`ndef`r`nabc`r`nmno`r`nfgh`r`nabc`r`n", "abc`r`ndef`r`nfgh`r`nmno`r`n", "U")
TestSort("1.0`r`n2`r`n1`r`n2.0`r`n", "1`r`n1.0`r`n2`r`n2.0`r`n", "U")

Var = 1.0`r`n2`r`n1`r`n2.0`r`n
Sort, Var, NU
; Because it's numerically sorted, the order of 2 with respect to 2.0 is not guaranteed, thus it is unknown which
; of the two will be eliminated as a dupe:
if Var <> 1`r`n2.0`r`n
	msgbox **MAYBE** a sort problem (see comment in code) "%Var%"
ChkLen(Var)

; TEST DYNAMIC VARIABLE
Var = 5,3,7,9,1,13,999,-4
VarName = Va  ; 'r' omitted but put back in next line.
Sort, %VarName%r, D,N
if %VarName%r <> -4,1,3,5,7,9,13,999
	msgbox % "sort problem: """ . %VarName%r . """"
ChkLen(%VarName%r)


; TEST SOME BUILT-IN VARIABLES:

Loop %A_ScriptFullPath%
{
	if (A_LoopFileTimeModified = "" || A_LoopFileTimeCreated = ""  || A_LoopFileTimeAccessed = "" )
		MsgBox Prob.
	if A_LoopFileTimeModified is not date
		MsgBox %A_LoopFileTimeModified%
	if A_LoopFileTimeCreated is not date
		MsgBox %A_LoopFileTimeCreated%
	if A_LoopFileTimeAccessed is not date
		MsgBox %A_LoopFileTimeAccessed%
}

if (A_IpAddress1 == A_IpAddress2)
	MsgBox A_IpAddress1 == A_IpAddress2
if (A_YDay < 1 || A_YDay > 366)
	MsgBox %A_YDay%
if strlen(A_YWeek) <> 6
	MsgBox %A_YWeek%
if (A_MDay <> A_DD || A_DD < 1 || A_DD > 31)
	MsgBox %A_DD%
if (A_YYYY < 2007 || A_YYYY > 2017 
	|| A_Year < 2007 || A_Year > 2017)
	MsgBox %A_YYYY%`n%A_Year%
if (A_Mon <> A_MM)
	MsgBox %A_MM%
if (A_WDay < 1 || A_WDay > 7)
	MsgBox %A_WDay%
if (A_MSec < "000" || A_MSec > "999" || A_MSec < 0 || A_MSec > 999)
	MsgBox %A_MSec%
Var := A_MSec
Sleep 7
Var := A_MSec - Var
if var not between 3 and 20
	Gosub Error
if a_now <> %a_year%%a_mon%%a_mday%%a_hour%%a_min%%a_sec%
	msgbox, unexpected datetime result for %a_now%


; SCIENTIFIC NOTATION

SetFormat, Float, 0.6e
if !(A_FormatFloat == "0.6e")
	MsgBox "%A_FormatFloat%"
Var := 3.0+5
if !(Var == "8.000000e+000")
	Gosub Error
Var := 0.15e+1+0  ; Bug fixed in v1.0.46.12
if !(Var == "1.500000e+000")
	Gosub Error
Var := 1/10
if !(Var == "1.000000e-001")
	Gosub Error
Var := 10//10
if !(Var == "1")
	Gosub Error
Var := 2.2e-3 + 3
if !(Var == "3.002200e+000")
	Gosub Error
Var := -1.0E+001 * 3
Var := -Var
if !(Var == "3.000000e+001")
	Gosub Error
Var := 2.2e+3 + 3
if !(Var == "2.203000e+003")
	Gosub Error
Var := .2e+3 + 3  ; Not really a proper literal (not normalized?), but it works anyway.
if !(Var == "2.030000e+002")
	Gosub Error
1e4 := 123  ; Left value is a variable not a sci-notation literal. (bk-compat)
++1e4
if (1e4 <> 124)  ; Ensure 1e4 is seen as a variable, not a sci-notation numeric literal.
	MsgBox "%1e4%"
1e = 6
if (1e-4 <> 2)  ; Decimal point required to avoid ambiguity.
	MsgBox "%1e%"
if (1e+4 <> 10)  ; Decimal point required to avoid ambiguity.
	MsgBox "%1e%"
e := 2.718281828459045       ; BEGIN LASZLO'S EXAMPLE
x := e*10**-7 
Var := x*10**7
if !(Var == "2.718282e+000")
	Gosub Error
var := (e*10**-7)*10**7
if !(Var == "2.718282e+000")
	Gosub Error              ; END LASLZO'S EXAMPLE.
Setformat, Float, 0.6  ; REVERT TO DEFAULT FOR SUBSEQUENT TESTS.
if (A_FormatFloat <> "0.6")
	MsgBox "%A_FormatFloat%"

test =
m_array0 =
StringSplit, m_array, test
if m_array0 <> 0
	MsgBox, Prob0

test = ab 123`tde  ; i.e. seven non-whitespace chars
m_array0 =
StringSplit, m_array, test, , %a_space%%a_tab%
if m_array0 <> 7
	MsgBox, Expected 7 but got %m_array0%.

test = 1a2b3c
StringSplit, m_array, test
if m_array0 <> 6
	MsgBox, Single-char stringsplit didn't produce the expected 6 items.
if m_array1 <> 1
	MsgBox, Prob1
if m_array2 <> a
	MsgBox, Prob2
if m_array3 <> 2
	MsgBox, Prob3
if m_array4 <> b
	MsgBox, Prob4
if m_array5 <> 3
	MsgBox, Prob5
if m_array6 <> c
	MsgBox, Prob6


test = %a_space%red , green`t `t, `t `tblue
m_array0 =
StringSplit, m_array, test, `,, %a_space%%a_tab%
if m_array0 <> 3
	MsgBox, Expected 3 but got %m_array0%.
if m_array1 <> red
	Msgbox, Expected red.
if m_array2 <> green
	Msgbox, Expected green.
if m_array3 <> blue
	Msgbox, Expected blue.


test = string1`nitem2,string3`nitem4,5555, 606, 707, h8a,, `n,`n,14abc
stringsplit, m_array, test, `n`,
if m_array0 <> 14
	msgbox, StringSplit produced %m_array0% when 14 were expected.

if m_array1 <> string1
	msgbox, stringsplit problem #1
if m_array2 <> item2
	msgbox, stringsplit problem #2
if m_array3 <> string3
	msgbox, stringsplit problem #3
if m_array4 <> item4
	msgbox, stringsplit problem #4
if m_array5 <> 5555
	msgbox, stringsplit problem #5
if m_array6 <> 606
	msgbox, stringsplit problem #6
if m_array7 <> 707
	msgbox, stringsplit problem #7
if m_array8 <> %a_space%h8a
	msgbox, stringsplit problem #8
if m_array9 <>
	msgbox, stringsplit problem #9
if m_array10 <> %a_space%
	msgbox, stringsplit problem #10
if m_array11 <>
	msgbox, stringsplit problem #11
if m_array12 <>
	msgbox, stringsplit problem #12
if m_array13 <>
	msgbox, stringsplit problem #13
if m_array14 <> 14abc
	msgbox, stringsplit problem #14

global_vs_local0 = x  ; Necessary to trigger the bug.
TestSplitBug()
TestSplitBug()
{
	Var = ab
	StringSplit, global_vs_local, Var
	if (global_vs_local1 <> "a" || global_vs_local2 <> "b")  ; Must not refer to global_vs_local0 here because that would "hide" the bug.
		MsgBox StringSplit problem 1.
	global split2_0, split2_1, split2_2
	StringSplit, split2_, Var
	if (split2_1 <> "a" || split2_2 <> "b")
		MsgBox StringSplit problem 2.
}



; TEST THE NEW SUBSTR() FUNCTION.

Str := "0123456789 ABCDEFG 0123456789"

Var := substr(Str, 12)
if (Var <> "ABCDEFG 0123456789" || substr(Str, 12) <> "ABCDEFG 0123456789")
	Gosub Error

Var := substr(Str, 99)
if (Var <> "" || substr(Str, 99) <> "")
	Gosub Error

Var := substr(Str, 0)
if (Var <> "9" || substr(Str, 0) <> "9")
	Gosub Error

Var := substr(Str, -StrLen(Str) + 2)  ; Extract from right, all but the first character.
if (Var <> "123456789 ABCDEFG 0123456789")
	Gosub Error

Var := substr(Str, -StrLen(Str) + 1)  ; Extract from right, all chars.
if (Var <> Str)
	Gosub Error

Var := substr(Str, -StrLen(Str) - 55)  ; Extract from right, too many chars.
if (Var <> Str)
	Gosub Error

Var := substr(Str, 2, 0)  ; Extract 0 chars.
if (Var <> "")
	Gosub Error

Var := substr(Str, 2, 1) ; Extract only the second char.
if (Var <> "1")
	Gosub Error

Var := substr(Str, 1, StrLen(Str)) ; Extract all chars.
if (Var <> Str)
	Gosub Error

Var := substr(Str, 1, StrLen(Str) + 55) ; Extract all but with length too large.
if (Var <> Str)
	Gosub Error

Var := substr(Str, 1, StrLen(Str) - 1) ; Extract all chars except last.
if (Var <> "0123456789 ABCDEFG 012345678")
	Gosub Error

Var := substr(Str, 1, -1) ; Omit 1 char from end via negative length.
if (Var <> "0123456789 ABCDEFG 012345678")
	Gosub Error

Var := substr(Str, 2, -1) ; Same but with a non-default offset.
if (Var <> "123456789 ABCDEFG 012345678")
	Gosub Error

Var := substr(Str, 1, -StrLen(Str)) ; Omit exactly all characters.
if (Var <> "")
	Gosub Error

Var := substr(Str, 1, -StrLen(Str) + 1) ; Omit all characters except one.
if (Var <> "0")
	Gosub Error

Var := substr(Str, 1, -StrLen(Str) - 55) ; Omit too many characters.
if (Var <> "")
	Gosub Error

Str := substr(Str, 1)  ; Result assigned to itself (to ensure no crashes or misbehavior).
if (Str <> "0123456789 ABCDEFG 0123456789")
	Gosub Error

Str := substr(Str, 9)  ; Similar.
if (Str <> "89 ABCDEFG 0123456789")
	Gosub Error

Str := substr(Str, 2, 5)  ; Similar.
if (Str <> "9 ABC")
	Gosub Error

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Create basic files & folders for testing:
TargetDir = C:\A-Source\AutoHotkey\Test\Test Folder\This folder used by MAIN battery
FileCreateDir %TargetDir%
IfNotExist %TargetDir%\File1
	FileAppend, Test, %TargetDir%\File1
IfNotExist %TargetDir%\File2
	FileAppend, Test, %TargetDir%\File2
FileCreateDir %TargetDir%\Sub1
IfNotExist %TargetDir%\Sub1\File1
	FileAppend, Test, %TargetDir%\Sub1\File1
IfNotExist %TargetDir%\Sub2\File2
	FileAppend, Test, %TargetDir%\Sub1\File2

; Brief test of file-copy:
FileCreateDir %TargetDir%\DestOfCopy
FileCopy %TargetDir%\*.*, %TargetDir%\DestOfCopy\*.*  ; No overwrite because they're deleted/cleaned up later below.
if ErrorLevel
	MsgBox %ErrorLevel% files couldn't be copied; perhaps they already exist.
FileSetAttrib +R, %TargetDir%\DestOfCopy\*.*
FileDelete %TargetDir%\DestOfCopy\*.*
if ErrorLevel <> 2
	MsgBox %ErrorLevel% files couldn't be deleted but it was expected that some couldn't be deleted due to read-only.
FileSetAttrib -R, %TargetDir%\DestOfCopy\*.*
FileDelete %TargetDir%\DestOfCopy\*.*
if ErrorLevel
	MsgBox %ErrorLevel% files couldn't be deleted.

; Brief test of FileSetAttrib/Time:
FileSetAttrib +H, %TargetDir%
FileGetAttrib, Attrib, %TargetDir%
if not instr(Attrib, "H")
	MsgBox hidden attrib was expected.
FileSetAttrib ^H, %TargetDir%
FileGetAttrib, Attrib, %TargetDir%
if instr(Attrib, "H")
	MsgBox hidden attrib was NOT expected.

; Batch change of attrib & time:
FileSetAttrib +H, %TargetDir%\*.*, 1, 1
if ErrorLevel
	MsgBox %ErrorLevel% files/folders couldn't be made hidden.

FileSetTime,, %TargetDir%\*.*,, 1, 1  ; Set modification time to current time.
if ErrorLevel
	MsgBox %ErrorLevel% files/folders couldn't be timestamped.

; Verify what was done above:
Loop %TargetDir%\*.*, 1, 1
{
	FileGetAttrib, Attrib, %A_LoopFileFullPath%
	if not instr(Attrib, "H")
		MsgBox Hidden attrib was expected on %A_LoopFileFullPath%.
	FileGetTime, FileTime, %A_LoopFileFullPath%
	TimeDelta =
	TimeDelta -= %FileTime%, seconds
	if abs(TimeDelta) > 10
		MsgBox Modification time is different than the current time on %A_LoopFileFullPath%.
	FileCount := A_Index
}
if FileCount <> 6
	MsgBox FileCount (%FileCount%) <> 6
FileSetAttrib -H, %TargetDir%\*.*, 1, 1

; BASIC REGEX TEST (there are much more advanced ones in the RegEx test suite).
if RegExMatch("xxxabc123xyz", "abc.*xyz") <> 4  ; Returns 4, which is the position where the match was found.
	MsgBox RegExMatch() problem.
if RegExMatch("abc123123", "123$") <> 7  ; Returns 7 because the $ requires the match to be at the end.
	MsgBox RegExMatch() problem.
if RegExMatch("abc123", "i)^ABC") <> 1  ; Returns 1 because a match was achieved via the case-insensitive option.
	MsgBox RegExMatch() problem.
if RegExMatch("abcXYZ123", "abc(.*)123", SubPat) <> 1  ; Returns 1 and stores "XYZ" in SubPat1.
	MsgBox RegExMatch() problem.
if (SubPat1 <> "XYZ")
	MsgBox RegExMatch() problem.
if RegExMatch("abc123abc456", "abc\d+", "", 2) <> 7  ; Returns 7 instead of 1 due to StartingPos 2 vs. 1.
	MsgBox RegExMatch() problem.

if RegExReplace("abc123123", "123$", "xyz") <> "abc123xyz"  ; Returns "abc123xyz" because the $ allows a match only at the end.
	MsgBox RegExReplace() problem.
if RegExReplace("abc123", "i)^ABC") <> "123"  ; Returns "123" because a match was achieved via the case-insensitive option.
	MsgBox RegExReplace() problem.
if RegExReplace("abcXYZ123", "abc(.*)123", "aaa$1zzz") <> "aaaXYZzzz" ; Returns "aaaXYZzzz" by means of the $1 backreference.
	MsgBox RegExReplace() problem.
if RegExReplace("abc123abc456", "abc\d+", "", ReplacementCount) <> ""  ; Returns "" and stores 2 in ReplacementCount.
	MsgBox RegExReplace() problem.
if ReplacementCount <> 2
	MsgBox RegExReplace() problem.

var = xYz
StringUpper, var, var
if (var <> "XYZ")
	msgbox StringUpper
StringLower, var, var
if (var <> "xyz")
	msgbox StringLower
StringUpper, clipboard, var  ; Had been a bug in 1.0.45.
if (clipboard <> "XYZ")
	msgbox StringLower

; Test of things that should be autotrim'd and things that shouldn't:
x := "  thing  "
clipboard := x
y := x
if (clipboard <> "  thing  " || y <> "  thing  ")
	msgbox AutoTrim shouldn't affect := operator.
clipboard = %x%
y = %x%
if (clipboard <> "thing" || y <> "thing")
	msgbox autotrim not working or is off?
clipboard = %clipboard%xxx
if (clipboard <> "thingxxx")
	msgbox Append to clipboard failed.
Clipboard .= "yyy"
if (clipboard <> "thingxxxyyy")
	msgbox Append to clipboard failed.


; --------------------------------------------------
; LARGE TEST OF NESTED LOOPS AND A_LOOPXXX VARIABLES
; --------------------------------------------------
loop_fn()
{
	r := A_Index  ; Uses/accesses the caller's loop.
	Loop
		if a_index = 4
			return r
}

Loop 1
	var%A_Index% := "xyz" . loop_fn()
if not instr(var1, "xyz")
	Msgbox pre-v1.0.44.14 bug.

Var =
Loop 2
	Var := Var . loop_fn() . loop_fn()
if (Var <> "1122")  ; Buggy versions prior to v1.0.44.14 yielded "1424".
	MsgBox "%Var%"


; TEST FILE-PATTERN LOOP AND OTHER LOOPS:

colors = Red|Green|Blue

FileCreateDir LoopFolder\A\B
FileAppend, 1, LoopFolder\1
FileAppend, 2, LoopFolder\A\2
FileAppend, 3, LoopFolder\A\3
FileAppend, 4, LoopFolder\A\B\4

a = 0
str3 =
Loop, LoopFolder\*.*, 1, 1
{
++a

i = 0
Loop 10
{
	++i

	j = 0
	Loop 5
	{
		++j
		if (A_Index <> j)
			MsgBox LoopJ %j%
	}
	if (j <> 5)
		MsgBox j <> 5
	if (A_Index <> i)
		MsgBox LoopI %i%

	j = 0
	Loop, parse, colors, |
	{
		if i = 1
			str3 = %str3%%A_LoopFileName%  ; Inner loop accessing out loop's values.
		++j
		if (A_Index <> j)
			MsgBox LoopJ %j%
		if (j = 1 and A_LoopField <> "Red")
			MsgBox Red
		if (j = 2 and A_LoopField <> "Green")
			MsgBox Green
		if (j = 3 and A_LoopField <> "Blue")
		{
			MsgBox Blue
			Loop 1
				if (A_Index <> 1 or A_LoopField <> "Blue")
					MsgBox inner accessing outer's.
		}
	}
	if (j <> 3)
		MsgBox j <> 3
	if (A_Index <> i)
		MsgBox LoopI %i%

	j = 0
	str =
	Loop, LoopFolder\*.*, 1, 1
	{
		++j
		k = 0
		str2 =
		Loop, LoopFolder\*.*, 1, 1
		{
			str2 = %str2%%A_LoopFileName%
			++k
			if (A_Index <> k)
				MsgBox LoopK %k%
		}
		if (str2 <> "1A23B4")
			MsgBox "%str2%"`nvs.`n"1A23B4"
		if (k <> 6)
			MsgBox k <> 6
		str = %str%%A_LoopFileName%
		if (A_Index <> j)
			MsgBox LoopJ %j%
	}
	if (str <> "1A23B4")
		MsgBox "%str%"`nvs.`n"1A23B4"
	if (j <> 6)
		MsgBox j <> 6
	if (A_Index <> i)
		MsgBox LoopI %i%
}
if (i <> 10)
	Msgbox i <> 10

; Extra outer loop:
if (A_Index <> a)
	MsgBox LoopA %a%
if (A_LoopFileName = "3")
	Loop, Read, %A_LoopFileFullPath%, LoopWriteTest.txt
		FileAppend, Extra`n
}
if (a <> 6)
		MsgBox a <> 6
if (str3 <> "111AAA222333BBB444")
	msgbox Problem
FileRead, fr, LoopWriteTest.txt
IfNotInString, fr, Extra`r`n
	MsgBox IfNotInString fr
FileDelete LoopWriteTest.txt

FileRemoveDir, LoopFolder, true


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


DllCall("SetLastError", uint, 55)
if ErrorLevel
	MsgBox Problem
if A_LastError <> 55
	MsgBox A_LastError
DllCall("SetCurrentDirectoryA", str, "C:\nonexistant folder")
if A_LastError <> 2  ; "file not found"
	MsgBox A_LastError

Loop %A_ProgramFiles%\*.*, 1, 1
{
	IfNotInString, A_LoopFileDir, Program Files  ; Just a basic sanity test.
		msgbox problem
	break
}

; Test stuff that was fixed in v1.0.42:
If not InstallIsStopped {
}
else
	msgbox problem 0

If not BetweenSomething {
}
else
	msgbox problem 0

If not ContainsSomething {
}
else
	msgbox problem 0

var = 3
if var is not number
{
	msgbox problem not num
}

NotInstalled = x
If NotInstalled in y,x
{
}
else
	msgbox problem 1

NotInstalled = x
If NotInstalled not in y,x
	msgbox problem 2

NotInstalled = z
If NotInstalled in y,x
	msgbox problem 3

;---------------------

; originally/already tested ok:
Loop {filename
{
	msgbox File was found.
}

; originally/already tested ok:
Loop filename{   ; Unintentionally supported because it's not worth adding code to "unsupport".
	msgbox File "filename" was found.
}
Loop filename {   ; Same
	msgbox File "filename" was found.
}

xyz = 1
loop % xyz + 1 {
	xyz = %A_Index%
}
if xyz <> 2
	msgbox %xyz%

xyz = 0
Loop{
	xyz := a_index
	break
}
if xyz <> 1
	msgbox %xyz%

xyz = 0
Loop {
	xyz := a_index
	break
}
if xyz <> 1
	msgbox %xyz%

xyz = 0
Loop 5 {
	++xyz
}
if xyz <> 5
	msgbox %xyz%

xyz = 0
Loop 5{
	++xyz
}
if xyz <> 5
	msgbox %xyz%

xyz = 0
Loop, 5{
	++xyz
}
if xyz <> 5
	msgbox %xyz%

var = 5
xyz = 0
Loop, %var%{
	++xyz
}
if xyz <> 5
	msgbox %xyz%

var = 5
xyz = 0
Loop, %var% {
	++xyz
}
if xyz <> 5
	msgbox %xyz%

xyz = 0
Loop {
	++xyz
	if xyz = 5
		break
}
if xyz <> 5
	msgbox %xyz%

Loop C:\*.* {

}

;;;;;;;;;;;;;;;;;;;;;;;;;;

var := true
if var {    
	xyz = if var
  } else {      
	xyz = bad
}
if xyz <> if var
	msgbox %xyz%

var := true
if var{
	xyz = if var{
}else{
	xyz = bad
}
if xyz <> if var{
	msgbox %xyz%

var := true
if !var {
	xyz = bad
}else{
	xyz = if !var
}
if xyz <> if !var
	msgbox %xyz%

var := false
if not var{
	xyz = if not var
} else if (1 = 0) {
	xyz = bad
} else {
	xyz = bad
}
if xyz <> if not var
	msgbox %xyz%

var := false
if var{
	xyz = bad
} else if (1 < 2) {
	xyz = else if
} else {
	xyz = bad
}
if xyz <> else if
	msgbox %xyz%

var := true
if (var = false){
	xyz = bad
} else {
	xyz = varfalse
}
if xyz <> varfalse
	msgbox %xyz%

if ReturnTrue() {
	xyz = returntrue
} else {
	xyz = bad
}
if xyz <> returntrue
	msgbox %xyz%

if (5 > 0) and (3 > 0) {
	xyz = expr-no-paren
} else {
	xyz = bad
}
if xyz <> expr-no-paren
	msgbox %xyz%

if (5 > 15) or (3 > 15) {
	xyz = bad
} else {
	xyz = expr-no-paren2
}
if xyz <> expr-no-paren2
	msgbox %xyz%



;;;;;;;;;;;;;;;;;;;;;;;;;

if!1
	msgbox problem

if (1)
{
	sleep -1
} xyz = 1
if xyz != 1
	msgbox problem

if (1)
{
	xyz = (1)
}else{
	msgbox problem
}
if xyz <> (1)
	msgbox problem

if (0)
{
	msgbox problem
} else {
xyz = } else {
}
if xyz <> } else {
	msgbox problem


;;;;;;;;;;;;;;;;;;;;;

ReturnTrue() {
	return true
}

ReturnTrue2(){  ; No space
	return true
}




if (GetKeyState("bogus") <> "")
	MsgBox GetKeyState should yield blank for invalid key name.
if (GetKeyState("A") <> 0)
	MsgBox GetKeyState says 'A' is down.
if (GetKeyState("Capslock") <> 0)
	MsgBox GetKeyState says Capslock is down.
if (GetKeyState("Capslock", "T") <> 0)
	MsgBox GetKeyState says Capslock is ON.
;if (GetKeyState("Numlock", "T") <> 1)
;	MsgBox GetKeyState says Numlock is OFF.

ifnotexist, c:\autoexec.bat
	msgbox, c:\autoexec.bat (possibly a hidden file) doesn't exist even as a hidden file.
ifnotexist, c:\*.*
	msgbox, drive C is empty?!
ifexist, %ProgramFiles%xxx
	msgbox, something exists that shouldn't
if FileExist(ProgramFiles . "xxx")
	msgbox, something exists that shouldn't
if (FileExist(ProgramFiles . "\bogus.*") <> "")
	msgbox, something exists that shouldn't
if (FileExist(ProgramFiles . "\*.*") = "")
	msgbox, Program Files is empty?
if !InStr(FileExist(ProgramFiles), "D")
	msgbox, %ProgramFiles% doesn't exist?!

var := "0123456789"
if StrLen(var) <> 10
	MsgBox %Var%

winget, list, list
ChkLen(list)
winget, count, count
if (count <> list || count < 2)
	msgbox winget count discrep.
winget, list, list,,, Program Manager
if (list <> count - 1)
	MsgBox More than one program manager?
wingetclass, class, ahk_id %list1%
if Class <> Shell_TrayWnd
	MsgBox First window in z-order isn't Shell_TrayWnd?
ChkLen(class)

; ARRAY/VAR DATA INTEGRITY TEST:
SetBatchLines -1  ; Just to be sure.
Iterations = 110000
Loop, %Iterations%
	zArray%A_Index% := A_Index
Loop, %Iterations%
	if zArray%A_Index% <> %A_Index%
		MsgBox Data corruption: Array item #%A_Index% contains something other than its index.
; Now write a Z array at the end of the list to test a diff. section of the code:
Iterations = 11000
Loop, %Iterations%
	ZZZ%A_Index% := A_Index
Loop, %Iterations%
	if ZZZ%A_Index% <> %A_Index%
		MsgBox Data corruption: Array item #%A_Index% contains something other than its index.

; Test of a concat bug fixed in 1.0.25.05:
test =
test = %test% foo
test = %test% bar
if test <> foo bar
	msgbox %test%
; And doing it again triggered the bug due to test being large enough in capacity for performance boost method:
test =
test = %test% foo
test = %test% bar
if test <> foo bar
	msgbox %test%
ChkLen(test)

FileRead, Contents, %A_ScriptFullPath%
if ErrorLevel <> 0
	MsgBox FileRead problem.
StringLen, RawLength, Contents
;MsgBox, Length %RawLength%
IfNotInString, Contents, `r`n
	MsgBox, CRLF's were expected. (Length %RawLength%)
ChkLen(Contents)

FileRead, Contents, *t %A_ScriptFullPath%
if ErrorLevel <> 0
	MsgBox FileRead problem.
;StringLen, TextLength, Contents
;MsgBox %TextLength%
IfInString, Contents, `r`n
	MsgBox, CRLF's were NOT expected.
ChkLen(Contents)

FormatTime, time, 20040101, YDay
if time <> 1
	MsgBox time problem
FormatTime, time, 20040229, YDay
if time <> 60
	MsgBox time problem
FormatTime, time, 20040229, WDay
if time <> 1  ; It should be Sunday, which is 1.
	MsgBox time problem %WDay%
FormatTime, time, 20050101, YWeek
if time <> 200453
	MsgBox time problem
FormatTime, time, 20050101, YWeek
if time <> 200453
	MsgBox time problem
FormatTime, time, 20050102, YWeek
if time <> 200453
	MsgBox time problem
FormatTime, time, 20050103, YWeek
if time <> 200501
	MsgBox time problem
FormatTime, time, 20050109, YWeek
if time <> 200501
	MsgBox time problem
FormatTime, time, 20050110, YWeek
if time <> 200502
	MsgBox time problem
ChkLen(time)

FormatTime, time, 20050000130801  ; Date portion invalid.
if time <> 1:08 PM%A_Space%  ; Trailing space caused by presence of time prefix but absence of date suffix.
	MsgBox time problem: time == "%time%"
FormatTime, time, 20050000130801, HH:mm  ; Fixed in v1.0.48.04 so that invalid date doesn't stop the time portion from working.
if time <> 13:08
	MsgBox time problem: time == "%time%"
FormatTime, time, 20050115137701  ; Time portion invalid, but date portion ok.
if time <> %A_Space%Saturday, January 15, 2005
	MsgBox time problem: time == "%time%"

var = 19990511
if var is not time
	MsgBox %var% is not a valid date/time?!
var = 19991311
if var is time
	MsgBox %var% IS a valid date/time?!
var = 19990011
if var is time
	MsgBox %var% IS a valid date/time?!

var =
if var contains a,abc,def
	msgbox Problem with "var contains"
if var not contains ,x,y,z
	msgbox Problem with "var not contains"
if var not contains ,
	msgbox Problem with "var not contains"

var = abc
if var contains xx,abcd,def
	msgbox Problem with "var contains"
if var not contains abc
	msgbox Problem with "var not contains"
if var not contains ,
	msgbox Problem with "var not contains"
if var not contains xyz,ab
	msgbox Problem with "var not contains"

Run Notepad.exe, , , NewPID
WinWait Untitled - Notepad
WinGet, pid, PID
if pid <> %NewPID%
	MsgBox problem with pid.
Process, WaitClose, %NewPid%, 1
if ErrorLevel <> %NewPid%
	MsgBox problem with process waitclose 1
WinClose
Process, WaitClose, %NewPid%, 1
if ErrorLevel <> 0
	MsgBox problem with process waitclose 2

/*
Process, close, %NewPID%
Sleep, 500
Run Notepad.exe
Process , wait, Notepad.exe, 5
NewPID = %ErrorLevel%  ; Save the value immediately since ErrorLevel is often changed.
if NewPID = 0
{
	MsgBox The process did not appear within 5 seconds.
	return
}
; Otherwise:
MsgBox The new Process ID is %NewPID%.

Process, priority, %NewPID%, Low
Process, priority, , High  ; Have the script set itself to high priority.
WinClose Untitled - Notepad
Process, WaitClose, %PID%, 5
if ErrorLevel <> 0 ; The PID still exists.
	MsgBox The process did not close within 5 seconds.
*/


lower = 12
upper = 14
var = 11
if var between %lower% and %upper%
	msgbox problem with "between"
if var not between 11 and 14
	msgbox problem with "not between"

var = 1
if var between 0 and 0.9
	msgbox problem with "between"
if var not between 0 and 1.0
	msgbox problem with "not between"

var = .5
if var between 0 and 0
	msgbox problem with "between"
if var not between 0 and 1
	msgbox problem with "not between"

var = cb
if var between band and cat
	msgbox problem with "between"
if var not between band and cb
	msgbox problem with "not between"

var = C
if var between cat and cat
	msgbox problem with "between"
if var not between band and cat
	msgbox problem with "not between"

StringCaseSense, on
if var between band and cat
	msgbox problem with "not between"

x = abc
y = ABC
if x = %y%  ; This is tested in case some optimization tries to turn "if x=%y%" into "if (x=y)", which has different behavior with respect to StringCaseSense.
	MsgBox StringCaseSense problem.
if not (x = y)  ; This uses locale mode when StringCaseSense is on.
	MsgBox StringCaseSense problem.
x = Ä
y = ä
if x = %y%
	MsgBox StringCaseSense problem.
if not (x = y)  ; This uses locale mode when StringCaseSense is on.
	MsgBox StringCaseSense problem.
if (x == y)
	MsgBox problem with == operator.

StringCaseSense, off

; Locale stuff:
var = COOP
if var between con and co-op
	msgbox problem with string search vs. word-search (MSDN terminology) 1a
if (var >= "con" and var <= "co-op")
	msgbox problem with string search vs. word-search (MSDN terminology) 2a
if ("Ä" = "ä" || "Ä" == "ä" || !("Ä" != "ä")) ; Verified correct due to lack of locale-awareness.
	MsgBox Ää problem ; Normal case sensitive mode should recognize only A-Z as case-varying/capable.
var = Ä
IfEqual, Var, ä
	MsgBox Ää problem (since non-locale-awareness shouldn't see them as identical).
StringCaseSense On
if (!("Ä" = "ä") || "Ä" == "ä" || !("Ä" != "ä"))
	MsgBox Ää problem
IfEqual, Var, ä
	MsgBox Ää problem.
StringCaseSense Locale
if (!("Ä" = "ä") || "Ä" == "ä" || "Ä" != "ä")
	MsgBox Ää problem
IfNotEqual, Var, ä
	MsgBox Ää problem.
var = COOP
if not (var >= "con" and var <= "co-op")
	msgbox problem with string search vs. word-search (MSDN terminology) 1b
if var not between con and co-op
	msgbox problem with string search vs. word-search (MSDN terminology) 2b
StringCaseSense, off

var =
if var between band and cat
	msgbox problem with "between"
if var not between  and cb
	msgbox problem with "not between"
if var not between  and
	msgbox problem with "not between"

var = 0  ; Digits are alphabetically less than letters.
if var between band and cat
	msgbox problem with "between"
if var not between  and cb
	msgbox problem with "not between"
if var between and
	msgbox problem with "not between"

var = 0  ; Digits are alphabetically less than letters.
if var between andy and david
	msgbox problem with "between"
if var not between  and andy
	msgbox problem with "not between"
if var between and
	msgbox problem with "not between"


var =
if var in x,y,z
	msgbox Problem with "var in"
if var not in ,x,y,z
	msgbox Problem with "var not in"
if var not in ,
	msgbox Problem with "var not in"

var = ,
if var in x,,y
	msgbox Problem with "var in"
if var in ,  ; i.e. the list contains only the empty string
	msgbox Problem with "var in"
if var not in ,,
	msgbox Problem with "var not in"

var = ,
if var not in ,,,
	msgbox Problem with "var not in"
if var not in ,,
	msgbox Problem with "var not in"
if var in ,,,,
	msgbox Problem with "var not in"

var = red
if var not in red,green,blue
	msgbox Problem with "var not in"
if var in green,blue,redx,yyy
	msgbox Problem with "var in"

var = red,,
if var not in xxx,yyy,,zzz,,,pxx,,,red,,,,,green,blue
	msgbox Problem with "var not in"
if var in green,blue,redx,,,,,yyy
	msgbox Problem with "var in"

var = red,with,green
if var not in green,red,,with,,green,blue
	msgbox Problem with "var not in"
if var in green,blue,red,with,green,blue
	msgbox Problem with "var in"

StringCaseSense on
var = GrEEn
if var not in red,,,,,GrEEn,blue
	msgbox Problem with "var not in"
if var in green,blue,redx,,,,,yyy
	msgbox Problem with "var in"
StringCaseSense off

StringCaseSense Locale
var = GrEEnÄ
if var not in red,,,,,greenä,blue
	msgbox Problem with "var not in"
if var not contains red,ä,blue
	msgbox Problem with "var not in"
if var contains äbc
	msgbox Problem with "var not in"
StringCaseSense off

envset, var3, xyz
test = BADIFCOPIED
var2 = abc
var1 = yyy`%var2`%xxx`%var2`%yyy`%var3`%`%test`%
Transform, test, deref, %var1%
if test <> yyyabcxxxabcyyyxyz
	msgbox Problem with Transform, test, deref, var1
EnvGet, var, bogus_variable
if var <>
	msgbox "%var%"
EnvSet, with spaces, thing
if ErrorLevel
	msgbox Problem with EnvSet.
else
{
	EnvGet, Var, with spaces
	if Var <> thing
		MsgBox %Var%
}

filespec = C:\My Documents.abc\Thing.txt
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> Thing.txt
	MsgBox problem
if dir <> C:\My Documents.abc
	MsgBox problem
if ext <> txt
	MsgBox problem
if name_no_ext <> Thing
	MsgBox problem
if drive <> C:
	MsgBox problem

filespec = C:\My Documents\.txt
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> .txt
	MsgBox problem
if dir <> C:\My Documents
	MsgBox problem
if ext <> txt
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <> C:
	MsgBox problem
ChkLen(name)
ChkLen(dir)
ChkLen(ext)
ChkLen(name_no_ext)
ChkLen(drive)

filespec = .txt
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> .txt
	MsgBox problem
if dir <>
	MsgBox problem "%path%"
if ext <> txt
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <>
	MsgBox problem

filespec = C:\My Documents\.
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> .  ; Arguable, since . is illegal as a filename.  But it is a legal dir name, and dir names are sortof supported.
	MsgBox problem
if dir <> C:\My Documents
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <> C:
	MsgBox problem

filespec = .
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> .  ; See above comment
	MsgBox problem
if dir <>
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <>
	MsgBox problem

filespec = 
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <>
	MsgBox problem
if dir <>
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <>
	MsgBox problem

filespec = :
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <>
	MsgBox problem
if dir <> :
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <>
	MsgBox problem

filespec = C:\
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <>
	MsgBox problem
if dir <> C:
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <> C:
	MsgBox problem

filespec = \
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <>
	MsgBox problem
if dir <>
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <>
	MsgBox problem

filespec = \xxxxx\
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <>
	MsgBox problem
if dir <> \xxxxx
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <>
	MsgBox problem
if drive <>
	MsgBox problem

filespec = C:\My Documents\This Test.txt
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> This Test.txt
	MsgBox problem
if dir <> C:\My Documents
	MsgBox problem
if ext <> txt
	MsgBox problem
if name_no_ext <> This Test
	MsgBox problem
if drive <> C:
	MsgBox problem

filespec = C:\My Documents\This Test
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> This Test
	MsgBox problem
if dir <> C:\My Documents
	MsgBox problem
if ext <>
	MsgBox problem
if name_no_ext <> This Test
	MsgBox problem
if drive <> C:
	MsgBox problem

filespec = My Documents\This Test.abc
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> This Test.abc
	MsgBox problem
if dir <> My Documents
	MsgBox problem
if ext <> abc
	MsgBox problem
if name_no_ext <> This Test
	MsgBox problem
if drive <>
	MsgBox problem

filespec = \\Server01\My Documents\This Test.txt
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> This Test.txt
	MsgBox problem
if dir <> \\Server01\My Documents
	MsgBox problem
if ext <> txt
	MsgBox problem
if name_no_ext <> This Test
	MsgBox problem
if drive <> \\Server01
	MsgBox problem

filespec = C:My Documents\This Test.txt
SplitPath, filespec, name, dir, ext, name_no_ext, drive
if name <> This Test.txt
	MsgBox problem
if dir <> C:My Documents
	MsgBox problem
if ext <> txt
	MsgBox problem
if name_no_ext <> This Test
	MsgBox problem
if drive <> C:
	MsgBox problem

;---------------------------------------------------


; First make sure math works, since most other things depend on at least that:
math = 10
math -= 10
if math <> 0
{
	msgbox, math problem: 10 - 10 = %math%
	exit
}
math = 10
math += 10
if math <> 20
{
	msgbox, math problem: 10 + 10 = %math%
	exit
}
math *= -5
if math <> -100
{
	msgbox, math problem: 20 * -5 = %math%
	exit
}
math /= -2
if math <> 50
{
	msgbox, math problem: -100 * -2 = %math%
	exit
}


; Test basic variable operation:
expandby = 0123456789
StringLen, expandby_length, expandby
if (expandby_length <> 10 or StrLen(expandby) <> 10)  ; Tests the StrLen() function.
	MsgBox, expandby_length is not what was expected
begin = <begin>
end = <end>
stringlen, begin_len, begin
stringlen, end_len, end
loop, 10
{
	expand = <begin>%expand%%expandby%<end>
	expand_length += %expandby_length%
	expand_length += %begin_len%
	expand_length += %end_len%
}
StringLen, expand_length_new, expand
if expand_length <> %expand_length_new%
	MsgBox, Calculated length %expand_length% <> actual length %expand_length_new%

; Test empty var handling:
empty = <%empty%>
if empty <> <>
	msgbox, empty var problem


; Test StringLen and dyanmic vars:
InputVar = x
x = 1234
StringLen, OutputVar, %InputVar%
if OutputVar <> 4
	MsgBox Length <> 4

; Test number and string comparisons:
c1 = 5
c2 = 10
Gosub, IndicateCompare
comp_expected = <><<=
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%
c1 = -15
c2 = -15
Gosub, IndicateCompare
comp_expected = =<=>=
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%
c1 = -20
c2 = -15
Gosub, IndicateCompare
comp_expected = <><<=
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%
c1 = -20
c2 = 
Gosub, IndicateCompare
comp_expected = <>>>=  ; Since they will be compared as strings, and since anything is greater than blank
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%
c1 = -20
c2 = 10
Gosub, IndicateCompare
comp_expected = <><<=
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%
c1 = 10
c2 = 
Gosub, IndicateCompare
comp_expected = <>>>=
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%
c1 = 10
c2 = -10
Gosub, IndicateCompare
comp_expected = <>>>=
if comp <> %comp_expected%
	msgbox, comp (%comp%) <> comp_expected (%comp_expected%) for:`n%c1% and %c2%

if nothing = 0
	msgbox, blank = 0 (not supposed to be)
c1 = 0
if c1 != 0
	msgbox, 0 <> 0???
if c1 =
	msgbox, 0 = blank (not supposed to be)
;if x <> a_OStype
;	msgbox, %a_OStype%`n%a_OSversion%
;if a_min < 31
;	msgbox, %a_min%

min = %a_min%
ifnotinstring, a_min, %min%
	msgbox, problem with a_min
ifnotinstring, min, %a_min%
	msgbox, problem with a_min


; Test the string commands:
extract = 5
test = haystack string

stringleft, new, test, %extract%
if new <> hayst
	msgbox, stringleft problem 1
ChkLen(new)
stringleft, new, test, 555
if new <> %test%
	msgbox, stringleft problem 2
ChkLen(new)

stringright, new, test, %extract%
if new <> tring
	msgbox, stringright problem 1
ChkLen(new)
stringright, new, test, 555
if new <> %test%
	msgbox, stringright problem 2
ChkLen(new)

stringmid, new, test, 4, %extract%
if new <> stack
	msgbox, stringmid problem 1
ChkLen(new)
stringmid, new, test, 4, 555
if new <> stack string
	msgbox, stringmid problem 2
ChkLen(new)
stringmid, new, test, 4
if new <> stack string
	msgbox %new%
ChkLen(new)
stringmid, new, test, 888
if new <>
	msgbox problem 3
ChkLen(new)
stringmid, new, test, 1, %nonexistent%  ; Var/expr that resolves to blank is not the same as blank (for backward compatibility).
if new <>
	msgbox problem 4
ChkLen(new)
stringmid, new, test, 1  ; Var/expr that resolves to blank is not the same as blank (for backward compatibility).
if new <> haystack string
	msgbox problem 5
ChkLen(new)

InputVar = The Red Fox
StringMid, new, InputVar, 7, 3, L
if new <> Red
	msgbox %new%
ChkLen(new)
StringMid, new, InputVar, 14, 6, L
if new <> Fox
	msgbox %new%
ChkLen(new)
StringMid, new, InputVar, 14,, L
if new <> The Red Fox
	msgbox %new%
ChkLen(new)
StringMid, new, InputVar, 2147483647,, L
if new <> The Red Fox
	msgbox %new%
ChkLen(new)
StringMid, new, InputVar, 1,, L
if new <> T
	msgbox %new%
ChkLen(new)

stringtrimleft, new, test, %extract%
if new <> ack string
	msgbox, stringtrimleft problem 1
ChkLen(new)
stringtrimleft, new, test, 555
if new <>
	msgbox, stringtrimleft problem 2
ChkLen(new)
stringtrimright, new, test, %extract%
if new <> haystack s
	msgbox, stringtrimright problem 1
ChkLen(new)
stringtrimright, new, test, 555
if new <>
	msgbox, stringtrimright problem 2
ChkLen(new)

; Test string commands when source & dest are identical:
Var = quick brown fox
StringRight, var, var, 77
if var <> quick brown fox
	MsgBox StringRight
ChkLen(var)
Var = 7
Var += Var ; source/dest are same.
if Var <> 14
	MsgBox += problem

StringGetPos, pos, test, stack
if (pos <> 3 || InStr(test, "stack") <> 4)
	msgbox, StringGetPos problem 1
ChkLen(pos)
StringGetPos, pos, test, %test%
if (pos <> 0 || InStr(test, test) <> 1)
	msgbox, StringGetPos problem 2
if errorlevel <> 0
	msgbox, StringGetPos should not have set errorlevel to non-zero
StringGetPos, pos, test, %blankvar%  ;blank is always found at pos zero
if (pos <> 0 || InStr(test, blankvar) <> 1)
	msgbox, StringGetPos problem 3
if errorlevel <> 0
	msgbox, StringGetPos should not have set errorlevel to non-zero
StringGetPos, pos, test, bogus
;msgbox, %pos%
if errorlevel = 0
	msgbox, StringGetPos should have set errorlevel to non-zero

ErrorLevel = Start
TargetStr = abc123

StringGetPos, pos, TargetStr, abc, R
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
if pos <> 0
	MsgBox, pos %pos% was supposed to be 0.

StringGetPos, pos, TargetStr, 123, R
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
if pos <> 3
	MsgBox, pos %pos% was supposed to be 3.

StringGetPos, pos, TargetStr, abc, R
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
if pos <> 0
	MsgBox, pos %pos% was supposed to be 0.

TargetStr = 

StringGetPos, pos, TargetStr, bogus
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.

nothing = 
TargetStr = F

StringGetPos, pos, TargetStr, ff
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.

StringGetPos, pos, TargetStr, x
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.

StringGetPos, pos, TargetStr, f
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
if pos <> 0
	MsgBox, pos %pos% was supposed to be 0.

TargetStr = FFFFFF

StringGetPos, pos, TargetStr, %nothing%  ; Find the empty string
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
if pos <> 0
	MsgBox, pos %pos% was supposed to be 0.

StringGetPos, pos, TargetStr, bogus
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.

StringGetPos, pos, TargetStr, FF
if pos <> 0
	MsgBox, pos %pos% was supposed to be 0.
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.

StringGetPos, pos, TargetStr, FF, L3
if pos <> 4
	MsgBox, pos %pos% was supposed to be 4.
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.

StringGetPos, pos, TargetStr, FF, R3
if pos <> 0
	MsgBox, pos %pos% was supposed to be 0.
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.

StringGetPos, pos, TargetStr, FF, R
if pos <> 4
	MsgBox, pos %pos% was supposed to be 4.
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.

StringGetPos, pos, TargetStr, FF, R4
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.

StringGetPos, pos, TargetStr, FF, L4
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.

TargetStr = A real string with ä.

StringGetPos, pos, TargetStr, real
if pos <> 2
	MsgBox, pos %pos% was supposed to be 2.
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
StringGetPos, pos, TargetStr, Ä  ; when not locale-aware, it shouldn't see Ä==ä
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.

StringCaseSense, on
StringGetPos, pos, TargetStr, REAL
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.
if ErrorLevel <> 1
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 1.
StringGetPos, pos, TargetStr, Ä  ; when not locale-aware, it shouldn't see Ä==ä
if pos <> -1
	MsgBox, pos %pos% was supposed to be -1.

StringCaseSense Locale
StringGetPos, pos, TargetStr, REAL
if pos <> 2
	MsgBox, pos %pos% was supposed to be -1.
if ErrorLevel <> 0
	MsgBox, ErrorLevel %ErrorLevel% was supposed to be 0.
StringGetPos, pos, TargetStr, Ä  ; when not locale-aware, it shouldn't see Ä==ä
if pos < 3
	MsgBox, pos %pos% was supposed >= 3.

StringCaseSense, off
	
Haystack = 123abc123abc123

StringGetPos, pos, Haystack, bogus
if (pos > -1 || !ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc
if (pos <> 3 || ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc,, 3
if (pos <> 3 || ErrorLevel || InStr(Haystack, "abc", false, 4) <> 4)
	MsgBox problem

StringGetPos, pos, Haystack, abc,, 4
if (pos <> 9 || ErrorLevel || InStr(Haystack, "abc", false, 5) <> 10)
	MsgBox problem

StringGetPos, pos, Haystack, abc,, 10
if (pos > -1 || !ErrorLevel || InStr(Haystack, "ABC", false, 10) <> 10 || InStr(Haystack, "ABC", false, 11) <> 0)
	MsgBox problem

StringGetPos, pos, Haystack, abc,, 200
if (pos > -1 || !ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc, L2
if (pos <> 9 || ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc, R
if (pos <> 9 || ErrorLevel || InStr(Haystack, "ABC", false, 0) <> 10)
	MsgBox problem

if (InStr(HayStack, "") <> 1 || InStr(HayStack, "", true, 16) <> 16 || InStr(HayStack, "", true, 17) <> 0)
	MsgBox problem

if (InStr(HayStack, "ABC", true) <> 0 || InStr(HayStack, "abc", true) <> 4)  ; Case sensitive, not found.
	MsgBox problem

StringGetPos, pos, Haystack, abc, R2
if (pos <> 3 || ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc, R2, 3
if (pos <> 3 || ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc, R2, 4
if (pos <> -1 || !ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc, L2, 3
if (pos <> 9 || ErrorLevel)
	MsgBox problem

StringGetPos, pos, Haystack, abc, L2, 4
if (pos <> -1 || !ErrorLevel)
	MsgBox problem


; Locale test of StringReplace:
StringCaseSense Off
Var1 = Ää Ää
StringReplace, Var2, Var1, ä, b  ; It should only see the lowercase one when non-locale-aware.
if Var2 <> Äb Ää
	MsgBox StringReplace problem.
StringReplace, Var2, Var1, ä, b, UseErrorLevel  ; Implies "all".
if Var2 <> Äb Äb
	MsgBox StringReplace problem.
ChkLen(Var2)

StringCaseSense Locale
StringReplace, Var2, Var1, ä, b  ; It should spot the first one this time.
if Var2 <> bä Ää
	MsgBox StringReplace problem.
ChkLen(Var2)
StringReplace, Var2, Var1, ä, b, UseErrorLevel  ; Implies "all".
if (Var2 <> "bb bb" || ErrorLevel <> 4)
	MsgBox StringReplace problem.
ChkLen(Var2)
if A_StringCaseSense <> Locale
	MsgBox problem

StringCaseSense On
StringReplace, Var2, Var1, ä, b
if Var2 <> Äb Ää
	MsgBox StringReplace problem.
ChkLen(Var2)
StringReplace, Var2, Var1, ä, b, UseErrorLevel  ; Implies "all".
if Var2 <> Äb Äb
	MsgBox StringReplace problem.
ChkLen(Var2)

StringCaseSense Off


target = 
stringreplace, target, target, 12, 34
if ErrorLevel = 0
	msgbox ErrorLevel should have been 1.
if target <>
	msgbox target should have been blank
ChkLen(target)

target = abc123abc
stringreplace, target, target, abc, 123
if target <> 123123abc
	msgbox Stringreplace problem
ChkLen(target)

stringreplace, target, target, 123, abc, All
if target <> abcabcabc
	msgbox Stringreplace problem
ChkLen(target)

stringreplace, target, target, abc, 123, Allslow
if target <> 123123123
	msgbox Stringreplace problem
ChkLen(target)

stringreplace, target, target, 123, %String10K%  ; Should replace only one occurrence.
ChkLen(target)
if strlen(target) <> 10006
	msgbox strrplace length <> 10006
stringright, right, target, 6
if right <> 123123
	msgbox right <> 123123

; Ensure no infinite loop:
target = ab
stringreplace, target, target, ab, abab, All
if target <> abab
	msgbox Stringreplace problem
ChkLen(target)

; Ensure built-in vars work with Stringreplace ok:
StringReplace, target, A_ScriptFullPath, not-found, xxx
if (target <> A_ScriptFullPath)
	msgbox Stringreplace problem
ChkLen(target)

; Test of fast-mode StringReplace, since all the others here probably use slow due to being too small to warrant fast:
IfNotInString, String10K, 9
	msgbox Stringreplace test string lacks 9's to test with.
stringreplace, target, String10K, 9, `n, All
if ErrorLevel <> 0
	msgbox ErrorLevel was expected to be 0 (success).
IfInString, target, 9
	msgbox Stringreplace problem
ChkLen(target)

StringReplace, Correct, String10K, 7,, AllSlow
StringReplace, FormerlyWrong, String10K, 7,, All
if Correct <> %FormerlyWrong%
	MsgBox Stringreplace Problem 1
ChkLen(Correct)
ChkLen(FormerlyWrong)

StringReplace, Correct, String10K, 23, longer string, AllSlow
StringReplace, FormerlyWrong, String10K, 23, longer string, All
if Correct <> %FormerlyWrong%
	MsgBox Stringreplace Problem 2
ChkLen(Correct)
ChkLen(FormerlyWrong)


target = 11111111111111111111111111111
clipboard = %target%
stringlen, startlen, target
;stringreplace, target2, target, 1, 22, A
;msgbox, "%target2%"
stringreplace, target, target, 1, 22, A
;msgbox, "%target%"
stringlen, endlen, target
expected_endlen = %startlen%
expected_endlen *= 2
if expected_endlen <> %endlen%
{
	listvars
	msgbox, strreplace: expected(%expected_endlen%) <> actual(%endlen%)
	exitapp
}
stringreplace, target, target, bogus, x, A
if errorlevel = 0
	msgbox, stringreplace should have set errorlevel to non-zero
ChkLen(target)
target = xxxxxxNeedlexxxxxNeedlexxxxxx
stringreplace, target, target, Needle, -replacedwithsomething-, a
;msgbox, %target%
stringlen, targetlen, target
;msgbox, %targetlen%
ChkLen(target)

stringlen, startlen, clipboard
stringreplace, clipboard, clipboard, 1, 22, A
;msgbox, "%clipboard%"
stringlen, endlen, clipboard
expected_endlen = %startlen%
expected_endlen *= 2
if expected_endlen <> %endlen%
{
	listvars
	msgbox, strreplace CLIPBOARD: expected(%expected_endlen%) <> actual(%endlen%)
	exitapp
}


; works
;stringmid, new, a_min, 2, %extract%
;msgbox, %new%


; clipboard operation:
clipboard = yyy
clipboard = xxx%clipboard%yyy
if clipboard <> xxxyyyyyy
	msgbox, clipboard(%clipboard%) is not what was expected.
;msgbox, %clipboard%

var = lijiljoijoimoimoim
clipboard = %var%
if clipboard <> %var%
	msgbox, clipboard isn't what was expected
clipboard = <<%clipboard%>>
clipboard = ++%clipboard%++
;msgbox, %clipboard%

clipboard = 55
clipboard /= %clipboard%
if clipboard <> 1
	msgbox, clipboard should've been 1

stringleft, clipboard, a_scriptname, 1
if clipboard <> M
	msgbox, clipboard should have been M (%a_scriptname%)

clipboard = MAIN something with ä
ifnotinstring, clipboard, MAIN
	msgbox, MAIN should've been in the string
if (InStr(clipboard, "MAIN") <> 1)
	msgbox, MAIN should've been first char in the string
IfInString, clipboard, Ä
	msgbox, Ä should not be equal to ä when not locale-aware.

Stringcasesense, locale
ifnotinstring, clipboard, main
	msgbox, MAIN should've been first char in the string
if not InStr(clipboard, "main")
	msgbox, MAIN should've been first char in the string
if not InStr(clipboard, "Ä")
	msgbox, Ä should match ä in this mode.
if InStr(clipboard, "Ä", true)
	msgbox, Ä shouldn't match ä in this mode.
IfNotInString, clipboard, Ä
	msgbox, Ä should match ä in this mode.

Stringcasesense, on
ifinstring, clipboard, main
	msgbox, "main" should NOT have been in the string due to stringcasesense
if (InStr(clipboard, "main", true))
	msgbox, "main" should NOT have been in the string due to true as last param
IfInString, clipboard, Ä
	msgbox, Ä should not be equal to ä when not case sensitive.

upper = ABC
if upper = abc
	msgbox, %upper% should not equal abc due to stringcasesense
stringcasesense, off
if upper <> abc
	msgbox, %upper% should have equaled abc due to stringcasesense
clipboard = something
stringleft, clipboard, clipboard, 5
if clipboard <> somet
	msgbox, clipboard is %clipboard%, not somet
clipboard = abcdefabcdefabcdefabcdefabcdefabcdefabcdefabcdef
stringreplace, clipboard, clipboard, abcdef, a, a
;msgbox, An amount of memory was reserved for the clipboard`, but it wasn't fully used due to stringreplace (probably not a problem): %clipboard%

; Try something that isn't found:
Clipboard = xyz123
stringreplace, clipboard, clipboard, abcdef, a, UseErrorLevel
if (clipboard <> "xyz123")
	msgbox stringreplace
if ErrorLevel <> 0
	msgbox Stringreplace ErrorLevel <> 0 


;;;;;;;;;;;;;;;;;;;


batchlines = %a_numbatchlines%
setbatchlines 333
if a_numbatchlines <> 333
	msgbox batchlines %a_numbatchlines%
setbatchlines %batchlines%

TitleMatchMode = %a_TitleMatchMode%
setTitleMatchMode, 3
if a_TitleMatchMode <> 3
	msgbox TitleMatchMode
setTitleMatchMode %TitleMatchMode%

TitleMatchModeSpeed = %a_TitleMatchModeSpeed%
setTitleMatchMode, slow
if a_TitleMatchModeSpeed <> slow
	msgbox TitleMatchModeSpeed
setTitleMatchMode %TitleMatchModeSpeed%

DetectHiddenWindows = %a_DetectHiddenWindows%
DetectHiddenWindows, on
if a_DetectHiddenWindows <> on
	msgbox DetectHiddenWindows
DetectHiddenWindows %DetectHiddenWindows%

DetectHiddenText = %a_DetectHiddenText%
DetectHiddenText, off
if a_DetectHiddenText <> off
	msgbox DetectHiddenText
DetectHiddenText %DetectHiddenText%

AutoTrim = %a_AutoTrim%
AutoTrim, off
if a_AutoTrim <> Off
	msgbox AutoTrim
AutoTrim %AutoTrim%

StringCaseSense = %a_StringCaseSense%
StringCaseSense, on
if a_StringCaseSense <> On
	msgbox StringCaseSense
StringCaseSense %StringCaseSense%

FormatInteger = %a_FormatInteger%
setformat, integer, h
if a_FormatInteger <> H
	msgbox FormatInteger
setformat, integer, %FormatInteger%

FormatFloat = %a_FormatFloat%
setformat, float, 5.18
if a_FormatFloat <> 5.18
	msgbox FormatFloat %a_formatfloat%
setformat, float, %FormatFloat%

KeyDelay = %a_KeyDelay%
setKeyDelay, 333
if a_KeyDelay <> 333
	msgbox KeyDelay
setKeyDelay %KeyDelay%

WinDelay = %a_WinDelay%
setWinDelay, 444
if a_WinDelay <> 444
	msgbox WinDelay
setWinDelay %WinDelay%

ControlDelay = %a_ControlDelay%
setControlDelay, 555
if a_ControlDelay <> 555
	msgbox ControlDelay
setControlDelay %ControlDelay%

MouseDelay = %a_MouseDelay%
setMouseDelay, 666
if a_MouseDelay <> 666
	msgbox MouseDelay
setMouseDelay %MouseDelay%

DefaultMouseSpeed = %a_DefaultMouseSpeed%
setDefaultMouseSpeed, 50
if a_DefaultMouseSpeed <> 50
	msgbox DefaultMouseSpeed
setDefaultMouseSpeed %DefaultMouseSpeed%


if a_IconFile <>
	msgbox iconfile
if A_IconNumber <>
	msgbox A_IconNumber unexpectedly = %A_IconNumber% vs. blank
if a_iconhidden <> 0
	msgbox iconhidden

menu, tray, icon, ..\..\Release\AutoHotkey.exe, 2  ; TEST THAT IT GETS CONVERTED TO absolute path as it should.
if a_IconFile <> C:\A-Source\AutoHotkey\Release\AutoHotkey.exe
	msgbox iconfile
if A_IconNumber <> 2
	msgbox A_IconNumber is %A_IconNumber% vs. 2.
if a_iconhidden <> 0
	msgbox iconhidden

menu, tray, NoIcon
if a_iconhidden <> 1
	msgbox iconhidden

menu, tray, icon, C:\A-Source\AutoHotkey\source\resources\icon_filetype.ico
if a_IconFile <> C:\A-Source\AutoHotkey\source\resources\icon_filetype.ico
	msgbox iconfile
if A_IconNumber <> 1
	msgbox A_IconNumber is %A_IconNumber% vs. 1
if a_iconhidden <> 1
	msgbox iconhidden

menu, tray, icon, *
if a_IconFile <>
	msgbox iconfile
if A_IconNumber <>
	msgbox A_IconNumber is %A_IconNumber% vs. blank.
if a_iconhidden <> 1
	msgbox iconhidden

menu, tray, Icon
if a_iconhidden <> 0
	msgbox iconhidden



SetBatchLines -1
Loop, 256
{
	ascii = %a_index%
	--ascii
	transform, out, chr, %ascii%
	if out is digit
		if out <>
		{
			;msgbox %out% (ascii %ascii%)
			if out < 0
				msgbox %out% (ascii %ascii%) is seen as a digit!
			else if out > 9
				msgbox %out% (ascii %ascii%) is seen as a digit!
		}
}

Var = äöüÄÖÜß¤ïéâçëï
Transform, Var, HTML, %Var%
if Var <> &auml;&ouml;&uuml;&Auml;&Ouml;&Uuml;&szlig;&curren;&iuml;&eacute;&acirc;&ccedil;&euml;&iuml;
	MsgBox Problem

Transform, out, asc, ~
if (out <> 126 || Asc("~") <> 126)
	MsgBox %out% should have been 126.
Transform, out, asc, ‚  ; This is not a comma but a char that looks like it.
if (out <> 130 || Asc("‚") <> 130)  ; Ensure chars are unsigned.
	MsgBox %out% should have been 130.

Transform, out, chr, 126
if (out <> "~" || Chr(126) <> "~")
	msgbox %out% should have been ~
; Ensure chars are unsigned:
Transform, out, chr, 130
if (out <> "‚" || Chr(130) <> "‚")  ; This is not a comma but a char that looks like it.
	msgbox %out% should have been ‚

Transform, out, mod, 17, 3
if (out <> 2 || mod(17, 3) <> 2)
	Msgbox %out% should have been 2.
Transform, out, mod, -17, 3
if (out <> -2 || mod(-17, 3) <> -2)
	Msgbox %out% should have been -2.
Transform, out, mod, 17, -3
if (out <> 2 || mod(17, -3) <> 2)
	Msgbox %out% should have been 2.
Transform, out, mod, -17, -3
if (out <> -2 || mod(-17, -3) <> -2)
	Msgbox %out% should have been -2.
Transform, out, mod, -17, 3.5
if (out <> -3 || mod(-17, 3.5) <> -3)
	Msgbox %out% should have been -3.
Transform, out, mod, 17, -3.5  ; modulus operation is mathematically defined to return a positive in this case.
if (out <> 3 || mod(17, -3.5) <> 3)
	Msgbox %out% should have been 3.
Transform, out, mod, 0xffffffffff, 515  ; I don't think the full capacity of 64 bit ints is possible, since doubles are used.
if out <> 265
	Msgbox %out% should have been 265.

CheckFloat(result, expected)
{
	if abs(result-expected) > 0.00001
		MsgBox Floating point result %result% differs too much from expected result %expected%.
}

Transform, out, mod, 3.14, 0.7
CheckFloat(out, 0.34)

var := mod(3.14, 0.7)
CheckFloat(var, 0.34)

if round(mod(3.14, 0.7), 2) <> .34  ; Rounding is required, probably due to internally higher precision than what was stored.
	MsgBox %var%

Transform, out, pow, 3, 4
if out <> 81
	Msgbox %out% should have been 81.
; ensured that it didn't have decimal point: MsgBox %out%
Transform, out, pow, 3, 0.5
if out <> 1.732051
	Msgbox %out% should have been 1.732051.
Transform, out, pow, 3, -1
if out <> 0.333333
	Msgbox %out% should have been 0.333333.
Transform, out, pow, 3, -2
if out <> 0.111111
	Msgbox %out% should have been 0.111111.
thing = -3
Transform, out, pow, %thing%, 2
if out <> 9
	Msgbox %out% should have been 9.
Transform, out, pow, 2999999999, 2  ; Something that will barely fit in a signed 64-bit number.
if out <> 8999999994000000000  ; Correct answer is one more than this, but doubles cause round-off
	Msgbox %out% should have been 8999999994000000000.

thing = -3
Transform, out, sqrt, %thing%
if (out <> "" || sqrt(thing) <> "")
	Msgbox %out% should have been blank.
Transform, out, sqrt, 0
if (out <> 0 || sqrt(0) <> 0)
	Msgbox %out% should have been 0.
Transform, out, sqrt, 4
if (out <> 2 || sqrt(4) <> 2)
	Msgbox %out% should have been 2.
Transform, out, sqrt, 313
if (round(out, 6) <> 17.691806 || round(sqrt(313), 6) <> 17.691806)
	Msgbox %out% should have been 17.691806.
Transform, out, sqrt, 8999999994000000001
if (out <> 2999999999 || sqrt(8999999994000000001) <> 2999999999)
	Msgbox %out% should have been 2999999999.


thing = -3
Transform, out, log, %thing%
if (out <> "" || log(thing) <> "")
	Msgbox %out% should have been blank.
Transform, out, log, 100
if (out <> 2 || log(100) <> 2)
	Msgbox %out% should have been 2.
Transform, out, ln, 100
if (round(out, 6) <> 4.605170 || round(ln(100), 6) <> 4.605170)
	Msgbox %out% should have been 4.605170.
Transform, out, ln, 8999999994000000001
if (round(out, 6) <> 43.643756 || round(ln(8999999994000000001), 6) <> 43.643756)
	Msgbox %out% should have been 43.643756.

Transform, out, exp, -1.5
if (round(out, 6) <> 0.223130 || round(exp(-1.5), 6) <> 0.223130)
	Msgbox %out% should have been 0.223130.

Transform, out, Round, 1.7, 0
if (out <> 2 || Round(1.7, 0) <> 2)
	Msgbox %out% should have been 2.
Transform, out, Round, -1.7
if (out <> -2 || Round(-1.7) <> -2)
	Msgbox %out% should have been -2.
Transform, out, Round, 5.5
if (out <> 6 || Round(5.5) <> 6)
	Msgbox %out% should have been 6.
; this ensured it didn't contain a decimal point: MsgBox %out%
Transform, out, Round, 5.58925, 2
if (out <> 5.59 || Round(5.58925, 2) <> 5.59)
	Msgbox %out% should have been 5.59.
Transform, out, Round, 5.58925
if (out <> 6 || Round(5.58925) <> 6)
	Msgbox %out% should have been 6.
; this ensured it didn't contain a decimal point: MsgBox %out%
Transform, out, Round, 1055.58925, -1
if (out <> 1060 || Round(1055.58925, -1) <> 1060)
	Msgbox %out% should have been 1060.
Transform, out, Round, 1055.58925, -2
if (out <> 1100 || Round(1055.58925, -2) <> 1100)
	Msgbox %out% should have been 1100.
Transform, out, Round, 8999999994888888881
if (out <> 8999999994888889344 || Round(8999999994888888881) <> 8999999994888889344) ; Off a little due to use of doubles internally
	Msgbox %out% should have been 8999999994888889344.
Transform, out, Round, 8999999994888, -4
if out <> 8999999990000  ; Off a little due to use of doubles internally
	Msgbox %out% should have been 8999999990000.

Transform, out, ceil, 4.5
if (out <> 5 || ceil(4.5) <> 5)
	msgbox %out% should have been 5
Transform, out, floor, 4.5
if (out <> 4 || floor(4.5) <> 4)
	msgbox %out% should have been 4

if Ceil(0.8) != 1
	Goto Problem
if Ceil(0.2) != 1
	Goto Problem
if Ceil(0.0) != 0
	Goto Problem
if Ceil(0) != 0
	Goto Problem
if Ceil(1) != 1
	Goto Problem
if Ceil(1.0) != 1
	Goto Problem
if Ceil(62/61) != 2
	Goto Problem
if Ceil(-0.8) != 0
	Goto Problem
if Ceil(-0.2) != 0
	Goto Problem
if Ceil(-1.2) != -1
	Goto Problem
if Ceil(-1) != -1
	Goto Problem
if Ceil(-1.0) != -1
	Goto Problem
if Ceil(-62/61) != -1
	Goto Problem
Goto SkipProblem

Problem:
ListLines
MsgBox Problem.
SkipProblem:

Loop 1000
{
	if Floor((A_Index+2000002) / (A_Index+2000001)) <> 1
		MsgBox Floor %A_Index%
	if Ceil((A_Index+1) / A_Index) <> 2
		MsgBox Ceil %A_Index%
	if Floor((-A_Index-2) / (A_Index+1)) <> -2
		MsgBox Floor neg1 %A_Index%
	if Floor((-A_Index-2000002) / (A_Index+2000001)) <> -2
		MsgBox Floor neg2 %A_Index%
	if Ceil((-A_Index-2) / (A_Index+1)) <> -1
		MsgBox Ceil neg %A_Index%
}

Transform, out, abs, 4.5
if (out <> 4.5 || abs(4.5) <> 4.5)
	msgbox %out% should have been 4.5
; This ensured that no decimal point was present:
;Transform, out, abs, -4
;msgbox %out%
Transform, out, abs, -4.5
if (out <> 4.5 || abs(-4.5) <> 4.5)
	msgbox %out% should have been 4.5
Transform, out, abs, 8999999994888888881
if (out <> 8999999994888888881 || abs(8999999994888888881) <> 8999999994888888881)
	msgbox %out% should have been 8999999994888888881
Transform, out, abs, -8999999994888888881
if (out <> 8999999994888888881 || abs(-8999999994888888881) <> 8999999994888888881)
	msgbox %out% should have been 8999999994888888881

Transform, out, sin, 0.8
if (round(out, 6) <> 0.717356 || round(sin(0.8), 6) <> 0.717356)
	msgbox %out% should have been 0.717356
Transform, out2, asin, %out%
if (round(out2, 2) <> 0.8 || round(asin(out), 2) <> 0.8)
{
	msgbox %out2% should have been 0.8
	msgbox % round(asin(out), 2)
}


Transform, out, cos, 0.8
if (round(out, 6) <> 0.696707 || round(cos(0.8), 6) <> 0.696707)
	msgbox %out% should have been 0.696707
Transform, out2, acos, %out%
if (round(out2, 2) <> 0.8 || round(acos(out), 2) <> 0.8)
	msgbox %out2% should have been 0.8

Transform, out, tan, 0.8
if (round(out, 6) <> 1.029639 || round(tan(0.8), 6) <> 1.029639)
	msgbox %out% should have been 1.029639
Transform, out2, atan, %out%
if (round(out2, 2) <> 0.8 || round(atan(out), 2) <> 0.8)
	msgbox %out2% should have been 0.8

Transform, out, asin, 1.5
if (out <> "" || asin(1.5) <> "")
	msgbox %out% should have been blank

Transform, out, acos, -20
if (out <> "" || acos(-20) <> "")
	msgbox %out% should have been blank

;setformat, float, 0.30
Transform, pi, atan, 1
pi *= 4
if (round(pi, 5) <> 3.14159 || round(atan(1)*4, 2) <> 3.14)
	msgbox %pi% should have been 3.141592


Transform, out, bitand, -1, -1
if out <> -1
	Msgbox %out% should have been -1
Transform, out, bitand, 0xFFFFFFFFFFFFFFFF, 0x0F0E0D0C0B0A0908
if out <> 0x0F0E0D0C0B0A0908
	Msgbox %out% should have been 0x0F0F0F0F0F0F0F0F
Transform, out, bitand, 0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF
if out <> 0x7FFFFFFFFFFFFFFF  ; this happens because ATOI64() truncates values larger than the largest signed 64 bit number.
	Msgbox %out% should have been 0x7FFFFFFFFFFFFFFF
Transform, out, bitand, -1, 1
if out <> 1
	Msgbox %out% should have been 1

Transform, out, bitor, -1, 1
if out <> -1
	Msgbox %out% should have been -1
Transform, out, bitor, 0xF0E0D0, 0xFFF000
if out <> 0xFFF0D0
	Msgbox %out% should have been 0xFFF0D0

Transform, out, bitxor, 0xF0E0D0, 0xFFF000
if out <> 0x0F10D0
	Msgbox %out% should have been 0x0F10D0

Transform, out, bitshiftleft, 5, 3  ; equivalent to 5 * (2^3)
if out <> 40
	Msgbox %out% should have been 40

Transform, out, bitshiftright, 100, 3  ; equivalent to 100 / (2^3)
if out <> 12
	Msgbox %out% should have been 12

Transform, out, bitshiftright, 100, 0
if out <> 100
	Msgbox %out% should have been 100

; Generates load-time error as it should: Transform, out, mod, 100, 0

; This part was used to check all the various load-time validation items:
;Transform, out, bitshiftleft, 1, 0
;msgbox %out%
;exitapp

;SetFormat, integer, hex
;out = 0
;out--
;msgbox %out%

var = 12
Transform, out, chr, var + 1
if out <> `r
	MsgBox Trans expr.
Transform, out, round, var / 5
if (out <> 2 || Round(var / 5) <> 2)
	MsgBox Trans expr.
Transform, out, round, var / 5 + .5
if out <> 3
	MsgBox Trans expr.
Var2 = 2
Transform, out, round, var / 5, var2 - 1
if out <> 2.4
	MsgBox Trans expr.

; -1 for testing.  But it looks like -1 is too fast to allow the target window to update prior to next Get
; to check its status.  But zero works most of the time.  The default (10) is better.
SetControlDelay, 10
SetTitleMatchMode, 2
SetBatchLines -1

SetTimer, ControlMoveTimer
InputBox, test, ControlMoveTimer

ErrorLevel = Start

;ControlGet, selected, selected, , Edit1, Untitled - Notepad
;MsgBox "%selected%"
;ExitApp

; GET THE CURR. SELECTED TEXT IN THE FOCUSED EDIT CONTROL OF THE ACTIVE WINDOW:
;Sleep, 2000
;ControlGetFocus, control, A
;ControlGet, selected, selected, , %control%, A
;MsgBox "%selected%" (ErrorLevel %ErrorLevel%)
;ExitApp

IfWinNotExist, Untitled - Notepad
{
	Run, notepad
	WinWait, Untitled - Notepad
}
WinActivate
Send, ^{home}0123456789 0123456789 0123456789{enter}0123456789 0123456789 0123456789{enter 2}
Send, ^{home}{down}^{right}{right 3}^+{right}
;Send, !{tab}
;Sleep, 50
ErrorLevel = init
ControlGet, selected, selected, , Edit1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if selected <> 3456789%a_space%
	MsgBox "%selected%" is not what was expected.
ChkLen(selected)

WinClose
WinWaitActive, Notepad, The text in the, 0.5
if ErrorLevel = 0
	Send, n

; It needs a little extra time to close (only when another hook script is running and this one has timers?)
; Otherwise, the last found window set inside the loop below will be the Notepad window that is in the
; process of closing, which is not what we want.  Update: just changed pad to metapad below so this is
; no longer needed:
;Sleep, 20

;WinActivate, Outlook
;WinWait, pad
;WinActivate
;IfWinNotActive, pad, , MsgBox Problem

Loop, 2
{
IfWinNotExist, metapad  ; Use the word metapad vs. pad, see above for explanation.
	Goto, SkipControlCmdTest
WinActivate
if a_index > 1
	Send, !{tab}  ; make sure the window isn't active for the 2nd test.

; Doesn't work with metapad, see comments in code for ControlGet->Selected:
;ControlGet, selected, selected, , RichEdit20A1
;MsgBox "%selected%"
;ExitApp

Edit
WinWaitActive, %A_ScriptFullPath%

ErrorLevel = init
ControlGet, line, line, 10, RichEdit20A1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if line <>;ensure comma delimiter works without whitespace, and with tabs`r
	MsgBox This line isn't what was expected:`n"%line%"
ChkLen(line)

ErrorLevel = init
ControlGet, lines, LineCount, , RichEdit20A1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if lines < 1800
	msgbox, %lines% seems too small.
if lines > 6000
	msgbox, %lines% seems too large.

ErrorLevel = init
ControlGet, curr_line, CurrentLine, , RichEdit20A1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
;msgbox, %curr_line%
;ExitApp
if curr_line < 1
	msgbox, %curr_line% seems too small.
if curr_line > 6000
	msgbox, %curr_line% seems too large.

ErrorLevel = init
ControlGet, curr_col, CurrentCol, , RichEdit20A1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
;MsgBox %curr_col%
;ExitApp
if curr_line < 1
	msgbox, %curr_line% seems too small.
if curr_line > 6000
	msgbox, %curr_line% seems too large.

WinActivate
Send, !{enter}
if a_index = 2
	Send, !{tab}
WinWait, Settings, When launching viewers:
ErrorLevel = init

Control, TabRight, 2, SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
Sleep, 50  ; Needs a little extra time in this case.
ControlGet, WhichTab, Tab, , SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if WhichTab <> 3
	MsgBox, tab %WhichTab% was expected to be 3.

Control, TabLeft, , SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, WhichTab, Tab, , SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if WhichTab <> 2
	MsgBox, tab %WhichTab% was expected to be 2.

Control, TabRight, , SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, WhichTab, Tab, , SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if WhichTab <> 3
	MsgBox, tab %WhichTab% was expected to be 3.

ErrorLevel = init
ControlGet, checked, Checked, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if checked <> 0
	MsgBox Checked is not 0 as expected.
ErrorLevel = init
Control, check, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, checked, Checked, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if checked <> 1
	MsgBox Checked is not 1 as expected.
ErrorLevel = init
Control, Uncheck, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, checked, Checked, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if checked <> 0
	MsgBox Checked is not 0 as expected.

ErrorLevel = init
ControlGet, Enabled, Enabled, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if Enabled <> 1
	MsgBox Enabled is not 1 as expected.
ErrorLevel = init
Control, Disable, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, Enabled, Enabled, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if Enabled <> 0
	MsgBox Enabled is not 0 as expected.
ErrorLevel = init
Control, Enable, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, Enabled, Enabled, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if Enabled <> 1
	MsgBox Enabled is not 1 as expected.

ErrorLevel = init
ControlGet, Visible, Visible, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if Visible <> 1
	MsgBox Visible is not 1 as expected.
ErrorLevel = init
Control, Hide, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, Visible, Visible, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if Visible <> 0
	MsgBox Visible is not 0 as expected.
ErrorLevel = init
Control, Show, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, Visible, Visible, , Hide &go to offset
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if Visible <> 1
	MsgBox Visible is not 1 as expected.

ErrorLevel = init
ControlGet, entry, FindString, Unicode, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if entry <> 3
	MsgBox Entry %entry% was expected to be 3.

ErrorLevel = init
ControlGet, selection, Choice, , ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if selection <> DOS Text
	MsgBox Selection %selection% was expected to be DOS text.

ErrorLevel = init
Control, choose, 3, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%

ErrorLevel = init
ControlGet, selection, Choice, , ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if selection <> Unicode
	MsgBox Selection %selection% was expected to be Unicode.
ChkLen(selection)

ErrorLevel = init
Control, Add, New Entry, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%

ErrorLevel = init
ControlGet, entry_num, FindString, New Entry, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if entry_num <> 5
	MsgBox entry_num %entry_num% was expected to be 5.

ErrorLevel = init
Control, ChooseString, New Entry, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%

ErrorLevel = init
ControlGet, selection, Choice, , ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if selection <> New Entry
	MsgBox Selection %selection% was expected to be New Entry.
ChkLen(selection)

ErrorLevel = init
Control, Delete, 1, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%

ErrorLevel = init
ControlGet, entry_num, FindString, New Entry, ComboBox1
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if entry_num <> 4
	MsgBox entry_num %entry_num% was expected to be 4.

Control, TabLeft, 2, SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
ErrorLevel = init
ControlGet, WhichTab, Tab, , SysTabControl321
if ErrorLevel <> 0
	MsgBox ErrorLevel = %ErrorLevel%
if WhichTab <> 1
	MsgBox, tab %WhichTab% was expected to be 1.

ControlSetText, Edit3, Some Text
ControlFocus, Edit3
ControlSend, Edit3, {home}^{right}
Control, EditPaste, more%a_space%, Edit3
ControlSend, Edit3, {home}+{end}
ControlGet, sel, Selected, , Edit3
if sel <> Some more Text
	MsgBox sel "%sel%" was expected to be "Some more Text"
ChkLen(sel)

if a_index = 1
{
	ErrorLevel = init
	Control, TabRight, 2, SysTabControl321
	if ErrorLevel <> 0
		MsgBox ErrorLevel = %ErrorLevel%
	ErrorLevel = init
	ControlGet, WhichTab, Tab, , SysTabControl321
	if ErrorLevel <> 0
		MsgBox ErrorLevel = %ErrorLevel%
	if WhichTab <> 3
		MsgBox, tab %WhichTab% was expected to be 3.
	
	ErrorLevel = init
	Control, ShowDropDown, , ComboBox1
	if ErrorLevel <> 0
		MsgBox ErrorLevel = %ErrorLevel%
	ErrorLevel = init
	ControlMove, Hide &go to offset, 100, 100
	if ErrorLevel <> 0
		MsgBox ErrorLevel = %ErrorLevel%
	Sleep, 1000
	ErrorLevel = init
	Control, HideDropDown, , ComboBox1
	if ErrorLevel <> 0
		MsgBox ErrorLevel = %ErrorLevel%
	Sleep, 500
}

; Test address operator (&) with SendMsg).
ControlTextSize = 300  ; Must be 32767 or less or it won't work on Windows 95.
VarSetCapacity(ControlText, ControlTextSize)
SendMessage, 0xD, ControlTextSize, &ControlText, Edit1  ; 0xD is WM_GETTEXT
ControlTextLen := strlen(ControlText)
if ControlTextLen < 1
	MsgBox ControlText

WinClose
}

SkipControlCmdTest:

FileSelectFile, file, , , Pick a filename in a deeply nested folder:
if file =
{
	MsgBox, 4,, Continue?
	;IfMsgBox, No, ExitApp
}
else
{
	StringLen, pos_prev, file
	pos_prev++ ; Adjust to be the position after the last char.
	loop
	{
		StringGetPos, pos, file, \, R%a_index%
		if pos < 0
			break
		length = %pos_prev%
		length -= %pos%
		length--
		pos_prev = %pos%
		pos += 2  ; Adjust for use with StringMid.
		StringMid, path_component, file, %pos%, %length%
		MsgBox Path component #%a_index% (from the right) is:`n%path_component%
	}
}


SetBatchLines, -1
fields = 0
Loop read, C:\A-Source\AutoHotkey\Test\Loop parse CSV (THIS IS USED by the large test script).csv, CSV
	Loop parse, A_LoopReadLine, CSV
	{
		++fields
		field = %A_LoopField%  ; Referencing the variable may help expose errors such as string overflow.
		;msgbox, %field%
	}
if fields <> 4731
	MsgBox, %fields% is not equal to 4731


var = 0xFF
if var <> 255
	msgbox hex problem 1a
if var <> 255.0
	msgbox hex problem 1b
var = 255
if var <> 0xFF
	msgbox hex problem 2

SetFormat, integer, h
var++
if var <> 256
	msgbox hex problem 3 (%var%)
stringleft, var2, var, 2
if var2 <> 0x
	msgbox hex problem 4

var = +0xFE
var -= 2
if var <> 252
	msgbox hex problem 4a

; OK:
;var = 0xFF
;var++
;msgbox %var%
;exitapp

; NOT OK (but now it is, due to fix):
;var = 0
;--var
;msgbox %var%
;exitapp


var = -0xFE
var -= 3
;setformat, integer, d
;var += 0
;msgbox, %var%
;exitapp
;msgbox %var%
if var <> -257 ;same as -0x101
	msgbox hex problem 4b


;var = 0
;var += 0
;msgbox %var%

setformat, integer, d
var = 0xFE
var += 0
stringleft, var2, var, 2
if var2 <> 25
	msgbox hex problem 5

var = 15.0
if var <> 0x0F
	msgbox hex problem 6


SetBatchLines, -1
if A_OSType = WIN32_WINDOWS  ; Win9x
	TimerPeriodAllowed = 60   ; Even though Sleep granularity is good, timer granularity isn't.
else
	TimerPeriodAllowed = 16  ; With new dual-core vs. AthlonXP, had to increase from 12 to 16.
SetTimer, TimerSpeedTest, 10
Sleep, 500
WinWait, boguswin, , 0.5
SetTimer, TimerSpeedTest, off
if TimerSpeedProblemCount > 0
	MsgBox, There were %TimerSpeedProblemCount% timer problems.

FileDelete, C:\A-Source\AutoHotkey\Test\File-read loop test file out.txt

Loop, read, C:\A-Source\AutoHotkey\Test\File-read loop test file.txt, *C:\A-Source\AutoHotkey\Test\File-read loop test file out.txt
{
	IfInString, A_LoopReadLine, family
	{
		FileAppend, %A_LoopReadLine%`n
		if ErrorLevel <> 0
			MsgBox, FileAppend problem
		else
		{
			Loop, parse, A_LoopReadLine, %A_Space%
				FileAppend, %A_LoopField%`n
		}
	}
}

problem = n
Loop, read, C:\A-Source\AutoHotkey\Test\File-read loop test file out.txt
{
	;MsgBox, %A_LoopReadLine%
	if a_index = 1
	{
		if A_LoopReadLine <> family Jeff
			problem = y
	}
	else if a_index = 2
	{
		if A_LoopReadLine <> family
			problem = y
	}
	else if a_index = 3
	{
		if A_LoopReadLine <> jeff
			problem = y
	}
	else if a_index = 4
	{
		if A_LoopReadLine <> family Betty
			problem = y
	}
	else if a_index = 5
	{
		if A_LoopReadLine <> family
			problem = y
	}
	else if a_index = 6
	{
		if A_LoopReadLine <> Betty
			problem = y
	}
	else
		MsgBox, More lines than expected!
}
if problem = y
	MsgBox, There was a problem with the file-read/append and/or parsing loop.



problem = n
char_array = zdfg123
loop, parse, char_array
{
	;MsgBox, %A_LoopField%
	if a_index = 1
	{
		if A_LoopField <> z
			problem = y
	}
	else if a_index = 2
	{
		if A_LoopField <> d
			problem = y
	}
	else if a_index = 3
	{
		if A_LoopField <> f
			problem = y
	}
	else if a_index = 4
	{
		if A_LoopField <> g
			problem = y
	}
	else if a_index = 5
	{
		if A_LoopField <> 1
			problem = y
	}
	else if a_index = 6
	{
		if A_LoopField <> 2
			problem = y
	}
	else if a_index = 7
	{
		if A_LoopField <> 3
			problem = y
	}
	else
		MsgBox, More lines than expected #2!
}
if problem = y
	MsgBox, There was a problem with single-char parsing loop.


AutoTrim = %a_AutoTrim%
AutoTrim, off
Clipboard = abc
Gosub, ClipSaveAndRestore
MsgBox Copy some files, formatted text, or press PrintScreen to put something on the clipboard.
Gosub, ClipSaveAndRestore

MsgBox, 4,, Test saving and restoring the clipboard to file?
IfMsgBox, Yes
	Gosub, ClipSaveAndRestoreFromFile
AutoTrim %AutoTrim%



MsgBox, 4, , Do the sound test?
IfMsgBox, NO, Goto, SkipSoundTest

SoundFile = C:\Compress\Miranda\AUTH.WAV
IfNotExist, %SoundFile%
	SoundFile = %A_WinDir%\Media\Ding.wav
IfNotExist, %SoundFile%, MsgBox, Sound file "%SoundFile%" doesn't exist.


SoundGetWaveVolume, vol_orig_wave
;msgbox, %vol_orig_wave%
;exitapp

vol_new = %vol_orig_wave%

vol_new -= 20
if vol_new < 0
	vol_new = 80
SoundSetWaveVolume, %vol_new%
SoundGetWaveVolume, vol
vol_diff = %vol_new%
vol_diff -= %vol%
diff_ok = y
if vol_diff > 0.1
	diff_ok = n
if vol_diff < -0.1
	diff_ok = n
if diff_ok = n
{
	MsgBox, Difference between volumes (%vol_new% - %vol% = %vol_diff%) is larger in magnitude than expected.
	ExitApp
}

SoundGet, master_mute, , mute
if master_mute = on
	msgbox, Master is already muted?!

SoundGet, microphone_mute, MICROPHONE, mute
if microphone_mute = off
	msgbox, Microphone is NOT muted?!



sound_sleep_time = 200

SoundSetWaveVolume, 50
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing 50`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSetWaveVolume, -30
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Reduced by 30`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSetWaveVolume, +30
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Increased by 30`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSetWaveVolume, 80
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing 80`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%


SoundGet, vol_orig_master
SoundSetWaveVolume, 99  ; Make it loud so that diff's between master vol can be heard better.

SoundSet, 50
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing Master 50`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSet, -30
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Reduced Master by 30`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSet, +30
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Increased Master by 30`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSet, 80
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing Master 80`%
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%


SoundSetWaveVolume, %vol_orig_wave%  ; Restore orig.
SoundSet, %vol_orig_master%  ; restore

SoundSet, 1, master, mute
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing Master muted
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSet, 0, master, mute
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing Master unmuted
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSet, +1, wave, mute
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing Wave muted
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%

SoundSet, +1, wave, mute
IfNotEqual, ErrorLevel, 0, MsgBox, Sound Error: %ErrorLevel%
SplashTextOn, , , Playing Wave unmuted
SoundPlay, %SoundFile%, wait
Sleep, %sound_sleep_time%


SkipSoundTest:
SplashTextOff  ; For both the sound test and the timer test.

now += 0, days
if now <> %a_now%
	msgbox, unexpected datetime result for %now% 1
if now <> %a_year%%a_mon%%a_mday%%a_hour%%a_min%%a_sec%
	msgbox, unexpected datetime result for %now% 2

until_utc = %A_NowUTC%
until_utc -= %A_Now%, hours
if until_utc <> 4
	if until_utc <> 5
		msgbox time until utc is unexpectedly %until_utc%

Msgbox, Next, select a file to display stats on, or cancel it to exit.
FileSelectFile, filename
if filename =
{
	MsgBox You pressed cancel. The script will exit.
	ExitApp
}
Loop, %filename%
{
	MsgBox, %A_LoopFilename%`n%A_LoopFileSize%`n%A_LoopFileSizeKB%K`n%A_LoopFileSizeMB%M
}


; Test a little bit of math operation:
var1 = bogus
var2 = 5
var1 -= %var2%
if var1 <> -5
	msgbox, expected -5 but got %var1%
var1 = 5
var2 = bogus
var1 -= %var2%
if var1 <> 5
	msgbox, expected 5 but got %var1%
var1 = 5
var2 = 10
var1 -= %var2%
if var1 <> -5
	msgbox, expected -5 but got %var1%
var1 += 2.5
if var1 <> -2.5
	msgbox, expected -2.5 but got %var1%


msgbox, 4, , Do the test of Send command?
ifmsgbox, yes
{
	run, notepad
	winwait, Untitled - Notepad
	winactivate
	Gosub, SendCmdHotkeys
	TheHotkeys = #8 ^+8 ^8
	Msgbox, Activate notepad and press the following hotkeys: %TheHotkeys%`n`nThen return to this dialog and press OK to continue.
	SetScrollLockState, AlwaysOff  ; Install the hook dynamically.
	MsgBox, The hook has been installed`, try the following hotkeys again: %TheHotkeys%`n`nThen return to this dialog and press OK to continue.
}
Msgbox, 4, , Continue with the rest of the test?
IfMsgBox, no
	ExitApp

RegTestValue = %String10K%
if A_OSType <> WIN32_WINDOWS  ; Not Win9x
	loop, 3  ; Make it larger than 64K.  Doing it 3 times should make it about 80K
		RegTestValue = %RegTestValue%%RegTestValue%

RegTestType = REG_SZ
Gosub, RegTest

RegTestType = REG_EXPAND_SZ
Gosub, RegTest

; Reduce the size also, since REG_MULTI_SZ doesn't support values over 64K:
StringReplace, RegTestValue, RegTestValue, 6789, `n, All
RegTestType = REG_MULTI_SZ
Gosub, RegTest

RegTestValue = 4294967290  ; Close to UINT_MAX
RegTestType = REG_DWORD
Gosub, RegTest

RegTestValue = 01A9FF77
RegTestType = REG_BINARY
Gosub, RegTest

RegTestValue = 
RegTestType = REG_SZ
Gosub, RegTest

RegTestValue = 
RegTestType = REG_MULTI_SZ
Gosub, RegTest

RegTestValue = 0
RegTestType = REG_DWORD
Gosub, RegTest

RegTestValue = 
RegTestType = REG_BINARY
Gosub, RegTest

msgbox, filedialogs: Dismiss this then press Win-F8; answer them in reverse order (i.e. oldest first) with ccc for oldest and ddd for newest

FileSelectFile, file2
ifnotinstring, file2, ccc  ; since it returns the full path.
	msgbox, you entered "%file2%" not ccc for filename or there was a problem.


source = xxxxxxxx              10      xxxxxxxxxxx
stringmid, number_with_leading_spaces, source, 9, 21
;msgbox, %number_with_leading_spaces%
number_with_leading_spaces += 10
if number_with_leading_spaces <> 20
	msgbox, sum should have been 20

counter = 0
repeat, 5
	envadd, counter, 2
	envsub, counter, 1
endrepeat
if counter <> 5
	msgbox, counter should've been 5
;listlines
;msgbox
;exitapp

counter = 0
/*
repeat, 0  ; infinite
	envadd, counter, 2
	envsub, counter, 1
	ifgreater, counter, 4, goto, breakfromrepeat
endrepeat
*/
breakfromrepeat:
if counter <> 5
	msgbox, counter should've been 5
;listlines
;msgbox
;exitapp

wingettitle, active_title1, a
wingetactivetitle, active_title2
if active_title1 <> %active_title2%
	msgbox, %active_title1% <> %active_title2%
ChkLen(active_title1)

SetEnv, test, 5
EnvAdd, test, 5
ifnotequal, test, 10, msgbox, old-cmd not-equal
title =
;ifequal, test, 10, ifless, nothing33, 7, ifwinnotactive, syzoidsoa, , wingetactivestats, title, w, h, x, y
;ifequal, test, 10, ifless, nothing33, 7, ifwinnotactive, syzoidsoa, , run, C:\new`,text.txt
;exitapp
ifwinnotactive, syzoidsoa, , ifequal, test, 10, ifequal, test, 10, wingetactivestats, title, w, h, x, y
;msgbox, %title%`n%w%`n%h%`n%x%`n%y%
if title =
	msgbox, active title is blank?
;ListLines
;Msgbox
;ExitApp

settitlematchmode, 2
settitlematchmode, fast
ifwinexist, pad, , ifwinnotexist, bogus`,`,`,bogus, , winactivate
msgbox, 4, , press yes
ifmsgbox, yes, ifwinexist, xxxxyyyy, , lll=ok
ifnotequal, lll, ok, msgbox, something didn't work


; test operators without whitespace
var3=10000
if var3<>10000
	msgbox, problem when using operators without whitespace?
var3/=10
if var3<>1000
	msgbox, problem when using operators without whitespace?


targetfile = c:\script-test-file.txt
filedelete, %targetfile%
fileappend, test1`ntest2`ntest3`n, %targetfile%
if errorlevel <> 0
{
	msgbox, fileappend error
	exitapp
}
i = 0
all_lines =
loop
{
	i += 1
	filereadline, line, %targetfile%, %i%
	if errorlevel <> 0
		break
	all_lines = %all_lines%`n"%line%"
}
msgbox, %all_lines%
filedelete, %targetfile%


; Make sure commands don't crash when their optional output params are omitted:
ifwinnotexist, Calc
	run, calc
winwait, Calc
winwait, Calc, , 0, ulator
if ErrorLevel = 0
	msgbox, the following line didn't perform as expected`nwinwait`, Calc`, `, 0`, ulator
WinGetPos, , , , h
MouseGetPos, mousex
MouseGetPos, , mousey
mousegetpos, clipboard, clipboard  ; kinda silly, but just to make sure it doesn't crash
will_this_assignment_fail = %clipboard%  ; i.e. clipboard isn't in use.


Sleep, 1000
WinGetText, text, a
if text =
	msgbox, foreground window had no text
ChkLen(text)
WinGetTitle, title, a
if title =
	msgbox, foreground window had no title
ChkLen(title)
stringlen, textlen, text
;msgbox, For window "%title%": Length = %textlen%`n%text%
;ListVars
;msgbox


ifwinexist, Program Manager
{
	wingetpos, x, y, w, h
	;msgbox, Program Manager X%x% Y%y% Width%w% Height%h%
}
else
	msgbox, Program Manager doesn't exist

ifwinnotexist, Calc
	run, calc
winwait, Calc
WinGetText, text, a
ifnotinstring, text, Degrees
	msgbox, calc doesn't have the text expected.
; tested ok: winwaitnotactive

ifwinexist
{
}
else
	msgbox, ifwinexist with zero params should have been true`, but it was false.
winclose
winwaitclose


ifwinnotexist, Calc
	run, calc
winwait, Calc
winactivate
WinSetTitle, , , **** New Title #1 ****
Sleep, 100
WinSetTitle, <<<<<< ANOTHER NEW TITLE using single-parameter mode >>>>>>
Sleep, 500
WinMove, , , 200, 200
Sleep, 500
WinMove, 0, 0  ; try 2-param mode
wingetpos, x, y, w, h
if x <> 0
	msgbox, x%x% should be zero?
if y <> 0
	msgbox, y%y% should be zero?
WinMove, , , 200, 300
wingetpos, x, y
if x <> 200
	msgbox, x%x% should be 200?
if y <> 300
	msgbox, y%y% should be 300?
;msgbox, calc's wingetpos: X%x% Y%y% Width%w% Height%h%



msgbox, 4, , Do the WinMove test (it will alter the size of notepad in a way that will be saved)?
ifmsgbox, yes
{
	size100 = 100
	word_default = default
	ifwinnotexist, Calc
		run, calc
	winwait, Calc
	winactivate
	movesleep = %size100%
	winmove, , , 0, 0, default, %word_default%
	Sleep, %movesleep%
	winmove, , , 200, default, %word_default%, default
	Sleep, %movesleep%
	winmove, , , %word_default%, 200, default, %word_default%
	Sleep, %movesleep%
	winmove, , , 400, default, %word_default%, default
	winclose
	
	run, notepad
	winwait, Untitled - Notepad
	winactivate
	winmove, , , , , 500, 500
	Sleep, %movesleep%
	winmove, , , , , %word_default%, 300
	Sleep, %movesleep%
	winmove, , , , , 300, default
	Sleep, %movesleep%
	winmove, , , , , 100
	Sleep, %movesleep%
	winmove, , , , , , %size100%
	winclose
}


ifwinnotexist, Calc
	run, calc
winwait, Calc
sleep, 500
winminimize
sleep, 500
winactivate
winwaitActive, , , 2
If ErrorLevel <> 0
{
	msgbox, not active 1
	exitapp
}
WinClose, a  ; a=active window
winwaitclose, Calc, , 2
If ErrorLevel <> 0
{
	msgbox, not closed
	exitapp
}

run, calc
winwait, Calc, , 2
If ErrorLevel <> 0
{
	msgbox, no calc exists
	exitapp
}
ifwinactive, Calc
	winminimize, a
sleep, 2000  ; give it time, to make sure it's not the foreground anymore
ifwinexist, Calc
{
	winclose
	winwaitclose, Calc, , 2
	If ErrorLevel <> 0
	{
		msgbox, not closed
		exitapp
	}
}




MouseMove, 100, 100, 0
MouseGetPos, mousex, mousey
mouseok = y
if mousex <> 100
	mouseok = n
if mousey <> 100
	mouseok = n
if mouseok <> y
	msgbox, mouse coords from MouseGetPos were expected to be 100`,100 but are really %mousex%`,%mousey%

msgbox, 4, , test capslock & numlock states (might want to shut down any other scripts that use the hook`, for this)?
ifmsgbox, yes
{
Setcapslockstate, on
msgbox, capslock should be on now
Setcapslockstate, off
msgbox, capslock should be off now
setcapslockstate, alwayson
msgbox, capslock should be alwayson now
setcapslockstate
msgbox, capslock should be normal/neutral now

SetNumlockstate, on
msgbox, Numlock should be on now (Note: Numlock is a squirrely key`, so it's tested as though it's a separate case).
SetNumlockstate, off
msgbox, Numlock should be off now
setNumlockstate, alwayson
msgbox, Numlock should be alwayson now
setNumlockstate
msgbox, Numlock should be normal/neutral now
}



inputbox, inputx, Input, Press Win-F10 to invoke another InputBox`, then dismiss this by entering xyz`, prior to dismissing that one.  Can also press Win-F12 to add some MsgBoxes to the fray.`n`n(Any unexpected results will be reported)

if inputx <> xyz
	msgbox, you didn't enter xyz or there was a problem.


msgbox, 4, , Press Win-F12 to invoke another msgbox`, then dismiss this one with YES prior to dismissing that one.`n`n(Any unexpected results will be reported)
ifmsgbox, no
	msgbox, you didn't press yes`, or there was a problem.


msgbox, 4, , try to briefly hide the foreground window?
ifmsgbox, yes
{
	sleep, 200  ; give things time to settle
	wingettitle, title, a
	winhide, %title%
	sleep, 1000
	winshow, %title%
}

i = 0
loop
{
	i += 1
	if i < 5
	{
		continue
		msgbox, didn't continue
	}
	if i >= 5
	{
		break
		msgbox, didn't break
	}
	msgbox, loop: this message should never appear
}
if i > 5
	msgbox, i was expected to be 5 but it's %i%


msgbox, 4, , do the mouse DRAG and CLICK test in current foreground window?
ifmsgbox, yes
{
	sleep, 200
	mouseclickdrag, left, 100, 100, 400, 400, 0
	sleep, 1000
	mouseclickdrag, left, , , 100, 300, 10  ; i.e. use the current starting position
	sleep, 1000
	mouseclickdrag, left, 100, 100, 500, 500, 0
	sleep, 1000
	leftclickdrag, 50, 50, 222, 222
	sleep, 1000
	mouseclick, left, 200, 200
	send, {shiftdown}
	leftclick, 600, 600
	send, {shiftup}
}


msgbox, 4, , do the mouse move test?
ifmsgbox, yes
{
	MouseMove, 0, 0
	MouseMove, 200, 200, 20
	MouseMove, 500, 400, 5
	SetDefaultMouseSpeed, 0
	MouseMove, 100, 0
	sleep, 500
	MouseMove, 0, 100
	sleep, 500
	MouseMove, -50, -50  ; valid if window isn't full screen
	sleep, 500
}

; MSGBOX
msgbox, 2, title, text (3 second timeout -- let it time out to skip this section), 3
ifmsgbox, timeout
	timeout = y
gosub, displaymsgboxresult
if timeout = y
	goto, skipmsgbox
msgbox, 2, try the other buttons, try the other buttons
gosub, displaymsgboxresult
msgbox, 2, try the other buttons, try the other buttons
gosub, displaymsgboxresult
msgbox, 1, mode1, mode1
gosub, displaymsgboxresult
msgbox, 3, mode3, mode3
gosub, displaymsgboxresult
skipmsgbox:


MsgBox, 4, , Test random number "randomness"?
ifmsgbox, yes
{
	; Note: there was some strangeness with varying batch lines vs. cpu time consumed, but I think that may
	; have been due to limitations of Windows Task Manager (i.e. that it can't accurately measure CPU time
	; consumed when the utilization of a process is very low:
	;SetBatchLines, 10000
	;loop, 1000000
	SetBatchLines, -1
	loop, 100000
	{
		random, rand, 0, 9
		if rand = 0
			rand0 += 1
		else
		if rand = 1
			rand1 += 1
		else
		if rand = 2
			rand2 += 1
		else
		if rand = 3
			rand3 += 1
		else
		if rand = 4
			rand4 += 1
		else
		if rand = 5
			rand5 += 1
		else
		if rand = 6
			rand6 += 1
		else
		if rand = 7
			rand7 += 1
		else
		if rand = 8
			rand8 += 1
		else
		if rand = 9
			rand9 += 1
		else
			rand_bad += 1
	}
	msgbox, Distribution of randomly generated numbers between 0 and 9`n0: %rand0%`n1: %rand1%`n2: %rand2%`n3: %rand3%`n4: %rand4%`n5: %rand5%`n6: %rand6%`n7: %rand7%`n8: %rand8%`n9: %rand9%`n`nOut-of-bounds randoms: %rand_bad%

	rand0 = 0
	rand1 = 0
	rand2 = 0
	rand3 = 0
	rand4 = 0
	rand5 = 0
	rand6 = 0
	rand7 = 0
	rand8 = 0
	rand9 = 0

	loop, 100000
	{
		random, rand, -9, 0
		if rand = 0
			rand0 += 1
		else
		if rand = -1
			rand1 += 1
		else
		if rand = -2
			rand2 += 1
		else
		if rand = -3
			rand3 += 1
		else
		if rand = -4
			rand4 += 1
		else
		if rand = -5
			rand5 += 1
		else
		if rand = -6
			rand6 += 1
		else
		if rand = -7
			rand7 += 1
		else
		if rand = -8
			rand8 += 1
		else
		if rand = -9
			rand9 += 1
		else
			rand_bad += 1
	}
	msgbox, Distribution of NEGATIVE randomly generated numbers between 0 and NEGATIVE 9`n0: %rand0%`n1: %rand1%`n2: %rand2%`n3: %rand3%`n4: %rand4%`n5: %rand5%`n6: %rand6%`n7: %rand7%`n8: %rand8%`n9: %rand9%`n`nOut-of-bounds randoms: %rand_bad%

	rand0 = 0
	rand1 = 0
	rand2 = 0
	rand3 = 0
	rand4 = 0
	rand5 = 0
	rand6 = 0
	rand7 = 0
	rand8 = 0
	rand9 = 0

	loop, 100000
	{
		random, rand, -4, 5
		if rand = 0
			rand0 += 1
		else
		if rand = -4
			rand1 += 1
		else
		if rand = -3
			rand2 += 1
		else
		if rand = -2
			rand3 += 1
		else
		if rand = -1
			rand4 += 1
		else
		if rand = 1
			rand5 += 1
		else
		if rand = 2
			rand6 += 1
		else
		if rand = 3
			rand7 += 1
		else
		if rand = 4
			rand8 += 1
		else
		if rand = 5
			rand9 += 1
		else
			rand_bad += 1
	}
	msgbox, Distribution of randomly generated numbers between -4 and 5`n0: %rand0%`n1: %rand1%`n2: %rand2%`n3: %rand3%`n4: %rand4%`n5: %rand5%`n6: %rand6%`n7: %rand7%`n8: %rand8%`n9: %rand9%`n`nOut-of-bounds randoms: %rand_bad%


	found = n
	loop, 100
	{
		random, rand, 0, 2147483647
		if rand > 1547483647
			found = y
	}
	if found = n
		MsgBox, possible randomness problem.

	found = n
	loop, 100
	{
		random, rand, -2147483647, 2147483647
		if rand < -1547483647
			found = y
	}
	if found = n
		MsgBox, possible randomness problem.
}



msgbox, 4, , do the create/removedir test?
ifmsgbox, yes
{
	; Dirs can be created in working dir (i.e. relative path):
	target_dir = TEST-FileCreateDir\SubDir
	filecreatedir %target_dir%
	if ErrorLevel <> 0
		msgbox, problem creating dir
	;sleep, 2000
	fileremovedir, %target_dir%
	if ErrorLevel <> 0
		msgbox, problem removing child dir
	stringgetpos, last_backslash, target_dir, \, r
	stringleft, parent_dir, target_dir, %last_backslash%
	fileremovedir, %parent_dir%
	if ErrorLevel <> 0
		msgbox, problem removing parent dir
}


ListVars
MsgBox, done
ExitApp


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


ClipSaveAndRestore:
ClipOrig = %Clipboard%
ClipSaved = %ClipboardAll%
ClipSaved2 = %ClipSaved%  ; To add more to the test.
Clipboard =  ; just for testing.
Clipboard = %ClipSaved2%

ClipSaved2 = 123  ; To test that var's binary_clip flag gets reset.
if ClipSaved2 <> 123
	MsgBox problem.

if Clipboard <> %ClipOrig%
{
	StringLen, ClipLen, Clipboard
	StringLen, ClipOrigLen, ClipOrig
	ListVars  ; To check length of Saved.
	MsgBox #1`nOrig Length %ClipOrigLen% <> New Length %ClipLen%`nOrig: "%ClipOrig%"`nis not`nNew: "%Clipboard%".
}

; Test := operator:
ClipOrig := Clipboard
ClipSaved := ClipboardAll
ClipSaved2 := ClipSaved  ; To add more to the test.
Clipboard :=  ; just for testing.
Clipboard := ClipSaved2

if (Clipboard <> ClipOrig)
{
	StringLen, ClipLen, Clipboard
	StringLen, ClipOrigLen, ClipOrig
	ListVars  ; To check length of Saved.
	MsgBox #2`nOrig Length %ClipOrigLen% <> New Length %ClipLen%`nOrig: "%ClipOrig%"`nis not`nNew: "%Clipboard%".
}

; And again with dynamic variables:
ClipOrig = %Clipboard%
DynClipboardAll = ClipboardAll  ; Extra testing.
ClipSaved := %DynClipboardAll%  ; Extra testing.
DynClipSaved = ClipSaved
ClipSaved2 := %DynClipSaved%  ; To add more to the test.
Clipboard =  ; just for testing.
DynClipSaved2 = ClipSaved2
Clipboard := %DynClipSaved2%

if Clipboard <> %ClipOrig%
{
	StringLen, ClipLen, Clipboard
	StringLen, ClipOrigLen, ClipOrig
	ListVars  ; To check length of Saved.
	MsgBox #3`nOrig Length %ClipOrigLen% <> New Length %ClipLen%`nOrig: "%ClipOrig%"`nis not`nNew: "%Clipboard%".
}
return


ClipSaveAndRestoreFromFile:
ClipOrig = %Clipboard%

FileAppend, %ClipboardAll%, ClipboardSaved.bin    ; #1
if ErrorLevel
{
	MsgBox FileAppend Error
	return
}

ClipSaved = %ClipboardAll%
FileAppend, %ClipSaved%, ClipboardSaved2.bin    ; #2
if ErrorLevel
{
	MsgBox FileAppend Error
	return
}

Clipboard =  ; just for testing.

FileRead, Clipboard, *c ClipboardSaved.bin    ; #1
if ErrorLevel
{
	MsgBox FileRead Error
	return
}

if Clipboard <> %ClipOrig%
{
	StringLen, ClipLen, Clipboard
	StringLen, ClipOrigLen, ClipOrig
	ListVars  ; To check length of Saved.
	MsgBox Original Length %ClipOrigLen% <> New Length %ClipLen%`nOrig:"%ClipOrig%"`nis not`nNew: "%ClipOrig%".
}

FileRead, Clipboard, *c ClipboardSaved2.bin    ; #2
if ErrorLevel
{
	MsgBox FileRead Error
	return
}

if Clipboard <> %ClipOrig%
{
	StringLen, ClipLen, Clipboard
	StringLen, ClipOrigLen, ClipOrig
	ListVars  ; To check length of Saved.
	MsgBox Original Length %ClipOrigLen% <> New Length %ClipLen%`nOrig:"%ClipOrig%"`nis not`nNew: "%ClipOrig%".
}

; Read into intermediate variable first:
FileRead, ClipSaved, *c ClipboardSaved2.bin    ; #2
if ErrorLevel
{
	MsgBox FileRead Error
	return
}
Clipboard := ClipSaved

if Clipboard <> %ClipOrig%
{
	StringLen, ClipLen, Clipboard
	StringLen, ClipOrigLen, ClipOrig
	ListVars  ; To check length of Saved.
	MsgBox Original Length %ClipOrigLen% <> New Length %ClipLen%`nOrig:"%ClipOrig%"`nis not`nNew: "%ClipOrig%".
}
return  ; from this subroutine.



ControlMoveTimer:
IfWinNotExist, ControlMoveTimer, , return
;WinMove, , -160
SetTimer, ControlMoveTimer, off
WinActivate
ControlMove, OK, 73, 160
Sleep, 500
ControlMove, OK, 10
Sleep, 500
ControlMove, OK, 73, 140
Sleep, 500
ControlMove, OK, , , 200, 40
Sleep, 500
WinClose
return



IndicateCompare:
comp =   ; Must init for successive calls to work.
if c1 = %c2%
	comp = =
if c1 <> %c2%
	comp = %comp%<>
if c1 < %c2%
	comp = %comp%<
if c1 <= %c2%
	comp = %comp%<=
if c1 > %c2%
	comp = %comp%>
if c1 >= %c2%
	comp = %comp%>=
return



displaymsgboxresult:
msgboxresult =
ifmsgbox, ok
	msgboxresult = %msgboxresult% + ok
ifmsgbox, cancel
	msgboxresult = %msgboxresult% + cancel
ifmsgbox, abort
	msgboxresult = %msgboxresult% + abort
ifmsgbox, retry
	msgboxresult = %msgboxresult% + retry
ifmsgbox, ignore
	msgboxresult = %msgboxresult% + ignore
ifmsgbox, yes
	msgboxresult = %msgboxresult% + yes
ifmsgbox, no
	msgboxresult = %msgboxresult% + no
ifmsgbox, timeout
	msgboxresult = %msgboxresult% + timeout
if msgboxresult =
	msgboxresult = msgbox result was not one of the allowed values.
msgbox, %msgboxresult%
return



RegTest:
; Caller must have set RegTestType and RegTestValue for us.
RegWrite, %RegTestType%, HKEY_CURRENT_USER, Software\AutoHotkey, TestValue, %RegTestValue%
if ErrorLevel <> 0
{
	msgbox, Failed to write %RegTestType% value "%RegTestValue%".
	exitapp
}
Loop, HKCU, Software\AutoHotkey, 1, 1
{
	if (A_LoopRegName = "TestValue")
	{
		RegRead, Value
		if (Value <> RegTestValue)
			MsgBox registry loop
		break
	}
}
RegRead, RegTestValueConfirm, HKEY_CURRENT_USER, Software\AutoHotkey, TestValue
if ErrorLevel <> 0
{
	msgbox, failed to read reg key %RegTestType% value
	exitapp
}
mismatch = n
if RegTestValue <> %RegTestValueConfirm%
{
	mismatch = y
	if RegTestType = REG_DWORD
		if RegTestValueConfirm = 0
			if RegTestValue =
					mismatch = n  ; It's not really a mismatch, since writing a blank really writes a zero.
}
if mismatch = y
{
		
	listvars
	msgbox, %RegTestType% value read from reg doesn't match what was written.  ; Don't display them, they might be large.
	exitapp
}
;if RegTestType = REG_MULTI_SZ
;	msgbox, %RegTestType% type was written and confirmed.  It will now be deleted.
RegDelete, HKEY_CURRENT_USER, Software\AutoHotkey, TestValue
if ErrorLevel <> 0
{
	msgbox, failed to delete reg key %RegTestType% value
	exitapp
}
return



#F8::
FileSelectFile, file1
ifnotinstring, file1, ddd
	msgbox, you entered "%file1%" not ddd for filename or there was a problem.
return

#F10::
InputBox, Inputy, Enter the string abc
if inputy <> abc
	msgbox, you didn't enter abc or there was a problem.
return

#F12::
;errorlevel = 0
;msgbox, errorlevel = %errorlevel%
msgbox, 4, , press No
ifmsgbox, yes
	msgbox, you didn't press no`, or there was a problem
return


;^!k::KeyHistory
;#k::KeyHistory
;^+k::KeyHistory
;^!l::ListLines
;^!v::ListVars
;^!h::ListHotkeys


SendCmdHotkeys:
#8::
^+8::
^8::  ; Not ^!8 because the keys sent below include a combo that's ctrl-alt-del, which invokes Task Manager.

SetKeyDelay, 1
#HotkeyModifierTimeout 100

; Test {ASC nnnn}
clipboard =
;send, A+{home}^c
send, {ASC 65}+{home}^c
ClipWait, 1
if clipboard <> A
	msgbox clipboard should have been A but is %clipboard%

; Test {alt/shift/ctrl down/up}
Send, {altdown}e{altup}c{end}!ep{end}{shiftdown}{home}{shiftup}

clipboard =
; Test on separate lines to make sure it's persistent between commands.
; NOTE: By design, this part will not work if one of the hotkey modifiers is the CONTROL key,
; since the program does not track "persisent" (e.g. CtrlDown) modifiers from one SEND to the
; next (i.e. it maintains them only within the same send cmd).  In other words, it doesn't
; puts CTRL back up for the C event because it thinks CTRL is just a physical modifier, since
; the user is in fact holding it down physically.
;Send, {ctrldown}
;Send, c
;Send, {ctrlup}
Send, {ctrldown}c{ctrlup}

clipwait, 1
Send, {enter}bogus info{enter}
Send, ^{home}^+{end}{del}  ; clear all text
if clipboard <> AA
	msgbox clipboard should have been AA
return

;#InstallKeybdHook   ; just a test comment



TimerSpeedTest:
if TimerSpeedLastTick =
{
	TimerSpeedProblemCount = 0
	TimerSpeedLastTick = %a_tickcount%
	return
}
TimerSpeedElapsed = %a_tickcount%
TimerSpeedElapsed -= %TimerSpeedLastTick%
if TimerSpeedElapsed > %TimerPeriodAllowed%
{
	++TimerSpeedProblemCount
	SplashTextOn, , , %TimerSpeedElapsed%
}
TimerSpeedLastTick = %a_tickcount%
return



ChkLen(ByRef str)
{
	true_len := DllCall("lstrlen", str, str)
	StringLen, L, str
	if (L <> true_len || StrLen(str) <> true_len)
		MsgBox ChkLen() problem.
}

Error:
ListLines
MsgBox Problem. Var = "%Var%"
return
