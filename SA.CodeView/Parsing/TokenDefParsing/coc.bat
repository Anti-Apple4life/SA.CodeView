@"..\..\Coco\Coco.exe" -frames "..\..\Coco" Grammar.atg -namespace SA.CodeView.Parsing.TokenDefParsing
@IF %ERRORLEVEL% == 0 GOTO SUCCESS
:SUCCESS
:END
@del Parser.cs.old
@del Scanner.cs.old
@pause
