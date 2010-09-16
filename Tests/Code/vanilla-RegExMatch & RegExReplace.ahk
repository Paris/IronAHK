#Include %A_ScriptDir%/header.ahk

#NoEnv
SetBatchLines -1

RegExMatch("abc", "abc(")  ; Kept DELIBERATELY with a syntax error to ensure stability of the cache (bug-fix in 1.0.45.03).

VarSetCapacity(bigstr, 200000)  ; For performance
Loop 10000
	bigstr = %bigstr%0123`r`nabc`r`n789`r`n
StringReplace, bigstr_repl, bigstr, abc`r`n, XYZ456`r`n, UseErrorLevel
ReplCount := ErrorLevel

newstr := RegExReplace(bigstr, "^a.c$", "XYZ456", count)  ; Not found because multiline option is absent.
if count <> 0
	MsgBox Count %count% should have been 0.
if (newstr <> bigstr)
	MsgBox newstr should have been the same as bigstr.
newstr := RegExReplace(bigstr, "m)^a.c$", "XYZ456", count)  ; Match found due to 'm' option.
if (count <> ReplCount)
	MsgBox Count %count% should have been %ReplCount%.
if (newstr <> bigstr_repl)
	MsgBox newstr should have been the same as bigstr_repl.
newstr := RegExReplace(bigstr, "m)^[0-3]*`r`na.c`r`n[7-9]*`r`n", "", count)
if (newstr <> "")
	msgbox newstr was supposed to be empty.
if count <> 10000
	msgbox count was supposed to be 10000

; BUG FIX FOR v1.0.47.05:
ref:="5641-1139732532"
regexmatch(ref, "(1-\d{9,10})", ref)
if (ref <> "1-1139732532" || ref1 <> "1-1139732532")
	MsgBox v1.0.47.05
ref1:="5641-1139732532"
regexmatch(ref1, "((1-)\d{9,10})", ref)
if (ref <> "1-1139732532" || ref1 <> "1-1139732532" || ref2 <> "1-")
	MsgBox v1.0.47.05
refnamed:="5641-1139732532"
regexmatch(refnamed, "(?P<named>(1-)\d{9,10})", ref)
if (ref <> "1-1139732532" || refnamed <> "1-1139732532" || ref2 <> "1-")
	MsgBox v1.0.47.05
ref := "xyz"
regexreplace(ref, "xyz", "abc", ref)
if (ref <> 1)
	MsgBox %ref%


; TEST REPLACE():
testR(1, "", "", "", "")  ; Pretty obscure, but 1 does seem to be the correct number of replacements.
testR(3, "xxx", "abc", ".", "x")
testR(2, "xx", "abc", ".*", "x")  ; Confirmed correct by http://www.regextester.com. Explanation? Replaces abc by x, then the empty string at the end with x.
testR(1, "x", "abc", ".*", "x", 1)  ; Same, but limit to 1 replacement.
testR(5, "zzyzzyz", "xyxy", "x?", "z")  ; Confirmed.  Explanation: Replaces x by z, then the empty string before y by z.  It also replaces the empty string at the end of haystack to z.
testR(2, "zzyxy", "xyxy", "x?", "z", 2)  ; Same but limit to 2 replacements.
testR(5, "bbbbbbbbbb", "aaaaa", "a", "bb")  ; Replace small with larger.
testR(5, "aaaaa", "bbbbbbbbbb", "bb", "a")  ; Converse.
testR(5, "bbbbb", "aaaaa", "a", "b")
testR(3, "bbbaa", "aaaaa", "a", "b", 3)  ; Limit the number of replacements.
testR(0, "aaaaa", "aaaaa", "a", "b", 0)
testR(4, "aaabaca", "abc", "", "a")  ; Confirmed correct by http://www.regextester.com
testR(1, "azc", "abc", "b", "z")
	; TEST PCRE_NEWLINE_LF, PCRE_MULTILINE, and related
