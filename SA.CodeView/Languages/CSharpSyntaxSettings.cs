﻿using System;
using System.Collections.Generic;
using System.Text;
using SA.CodeView.ParsingOnElements;
using System.Drawing;
using SA.CodeView.Parsing;

namespace SA.CodeView.Languages
{
	class CSharpSyntaxSettings : SyntaxSettings
	{
		//=========================================================================================
		public CSharpSyntaxSettings()
		{
			this.Operators = new char[] { '=', '+', '-', '/', '*', '%', '>', '<', '&', '|', '^', '~', '(', ')', '[', ']', ',' };
		}
		//=========================================================================================
		protected override List<SA.CodeView.ParsingOnElements.BaseElement> CreateElements()
		{
			var elements = new List<BaseElement>();
			return elements;
		}
		//=========================================================================================
		protected override Dictionary<string, TextStyle> CreateTextStyles()
		{
			var styles = base.CreateTextStyles();
			this.AddStyle(styles, new TextStyle(S_OPERATORS, Color.Black));
			this.AddStyle(styles, new TextStyle(S_STRINGS, Color.Maroon));
			this.AddStyle(styles, new TextStyle(S_MULTI_COMMENT, Color.Green));
			this.AddStyle(styles, new TextStyle(S_SINGLE_COMMENT, Color.Green));
			this.AddStyle(styles, new TextStyle(S_IDENTIFIER, Color.Black));
			this.AddStyle(styles, new TextStyle(S_NUMBERS, Color.Black));
			this.AddStyle(styles, new TextStyle(S_KEYWORDS_1, Color.Blue, true));
			return styles;
		}
		//=========================================================================================
		internal override ScannerSpecification CreateScannerSpecification()
		{
			var oSpec = new ScannerSpecification();
			oSpec.AddLiteral("l", CharType.Letters, '_');
			oSpec.AddLiteral("d", CharType.Numbers);
			oSpec.AddLiteral("minus", '-');
			oSpec.AddLiteral("asterisk", '*');
			oSpec.AddLiteral("slash", '/');
			oSpec.AddLiteral("backSlash", '\\');
			oSpec.AddLiteral("operators", '=', '>', '<', ';', ',', '.', '+', ')', '(', '|', '&');
			oSpec.AddLiteral("singleQuote", '\'');
			oSpec.AddLiteral("doubleQuote", '"');
			oSpec.AddLiteral("caretReturn", '\n');

			oSpec.AddTokenDeclaration("id", "l{l|d}");
			oSpec.AddTokenDeclaration("number", "d{d}");
			oSpec.AddTokenDeclaration("operator", "(operators|minus)");
			oSpec.AddBoundedToken("charConst", "singleQuote", "singleQuote", "backSlash");
			oSpec.AddBoundedToken("comment1", "slash asterisk", "asterisk slash", null);
			oSpec.AddBoundedToken("comment2", "slash slash", "caretReturn", null);
			oSpec.AddBoundedToken("stringConst", "doubleQuote", "doubleQuote", "backSlash");

			return oSpec;
		}
		//=========================================================================================
		protected override void FillKeywordGroups(Dictionary<string, TextStyle> dictionary)
		{
			base.FillKeywordGroups(dictionary);
			TextStyle oStyle;

			///Ключевые слова первого эшелона
			oStyle = this.GetStyleByName(S_KEYWORDS_1);
			this.LoadKeywordsToGroup(dictionary, oStyle, @"
abstract	
as	
base	
bool

break	
byte	
case	
catch

char	
checked	
class	
const

continue	
decimal	
default	
delegate

do	
double	
else	
enum

event	
explicit	
extern	
false

finally	
fixed	
float	
for

foreach	
goto	
if	
implicit

in
int	
interface

internal	
is	
lock	
long

namespace	
new	
null	
object

operator	
out
override

params	
private	
protected	
public

readonly	
ref	
return	
sbyte

sealed	
short	
sizeof	
stackalloc

static	
string	
struct	
switch

this	
throw	
true	
try

typeof	
uint	
ulong	
unchecked

unsafe	
ushort	
using	
virtual

void	
volatile	
while



add 	
alias 	
ascending 

descending 	
dynamic 	
from 

get 	
global 	
group 

into 	
join 	
let 

orderby 	
partial

remove 	
select 	
set 

value 	
var

where
yield
");
		}
		//=========================================================================================
		internal override TextStyle GetStyleFor(Token token, string styleName)
		{
			if (token.TokenTypeName == "charConst")
				return this.TextStyles[S_STRINGS];

			return base.GetStyleFor(token, styleName);
		}
		//=========================================================================================
	}
}
