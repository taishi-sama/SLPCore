using Microsoft.VisualBasic;
using SLPCore.ThreecodedIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedVM
{
    public class IMtoVMTranslator
    {
        private unsafe class PointerOffset
        {
            public VMValue* offsetVars;
            public Dictionary<string, uint> variables;
            public VMValue* offsetConsts;
            public Dictionary<string, uint> consts;

            public PointerOffset(VMValue* offsetVars, Dictionary<string, uint> variables, VMValue* offsetConsts, Dictionary<string, uint> consts)
            {
                this.offsetVars = offsetVars;
                this.variables = variables;
                this.offsetConsts = offsetConsts;
                this.consts = consts;
            }

            public VMValue* this[string? name] { get 
                {
                    if (name == null)
                        return null;
                    if (consts.ContainsKey(name))
                        return offsetConsts + consts[name];
                    else if (variables.ContainsKey(name))
                        return offsetVars + variables[name];
                    else 
                        return null;
                } }
        }
        public static ThreecodedVM Process(CommandsEmitter commands, Dictionary<string, VMValue> constants, List<string> stringConsts)
        {
            //throw new NotImplementedException();

            var vm = new ThreecodedVM();
            vm.constStringPool = stringConsts;
            var vars = GetLocalVariables(commands, constants);
            var varsDict = AllocateMemory(vm.registersGuard, vars);
            var constDict = AllocateMemory(vm.constsGuard, constants.Keys);
            var labels = GetLabels(commands);
            List<VMCommand> cmds = new List<VMCommand>();
            unsafe
            {
                var offsets = new PointerOffset(vm.registers, varsDict, vm.constants, constDict);
                foreach (var (key, offset) in constDict)
                {
                    //Console.WriteLine($"Var \"{key}\" with offset {offset}(Placement {(ulong)(vm.constants + offset),10:x}) and value {constants[key].i64}");
                    (*(vm.constants + offset)) = constants[key];
                }

                foreach (var (i, c) in Enumerable.Range(0, commands.Count()).Zip(commands))
                {
                    switch (c.Opcode)
                    {
                        case Opcodes.NOP:
                            cmds.Add(new VMCommand() { opcode = VMOpcodes.NOP });
                            break;
                        case Opcodes.ADDI:
                            cmds.Add(new VMCommand(VMOpcodes.ADDI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.ADDF:
                            cmds.Add(new VMCommand(VMOpcodes.ADDF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.SUBI:
                            cmds.Add(new VMCommand(VMOpcodes.SUBI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.SUBF:
                            cmds.Add(new VMCommand(VMOpcodes.SUBF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.MULI:
                            cmds.Add(new VMCommand(VMOpcodes.MULI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.MULF:
                            cmds.Add(new VMCommand(VMOpcodes.MULF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.DIVI:
                            cmds.Add(new VMCommand(VMOpcodes.DIVI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.DIVF:
                            cmds.Add(new VMCommand(VMOpcodes.DIVF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.NEGI:
                            cmds.Add(new VMCommand(VMOpcodes.NEGI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.NEGF:
                            cmds.Add(new VMCommand(VMOpcodes.NEGF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.MOV8:
                            {
                                VMValue* from = offsets[c.In1!];
                                VMValue* to = offsets[c.Out1!];
                                cmds.Add(new VMCommand(VMOpcodes.MOV8) { in1 = from, out1 = to });
                            }
                            break;
                        case Opcodes.MULFI:
                            cmds.Add(new VMCommand(VMOpcodes.MULFI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.DIVFI:
                            cmds.Add(new VMCommand(VMOpcodes.DIVFI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.WRTI:
                            {
                                VMValue* from = offsets[c.In1!];
                                cmds.Add(new VMCommand(VMOpcodes.WRTI) { in1 = from });
                            }
                            break;
                        case Opcodes.WRTF:
                            cmds.Add(new VMCommand(VMOpcodes.WRTF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.WRTS:
                            cmds.Add(new VMCommand(VMOpcodes.WRTS, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.EQI:
                            cmds.Add(new VMCommand(VMOpcodes.EQI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.NEQI:
                            cmds.Add(new VMCommand(VMOpcodes.NEQI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.LEQI:
                            cmds.Add(new VMCommand(VMOpcodes.LEQI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.GEQI:
                            cmds.Add(new VMCommand(VMOpcodes.GEQI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.LI:
                            cmds.Add(new VMCommand(VMOpcodes.LI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.GI:
                            cmds.Add(new VMCommand(VMOpcodes.GI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.EQF:
                            cmds.Add(new VMCommand(VMOpcodes.EQF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.NEQF:
                            cmds.Add(new VMCommand(VMOpcodes.NEQF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.LEQF:
                            cmds.Add(new VMCommand(VMOpcodes.LEQF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.GEQF:
                            cmds.Add(new VMCommand(VMOpcodes.GEQF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.LF:
                            cmds.Add(new VMCommand(VMOpcodes.LF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.GF:
                            cmds.Add(new VMCommand(VMOpcodes.GF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.ANDB:
                            cmds.Add(new VMCommand(VMOpcodes.ANDB, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.ORB:
                            cmds.Add(new VMCommand(VMOpcodes.ORB, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.NEGB:
                            cmds.Add(new VMCommand(VMOpcodes.NEGB, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.FTOI:
                            cmds.Add(new VMCommand(VMOpcodes.FTOI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.ITOF:
                            cmds.Add(new VMCommand(VMOpcodes.ITOF, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.JMP:
                            {
                                var to = labels[c.GotoMark!];
                                var diff = to - i - 1;
                                cmds.Add(new VMCommand(VMOpcodes.JMP, (VMValue*)diff, null, null));
                            }
                            break;
                        case Opcodes.JMPT:
                            {
                                var to = labels[c.GotoMark!];
                                var diff = to - i - 1;
                                VMValue* cond = offsets[c.In2!];

                                cmds.Add(new VMCommand(VMOpcodes.JMPT, (VMValue*)diff, cond, null));
                            }
                            break;
                        case Opcodes.JMPF:
                            {
                                var to = labels[c.GotoMark!];
                                var diff = to - i - 1;
                                VMValue* cond = offsets[c.In2!];
                                cmds.Add(new VMCommand(VMOpcodes.JMPF, (VMValue*)diff, cond, null));
                            }
                            break;
                        case Opcodes.STOP:
                            cmds.Add(new VMCommand(VMOpcodes.STOP));
                            break;
                        case Opcodes.MILLS:
                            cmds.Add(new VMCommand(VMOpcodes.MILLS, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                        case Opcodes.ADDFINVI:
                            cmds.Add(new VMCommand(VMOpcodes.ADDFINVI, offsets[c.In1!], offsets[c.In2!], offsets[c.Out1!]));
                            break;
                    }
                }
            }
            //throw new NotImplementedException();
            vm.commands = cmds.ToArray();
            return vm;
        }

        static IEnumerable<string> GetLocalVariables(CommandsEmitter commands, Dictionary<string, VMValue> constants)
        {
            var localVariables = new List<string>();
            foreach (var c in commands) 
            {
                if (c.Out1 is not null && !constants.ContainsKey(c.Out1))
                    localVariables.Add(c.Out1);
                
            }
            return localVariables.Distinct().ToList();
        }
        static Dictionary<string, int> GetLabels(CommandsEmitter commands)
        {
            var labels = new Dictionary<string, int>();
            foreach (var (i, c) in Enumerable.Range(0, commands.Count()).Zip(commands))
            {
                foreach (var mark in c.Marks)
                {
                    labels.Add(mark, i);
                }
            }
            return labels;
        }
        static Dictionary<string, uint> AllocateMemory(MemoryGuard memoryGuard, IEnumerable<string> variables)
        {
            var dict = new Dictionary<string, uint>();
            foreach (var (v, i) in variables.Zip(Enumerable.Range(0, (int)memoryGuard.size)))
            {
                dict.Add(v, (uint)i);
            }
            return dict;
        }
    }
}