testR(0, "123`r`nabc`r`n789", "123`r`nabc`r`n789", "^[0-9]*$", "xxx")   ; Not found due to anchoring.
testR(2, "xxx`r`nabc`r`nxxx", "123`r`nabc`r`n789", "m)^[0-9]*$", "xxx") ; Found because now anchoring sees the newlines.
testR(2, "xxx`r`nabc`r`nxxx", "123`r`nabc`r`n789", "m`a)^[0-9]*$", "xxx") ; Same but in NEWLINE_ANY mode.
; testR(3, "xxx`r`nabc`rxxx`nxxx", "123`r`nabc`r`n789", "m`a)^[0-9]*$", "xxx") ; Same as above but valid only for PCRE 7.0 (hence it's now commented out).
testR(0, "a`r`n", "a`r`n", "m`a)^[0-9]*$", "xxx")
; testR(1, "a`rxxx`n", "a`r`n", "m`a)^[0-9]*$", "xxx") ; Same as above but valid only for PCRE 7.0 (hence it's now commented out).
testR(1, "xxx`r`n", "`r`n", "m`a)^[0-9]*$", "xxx")
testR(1, "xxx`r`n", "`r`n", "m`a)^.*$", "xxx")
; Old/wrong methods, so now commented out in favor of above:
; testR(2, "xxx`rxxx`n", "`r`n", "m`a)^[0-9]*$", "xxx")
; testR(2, "xxx`rxxx`n", "`r`n", "m`a)^.*$", "xxx")
testR(2, "xxx`nabc`nxxx", "123`nabc`n789", "m`n)^[0-9]*$", "xxx") ; Same but with LF vs. CRLF.
testR(2, "xxx`rabc`rxxx", "123`rabc`r789", "m`r)^[0-9]*$", "xxx") ; Same but with CR vs. CRLF.
	; TEST THINGS THAT AREN'T QUITE BACKREFERENCES
testR(1, "abc$", "abc", "abc", "abc$")
testR(1, "abc$", "abc", "abc", "abc$$")
testR(1, "abc${}${5}${", "abc", "abc", "abc$${}${$}$${5}${")
testR(1, "a$xbc${xx", "abc", "abc", "a$xbc${xx")  ; Unclosed braces are transcribed literally.
testR(1, "abc", "abc", "abc", "abc${-5}")  ; Negative or out of bounds treated as blank.
testR(1, "abc", "abc", "abc", "abc${99}")  ; Same.
	; TEST NUMBERED BACKREFERENCES
testR(1, "abcabc", "abc", "abc", "abc${0}")
testR(1, "xyz123abcx|abc123xyz|abc123xyz", "abc123xyz", "([a-z]+)([0-9]+)([a-z]+)", "$3${2}$1$9${77}x|$0|${0}")
testR(1, "XYZ123Abcx", "abc123xyz", "([a-z]+)([0-9]+)([a-z]+)", "$U3$L{2}$T1$9${77}x") ; Uses the U/L/T case transformations.
testR(1, "PhiLho", "Philippe Lhoste", "^(\w{3})\w*\s+\b(\w{3})\w*$", "$1$2")
	; TEST NAMED BACKREFERENCES
testR(1, "123|badc|89", "123abcd89", "(a)(?P<x>b)(c)(?P<y>d)", "|${x}$1${y}$3${bogus}|")
	; TEST OPTION 's' (PCRE_DOTALL) (note: CRLF requires two dots to match when dotall is in effect).
testR(1, "abc123x", "abc123`r`ndef", "s)..def", "x")  ; With dot-all
testR(0, "abc123`r`ndef", "abc123`r`ndef", "..def", "x")  ; Without it (not found, so not replacment).
	; TEST STARTINGPOS
testR(2, "abcdxx", "abcdefgefg", "efg", "x", -1, 5)
testR(1, "abcdefgx", "abcdefgefg", "efg", "x", -1, 6)  ; Starts one too far to the right to find the first match.
testR(0, "abcdefgefg", "abcdefgefg", "efg", "x", -1, 9)  ; None found.
testR(1, "abc`nxyz`n", "abc`n`n", "m`n)^$", "xyz")  ; As documented, in multiline mode circumflex (^) does not match after a newline at the very end of the string.
testR(1, "abc`r`nxyz`r`n", "abc`r`n`r`n", "m`a)^$", "xyz")  ; Similar to above. NOTE: As documented, in multiline mode circumflex (^) does not match after a newline at the very end of the string.
testR(1, "`rx", "`r`nA", "[`r`n]A", "x")  ; Possibly tests a change in PCRE 7.4.
testR(5, "xxxBxCx", "ABC", "Z*|A", "x")  ; Tests a bug fixed in v1.0.48.04.
testR(3, "123--678-90", "1234567890", "123\K|678\K|45", "-")  ; Similar to above.
testR(2, "123-456-67890", "1234567890", "123\K|456\K" , "-")  ; Pending fix; need new option "PCRE_NOTEMPTY_ATSTART" coming in PCRE 8.0.

; TEST MATCH()
	; TEST BORDERLINE/SPECIAL CASES
testM(1, "", "", "")  ; Empty string found in itself at pos 1.
testM(1, "", "abc", "")  ; Empty string found at pos 1.
testM(3, "c", "abc", "[a-z]+", 3)
testM(0, "", "abc", "[a-z]+", 5)   ; Test StartingPos greater than length of string.
testM(4, "", "abc", "", 5)  ; Finds empty string though?
testM(1, 0, "abc", "P)")  ; Position mode, which should yield 0 for length of main pattern.
	; TEST OPTIONS PARSING, BORDERLINE CASES
testM(1, "abc", "abc", ")abc")  ; Empty string found in itself at pos 1.
testM(4, "i)abc", "abci)abc", "i[)]abc")  ; Empty string found in itself at pos 1.
testM(4, "i)abc", "abci)abc", "i\)abc")  ; Empty string found in itself at pos 1.
	; TEST OPTION 'i' (PCRE_CASELESS)
testM(4, "aBc", "123aBc789", "i)abc")
testM(0, "", "123aBc789", "abc")  ; Counterpoint to above.
testM(4, "3", "123aBc789", "iP)abc")  ; Position mode.
	; TEST OPTION 'm' (PCRE_MULTILINE)
testM(6, "abc", "123`r`nabc`r`n789", "m)^abc$")
testM(0, "", "123`nabc`r`n789", "^abc$")  ; Counterpoint to above (i.e. no multiline)
testM(6, "abc", "123`r`nabc`r`n789", "m)^abc$", 6)
testM(0, "", "123`r`nabc`r`n789", "m)^abc$", 7)  ; Not found if StartingPos a little too far right.
testM(6, 3, "123`r`nabc`r`n789", "mP)^abc$")  ; Position mode
testM(0, "", "x`n", "`nm)^$")  ; As documented, in multiline mode circumflex (^) does not match after a newline at the very end of the string.
	; TEST OPTION 's' (PCRE_DOTALL) (note: CRLF requires two dots to match when dotall is in effect).
testM(0, "", "123`r`nabc`r`n789", "123..abc")  ; First with no dot-all.
testM(1, "123`r`nabc", "123`r`nabc`r`n789", "s)123..abc")  ; Same but with dot-all.
testM(1, "123`nabc", "123`nabc`n789", "123.abc")  ; Now with dot-all & LF (works with or without the s) because `n isn't a valid newline char.
testM(1, "7", "123`nabc`n789", "sP)123.abc")  ; Position mode.
testM(0, "", "`r`n", "^.+$")  ; No match because ^-anchor requires a match at beginning, but dot can't match `r or `n without dot-all.
testM(1, "`r`n", "`r`n", "s)^.+$")  ; Dot-all achieves a match.
	; TEST OPTION 'A' Anchored.
testM(0, "", "123aBc789", "A)aBc")
testM(1, "123", "123aBc789", "A)123")
	; TEST OPTION `n (PCRE_NEWLINE_LF) and related:
testM(1, "123", "123`r`nabc`r`n789", "m)^123$")
testM(1, "123", "123`nabc`n789", "m`n)^123$")
testM(0, "", "123`nabc`n789", "m)^123$")  ; Not found because wrong NEWLINE chars.
testM(0, "", "123`r`nabc`r`n789", "`nm)^123$") ; Same.
testM(1, "123`t`r", "123`t`r`nabc`r`n789", "m`n)^123`t`r$")  ; Variation.
	; TEST OPTION `a (PCRE_NEWLINE_ANY):
testM(1, "123", "123`r`nabc`r`n789", "m`a)^123$")
testM(1, "123", "123`nabc`n789", "m`a)^123$")
testM(6, "abc", "123`r`nabc`n789", "m`a)^abc$")  ; Mixture of LF & CRFL.
testM(5, "abc", "123`nabc`r`n789", "m`a)^abc$")  ; Converse.
testM(1, "123`t`r", "123`t`r`nabc`r`n789", "m`a)^123`t`r$")  ; Variation.
testM(1, "123`r`nabc", "123`r`nabc`n789", "sm`a)^123..abc$") ; Two dots are still required to match CRLF even in "ANY" mode.
testM(0, "", "123`nabc`n789", "`a)123.abc")  ; Not found because dot doesn't match `n without s option.
testM(1, "123`nabc", "123`nabc`n789", "s`a)123.abc")  ; Same but with s option.
	; TEST NAMED SUBPATTERNS
testM(1, "20061021", "20061021", "(?P<Year>\d{4})(?P<Month>\d{2})(?P<Day>\d{2})")
testM(1, "20061021", "20061021", "(?P<Year>[[:digit:]]{4})(?P<Month>\d{2})(?P<Day>\d{2})")
RegExMatch("20061021", "(?P<Year>\d{4})(?P<Month>\d{2})(?P<Day>\d{2})", Array) ; Same but test that the output array is created properly.
	if (ArrayYear != "2006" ||  ArrayMonth != "10" || ArrayDay != "21")
		MsgBox Match(): Named subpat problem.
	if (Array1 <> "" || Array2 <> "" || Array3 <> "" || Array4 <> "")
		MsgBox Match() isn't supposed to create numeric array elements for named subpatterns.
RegExMatch("20061021", "P)(?P<Year>\d{4})(?P<Month>\d{2})(?P<Day>\d{2})", Array) ; Same but in position mode.
	if (   ArrayPosYear != 1 ||  ArrayPosMonth != 5 || ArrayPosDay != 7
		|| ArrayLenYear != 4 ||  ArrayLenMonth != 2 || ArrayLenDay != 2   )
		MsgBox Match(): Named subpat problem.
	; TEST OPTION 'x' (PCRE_EXTENDED)
; ...(NO CURRENT TESTS FOR THIS AND OTHER OPTIONS)
	; TEST GENERAL STUFF:
testM(7, "abc`t`r`n789", "123`t`r`nabc`t`r`n789", "abc`t`r`n.*$")
testM(7, "abc`t`r`n789", "123`t`r`nabc`t`r`n789", "abc\t\r\n.*$") ; Same as above but let PCRE escape needle via backslash.
testM(0, "", "123aBc789", "xyz")
testM(10, "aBc", "123aBc789aBc", "aBc$")
testM(6, "c789", "123aBc789", "(xyz)|([a-z]+)7(.)(x*)9", 1, "", "c", "8", "")
testM(6, "4", "123aBc789", "P)(xyz)|([a-z]+)7(.)(x*)9", 1, "0", "0", "6", "1")  ; Position mode.
testM(4, "abc", "xyzabc", "(?!.*xyz.*)abc")  ; Look-ahead assertion.
testM(5, "123", "xyz`v123", "`am)^\d+$")  ; Vertical-tab (chr11) recognized as a newline due to PCRE_NEWLINE_ANY vs. PCRE_NEWLINE_ANYCRLF.
testM(5, "123", "xyz`f123", "`am)^\d+$")  ; Similar to above but for formfeed.
testM(5, "123", "xyz" . chr(0x85) . "123", "`am)^\d+$")  ; Similar to above but for NEL (next-line, chr(0x85)
testM(1, "xyz" . "`r`n" . "123", "xyz" . "`r`n" . "123", "xyz\R\d+")  ; Similar but uses \R
testM(1, "xyz" . chr(0x85) . "123", "xyz" . chr(0x85) . "123", "xyz\R\d+")  ; Similar.
testM(0, "", "xyz" . chr(0x85) . "123", "(*BSR_ANYCRLF)xyz\R\d+")  ; Same but fails due to restricting it to only CR, LF, and CRLF.
testM(1, "xyz`r`n123", "xyz`r`n123", "(*BSR_ANYCRLF)xyz\R\d+")
testM(0, "", "xyz`v123", "`am)(*ANYCRLF)^\d+$")  ; Vertical-tab (chr11) not recognized due to ANYCRLF forcing CR, LF, and CRLF only.
testM(6, "123", "xyz`r`n123", "`am)(*ANYCRLF)^\d+$")

MsgBox Done



testR(aExpectedCount, aExpectedResult, aHaystack, aNeedle, aRepl, aLimit = -1, aStartPos = 1)
{
	static test_number
	++test_number

	ErrorLevel = Not Initialized  ; To catch bugs where it wasn't properly set by the command.
	actual_result := RegExReplace(aHaystack, aNeedle, aRepl, actual_count, aLimit, aStartPos)
	if ErrorLevel
	{
		MsgBox Replace() Test #%test_number%`nErrorLevel = "%ErrorLevel%"`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"`nReplacement = "%aRepl%"
		return  ; Show just one error per test.
	}
	if not (actual_result == aExpectedResult)
	{
		MsgBox Replace() Test #%test_number%`nActual result (%actual_result%) <> expected (%aExpectedResult%).`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"`nReplacement = "%aRepl%"
		return  ; Show just one error per test.
	}
	if (actual_count <> aExpectedCount)
	{
		MsgBox Replace() Test #%test_number%`nActual replacement count (%actual_count%) <> expected (%aExpectedCount%).`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"`nReplacement = "%aRepl%"
		return  ; Show just one error per test.
	}
	if (strlen(actual_result) <> strlen(aExpectedResult))  ; THIS CHECKS INTERNALLY-STORED LENGTH FOR CORRUPTION (but make the above test take precedence in case the length discrepancy is due merely to the two strings not being equal).
	{
		MsgBox Replace() Test #%test_number%`nActual length <> expected length.`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"`nReplacement = "%aRepl%"
		return  ; Show just one error per test.
	}
}


testM(aExpectedPos, aExpectedFoundStr, aHaystack, aNeedle, aOffset = 1
	, aSub1 = -1, aSub2 = -1, aSub3 = -1, aSub4 = -1)
{
	static test_number
	++test_number

	ErrorLevel = Not Initialized  ; To catch bugs where it wasn't properly set by the command.
	FoundPos := RegExMatch(aHaystack, aNeedle, match, aOffset)

	if ErrorLevel
	{
		MsgBox Test #%test_number%`nErrorLevel = "%ErrorLevel%"`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"
		return  ; Show just one error per test.
	}
	if (FoundPos <> aExpectedPos)
	{
		MsgBox Test #%test_number%`nFoundPos actual (%FoundPos%) <> expected (%aExpectedPos%).`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"
		return  ; Show just one error per test.
	}
	if not (aExpectedFoundStr == match)
	{
		MsgBox Test #%test_number%`nFoundStr actual (%match%) <> expected (%aExpectedFoundStr%).`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"
		return  ; Show just one error per test.
	}
	if RegExMatch(aNeedle, "[a-zA-z`r`n]*P")  ; Verify the SubN items as though they contain positions.
	{
		v = 1
		Loop 4
		{
			expected := aSub%A_Index%
			if (expected = -1)
				continue
			if mod(A_Index, 2)
				actual := matchPos%v%
			else
			{
				actual := matchLen%v%
				++v
			}
			if (actual <> expected)
				MsgBox Test #%test_number%`nSubstring #%A_Index% actual (%actual%) <> expected (%expected%).`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"
		}
	}
	else  ; Verify the SubN items as though they contain substrings that matched the subpatterns.
	{
		Loop 4
		{
			expected := aSub%A_Index%
			if (expected = -1)
				continue
			actual := match%A_Index%
			if (actual <> expected)
				MsgBox Test #%test_number%`nSubstring #%A_Index% actual (%actual%) <> expected (%expected%).`nHaystack = "%aHaystack%"`nNeedle = "%aNeedle%"
		}
	}
}
