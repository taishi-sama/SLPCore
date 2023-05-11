using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.StackVM
{
    public class OpcodesEmitter : IEnumerable<Opcodes>
    {
        public List<Opcodes> OpcodeList = new();
        public byte[] AsByteArray()
        {
            return OpcodeList.Cast<byte>().ToArray();
        }
        public List<Opcodes> AsOpcodeList()
        {
            return OpcodeList;
        }
        public void AddOpcodes(IEnumerable<Opcodes> opcodes)
        {
            OpcodeList.AddRange(opcodes);
        }

        public void PushNOP() { OpcodeList.Add(Opcodes.NOP); }
        public void PushADDI() { OpcodeList.Add(Opcodes.ADDI); }
        public void PushADDF() { OpcodeList.Add(Opcodes.ADDF); }
        public void PushSUBI() { OpcodeList.Add(Opcodes.SUBI); }
        public void PushSUBF() { OpcodeList.Add(Opcodes.SUBF); }
        public void PushMULI() { OpcodeList.Add(Opcodes.MULI); }
        public void PushMULF() { OpcodeList.Add(Opcodes.MULF); }
        public void PushDIVI() { OpcodeList.Add(Opcodes.DIVI); }
        public void PushDIVF() { OpcodeList.Add(Opcodes.DIVF); }
        public void PushNEGI() { OpcodeList.Add(Opcodes.NEGI); }
        public void PushNEGF() { OpcodeList.Add(Opcodes.NEGF); }

        public void PushPUSH8(long val) { 
            OpcodeList.Add(Opcodes.PUSH8);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushPUSH1(byte val)
        {
            OpcodeList.Add(Opcodes.PUSH1);
            OpcodeList.Add((Opcodes)val);
        }
        public void PushPUSH8(double val)
        {
            OpcodeList.Add(Opcodes.PUSH8);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushPOPL(ushort val)
        {
            OpcodeList.Add(Opcodes.POPL);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushPUSHL(ushort val)
        {
            OpcodeList.Add(Opcodes.PUSHL);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushWRTI() { OpcodeList.Add(Opcodes.WRTI); }
        public void PushWRTF() { OpcodeList.Add(Opcodes.WRTF); }
        public void PushWRTS() { OpcodeList.Add(Opcodes.WRTS); }

        public void PushEQI() { OpcodeList.Add(Opcodes.EQI); }
        public void PushLEQI() { OpcodeList.Add(Opcodes.LEQI); }
        public void PushLI() { OpcodeList.Add(Opcodes.LI); }

        public void PushEQF() { OpcodeList.Add(Opcodes.EQF); }
        public void PushLEQF() { OpcodeList.Add(Opcodes.LEQF); }
        public void PushLF() { OpcodeList.Add(Opcodes.LF); }
        public void PushANDB() { OpcodeList.Add(Opcodes.ANDB); }
        public void PushORB() { OpcodeList.Add(Opcodes.ORB); }
        public void PushNEGB() { OpcodeList.Add(Opcodes.NEGB); }

        public void PushFTOI() { OpcodeList.Add(Opcodes.FTOI); }
        public void PushITOF() { OpcodeList.Add(Opcodes.ITOF); }
        public void PushSWAP() { OpcodeList.Add(Opcodes.SWAP); }
        public void PushDUP2() { OpcodeList.Add(Opcodes.DUP2); }
        public void PushDROP() { OpcodeList.Add(Opcodes.DROP); }
        public void PushJMP2(short val) { 
            OpcodeList.Add(Opcodes.JMP2);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushJMPT2(short val)
        {
            OpcodeList.Add(Opcodes.JMPT2);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushJMPF2(short val)
        {
            OpcodeList.Add(Opcodes.JMPF2);
            OpcodeList.AddRange(BitConverter.GetBytes(val).Cast<Opcodes>());
        }
        public void PushSTOP() { OpcodeList.Add(Opcodes.STOP); }
        public void PushMILLIS() { OpcodeList.Add(Opcodes.MILLS); }

        public IEnumerator<Opcodes> GetEnumerator()
        {
            return ((IEnumerable<Opcodes>)OpcodeList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)OpcodeList).GetEnumerator();
        }
    }
}
