using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedVM
{
    public unsafe class MemoryGuard
    {
        public VMValue* array;
        public uint size;
        public MemoryGuard(uint size) { array = (VMValue*)NativeMemory.AllocZeroed(size, (nuint)sizeof(VMValue)); this.size = size; }
        ~MemoryGuard() { NativeMemory.Free(array); }
    }
    public struct ThreecodedVM
    {
        

        public readonly MemoryGuard constsGuard;
        public readonly MemoryGuard registersGuard;
        unsafe public VMValue* constants;
        unsafe public VMValue* registers;
        public List<string> constStringPool = new();
        public VMCommand[]? commands;
        public Stopwatch Stopwatch { get; private set; } = Stopwatch.StartNew();
        public ThreecodedVM()
        {
            constsGuard = new MemoryGuard(1024*256);
            registersGuard = new MemoryGuard(1024*256);
            unsafe
            {
                constants = constsGuard.array;
                registers = registersGuard.array;
            }
        }
        public void VMRun()
        {
            var progCounter = 0;
            if (commands is null) { throw new NullReferenceException(); }
            unsafe {
                while (true)
                {
                    ref var com = ref commands[progCounter];
                    switch (com.opcode)
                    {
                        case VMOpcodes.NOP:
                            break;
                        case VMOpcodes.ADDI:
                            (*com.out1).i64 = (*com.in1).i64 + (*com.in2).i64;
                            break;
                        case VMOpcodes.ADDF:
                            (*com.out1).f64 = (*com.in1).f64 + (*com.in2).f64;
                            break;
                        case VMOpcodes.SUBI:
                            (*com.out1).i64 = (*com.in1).i64 - (*com.in2).i64;
                            break;
                        case VMOpcodes.SUBF:
                            (*com.out1).f64 = (*com.in1).f64 - (*com.in2).f64;
                            break;
                        case VMOpcodes.MULI:
                            (*com.out1).i64 = (*com.in1).i64 * (*com.in2).i64;
                            break;
                        case VMOpcodes.MULF:
                            (*com.out1).f64 = (*com.in1).f64 * (*com.in2).f64;
                            break;
                        case VMOpcodes.DIVI:
                            (*com.out1).i64 = (*com.in1).i64 / (*com.in2).i64;
                            break;
                        case VMOpcodes.DIVF:
                            (*com.out1).f64 = (*com.in1).f64 / (*com.in2).f64;
                            break;
                        case VMOpcodes.NEGI:
                            (*com.out1).i64 = -(*com.in1).i64;
                            break;
                        case VMOpcodes.NEGF:
                            (*com.out1).f64 = -(*com.in1).f64;
                            break;
                        case VMOpcodes.MOV8:
                            (*com.out1) = (*com.in1);
                            break;
                        case VMOpcodes.MULFI:
                            (*com.out1).f64 = (*com.in1).f64 * (*com.in2).i64;
                            break;
                        case VMOpcodes.DIVFI:
                            (*com.out1).f64 = (*com.in1).f64 / (*com.in2).i64;
                            break;
                        case VMOpcodes.WRTI:
                            Console.WriteLine((*com.in1).i64);
                            break;
                        case VMOpcodes.WRTF:
                            Console.WriteLine((*com.in1).f64);
                            break;
                        case VMOpcodes.WRTS:
                            Console.WriteLine(constStringPool[(int)(*com.in1).conststr]);
                            break;
                        case VMOpcodes.EQI:
                            (*com.out1).@bool = (*com.in1).i64 == (*com.in2).i64;
                            break;
                        case VMOpcodes.NEQI:
                            (*com.out1).@bool = (*com.in1).i64 != (*com.in2).i64;
                            break;
                        case VMOpcodes.LEQI:
                            (*com.out1).@bool = (*com.in1).i64 <= (*com.in2).i64;
                            break;
                        case VMOpcodes.GEQI:
                            (*com.out1).@bool = (*com.in1).i64 >= (*com.in2).i64;
                            break;
                        case VMOpcodes.LI:
                            (*com.out1).@bool = (*com.in1).i64 < (*com.in2).i64;
                            break;
                        case VMOpcodes.GI:
                            (*com.out1).@bool = (*com.in1).i64 > (*com.in2).i64;
                            break;
                        case VMOpcodes.EQF:
                            (*com.out1).@bool = (*com.in1).f64 == (*com.in2).f64;
                            break;
                        case VMOpcodes.NEQF:
                            (*com.out1).@bool = (*com.in1).f64 != (*com.in2).f64;
                            break;
                        case VMOpcodes.LEQF:
                            (*com.out1).@bool = (*com.in1).f64 <= (*com.in2).f64;
                            break;
                        case VMOpcodes.GEQF:
                            (*com.out1).@bool = (*com.in1).f64 >= (*com.in2).f64;
                            break;
                        case VMOpcodes.LF:
                            (*com.out1).@bool = (*com.in1).f64 < (*com.in2).f64;
                            break;
                        case VMOpcodes.GF:
                            (*com.out1).@bool = (*com.in1).f64 > (*com.in2).f64;
                            break;
                        case VMOpcodes.ANDB:
                            (*com.out1).@bool = (*com.in1).@bool & (*com.in2).@bool;
                            break;
                        case VMOpcodes.ORB:
                            (*com.out1).@bool = (*com.in1).@bool | (*com.in2).@bool;
                            break;
                        case VMOpcodes.NEGB:
                            (*com.out1).@bool = !(*com.in1).@bool;
                            break;
                        case VMOpcodes.FTOI:
                            (*com.out1).i64 = (long)(*com.in1).f64;
                            break;
                        case VMOpcodes.ITOF:
                            (*com.out1).f64 = (*com.in1).i64;
                            break;
                        case VMOpcodes.JMP:
                            progCounter += (int)(com.in1);
                            break;
                        case VMOpcodes.JMPT:
                            if ((*com.in2).@bool)
                                progCounter += (int)(com.in1);
                            break;
                        case VMOpcodes.JMPF:
                            if (!(*com.in2).@bool)
                                progCounter += (int)(com.in1);
                            break;
                        case VMOpcodes.STOP:
                            goto loopend;
                        case VMOpcodes.MILLS:
                            (*com.out1).f64 = Stopwatch.ElapsedMilliseconds;
                            break;
                        case VMOpcodes.ADDFINVI:
                            (*com.out1).f64 = (*com.in1).f64 + 1.0 / (*com.in2).i64;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    progCounter++;
                }
            }
        loopend:;
        }
        public string DumpCommands()
        {
            var t = new StringBuilder();
            if (commands is null) { throw new NullReferenceException(); }
            foreach ((var command, var index) in commands.Zip(Enumerable.Range(0, commands.Length)))
            {
                unsafe
                {
                    t.AppendLine($"{index,4}: {command.opcode.ToString(),6}: 0x{(ulong)command.in1,-11:x} | 0x{(ulong)command.in2,-11:x} -> 0x{(ulong)command.out1,-11:x}");
                }
            }
            return t.ToString();
        }
    }
}
