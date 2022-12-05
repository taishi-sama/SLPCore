using SLPCore.Exceptions;
using SLPCore.Operators;
using SLPCore.Visitors;
using SLPCore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SLPCore.AST;

namespace SLPCore.Types
{
    public class TypecheckVisitor : IASTVisitor<TypeSLP>
    {
        public TypecheckVisitor(TypeNameTable typeTable, ConvertorTable convertorTable, TypeOfExpression typeExprTable)
        {
            this.typeNameTable = typeTable;
            this.convertorTable = convertorTable;
            this.typeTable = typeExprTable;
        }

        TypeNameTable typeNameTable;
        ConvertorTable convertorTable;
        TypeContext typeContext = new TypeContext();
        
        TypeOfExpression typeTable;
        //Dictionary<ExprNode, TypeSLP> ExpressionTypes = new Dictionary<ExprNode, TypeSLP>();

        public TypeSLP BinaryOpNodeVisit(BinaryOpNode node)
        {
            var left = node.Left.Accept(this);
            var right = node.Right.Accept(this);
            var i64 = typeNameTable.GetLangType("i64");
            var f64 = typeNameTable.GetLangType("f64");
            var bool_t = typeNameTable.GetLangType("bool");
            switch (node.Operation)
            {
                case BinaryOperations.Add:
                case BinaryOperations.Subtract:
                case BinaryOperations.Multiply:
                case BinaryOperations.Divide:

                    if (convertorTable.IsImplicitConvertable(left, i64) && convertorTable.IsImplicitConvertable(right, i64))
                    {
                        typeTable[node] = i64;
                        return i64;
                    }
                    if (convertorTable.IsImplicitConvertable(left, f64) && convertorTable.IsImplicitConvertable(right, f64))
                    {
                        typeTable[node] = f64;
                        return f64;
                    }
                    if (!convertorTable.IsImplicitConvertable(left, f64) && !convertorTable.IsImplicitConvertable(left, i64))
                        throw new TypeIsNotConverable(node.Line, left.Name, "f64 or i64");
                    if (!convertorTable.IsImplicitConvertable(right, f64) && !convertorTable.IsImplicitConvertable(right, i64))
                        throw new TypeIsNotConverable(node.Line, right.Name, "f64 or i64");
                    throw new NotImplementedException();
                case BinaryOperations.CompEqual:
                case BinaryOperations.CompUnequal:
                    if (convertorTable.IsImplicitConvertable(left, right) || convertorTable.IsImplicitConvertable(right, left))
                    {
                        typeTable[node] = bool_t;
                        return bool_t;
                    }
                    throw new TypeIsNotConverable(node.Line, left.Name, right.Name);
                    
                
                case BinaryOperations.CompGreater:
                case BinaryOperations.CompLesser:
                case BinaryOperations.CompGreaterEqual:
                case BinaryOperations.CompLesserEqual:
                    if (convertorTable.IsImplicitConvertable(left, i64) && convertorTable.IsImplicitConvertable(right, i64))
                    {
                        typeTable[node] = bool_t;
                        return bool_t;
                    }
                    if (convertorTable.IsImplicitConvertable(left, f64) && convertorTable.IsImplicitConvertable(right, f64))
                    {
                        typeTable[node] = bool_t;
                        return bool_t;
                    }
                    if (!convertorTable.IsImplicitConvertable(left, f64) && !convertorTable.IsImplicitConvertable(left, i64))
                        throw new TypeIsNotConverable(node.Line, left.Name, "f64 or i64");
                    if (!convertorTable.IsImplicitConvertable(right, f64) && !convertorTable.IsImplicitConvertable(right, i64))
                        throw new TypeIsNotConverable(node.Line, right.Name, "f64 or i64");
                    break;
                case BinaryOperations.LogicOr:
                case BinaryOperations.LogicAnd:
                    if (convertorTable.IsImplicitConvertable(left, bool_t) && convertorTable.IsImplicitConvertable(right, bool_t))
                    {
                        typeTable[node] = bool_t;
                        return bool_t;
                    }
                    throw new TypeIsNotConverable(node.Line, left.Name, "bool");
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public TypeSLP ConstantNodeVisit(ConstantNode node)
        {
            var t = node.TypeNode.Accept(this);
            typeTable[node] = t;
            return t;
        }

        public TypeSLP IdNodeVisit(IdNode node)
        {
            var t = typeContext.GetTypeOfID(node.Name);
            typeTable[node] = t;
            return t;
        }

        public TypeSLP UnaryOpNodeVisit(UnaryOpNode unaryOpNode)
        {
            var i64 = typeNameTable.GetLangType("i64");
            var f64 = typeNameTable.GetLangType("f64");
            var bool_t = typeNameTable.GetLangType("bool");
            var e = unaryOpNode.Expr.Accept(this);
            switch (unaryOpNode.Operation)
            {
                case UnaryOperations.Negate:
                    if (convertorTable.IsImplicitConvertable(e, i64))
                    {
                        typeTable[unaryOpNode] = i64;
                        return i64;
                    }
                    else if (convertorTable.IsImplicitConvertable(e, f64))
                    {
                        typeTable[unaryOpNode] = f64;
                        return f64;
                    }
                    throw new TypeIsNotConverable(unaryOpNode.Line, e.Name, "i64|f64");

                //typeTable[unaryOpNode] = t;

                case UnaryOperations.LogicalNot:
                    if (convertorTable.IsImplicitConvertable(e, bool_t))
                    {
                        typeTable[unaryOpNode] = bool_t;
                        return bool_t;
                    }
                    //typeTable[unaryOpNode] = t;
                    throw new TypeIsNotConverable(unaryOpNode.Line, e.Name, "bool");
                    
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public TypeSLP TypeNodeVisit(TypeNode typeNode)
        {
            return typeNameTable.GetLangType(typeNode.TypeName);
        }

        public TypeSLP CastNodeVisit(CastNode castNode)
        {
            var from = castNode.ExprNode.Accept(this);
            var to = castNode.TypeNode.Accept(this);
            if (convertorTable.IsExplicitConvertable(from, to))
            {
                typeTable[castNode] = to;
                return to;
            }
            throw new TypeIsNotConverable(castNode.Line, from.Name, to.Name);
        }
        public TypeSLP DecNodeVisit(DecNode node)
        {
            if (node.exprNodes.Count != 0)
            {
                var exprT = node.idNodes.Zip(node.exprNodes.Select(x=>x.Accept(this)), (x, y)=> (x,y));
                foreach (var (x, y) in exprT)
                {
                    typeContext.DeclareVarType(x.Name, y);
                }
                return null;
            }
            else
            {
                var type = node.typeNode?.Accept(this);
                foreach (var decl in node.idNodes)
                {
                    typeContext.DeclareVarType(decl.Name, type);
                    typeTable[decl] = type;
                }
                return type;
            }
        }

        public TypeSLP AssingNodeVisit(AssignNode node)
        {
            var lhs = node.Id.Accept(this);
            var rhs = node.Expr.Accept(this);
            if (lhs == null)
            {
                typeContext.RedefineVarType(node.Id.Name, rhs);
                typeTable[node.Id] = rhs;
                return null;
            }
            if (lhs.Equals(rhs))
                return null;
            if (convertorTable.IsImplicitConvertable(rhs, lhs))
                return null;
            if (convertorTable.IsExplicitConvertable(rhs, lhs))
                throw new TypeExplicitCastRequired(node.Line, rhs.Name, lhs.Name);
            throw new TypeIsNotConverable(node.Line, rhs.Name, lhs.Name);
        }

        public TypeSLP BlockNodeVisit(BlockNode node)
        {
            var prev = this.typeContext;
            typeContext = new TypeContext(prev);

            foreach (var item in node.StList)
            {
                item.Accept(this);
            }
            typeContext = prev;
            return null;
        }

        public TypeSLP CycleNodeVisit(CycleNode node)
        {
            var i64 = typeNameTable.GetLangType("i64");

            var t = node.Expr.Accept(this);
            if (!convertorTable.IsImplicitConvertable(t, i64))
                throw new TypeIsNotConverable(node.Line, t.Name, i64.Name);
            node.Stat.Accept(this);
            return null;
        }

        public TypeSLP ForNodeVisit(ForNode node)
        {
            var i64 = typeNameTable.GetLangType("i64");
            var from = node.ExprFrom.Accept(this);
            var to = node.ExprTo.Accept(this);
            if (!convertorTable.IsImplicitConvertable(from, i64))
                throw new TypeExplicitCastRequired(node.Line, from.Name, i64.Name);
            if (!convertorTable.IsImplicitConvertable(to, i64))
                throw new TypeExplicitCastRequired(node.Line, from.Name, i64.Name);
            typeContext.DeclareVarType(node.Id.Name, i64);
            node.Stat.Accept(this);
            return null;
            //throw new NotImplementedException();

        }

        public TypeSLP IfNodeVisit(IfNode node)
        {
            var from = node.CondExpr.Accept(this);
            var to = typeNameTable.GetLangType("bool");
            if (!convertorTable.IsImplicitConvertable(from, to))
                throw new TypeIsNotConverable(node.Line, from.Name, to.Name);
            node.ExprMainBranch.Accept(this);
            node.ExprAltBranch?.Accept(this);
            return null;
        }

        public TypeSLP RepeatNodeVisit(RepeatNode node)
        {
            var from = node.Expr.Accept(this);
            var to = typeNameTable.GetLangType("bool");
            if (!convertorTable.IsImplicitConvertable(from, to))
                throw new TypeIsNotConverable(node.Line, from.Name, to.Name);
            node.Block.Accept(this);

            return null;
        }

        public TypeSLP WhileNodeVisit(WhileNode node)
        {
            var from = node.Expr.Accept(this);
            var to = typeNameTable.GetLangType("bool");
            if (!convertorTable.IsImplicitConvertable(from, to))
                throw new TypeIsNotConverable(node.Line, from.Name, to.Name);
            node.Stat.Accept(this);
            return null;
        }

        public TypeSLP WriteNodeVisit(WriteNode node)
        {
            node.Expr.Accept(this);
            return null;
        }

        public TypeSLP MilliTimeVisit(MilliTimeNode node)
        {
            return typeNameTable.GetLangType("f64");
        }
    }
}
