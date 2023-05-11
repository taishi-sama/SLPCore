using BenchmarkDotNet.Attributes;
using SLPCore.AST;
using SLPCore.Operators;
using SLPCore.StackVM;
using SLPCore.ThreecodedVM;
using SLPCore.Types;
using SLPCore.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedIM
{
    public class ThreecodedCompilerVisitor : IASTStatementVisitor<CommandsEmitter>, IASTExprVisitor<(CommandsEmitter, string variable)>
    {
        TypeOfExpression typeTable;
        TypeNameTable typeNameTable;
        ConvertorTable convertorTable;
        FromInitTable initializers;
        NameAllocator variableContext = new NameAllocator();
        public Dictionary<string, VMValue> constants = new Dictionary<string, VMValue>();
        Stack<string> temporaryVariables = new Stack<string>();
        public List<string> constStringPool = new();
        public ThreecodedCompilerVisitor(TypeOfExpression typeTable, TypeNameTable typeNameTable, ConvertorTable convertorTable, FromInitTable initializers)
        {
            this.typeTable = typeTable;
            this.typeNameTable = typeNameTable;
            this.convertorTable = convertorTable;
            this.initializers = initializers;
            constants.Add("0i64", new VMValue() { i64 = 0 });
            constants.Add("0f64", new VMValue() { f64 = 0 });
            constants.Add("1i64", new VMValue() { i64 = 1 });

        }

        public CommandsEmitter AssingNodeVisit(AssignNode node)
        {
            var commandsEmitter = new CommandsEmitter();
            var (t, exp_v) = node.Expr.Accept(this);
            commandsEmitter.AddOpcodes(t);
            commandsEmitter.EmitMOV8(exp_v, variableContext.GetLocalVariable(node.Id.Name));
            return commandsEmitter;
        }

        public CommandsEmitter BlockNodeVisit(BlockNode node)
        {
            variableContext.AddNamespaceLayer();
            var commandsEmitter = new CommandsEmitter();
            foreach (var item in node.StList)
            {
                var coms = item.Accept(this);
                commandsEmitter.AddOpcodes(coms);
            }
            variableContext.RemoveNamespaceLayer();
            return commandsEmitter;
        }

        public CommandsEmitter CycleNodeVisit(CycleNode node)
        {
            var ops = new CommandsEmitter();
            var (expr, exprVar) = node.Expr.Accept(this);
            ops.AddOpcodes(expr);
            var cycleVar = variableContext.AllocateTemporaryVariable();
            var condChechVar = variableContext.AllocateTemporaryVariable();
            ops.EmitMOV8(exprVar, cycleVar);
            
            ops.EmitLEQI(cycleVar, "0i64", condChechVar);
            var cycleLabel = variableContext.GetCycleLabelName();

            ops.AddLabelToLastCommand(cycleLabel);
            var cycleExitLabel = variableContext.GetCycleExitLabelName();
            ops.EmitJMPT(condChechVar, cycleExitLabel);

            var body = node.Stat.Accept(this);
            ops.AddOpcodes(body);
            ops.EmitSUBI(cycleVar, "1i64", cycleVar);
            ops.EmitJMP(cycleLabel);
            ops.EmitNOP();
            ops.AddLabelToLastCommand(cycleExitLabel);
            return ops;
        }

        public CommandsEmitter DecNodeVisit(DecNode node)
        {
            var coms = new CommandsEmitter();
            if (node.exprNodes.Count != 0)
            {
                var exprT = node.idNodes.Zip(node.exprNodes.Select(x => x.Accept(this)), (x, y) => (x, y)).ToArray();
                foreach (var (x, (y, z)) in exprT)
                {
                    coms.AddOpcodes(y);
                    coms.EmitMOV8(z, variableContext.AllocateLocalVariable(x.Name));
                }
                return coms;
            }
            else
            {
                foreach (var decl in node.idNodes)
                {
                    variableContext.AllocateLocalVariable(decl.Name);
                }
                return coms;
            }
        }

        public CommandsEmitter ForNodeVisit(ForNode node)
        {
            throw new NotImplementedException();
        }


        public CommandsEmitter IfNodeVisit(IfNode node)
        {
            var ops = new CommandsEmitter();
            var (condOps, condVar) = node.CondExpr.Accept(this);
            ops.AddOpcodes(condOps);
            

            if (node.ExprAltBranch is not null)
            {
                var EndIfMain = variableContext.GetElseLabelName();
                var EndIfGlobal = variableContext.GetIfLabelName();

                ops.EmitJMPF(condVar, EndIfMain);
                var mainOps = node.ExprMainBranch.Accept(this);
                ops.AddOpcodes(mainOps);
                ops.EmitJMP(EndIfGlobal);
                ops.EmitNOP();
                ops.AddLabelToLastCommand(EndIfMain);
                var altOps = node.ExprAltBranch.Accept(this);
                ops.AddOpcodes(altOps);
                ops.EmitNOP();
                ops.AddLabelToLastCommand(EndIfGlobal);
            }
            else
            {
                var EndIfGlobal = variableContext.GetIfLabelName();
                ops.EmitJMPF(condVar, EndIfGlobal);
                var mainOps = node.ExprMainBranch.Accept(this);
                ops.AddOpcodes(mainOps);
                ops.EmitNOP();
                ops.AddLabelToLastCommand(EndIfGlobal);
            }
            return ops;
        }


        public CommandsEmitter RepeatNodeVisit(RepeatNode node)
        {
            var ops = new CommandsEmitter();
            var body = node.Block.Accept(this);
            var (cond, condVar) = node.Expr.Accept(this);
            ops.EmitNOP();
            var repeat = variableContext.GetRepeatLabelName();
            ops.AddLabelToLastCommand(repeat);
            ops.AddOpcodes(body);
            ops.AddOpcodes(cond);
            ops.EmitJMPF(condVar, repeat);
            return ops;

        }


        public CommandsEmitter WhileNodeVisit(WhileNode node)
        {
            var ops = new CommandsEmitter();
            var (expr, condVar) = node.Expr.Accept(this);
            var body = node.Stat.Accept(this);
            ops.EmitNOP();
            var whileCond = variableContext.GetWhileLabelName();
            var whileExit = variableContext.GetWhileExitLabelName();
            ops.AddLabelToLastCommand(whileCond);
            ops.AddOpcodes(expr);
            ops.EmitJMPF(condVar, whileExit);
            ops.AddOpcodes(body);
            ops.EmitJMP(whileCond);
            ops.EmitNOP();
            ops.AddLabelToLastCommand(whileExit);
            return ops;
        }

        public CommandsEmitter WriteNodeVisit(WriteNode node)
        {
            var ops = new CommandsEmitter();
            var (exprCode, exprVar) = node.Expr.Accept(this);
            ops.AddOpcodes(exprCode);
            var t = typeTable[node.Expr];
            switch (t.Name)
            {
                case "i64":
                    ops.EmitWRTI(exprVar);
                    break;
                case "f64":
                    ops.EmitWRTF(exprVar);
                    break;
                case "bool":
                    ops.EmitWRTI(exprVar);
                    break;
                case "conststr":
                    ops.EmitWRTS(exprVar);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return ops;
        }

        public (CommandsEmitter, string variable) BinaryOpNodeVisit(BinaryOpNode node)
        {
            var ops = new CommandsEmitter();
            var (left, leftVar) = node.Left.Accept(this);
            var (right, rightVar) = node.Right.Accept(this);
            (var typeL, var typeR) = (typeTable[node.Left], typeTable[node.Right]);
            var outvar = variableContext.AllocateTemporaryVariable();
            if (typeL.Name == "f64" || typeR.Name == "f64")
            {
                ops.AddOpcodes(left);
                if (typeL.Name == "i64")
                {
                    var leftVarConv = variableContext.AllocateTemporaryVariable();
                    ops.EmitITOF(leftVar, leftVarConv);
                    leftVar = leftVarConv;
                }
                ops.AddOpcodes(right);
                if (typeR.Name == "i64")
                {
                    var rightVarConv = variableContext.AllocateTemporaryVariable();
                    ops.EmitITOF(rightVar, rightVarConv);
                    rightVar = rightVarConv;
                }   
                switch (node.Operation)
                {
                    case BinaryOperations.Add:
                        ops.EmitADDF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.Subtract:
                        ops.EmitSUBF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.Multiply:
                        ops.EmitMULF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.Divide:
                        ops.EmitDIVF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompEqual:
                        ops.EmitEQF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompUnequal:
                        ops.EmitNEQF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompGreater:
                        ops.EmitGF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompLesser:
                        ops.EmitLF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompGreaterEqual:
                        ops.EmitGEQF(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompLesserEqual:
                        ops.EmitLEQF(leftVar, rightVar, outvar);
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
                        ops.EmitADDI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.Subtract:
                        ops.EmitSUBI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.Multiply:
                        ops.EmitMULI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.Divide:
                        ops.EmitDIVI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompEqual:
                        ops.EmitEQI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompUnequal:
                        ops.EmitNEQI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompGreater:
                        ops.EmitGI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompLesser:
                        ops.EmitLI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompGreaterEqual:
                        ops.EmitGEQI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompLesserEqual:
                        ops.EmitLEQI(leftVar, rightVar, outvar);
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
                        ops.EmitORB(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.LogicAnd:
                        ops.EmitANDB(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompEqual:
                        ops.EmitEQI(leftVar, rightVar, outvar);
                        break;
                    case BinaryOperations.CompUnequal:
                        ops.EmitNEQI(leftVar, rightVar, outvar);
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
            return (ops, outvar);
        }

        (CommandsEmitter, string variable) IASTExprVisitor<(CommandsEmitter, string variable)>.ConstantNodeVisit(ConstantNode node)
        {
            var ops = new CommandsEmitter();
            var t = typeTable[node];
            var l = "";
            var v = new VMValue();
            switch (t.Name)
            {
                case "i64":
                    l = variableContext.GetConstLabelName($"i64_{node.Value}");
                    v.i64 = long.Parse(node.Value);
                    break;
                case "f64":
                    l = variableContext.GetConstLabelName($"f64_{node.Value}");
                    v.f64 = double.Parse(node.Value, System.Globalization.CultureInfo.InvariantCulture);
                    break;
                case "bool":
                    l = variableContext.GetConstLabelName($"bool_{node.Value}");
                    v.@bool = bool.Parse(node.Value);
                    break;
                case "conststr":
                    constStringPool.Add(node.Value.Substring(1, node.Value.Length - 2));

                    l = variableContext.GetConstLabelName($"conststr_{constStringPool.Count - 1}");
                    v.conststr = constStringPool.Count - 1;
                    break;
                default: throw new NotImplementedException($"Type is not implemented: {t.Name}");
            }
            constants.Add(l, v);
            return (ops, l);
        }

        (CommandsEmitter, string variable) IASTExprVisitor<(CommandsEmitter, string variable)>.IdNodeVisit(IdNode node)
        {
            var ops = new CommandsEmitter();
            return (ops, variableContext.GetLocalVariable(node.Name));
        }

        (CommandsEmitter, string variable) IASTExprVisitor<(CommandsEmitter, string variable)>.UnaryOpNodeVisit(UnaryOpNode node)
        {
            var ops = new CommandsEmitter();
            var (input, inVar) = node.Expr.Accept(this);
            var outVar = variableContext.AllocateTemporaryVariable();
            ops.AddOpcodes(input);
            switch (node.Operation)
            {
                case UnaryOperations.Negate:
                    switch (typeTable[node.Expr].Name)
                    {
                        case "i64":
                            ops.EmitNEGI(inVar, outVar);
                            break;
                        case "f64":
                            ops.EmitNEGF(inVar, outVar);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case UnaryOperations.LogicalNot:
                    ops.EmitNEGB(inVar, outVar);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return (ops, outVar);
        }

        (CommandsEmitter, string variable) IASTExprVisitor<(CommandsEmitter, string variable)>.TypeNodeVisit(TypeNode node)
        {
            throw new NotImplementedException();
        }

        (CommandsEmitter, string variable) IASTExprVisitor<(CommandsEmitter, string variable)>.CastNodeVisit(CastNode node)
        {
            throw new NotImplementedException();
        }

        (CommandsEmitter, string variable) IASTExprVisitor<(CommandsEmitter, string variable)>.MilliTimeVisit(MilliTimeNode node)
        {
            var ops = new CommandsEmitter();
            var m = variableContext.AllocateTemporaryVariable();
            ops.EmitMILLS(m);
            return (ops, m);
        }
    }
}
