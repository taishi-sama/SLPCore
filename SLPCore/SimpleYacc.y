%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public DecNode dlVal;
			public TypeNode tVal;
			public List<IdNode> idsList;
			public List<ExprNode> exprList; 
       }

%using SLPCore.AST;
%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON WHILE DO REPEAT UNTIL FOR TO WRITE LBRACKET RBRACKET IF THEN ELSE PERIOD VAR
%token PLUS MINUS STAR SLASH EQUALEQUAL BANGEQUAL GREATEREQUAL LESSEREQUAL GREATER LESSER BANG AS COLON
%token OR AND TRUE FALSE MILLITIME
%token <sVal> ID, INUM, RNUM, CONSTSTR

%type <eVal> expr ident equality comparation term factor unary primary logic_or logic_and cast_as
%type <stVal> assign statement cycle while repeat for write condition 
%type <blVal> stlist block 
%type <dlVal> declare 
%type <tVal> typeInd
%type <idsList> idlist
%type <exprList> exprlist
%%


progr   : block { root = $1; }
		;

stlist	:
		statement SEMICOLON
		{ 
			$$ = new BlockNode(@$, $1); 
		}
		
		| stlist statement SEMICOLON
			{ 
				$1.Add($2); 
				$$ = $1; 
			}
		;
block	: BEGIN stlist END { $$ = $2; }
		;

statement: assign { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
		| while   { $$ = $1; }
		| repeat  { $$ = $1; }
		| for	  { $$ = $1; }
		| write	  { $$ = $1; }
		| condition { $$ = $1; }
		| declare { $$ = $1; }
		
	;

ident 	: ID { $$ = new IdNode(@$, $1); }	
		;

typeInd	: ID { $$ = new TypeNode(@$, $1); }
		;

idlist	: ident { $$ = new List<IdNode>();
					$$.Add($1 as IdNode); }
		| idlist PERIOD ident 
		{ 
			$1.Add($3 as IdNode);
			$$ = $1;
		}
		;

exprlist : expr {$$ = new List<ExprNode>();
					$$.Add($1);}
		 | exprlist PERIOD expr
		 {
			$1.Add($3);
			$$ = $1;
		 };
declare	: VAR idlist { $$ = new DecNode(@$, $2); }
		| VAR idlist COLON typeInd { $$ = new DecNode(@$, $2, $4); }
		| VAR idlist ASSIGN exprlist { $$ = new DecNode(@$, $2, $4); }
		;

assign 	: ident ASSIGN expr { $$ = new AssignNode(@$, $1 as IdNode, $3); }
		;

//expr 	: T				{$$ = $1;}
//		| expr PLUS T   {$$ = new BinaryOpNode(@$, $1, $3, "+");}
//		| expr MINUS T  {$$ = new BinaryOpNode(@$, $1, $3, "-");}
//		;
//
//T    	: F				{$$ = $1;}
//		| T STAR F		{$$ = new BinaryOpNode(@$, $1, $3, "*");}
//		| T SLASH F		{$$ = new BinaryOpNode(@$, $1, $3, "/");}
//		;
//
//F    	: ident			{ $$ = $1 as IdNode; }
//		| INUM			{ $$ = new IntNumNode(@$, $1); }
//		| LBRACKET expr RBRACKET {$$ = $2;}
//		;

expr	: cast_as		{$$ = $1;}
		;
cast_as : logic_or		{$$ = $1;} //TODO 
		| cast_as AS typeInd {$$ = new CastNode(@$, $1, $3);}
		;

logic_or
		: logic_and		{$$ = $1;}
		| logic_or	OR	logic_and {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.LogicOr);}
		;

logic_and
		: equality		{$$ = $1;}
		| logic_and	AND	equality {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.LogicAnd);}
		;

equality
		: comparation	{$$ = $1;}
		| equality BANGEQUAL comparation {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.CompUnequal);}
		| equality EQUALEQUAL comparation {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.CompEqual);}
		;

comparation
		: term {$$ = $1;}
		| comparation LESSER		term {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.CompLesser);}
		| comparation GREATER		term {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.CompGreater);}
		| comparation LESSEREQUAL	term {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.CompLesserEqual);}
		| comparation GREATEREQUAL	term {$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.CompGreaterEqual);}
		;

term	: factor {$$ = $1;}
		| term PLUS	factor	{$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.Add);}
		| term MINUS factor	{$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.Subtract);}
		;

factor	: unary {$$ = $1;}
		| factor STAR unary		{$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.Multiply);}
		| factor SLASH unary	{$$ = new BinaryOpNode(@$, $1, $3, BinaryOperations.Divide);}
		;

unary	: primary {$$ = $1;}
		| BANG unary {$$ = new UnaryOpNode(@$, $2, UnaryOperations.LogicalNot);}
		| MINUS unary {$$ = new UnaryOpNode(@$, $2, UnaryOperations.Negate);}
		;

primary : INUM		{ $$ = new ConstantNode(@$, $1, new TypeNode(@$,"i64")); }
		| RNUM		{ $$ = new ConstantNode(@$, $1, new TypeNode(@$,"f64")); }
		| CONSTSTR	{ $$ = new ConstantNode(@$, $1, new TypeNode(@$,"conststr")); }
		| TRUE		{ $$ = new ConstantNode(@$, "true", new TypeNode(@$, "bool")); }
		| FALSE		{ $$ = new ConstantNode(@$, "false", new TypeNode(@$, "bool")); }
		| MILLITIME { $$ = new MilliTimeNode(@$); }
		| ident		{ $$ = $1 as IdNode; }
		| LBRACKET expr RBRACKET {$$ = $2;}
		;


cycle	: CYCLE expr statement { $$ = new CycleNode(@$, $2, $3); }
		;

while	: WHILE expr DO statement { $$ = new WhileNode(@$, $2, $4); }
		;

repeat  : REPEAT stlist UNTIL expr { $$ = new RepeatNode(@$, $2, $4); }
		;
for		: FOR VAR ident ASSIGN expr TO expr DO statement { $$ = new ForNode (@$, $3 as IdNode, $5, $7, $9);}
		;
write	: WRITE LBRACKET expr RBRACKET { $$ = new WriteNode (@$, $3);}
		;
condition:  IF expr THEN statement ELSE statement {$$ = new IfNode(@$, $2, $4, $6);}   |
			IF expr THEN statement {$$ = new IfNode(@$, $2, $4);} 
			
		;
%%

