using BenchmarkDotNet.Attributes;
using SLPCore.AST;
using SLPCore.Operators;
using SLPCore.Types;
using SLPCore.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ClosureVM
{
    public delegate RuntimeValue ExpressionLambda(ClosureVM vm);
    public delegate void StatementLambda(ClosureVM vm);
    public class ClosureCompileVisitor : IASTExprVisitor<ExpressionLambda>, IASTStatementVisitor<StatementLambda>
    {
        TypeOfExpression typeTable;
        TypeNameTable typeNameTable;
        ConvertorTable convertorTable;
        FromInitTable initializers;
        VariableContext variableContext = new VariableContext();
        public List<string> constStringPool = new();

        public ClosureCompileVisitor(TypeOfExpression typeTable, TypeNameTable typeNameTable, ConvertorTable convertorTable, FromInitTable initializers)
        {
            this.typeTable = typeTable;
            this.typeNameTable = typeNameTable;
            this.convertorTable = convertorTable;
            this.initializers = initializers;
        }

        public StatementLambda AssingNodeVisit(AssignNode node)
        {
            var expr = node.Expr.Accept(this);
            var t = variableContext.GetLocalVariableNumber(node.Id.Name);
            return (x) => { x.localVariables[t] = expr(x); };
        }

        public ExpressionLambda BinaryOpNodeVisit(BinaryOpNode node)
        {
            var left = node.Left.Accept(this);
            var right = node.Right.Accept(this);
            (var typeL, var typeR) = (typeTable[node.Left], typeTable[node.Right]);
            if (typeL.Name == "f64" && typeR.Name == "f64")
            {
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 + right(x).f64 }; };
                    case BinaryOperations.Subtract:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 - right(x).f64 }; };
                    case BinaryOperations.Multiply:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 * right(x).f64 }; };
                    case BinaryOperations.Divide:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 / right(x).f64 }; };
                    case BinaryOperations.CompEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 == right(x).f64 }; };
                    case BinaryOperations.CompUnequal:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 != right(x).f64 }; };
                    case BinaryOperations.CompGreater:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 > right(x).f64 }; };
                    case BinaryOperations.CompLesser:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 < right(x).f64 }; };
                    case BinaryOperations.CompGreaterEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 >= right(x).f64 }; };
                    case BinaryOperations.CompLesserEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 <= right(x).f64 }; };
                    case BinaryOperations.LogicOr:
                    case BinaryOperations.LogicAnd:
                    default:
                        throw new NotImplementedException();
                }
            }
            if (typeL.Name == "i64" && typeR.Name == "f64")
            {
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        return (x) => { return new RuntimeValue() { f64 = ((double)left(x).i64) + right(x).f64 }; };
                    case BinaryOperations.Subtract:
                        return (x) => { return new RuntimeValue() { f64 = ((double)left(x).i64) - right(x).f64 }; };
                    case BinaryOperations.Multiply:
                        return (x) => { return new RuntimeValue() { f64 = ((double)left(x).i64) * right(x).f64 }; };
                    case BinaryOperations.Divide:
                        return (x) => { return new RuntimeValue() { f64 = ((double)left(x).i64) / right(x).f64 }; };
                    case BinaryOperations.CompEqual:
                        return (x) => { return new RuntimeValue() { @bool = ((double)left(x).i64) == right(x).f64 }; };
                    case BinaryOperations.CompUnequal:
                        return (x) => { return new RuntimeValue() { @bool = ((double)left(x).i64) != right(x).f64 }; };
                    case BinaryOperations.CompGreater:
                        return (x) => { return new RuntimeValue() { @bool = ((double)left(x).i64) > right(x).f64 }; };
                    case BinaryOperations.CompLesser:
                        return (x) => { return new RuntimeValue() { @bool = ((double)left(x).i64) < right(x).f64 }; };
                    case BinaryOperations.CompGreaterEqual:
                        return (x) => { return new RuntimeValue() { @bool = ((double)left(x).i64) >= right(x).f64 }; };
                    case BinaryOperations.CompLesserEqual:
                        return (x) => { return new RuntimeValue() { @bool = ((double)left(x).i64) <= right(x).f64 }; };
                    case BinaryOperations.LogicOr:
                    case BinaryOperations.LogicAnd:
                    default:
                        throw new NotImplementedException();
                }
            }
            if (typeL.Name == "f64" && typeR.Name == "i64")
            {
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 + ((double)right(x).i64) }; };
                    case BinaryOperations.Subtract:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 - ((double)right(x).i64) }; };
                    case BinaryOperations.Multiply:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 * ((double)right(x).i64) }; };
                    case BinaryOperations.Divide:
                        return (x) => { return new RuntimeValue() { f64 = left(x).f64 / ((double)right(x).i64) }; };
                    case BinaryOperations.CompEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 == ((double)right(x).i64) }; };
                    case BinaryOperations.CompUnequal:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 != ((double)right(x).i64) }; };
                    case BinaryOperations.CompGreater:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 > ((double)right(x).i64) }; };
                    case BinaryOperations.CompLesser:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 < ((double)right(x).i64) }; };
                    case BinaryOperations.CompGreaterEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 >= ((double)right(x).i64) }; };
                    case BinaryOperations.CompLesserEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).f64 <= ((double)right(x).i64) }; };
                    case BinaryOperations.LogicOr:
                    case BinaryOperations.LogicAnd:
                    default:
                        throw new NotImplementedException();
                }
            }
            if (typeL.Name == "i64" && typeR.Name == "i64")
            {
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        return (x) => { return new RuntimeValue() { i64 = left(x).i64 + right(x).i64 }; };
                    case BinaryOperations.Subtract:
                        return (x) => { return new RuntimeValue() { i64 = left(x).i64 - right(x).i64 }; };
                    case BinaryOperations.Multiply:
                        return (x) => { return new RuntimeValue() { i64 = left(x).i64 * right(x).i64 }; };
                    case BinaryOperations.Divide:
                        return (x) => { return new RuntimeValue() { i64 = left(x).i64 / right(x).i64 }; };
                    case BinaryOperations.CompEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).i64 == right(x).i64 }; };
                    case BinaryOperations.CompUnequal:
                        return (x) => { return new RuntimeValue() { @bool = left(x).i64 != right(x).i64 }; };
                    case BinaryOperations.CompGreater:
                        return (x) => { return new RuntimeValue() { @bool = left(x).i64 > right(x).i64 }; };
                    case BinaryOperations.CompLesser:
                        return (x) => { return new RuntimeValue() { @bool = left(x).i64 < right(x).i64 }; };
                    case BinaryOperations.CompGreaterEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).i64 >= right(x).i64 }; };
                    case BinaryOperations.CompLesserEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).i64 <= right(x).i64 }; };
                    case BinaryOperations.LogicOr:
                    case BinaryOperations.LogicAnd:
                    default:
                        throw new NotImplementedException();
                }
            }
            if (typeL.Name == "bool" && typeR.Name == "bool")
            {
                switch (node.Operation)
                {
                    case BinaryOperations.LogicOr:
                        return (x) => { return new RuntimeValue() { @bool = left(x).@bool || right(x).@bool }; };
                    case BinaryOperations.LogicAnd:
                        return (x) => { return new RuntimeValue() { @bool = left(x).@bool && right(x).@bool }; };
                    case BinaryOperations.CompEqual:
                        return (x) => { return new RuntimeValue() { @bool = left(x).@bool == right(x).@bool }; };
                    case BinaryOperations.CompUnequal:
                        return (x) => { return new RuntimeValue() { @bool = left(x).@bool != right(x).@bool }; };
                    case BinaryOperations.Add:
                    case BinaryOperations.Subtract:
                    case BinaryOperations.Multiply:
                    case BinaryOperations.Divide:
                    case BinaryOperations.CompGreater:
                    case BinaryOperations.CompLesser:
                    case BinaryOperations.CompGreaterEqual:
                    case BinaryOperations.CompLesserEqual:
                    default:
                        throw new NotImplementedException();
                }
            }
                throw new NotImplementedException();

        }

        public StatementLambda BlockNodeVisit(BlockNode node)
        {
            var rc = variableContext;
            this.variableContext = new VariableContext(rc);
            var statements = node.StList.Select((x) => x.Accept(this)).ToArray();
            this.variableContext = rc;
            return (x) =>
            {
                foreach (var stmt in statements)
                {
                    stmt(x);
                }
            };
        }

        public ExpressionLambda CastNodeVisit(CastNode node)
        {
            var from = typeTable[node.ExprNode].Name;
            var to = node.TypeNode.TypeName;
            var expr = node.ExprNode.Accept(this);
            switch (from)
            {
                case "i64":
                    switch (to)
                    {
                        case "i64":
                            return expr;
                        case "f64":
                            return (x) => { return new RuntimeValue() { f64 = (double)(expr(x).i64) }; };
                        default:
                            throw new NotImplementedException();
                    }
                case "f64":
                    switch (to)
                    {
                        case "i64":
                            return (x) => { return new RuntimeValue() { i64 = (long)(expr(x).f64) }; };
                        case "f64":
                            return expr;
                        default:
                            throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public ExpressionLambda ConstantNodeVisit(ConstantNode node)
        {
            var t = typeTable[node];
            switch (t.Name)
            {
                case "i64":
                    var vali = new RuntimeValue() { i64 = long.Parse(node.Value) };
                    return (x) => { return vali; };
                case "f64":
                    var valf = new RuntimeValue() { f64 = double.Parse(node.Value, System.Globalization.CultureInfo.InvariantCulture) };
                    return (x) => { return valf; };
                case "bool":
                    var valb = new RuntimeValue() { @bool = bool.Parse(node.Value) };
                    return (x) => { return valb; };
                case "conststr":
                    constStringPool.Add(node.Value.Substring(1, node.Value.Length - 2));
                    var vals = new RuntimeValue() { conststr = constStringPool.Count - 1 };
                    return (x) => { return vals; };
                default:
                    throw new NotImplementedException($"Type is not implemented: {t.Name}");
            }
        }

        public StatementLambda CycleNodeVisit(CycleNode node)
        {
            var expr = node.Expr.Accept(this);
            var stmt = node.Stat.Accept(this);
            return (x) =>
            {
                var max = expr(x).i64;
                for (long i = 0; i < max; i++)
                {
                    stmt(x);
                }
            };
        }

        public StatementLambda DecNodeVisit(DecNode node)
        {
            if (node.exprNodes.Count != 0)
            {
                var exprT = node.idNodes.Zip(node.exprNodes.Select(x => x.Accept(this)), (x, y) => (x, y)).ToArray();
                var decs = exprT.Select(x => (variableContext.AllocateNumber(x.x.Name), x.y)).ToArray();
                return (x) =>
                {
                    foreach (var dec in decs)
                    {
                        x.localVariables[dec.Item1] = dec.y(x);
                    }
                };
            }
            else
            {
                foreach (var decl in node.idNodes)
                {
                    variableContext.AllocateNumber(decl.Name);
                }
                return (x) => {};
            }
        }

        public StatementLambda ForNodeVisit(ForNode node)
        {
            var fr = node.ExprFrom.Accept(this);
            var to = node.ExprTo.Accept(this);
            var variable = variableContext.AllocateNumber(node.Id.Name);
            var st = node.Stat.Accept(this);
            return (x) =>
            {
                var f = fr(x).i64;
                var t = to(x).i64;
                for (long i = f; i < t; i++)
                {
                    x.localVariables[variable].i64 = i;
                    st(x);
                }
            };
        }

        public ExpressionLambda IdNodeVisit(IdNode node)
        {
            var t = variableContext.GetLocalVariableNumber(node.Name);
            return (x) => { return x.localVariables[t]; };
        }

        public StatementLambda IfNodeVisit(IfNode node)
        {
            var cond = node.CondExpr.Accept(this);
            var trueBranch = node.ExprMainBranch.Accept(this);
            var falseBranch = node.ExprMainBranch.Accept(this);
            if (falseBranch is not null)
            {
                return (x) => {
                    if (cond(x).@bool)
                    {
                        trueBranch(x);
                    }
                    else
                    {
                        falseBranch(x);
                    }
                };
            }
            else
            {
                return (x) => {
                    if (cond(x).@bool)
                    {
                        trueBranch(x);
                    }
                };
            }
        }

        public ExpressionLambda MilliTimeVisit(MilliTimeNode node)
        {
            return (x) => { return new RuntimeValue() { f64 = x.sw.ElapsedMilliseconds }; };
        }

        public StatementLambda RepeatNodeVisit(RepeatNode node)
        {
            var body = node.Block.Accept(this);
            var expr = node.Expr.Accept(this);
            return (x) =>
            {
                do
                {
                    body(x);
                } while (!expr(x).@bool);
            };
        }

        public ExpressionLambda TypeNodeVisit(TypeNode node)
        {
            throw new NotImplementedException();
        }

        public ExpressionLambda UnaryOpNodeVisit(UnaryOpNode node)
        {
            var expr = node.Expr.Accept(this);
            switch (node.Operation)
            {
                case UnaryOperations.Negate:
                    switch (typeTable[node.Expr].Name)
                    {
                        case "i64":
                            return (x) => { return new RuntimeValue() { i64 = -expr(x).i64 }; };
                        case "f64":
                            return (x) => { return new RuntimeValue() { f64 = -expr(x).f64 }; };
                        default:
                            throw new NotImplementedException();
                    }
                case UnaryOperations.LogicalNot:
                    return (x) => { return new RuntimeValue() { @bool = !expr(x).@bool }; };
                default:
                    throw new NotImplementedException();
            }
        }

        public StatementLambda WhileNodeVisit(WhileNode node)
        {
            var expr = node.Expr.Accept(this);

            var body = node.Stat.Accept(this);
            return (x) =>
            {
                while(expr(x).@bool)
                {
                    body(x);
                };
            };
        }

        public StatementLambda WriteNodeVisit(WriteNode node)
        {
            var t = typeTable[node.Expr];
            var expr = node.Expr.Accept(this);
            switch (t.Name)
            {
                case "i64":
                    return (x) => { Console.WriteLine(expr(x).i64); };
                case "f64":
                    return (x) => { Console.WriteLine(expr(x).f64); };
                case "bool":
                    return (x) => { Console.WriteLine(expr(x).@bool); };
                case "conststr":
                    return (x) => { Console.WriteLine(x.constStringPool[(int)expr(x).conststr]); };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
