type
  Res = record
    f: double;
    i: int64;
    constructor Create(flt: double);
    begin
      f := flt;
    end;
    constructor Create(int: int64);
    begin
      i := int;
    end;
  end;

type
  Context = class
    locals: array [0..1023] of Res;
  end;

type expression = Context -> Res;
type statement = Context -> ();

function EmitFloatConst(f : double):expression;
begin
  Result:= function (c) -> begin Result := new Res(f); end;
end;

function EmitIntegerConst(i : int64):expression;
begin
  Result:= function (c) -> begin Result := new Res(i); end;
end;

function EmitLocalVariableGet(v: shortint):expression;
begin
  Result:= function (c) -> begin Result := c.locals[v];  end;
end;

function EmitLocalVariablePut(v: shortint; expr: expression):statement;
begin
  Result:= procedure (c) -> begin c.locals[v]:= expr(c) end;
end;

function EmitIntToFloat(expr: expression):expression;
begin
  Result:= function (c) -> begin Result.f := double(expr(c).i);    end;
end;

function EmitRepeatUntil(stmt: statement; cond:expression):statement;
begin
  Result := procedure (c) -> 
  begin
    repeat
      //Writeln(c.locals[1]);
      stmt(c);
    until not (cond(c).i = 0);
  end;
end;
function EmitCycle(repeats: expression; stmt: statement):statement;
begin
  Result := procedure (c) ->
  begin
    var t := repeats(c).i;
    loop t do
      stmt(c);
  end;
end;

function EmitCompareGreaterFloat(left: expression; right: expression):expression;
begin
  Result := function (c) ->
  begin
    if (left(c).f > right(c).f) then
      begin
      //Writeln('>');
      Result.i := 1;
      end
    else
      Result.i := 0;
  end;
end;

function EmitBlock(stmts: sequence of statement):statement;
begin
  var stmts_arr := stmts.ToArray();
  Result := procedure (c) -> 
  begin
    foreach var stmt in stmts_arr do
    begin
      stmt(c);
      end
  end;
end;
function EmitDivideFloatOnInt(left: expression; right: expression):expression;
begin
  Result := function (c) ->
  begin
    Result.f := left(c).f / right(c).i;
  end;
end;
function EmitMultiplyFloatOnInt(left: expression; right: expression):expression;
begin
  Result := function (c) ->
  begin
    Result.f := left(c).f * right(c).i;
  end;
end;
function EmitMillitimeFloat():expression;
begin
  Result := function (c) ->
  begin
    Result.f := Milliseconds();
  end;
end;
function EmitSubtractFloatOnFloat(left: expression; right: expression):expression;
begin
  Result := function (c) ->
  begin
    Result.f := left(c).f - right(c).f;
  end;
end;
function EmitSumFloatOnFloat(left: expression; right: expression):expression;
begin
  Result := function (c) ->
  begin
    Result.f := left(c).f + right(c).f;
  end;
end;
function EmitWriteFloat(expr: expression):statement;
begin
  Result := procedure (c) -> 
  begin
    Writeln(expr(c).f);
  end;
end;
begin
  // Index - Local variable name
  // 0        start
  // 1        a
  // 2        b
  // 3        endt
  var globalblock := new List<statement>();
  var line2 := EmitLocalVariablePut(0, EmitMillitimeFloat());
  globalblock.Add(line2);
  var line4 := EmitLocalVariablePut(1, EmitIntToFloat(EmitIntegerConst(2)));
  globalblock.Add(line4);
  var line5 := EmitLocalVariablePut(2, EmitIntToFloat(EmitIntegerConst(1)));
  globalblock.Add(line5);
  
  var repeatblock := new List<statement>();
  begin
    var line7 := EmitLocalVariablePut(1, EmitSumFloatOnFloat(EmitLocalVariableGet(1), EmitFloatConst(1.5))); 
    repeatblock.Add(line7);
    var line8 := EmitCycle(EmitIntegerConst(10), EmitLocalVariablePut(2, EmitDivideFloatOnInt(EmitMultiplyFloatOnInt(EmitLocalVariableGet(2), EmitIntegerConst(2)) , EmitIntegerConst(3))));  
    repeatblock.Add(line8);
  end;
  var line10 := EmitCompareGreaterFloat(EmitLocalVariableGet(1), EmitFloatConst(1000000));
  var repeatblock_stmt := EmitBlock(repeatblock);
  var line6 := EmitRepeatUntil(repeatblock_stmt, line10);
  globalblock.Add(line6);
  var line11 := EmitLocalVariablePut(3, EmitMillitimeFloat());
  globalblock.Add(line11);
  //var line12 := EmitWriteFloat(EmitSubtractFloatOnFloat(EmitLocalVariableGet(3),EmitLocalVariableGet(0)));
  //globalblock.Add(line12);
  var globalblock_stmt := EmitBlock(globalblock);
  
  
  var start := Milliseconds();
  var c := new Context();
  globalblock_stmt(c);
  var deltaStart := Milliseconds() - start;
  Println('Delta time:', deltaStart, 'ms');
end.

// Delta time: 1164 ms 
// Delta time: 1182 ms 
// Delta time: 1179 ms 
// Delta time: 1187 ms 
