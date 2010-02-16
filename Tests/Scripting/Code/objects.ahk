a = b
b :={ "one" : 1, two=2
	,	"three" =["pass", 4,{ "x", "1:=" = "!", "a": "b{c}"}] }
b["three"][2][1] = o

if (b.one != 1)
	FileAppend, fail, *

if (%a%["tw" . b.three[2][0]] != 2)
	FileAppend, fail, *

if ({ a : { b : "c" } }.a["b"] != "c")
	FileAppend, fail, *

if (([1,2,3][1] += 3) != 5)
	FileAppend, fail, *

FileAppend, % %a%.b["three"][0], *
