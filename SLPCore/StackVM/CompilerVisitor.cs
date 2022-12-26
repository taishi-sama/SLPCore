using SLPCore.AST;
using SLPCore.Operators;
using SLPCore.Types;
using SLPCore.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.StackVM
{
    public class CompilerVisitor : IASTVisitor<OpcodesEmitter>
    {
        TypeOfExpression typeTable;
        TypeNameTable typeNameTable;
        ConvertorTable convertorTable;
        FromInitTable initializers;
        VariableContext variableContext = new VariableContext();
        public List<string> constStringPool = new();
        public bool WriteOut { get; set; } = true;
        public CompilerVisitor(TypeOfExpression typeTable, TypeNameTable typeNameTable, ConvertorTable convertorTable, FromInitTable initializers)
        {
            this.typeTable = typeTable;
            this.typeNameTable = typeNameTable;
            this.convertorTable = convertorTable;
            this.initializers = initializers;
        }

        public OpcodesEmitter AssingNodeVisit(AssignNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.Expr.Accept(this));
            ops.PushPOPL(variableContext.GetLocalVariableNumber(node.Id.Name));
            return ops;
        }

        public OpcodesEmitter BinaryOpNodeVisit(BinaryOpNode node)
        {
            var ops = new OpcodesEmitter();
            var left = node.Left.Accept(this);
            var right = node.Right.Accept(this);
            (var typeL, var typeR) = (typeTable[node.Left], typeTable[node.Right]);
            if (typeL.Name == "f64" || typeR.Name == "f64")
            {
                ops.AddOpcodes(left);
                if (typeL.Name == "i64")
                    ops.PushITOF();
                ops.AddOpcodes(right);
                if (typeR.Name == "i64")
                    ops.PushITOF();
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        ops.PushADDF();
                        break;
                    case BinaryOperations.Subtract:
                        ops.PushSUBF();
                        break;
                    case BinaryOperations.Multiply:
                        ops.PushMULF();
                        break;
                    case BinaryOperations.Divide:
                        ops.PushDIVF();
                        break;
                    case BinaryOperations.CompEqual:
                        ops.PushEQF();
                        break;
                    case BinaryOperations.CompUnequal:
                        ops.PushEQF();
                        ops.PushNEGB();
                        break;
                    case BinaryOperations.CompGreater:
                        ops.PushLEQF();
                        ops.PushNEGB();
                        break;
                    case BinaryOperations.CompLesser:
                        ops.PushLF();
                        break;
                    case BinaryOperations.CompGreaterEqual:
                        ops.PushLF();
                        ops.PushNEGB();
                        break;
                    case BinaryOperations.CompLesserEqual:
                        ops.PushLEQF();
                        break;
                    case BinaryOperations.LogicOr:
                    case BinaryOperations.LogicAnd:
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (typeL.Name == "i64" && typeR.Name == "i64")
            {
                ops.AddOpcodes(left);
                ops.AddOpcodes(right);
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        ops.PushADDI();
                        break;
                    case BinaryOperations.Subtract:
                        ops.PushSUBI();
                        break;
                    case BinaryOperations.Multiply:
                        ops.PushMULI();
                        break;
                    case BinaryOperations.Divide:
                        ops.PushDIVI();
                        break;
                    case BinaryOperations.CompEqual:
                        ops.PushEQI();
                        break;
                    case BinaryOperations.CompUnequal:
                        ops.PushEQI();
                        ops.PushNEGB();
                        break;
                    case BinaryOperations.CompGreater:
                        ops.PushLEQI();
                        ops.PushNEGB();
                        break;
                    case BinaryOperations.CompLesser:
                        ops.PushLI();
                        break;
                    case BinaryOperations.CompGreaterEqual:
                        ops.PushLI();
                        ops.PushNEGB();
                        break;
                    case BinaryOperations.CompLesserEqual:
                        ops.PushLEQI();
                        break;
                    case BinaryOperations.LogicOr:
                    case BinaryOperations.LogicAnd:
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (typeL.Name == "bool" && typeR.Name == "bool")
            {
                ops.AddOpcodes(left);
                ops.AddOpcodes(right);
                switch (node.Operation)
                {

                    case BinaryOperations.LogicOr:
                        ops.PushORB();
                        break;
                    case BinaryOperations.LogicAnd:
                        ops.PushANDB();
                        break;
                    case BinaryOperations.CompEqual:
                        ops.PushEQI();
                        break;
                    case BinaryOperations.CompUnequal:
                        ops.PushEQI();
                        ops.PushNEGB();
                        break;
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
            else
                throw new NotImplementedException();
            return ops;
        }

        public OpcodesEmitter BlockNodeVisit(BlockNode node)
        {
            var ops = new OpcodesEmitter();
            var rc = variableContext;
            this.variableContext = new VariableContext(rc);
            foreach (var s in node.StList)
            {
                ops.AddOpcodes(s.Accept(this));
            }
            this.variableContext = rc;
            return ops;
        }

        public OpcodesEmitter CastNodeVisit(CastNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.Accept(this));
            switch (node.TypeNode.TypeName)
            {
                case "i64":
                    switch (typeTable[node.ExprNode].Name)
                    {
                        case "i64":
                            break;
                        case "f64":
                            ops.PushITOF();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case "f64":
                    switch (typeTable[node.ExprNode].Name)
                    {
                        case "f64":
                            break;
                        case "i64":
                            ops.PushFTOI();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();

            }
            return ops;
        }

        public OpcodesEmitter ConstantNodeVisit(ConstantNode node)
        {
            var ops = new OpcodesEmitter();
            var t = typeTable[node];
            switch (t.Name) {
                case "i64":
                    ops.PushPUSH8(long.Parse(node.Value));
                    break;
                case "f64":
                    ops.PushPUSH8(double.Parse(node.Value, System.Globalization.CultureInfo.InvariantCulture));
                    break;
                case "bool":
                    ops.PushPUSH1(bool.Parse(node.Value)? (byte)1 : (byte)0);
                    break;
                case "conststr":
                    constStringPool.Add(node.Value.Substring(1, node.Value.Length - 2));
                    ops.PushPUSH8(constStringPool.Count - 1);
                    break;
                default: throw new NotImplementedException($"Type is not implemented: {t.Name}");
            }
            return ops;
        }

        public OpcodesEmitter CycleNodeVisit(CycleNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.Expr.Accept(this));
            ops.PushDUP2();
            ops.PushPUSH1(0);
            ops.PushLEQI();
            
            var body = node.Stat.Accept(this);

            ops.PushJMPT2((short)(body.Count() + 2 + 1 + 3));
            ops.AddOpcodes(body);
            ops.PushPUSH1(1);
            ops.PushSUBI();
            ops.PushJMP2((short)(- 3 - 1 - 2 - body.Count() - 3 - 1 - 2 - 1 ));
            ops.PushDROP();
            return ops;
        }

        public OpcodesEmitter DecNodeVisit(DecNode node)
        {
            var ops = new OpcodesEmitter();
            if (node.exprNodes.Count != 0)
            {
                var exprT = node.idNodes.Zip(node.exprNodes.Select(x => x.Accept(this)), (x, y) => (x, y)).ToArray();
                foreach (var (x, y) in exprT)
                {
                    ops.AddOpcodes(y);
                    ops.PushPOPL(variableContext.AllocateNumber(x.Name));
                }
                return ops;
            }
            else
            {

                foreach (var decl in node.idNodes)
                {
                    variableContext.AllocateNumber(decl.Name);
                }
                return ops;
            }
        }

        public OpcodesEmitter ForNodeVisit(ForNode node)
        {
            var ops = new OpcodesEmitter();
            var fr = node.ExprFrom.Accept(this);
            var to = node.ExprTo.Accept(this);
            var variable = variableContext.AllocateNumber(node.Id.Name);
            var st = node.Stat.Accept(this);
            ops.AddOpcodes(fr);
            ops.PushPOPL(variable);
            ops.AddOpcodes(to);
            ops.PushDUP2();
            ops.PushPUSHL(variable);
            ops.PushLEQI();
            ops.PushJMPT2((short)(st.Count() + 2 + 3 + 1 + 3 + 3));
            ops.AddOpcodes(st);
            ops.PushPUSH1(1);
            ops.PushPUSHL(variable);
            ops.PushADDI();
            ops.PushPOPL(variable);
            ops.PushJMP2((short)(- 3 - 3 - 1 - 3 - 2 - st.Count() - 3 - 1 - 3 - 1));
            ops.PushDROP();

            return ops;
        }

        public OpcodesEmitter IdNodeVisit(IdNode node)
        {
            var ops = new OpcodesEmitter();
            ops.PushPUSHL(variableContext.GetLocalVariableNumber(node.Name));
            return ops;
        }

        public OpcodesEmitter IfNodeVisit(IfNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.CondExpr.Accept(this));
            var trueBranch = node.ExprMainBranch.Accept(this);
            var falseBranch = node.ExprAltBranch?.Accept(this) ?? new OpcodesEmitter();
            trueBranch.PushJMP2((short)falseBranch.Count());
            ops.PushJMPF2((short)trueBranch.Count());
            ops.AddOpcodes(trueBranch);
            ops.AddOpcodes(falseBranch);
            return ops;
        }

        public OpcodesEmitter MilliTimeVisit(MilliTimeNode node)
        {
            var ops = new OpcodesEmitter();
            ops.PushMILLIS();
            return ops;
        }

        public OpcodesEmitter RepeatNodeVisit(RepeatNode node)
        {
            var ops = new OpcodesEmitter();
            var body = node.Block.Accept(this);
            var expr = node.Expr.Accept(this);
            ops.AddOpcodes(body);
            ops.AddOpcodes(expr);
            ops.PushJMPF2((short)(-3-expr.Count()-body.Count()));
            return ops;
        }

        public OpcodesEmitter TypeNodeVisit(TypeNode node)
        {
            throw new NotImplementedException();
        }

        public OpcodesEmitter UnaryOpNodeVisit(UnaryOpNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.Expr.Accept(this));
            switch (node.Operation)
            {
                case UnaryOperations.Negate:
                    switch (typeTable[node.Expr].Name)
                    {
                        case "i64":
                            ops.PushNEGI();
                            break;
                        case "f64":
                            ops.PushNEGF();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case UnaryOperations.LogicalNot:
                    ops.PushNEGB();
                    break;
                default:
                    break;
            }
            return ops;

        }

        public OpcodesEmitter WhileNodeVisit(WhileNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.Expr.Accept(this));
            var st = node.Stat.Accept(this);
            ops.PushJMPF2((short)(st.Count() + 3));
            ops.AddOpcodes(st);
            ops.PushJMP2((short)(-ops.Count() - 3));
            return ops;
        }

        public OpcodesEmitter WriteNodeVisit(WriteNode node)
        {
            var ops = new OpcodesEmitter();
            ops.AddOpcodes(node.Expr.Accept(this));
            var t = typeTable[node.Expr];
            switch (t.Name)
            {
                case "i64":
                    ops.PushWRTI();
                    break;
                case "f64":
                    ops.PushWRTF();
                    break;
                case "bool":
                    ops.PushWRTI();
                    break;
                case "conststr":
                    ops.PushWRTS();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return ops;
        }
    }
}
