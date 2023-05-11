using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedIM
{
    public class CommandsEmitter : IEnumerable<IMCommand>
    {

        public IEnumerator<IMCommand> GetEnumerator()
        {
            return ((IEnumerable<IMCommand>)CommandList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)CommandList).GetEnumerator();
        }
        public void AddOpcodes(IEnumerable<IMCommand> сommands)
        {
            foreach (var c in сommands)
                CommandList.Add(c);
        }
        public List<IMCommand> CommandList = new List<IMCommand>();
        public void AddLabelToLastCommand(string label)
        {
            CommandList[CommandList.Count - 1].Marks.Add(label);
        }
        public void EmitNOP()
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.NOP });
        }

        public void EmitADDI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.ADDI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitADDF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.ADDF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitSUBI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.SUBI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitSUBF(string in1, string in2,    string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.SUBF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitMULI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.MULI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitMULF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.MULF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitDIVI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.DIVI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitDIVF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.DIVF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitNEGI(string in1, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.NEGI, In1 = in1, Out1 = out1 });
        }

        public void EmitNEGF(string in1, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.NEGF, In1 = in1, Out1 = out1 });
        }

        public void EmitMOV8(string in1, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.MOV8, In1 = in1, Out1 = out1 });
        }

        public void EmitMULFI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.MULFI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitDIVFI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.DIVFI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitWRTI(string in1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.WRTI, In1 = in1 });
        }
        public void EmitWRTF(string in1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.WRTF, In1 = in1 });
        }
        public void EmitWRTS(string in1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.WRTS, In1 = in1 });
        }
        public void EmitEQI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.EQI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitNEQI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.NEQI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitLEQI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.LEQI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitGEQI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.GEQI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitLI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.LI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitGI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.GI, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitEQF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.EQF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitNEQF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.NEQF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitLEQF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.LEQF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitGEQF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.GEQF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitLF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.LF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitGF(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.GF, In1 = in1, In2 = in2, Out1 = out1 });
        }

        public void EmitANDB(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand { Opcode = Opcodes.ANDB, In1 = in1, In2 = in2, Out1 = out1 });
        }
        public void EmitORB(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.ORB,
                In1 = in1,
                In2 = in2,
                Out1 = out1
            });
        }

        public void EmitNEGB(string in1, string out1)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.NEGB,
                In1 = in1,
                Out1 = out1
            });
        }

        public void EmitFTOI(string in1, string out1)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.FTOI,
                In1 = in1,
                Out1 = out1
            });
        }

        public void EmitITOF(string in1, string out1)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.ITOF,
                In1 = in1,
                Out1 = out1
            });
        }

        public void EmitJMP(string mark)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.JMP,
                GotoMark = mark,
            });
        }

        public void EmitJMPT(string in2, string mark)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.JMPT,
                In2 = in2,
                GotoMark = mark,
            });
        }

        public void EmitJMPF(string in2, string mark)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.JMPF,
                In2 = in2,
                GotoMark = mark,
            });
        }

        public void EmitSTOP()
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.STOP
            });
        }

        public void EmitMILLS(string out1)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.MILLS,
                Out1 = out1
            });
        }

        public void EmitADDFINVI(string in1, string in2, string out1)
        {
            CommandList.Add(new IMCommand()
            {
                Opcode = Opcodes.ADDFINVI,
                In1 = in1,
                In2 = in2,
                Out1 = out1
            });
        }
    }
}
