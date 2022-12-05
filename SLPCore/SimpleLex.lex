%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}* 
CONSTSTRING \"[^\"]*\"
%%

{INTNUM} { 
  yylval.sVal = yytext; 
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  yylval.sVal = yytext; 
  return (int)Tokens.RNUM;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

{CONSTSTRING} {
	yylval.sVal = yytext;
	return (int)Tokens.CONSTSTR;
}

":=" { return (int)Tokens.ASSIGN;		}
"==" { return (int)Tokens.EQUALEQUAL;	}
"!=" { return (int)Tokens.BANGEQUAL;	}

">=" { return (int)Tokens.GREATEREQUAL;	}
"<=" { return (int)Tokens.LESSEREQUAL;	}
">" { return (int)Tokens.GREATER;		}
"<" { return (int)Tokens.LESSER;		}
"!" { return (int)Tokens.BANG;			}

";"  { return (int)Tokens.SEMICOLON;	}
"("  { return (int)Tokens.LBRACKET;		}
")"  { return (int)Tokens.RBRACKET;		}
","  { return (int)Tokens.PERIOD;		}
"+"  { return (int)Tokens.PLUS;			}
"-"  { return (int)Tokens.MINUS;		}
"*"  { return (int)Tokens.STAR;			}
"/"  { return (int)Tokens.SLASH;		}
":"  { return (int)Tokens.COLON;		}


%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): Встречено {2}, а ожидалось {3}", yyline, yycol, args[0], string.Join(" или ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
  string errorMsg = string.Format("({0},{1}): Неизвестный символ {2}", yyline, yycol, yytext);
  throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,Tokens> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,Tokens>();
    keywords.Add("begin",	Tokens.BEGIN);
    keywords.Add("end",		Tokens.END);
    keywords.Add("cycle",	Tokens.CYCLE);
	keywords.Add("while",	Tokens.WHILE);
	keywords.Add("do",		Tokens.DO);
	keywords.Add("repeat",	Tokens.REPEAT);
	keywords.Add("until",	Tokens.UNTIL);
	keywords.Add("for",		Tokens.FOR);
	keywords.Add("to",		Tokens.TO);
	keywords.Add("write",	Tokens.WRITE);
	keywords.Add("if",		Tokens.IF);
	keywords.Add("then",	Tokens.THEN);
	keywords.Add("else",	Tokens.ELSE);
	keywords.Add("var",		Tokens.VAR);
	keywords.Add("or",		Tokens.OR);
	keywords.Add("and",		Tokens.AND);
	keywords.Add("as",		Tokens.AS);
	keywords.Add("true",	Tokens.TRUE);
	keywords.Add("false",	Tokens.FALSE);
	keywords.Add("millitime",Tokens.MILLITIME);
  }
  public static int GetIDToken(string s)
  {
	if (keywords.ContainsKey(s.ToLower()))
	  return (int)keywords[s];
	else
      return (int)Tokens.ID;
  }
  
}
