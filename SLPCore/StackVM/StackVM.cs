using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.StackVM
{
    public class StackVM
    {
        public byte[] bytecode = new byte[256*16];
        int programCounter = 0;
        int stackPointer = 0; // Указывает на самую верхнюю незанятую ячейку
        StackValue[] stack = new StackValue[1024];
        StackValue[] localVariables = new StackValue[1024];
        public List<string> constStringPool = new();
        public Stopwatch sw = Stopwatch.StartNew();
        public void VMRun()
        {
            var vm1 = this;
            while (vm1.Step()) ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Step()
        {
            programCounter++;
            switch ((Opcodes)bytecode[programCounter - 1])
            {
                case Opcodes.NOP:
                    return true;
                case Opcodes.ADDI:
                    AddI();
                    return true;
                case Opcodes.ADDF:
                    AddF();
                    return true;
                case Opcodes.SUBI:
                    SubI();
                    return true;
                case Opcodes.SUBF:
                    SubF();
                    return true;
                case Opcodes.MULI:
                    MulI();
                    return true;
                case Opcodes.MULF:
                    MulF();
                    return true;
                case Opcodes.DIVI:
                    DivI();
                    return true;
                case Opcodes.DIVF:
                    DivF();
                    return true;
                case Opcodes.NEGI:
                    NegI();
                    return true;
                case Opcodes.NEGF:
                    NegF();
                    return true;
                case Opcodes.PUSH1:
                    Push1();
                    return true;
                case Opcodes.PUSH8:
                    Push8();
                    return true;
                case Opcodes.POPL:
                    PopL();
                    return true;
                case Opcodes.PUSHL:
                    PushL();
                    return true;
                case Opcodes.WRTI:
                    WrtI();
                    return true;
                case Opcodes.WRTF:
                    WrtF();
                    return true;
                case Opcodes.WRTS:  
                    WrtS();
                    return true;
                case Opcodes.EQI:
                    EqI();
                    return true;
                case Opcodes.LEQI:
                    LeqI();
                    return true;
                case Opcodes.LI:
                    LI();
                    return true;
                case Opcodes.EQF:
                    EqF();
                    return true;
                case Opcodes.LEQF:
                    LeqF();
                    return true;
                case Opcodes.LF:
                    LF();
                    return true;
                case Opcodes.ANDB:
                    AndB();
                    return true;
                case Opcodes.ORB:
                    OrB();
                    return true;
                case Opcodes.NEGB:
                    NegB();
                    return true;
                case Opcodes.FTOI:
                    FtoI();
                    return true;
                case Opcodes.ITOF:
                    ItoF();
                    return true;
                case Opcodes.SWAP:
                    Swap();
                    return true;
                case Opcodes.DUP2:
                    Dup2();
                    return true;
                case Opcodes.DROP:
                    Drop();
                    return true;
                case Opcodes.JMP2:
                    Jmp2();
                    return true;
                case Opcodes.JMPT2:
                    JmpT2();
                    return true;
                case Opcodes.JMPF2:
                    JmpF2();
                    return true;
                case Opcodes.STOP:
                    return false;
                case Opcodes.MILLS:
                    Mills();
                    return true;
                
            }
            throw new NotImplementedException($"{bytecode[programCounter - 1]:x} is not valid opcode");

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddI()
        {
            stackPointer--;
            stack[stackPointer - 1].i64 += stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddF() 
        {
            stackPointer--;
            stack[stackPointer - 1].f64 += stack[stackPointer].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SubI()
        {
            stackPointer--;
            stack[stackPointer - 1].i64 -= stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SubF()
        {
            stackPointer--;
            stack[stackPointer - 1].f64 -= stack[stackPointer].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void MulI()
        {
            stackPointer--;
            stack[stackPointer - 1].i64 *= stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void MulF()
        {
            stackPointer--;
            stack[stackPointer - 1].f64 *= stack[stackPointer].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void DivI()
        {
            stackPointer--;
            stack[stackPointer - 1].i64 /= stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void DivF()
        {
            stackPointer--;
            stack[stackPointer - 1].f64 /= stack[stackPointer].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void NegI()
        {
            stack[stackPointer - 1].i64 = -stack[stackPointer - 1].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void NegF()
        {
            stack[stackPointer - 1].f64 = -stack[stackPointer - 1].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Push8()
        {
            stack[stackPointer].i64 = BitConverter.ToInt64(bytecode.AsSpan(programCounter));
            stackPointer++;
            programCounter += 8;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Push1()
        {
            stack[stackPointer].i64 = bytecode[programCounter];
            stackPointer++;
            programCounter += 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WrtI()
        {
            Console.WriteLine(stack[--stackPointer].i64);   
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WrtF()
        {
            Console.WriteLine(stack[--stackPointer].f64);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WrtS()
        {
            Console.WriteLine(constStringPool[(int)stack[--stackPointer].conststr]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EqI()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].i64 == stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void LeqI()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].i64 <= stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void LI()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].i64 < stack[stackPointer].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EqF()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].f64 == stack[stackPointer].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void LeqF()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].f64 <= stack[stackPointer].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void LF()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].f64 < stack[stackPointer].f64;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AndB()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].@bool && stack[stackPointer].@bool;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void OrB()
        {
            stackPointer--;
            stack[stackPointer - 1].@bool = stack[stackPointer - 1].@bool || stack[stackPointer].@bool;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void NegB()
        {
            stack[stackPointer - 1].@bool = !stack[stackPointer - 1].@bool;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void FtoI()
        {
            stack[stackPointer - 1].i64 = (long)stack[stackPointer - 1].f64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ItoF()
        {
            stack[stackPointer - 1].f64 = stack[stackPointer - 1].i64;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Swap()
        {
            (stack[stackPointer - 1], stack[stackPointer - 2]) = (stack[stackPointer - 2], stack[stackPointer - 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Dup2()
        {
            stack[stackPointer] = stack[stackPointer - 1];
            stackPointer++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Drop()
        {
            stackPointer--;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Jmp2()
        {
            programCounter += BitConverter.ToInt16(bytecode.AsSpan(programCounter)) + 2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void JmpT2()
        {
            var b = stack[--stackPointer].@bool;
            if (b)
            {
                programCounter += BitConverter.ToInt16(bytecode.AsSpan(programCounter)) + 2;
            }
            else
                programCounter += 2;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void JmpF2()
        {
            var b = stack[--stackPointer].@bool;
            if (b)
            {
                programCounter += 2;
            }
            else
                programCounter += BitConverter.ToInt16(bytecode.AsSpan(programCounter)) + 2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PopL()
        {
            localVariables[BitConverter.ToUInt16(bytecode.AsSpan(programCounter))] = stack[--stackPointer];
            programCounter += 2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushL()
        {
            stack[stackPointer++] = localVariables[BitConverter.ToUInt16(bytecode.AsSpan(programCounter))];
            programCounter += 2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Mills()
        {
            stack[stackPointer++].f64 = sw.Elapsed.TotalMilliseconds;
        }
    }
}
