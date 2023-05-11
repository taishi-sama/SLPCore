using SLPCore.AST;
using SLPCore;
using SLPCore.AST;
using SLPCore.Operators;
using SLPCore.Types;
using SLPCore.Visitors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SLPCore.Runtime
{
    public class RuntimeVisitor : IASTStatementVisitor<object>, IASTExprVisitor<RuntimeValue>, IASTTypeVisitor<TypeSLP>
    {
        Stopwatch stopwatch = new Stopwatch();
        RuntimeContext runtimeContext = new RuntimeContext();
        TypeOfExpression typeTable;
        TypeNameTable typeNameTable;
        ConvertorTable convertorTable;
        FromInitTable initializers;
        public bool WriteOut { get; set; } = true;
        public RuntimeVisitor(TypeOfExpression typeTable, TypeNameTable typeNameTable, ConvertorTable convertorTable, FromInitTable initializers)
        {
            this.typeTable = typeTable;
            this.typeNameTable = typeNameTable;
            this.convertorTable = convertorTable;
            this.initializers = initializers;
            stopwatch.Start();
        }

        public object AssingNodeVisit(AssignNode node)
        {
            var t = runtimeContext.GetRuntimeValue(node.Id.Name);
            if (t.langType is null)
                runtimeContext.SetVariable(node.Id.Name, node.Expr.Accept(this));
            else
                runtimeContext.SetVariable(node.Id.Name, convertorTable.ConvertTo(node.Expr.Accept(this), t.langType));
            return null;
        }

        public RuntimeValue BinaryOpNodeVisit(BinaryOpNode node)
        {
            
            var i64 = typeNameTable.GetLangType("i64");
            var f64 = typeNameTable.GetLangType("f64");
            var bool_t = typeNameTable.GetLangType("bool");
            var is_i64 = convertorTable.IsImplicitConvertable(typeTable[node.Left], i64) && convertorTable.IsImplicitConvertable(typeTable[node.Right], i64);
            switch (node.Operation)
            {
                case BinaryOperations.Add:
                case BinaryOperations.Subtract:
                case BinaryOperations.Multiply:
                case BinaryOperations.Divide:
                case BinaryOperations.CompEqual:
                case BinaryOperations.CompUnequal:
                case BinaryOperations.CompGreater:
                case BinaryOperations.CompLesser:
                case BinaryOperations.CompGreaterEqual:
                case BinaryOperations.CompLesserEqual:
                    var l = node.Left.Accept(this);
                    var r = node.Right.Accept(this);
                    if (is_i64)
                    {
                        var l_i64 = convertorTable.ConvertTo(l, i64);
                        var r_i64 = convertorTable.ConvertTo(r, i64);

                        switch (node.Operation)
                        {
                            case BinaryOperations.Add:
                                return new RuntimeValue(i64, (long)l_i64.value + (long)r_i64.value);
                            case BinaryOperations.Subtract:
                                return new RuntimeValue(i64, (long)l_i64.value - (long)r_i64.value);
                            case BinaryOperations.Multiply:
                                return new RuntimeValue(i64, (long)l_i64.value * (long)r_i64.value);
                            case BinaryOperations.Divide:
                                return new RuntimeValue(i64, (long)l_i64.value / (long)r_i64.value);
                            case BinaryOperations.CompEqual:
                                return new RuntimeValue(bool_t, (long)l_i64.value == (long)r_i64.value);
                            case BinaryOperations.CompUnequal:
                                return new RuntimeValue(bool_t, (long)l_i64.value != (long)r_i64.value);
                            case BinaryOperations.CompGreater:
                                return new RuntimeValue(bool_t, (long)l_i64.value >  (long)r_i64.value);
                            case BinaryOperations.CompLesser:
                                return new RuntimeValue(bool_t, (long)l_i64.value <  (long)r_i64.value);
                            case BinaryOperations.CompGreaterEqual:
                                return new RuntimeValue(bool_t, (long)l_i64.value >= (long)r_i64.value);
                            case BinaryOperations.CompLesserEqual:
                                return new RuntimeValue(bool_t, (long)l_i64.value <= (long)r_i64.value);
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        var l_f64 = convertorTable.ConvertTo(l, f64);
                        var r_f64 = convertorTable.ConvertTo(r, f64);

                        switch (node.Operation)
                        {
                            case BinaryOperations.Add:
                                return new RuntimeValue(f64, (double)l_f64.value + (double)r_f64.value);
                            case BinaryOperations.Subtract:
                                return new RuntimeValue(f64, (double)l_f64.value - (double)r_f64.value);
                            case BinaryOperations.Multiply:
                                return new RuntimeValue(f64, (double)l_f64.value * (double)r_f64.value);
                            case BinaryOperations.Divide:
                                return new RuntimeValue(f64, (double)l_f64.value / (double)r_f64.value);
                            case BinaryOperations.CompEqual:
                                return new RuntimeValue(bool_t, (double)l_f64.value == (double)r_f64.value);
                            case BinaryOperations.CompUnequal:
                                return new RuntimeValue(bool_t, (double)l_f64.value != (double)r_f64.value);
                            case BinaryOperations.CompGreater:
                                return new RuntimeValue(bool_t, (double)l_f64.value >  (double)r_f64.value);
                            case BinaryOperations.CompLesser:
                                return new RuntimeValue(bool_t, (double)l_f64.value <  (double)r_f64.value);
                            case BinaryOperations.CompGreaterEqual: 
                                return new RuntimeValue(bool_t, (double)l_f64.value >= (double)r_f64.value);
                            case BinaryOperations.CompLesserEqual:
                                return new RuntimeValue(bool_t, (double)l_f64.value <= (double)r_f64.value);
                            default:
                                throw new NotImplementedException();
                        }
                    }
                
                case BinaryOperations.LogicOr:
                    return new RuntimeValue(bool_t, (bool)convertorTable.ConvertTo(node.Left.Accept(this), bool_t).value || (bool)convertorTable.ConvertTo(node.Right.Accept(this), bool_t).value);
                case BinaryOperations.LogicAnd:
                    return new RuntimeValue(bool_t, (bool)convertorTable.ConvertTo(node.Left.Accept(this), bool_t).value && (bool)convertorTable.ConvertTo(node.Right.Accept(this), bool_t).value);
                default:
                    throw new NotImplementedException();
            }
        }

        public object BlockNodeVisit(BlockNode node)
        {
            var rc = runtimeContext;
            this.runtimeContext = new RuntimeContext(rc);
            foreach (var s in node.StList)
            {
                s.Accept(this);
            }
            this.runtimeContext = rc;
            return null;

        }

        public RuntimeValue CastNodeVisit(CastNode castNode)
        {
            var v = castNode.ExprNode.Accept(this);
            var t = typeTable[castNode];
            return convertorTable.ConvertTo(v, t);
        }

        public RuntimeValue ConstantNodeVisit(ConstantNode node)
        {
            var t = typeTable[node];
            return initializers.GetRuntimeValue(t, node.Value);
        }

        public object CycleNodeVisit(CycleNode node)
        {
            var i64 = typeNameTable.GetLangType("i64");
            var c = (long)(convertorTable.ConvertTo(node.Expr.Accept(this), i64).value);
            for (int i = 0; i < c; i++)
            {
                node.Stat.Accept(this);
            }
            return null;
        }

        public object DecNodeVisit(DecNode node)
        {
            if (node.exprNodes.Count != 0)
            {
                var exprT = node.idNodes.Zip(node.exprNodes.Select(x => x.Accept(this)), (x, y) => (x, y)).ToArray();
                foreach (var (x, y) in exprT)
                {
                    runtimeContext.InitializeRuntimeValue(x.Name, y);
                }
                return null;
            }
            else
            {
                
                foreach (var decl in node.idNodes)
                {
                    runtimeContext.InitializeRuntimeValue(decl.Name, new RuntimeValue(node.typeNode is null? null : typeTable[decl]));
                    
                }
                return null;
            }
        }

        public object ForNodeVisit(ForNode node)
        {
            var i64 = typeNameTable.GetLangType("i64");
            var f = convertorTable.ConvertTo(node.ExprFrom.Accept(this), i64);
            var t = (long)convertorTable.ConvertTo(node.ExprTo.Accept(this), i64).value;
            runtimeContext.InitializeRuntimeValue(node.Id.Name, f);
            for (long i = (long)f.value; i < t; i++)
            {
                runtimeContext.SetVariable(node.Id.Name, new RuntimeValue(i64, i));
                node.Stat.Accept(this);
            }
            return null;
        }

        public RuntimeValue IdNodeVisit(IdNode node)
        {
            return runtimeContext.GetRuntimeValue(node.Name);
        }

        public object IfNodeVisit(IfNode node)
        {
            var bool_t = typeNameTable.GetLangType("bool");
            var c = (bool)convertorTable.ConvertTo(node.CondExpr.Accept(this), bool_t).value;
            if (c)
                node.ExprMainBranch.Accept(this);
            else
                node.ExprAltBranch?.Accept(this);
            return null;
        }

        public RuntimeValue MilliTimeVisit(MilliTimeNode node)
        {
            var f64 = typeNameTable.GetLangType("f64");
            return new RuntimeValue(f64, stopwatch.Elapsed.TotalMilliseconds);
        }

        public object RepeatNodeVisit(RepeatNode node)
        {
            var bool_t = typeNameTable.GetLangType("bool");
            do
            {
                node.Block.Accept(this);
            } while (!(bool)convertorTable.ConvertTo(node.Expr.Accept(this), bool_t).value);
            return this;

        }

        public RuntimeValue TypeNodeVisit(TypeNode typeNode)
        {
            throw new NotImplementedException();
        }

        public RuntimeValue UnaryOpNodeVisit(UnaryOpNode unaryOpNode)
        {
            var i64 = typeNameTable.GetLangType("i64");
            var f64 = typeNameTable.GetLangType("f64");
            var bool_t = typeNameTable.GetLangType("bool");
            var e = unaryOpNode.Expr.Accept(this);
            switch (unaryOpNode.Operation)
            {
                case UnaryOperations.Negate:
                    if (convertorTable.IsImplicitConvertable(e.langType, i64))
                        return new RuntimeValue(i64, -(long)convertorTable.ConvertTo(e, i64).value);
                    else
                        return new RuntimeValue(f64, -(double)convertorTable.ConvertTo(e, f64).value);
                case UnaryOperations.LogicalNot:
                    return new RuntimeValue(bool_t, !(bool)convertorTable.ConvertTo(e, bool_t).value);
                default:
                    throw new NotImplementedException();

            }
        }

        public object WhileNodeVisit(WhileNode node)
        {
            var bool_t = typeNameTable.GetLangType("bool");
            while ((bool)convertorTable.ConvertTo(node.Expr.Accept(this), bool_t).value)
            {
                node.Stat.Accept(this);
            }
            return this;
        }

        public object WriteNodeVisit(WriteNode node)
        {
            var t = node.Expr.Accept(this);
            if (WriteOut)
                Console.WriteLine($"{t.value}");
            return null;
        }

        TypeSLP IASTTypeVisitor<TypeSLP>.TypeNodeVisit(TypeNode typeNode)
        {
            throw new NotImplementedException();
        }
    }
}
