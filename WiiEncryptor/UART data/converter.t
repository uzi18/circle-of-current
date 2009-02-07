var f := 0
fcn getWord (dataStream : int) : string
    var ch, lastCh : string := ""
    var returnStr := ""
    loop
	lastCh := ch
	get : dataStream, ch : 1
	if ch = "x" and lastCh = "0" then
	    returnStr := "0x"
	    get : dataStream, ch : 1
	    returnStr += ch
	    exit when eof (dataStream)
	    get : dataStream, ch : 1
	    returnStr += ch
	    exit when eof (dataStream)
	    result returnStr
	end if
	exit when eof (dataStream)
    end loop
    result returnStr
end getWord

proc convert (n : int)
    var dataIn, dataOut := 0
    var wordIn := ""
    var newLine := false
    open : dataIn, "Dump " + intstr (n) + ".csv", get
    open : dataOut, "Conv. Dump " + intstr (n) + ".csv", put
    if dataIn > 0 and dataOut > 0 then
	loop
	    wordIn := getWord (dataIn)

	    put : dataOut, wordIn ..
	    if (newLine) then
		put : dataOut, "\n" ..
	    else
		put : dataOut, "," ..
	    end if
	    newLine := not (newLine)
	    exit when eof (dataIn)
	end loop
    else
	f += 1
    end if
    close : dataIn
    close : dataOut
end convert
%convert (1)

var i : int := 1
loop
    if (File.Exists ("Dump " + intstr (i) + ".csv")) then
	convert (i)
    else
	exit
    end if
    i += 1
end loop
put (i - 1 - f) ..
put " Files converted"

