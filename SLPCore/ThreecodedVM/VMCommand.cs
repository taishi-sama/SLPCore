using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedVM
{
    public enum VMOpcodes
    {
        NOP = 0x00,
        /// <summary>
        /// in1.i64 + in2.i64 -> out1.i64
        /// </summary>
        ADDI = 0x01, 
        /// <summary>
        /// in1.f64 + in2.f64 -> out1.f64
        /// </summary>
        ADDF = 0x02, 
        /// <summary>
        /// in1.i64 - in2.i64 -> out1.i64
        /// </summary>
        SUBI = 0x03,
        /// <summary>
        /// in1.f64 - in2.f64 -> out1.f64
        /// </summary>
        SUBF = 0x04,
        /// <summary>
        /// in1.i64 * in2.i64 -> out1.i64 
        /// </summary>
        MULI = 0x05,
        /// <summary>
        /// in1.f64 * in2.f64 -> out1.f64
        /// </summary>
        MULF = 0x06,
        /// <summary>
        /// in1.i64 / in2.i64 -> out1.i64
        /// </summary>
        DIVI = 0x07,
        /// <summary>
        /// in1.f64 / in2.f64 -> out1.f64
        /// </summary>
        DIVF = 0x08,
        /// <summary>
        /// -in1.i64 -> out1.i64
        /// </summary>
        NEGI = 0x09,
        /// <summary>
        /// -in1.f64 -> out1.f64
        /// </summary>
        NEGF = 0x0A,
        /// <summary>
        /// in1 -> out1
        /// </summary>
        MOV8 = 0x11, 
        /// <summary>
        /// in1.f64 * in2.i64 -> out1.f64
        /// </summary>
        MULFI = 0x20,
        /// <summary>
        /// in1.f64 / in2.i64 -> out1.f64
        /// </summary>
        DIVFI = 0x21,
        //Печатают
        /// <summary>
        /// write(in1.i64)
        /// </summary>
        WRTI = 0x30,
        /// <summary>
        /// write(in1.f64)
        /// </summary>
        WRTF = 0x31,
        /// <summary>
        /// write(str[in1.str])
        /// </summary>
        WRTS = 0x32,

        /// <summary>
        /// in1.i64 == in2.i64 -> out1.@bool
        /// </summary>
        EQI = 0x40, 
        /// <summary>
        /// in1.i64 != in2.i64 -> out1.@bool
        /// </summary>
        NEQI = 0x41,
        /// <summary>
        /// in1.i64 <= in2.i64 -> out1.@bool
        /// </summary>
        LEQI = 0x42,
        /// <summary>
        /// in1.i64 >= in2.i64 -> out1.@bool
        /// </summary>
        GEQI = 0x43,
        /// <summary>
        /// in1.i64 <  in2.i64 -> out1.@bool
        /// </summary>
        LI = 0x44,  
        /// <summary>
        /// in1.i64 >  in2.i64 -> out1.@bool
        /// </summary>
        GI = 0x45,  
        /// <summary>
        /// in1.f64 == in2.f64 -> out1.@bool
        /// </summary>
        EQF = 0x50, 
        /// <summary>
        /// in1.f64 != in2.f64 -> out1.@bool
        /// </summary>
        NEQF = 0x51,
        /// <summary>
        /// in1.f64 <= in2.f64 -> out1.@bool
        /// </summary>
        LEQF = 0x52,
        /// <summary>
        /// in1.f64 >= in2.f64 -> out1.@bool
        /// </summary>
        GEQF = 0x53,
        /// <summary>
        /// in1.f64 <  in2.f64 -> out1.@bool
        /// </summary>
        LF = 0x54,  
        /// <summary>
        /// in1.f64 >  in2.f64 -> out1.@bool
        /// </summary>
        GF = 0x55,  

        /// <summary>
        /// in1.@bool & in2.@bool -> out1.@bool
        /// </summary>
        ANDB = 0x60,
        /// <summary>
        /// in1.@bool | in2.@bool -> out1.@bool
        /// </summary>
        ORB = 0x61,
        /// <summary>
        /// !in1.@bool -> out1.@bool
        /// </summary>
        NEGB = 0x62,

        /// <summary>
        /// (long)in1.f64 -> out1.i64
        /// </summary>
        FTOI = 0x70, 
        /// <summary>
        /// (double)in1.i64 -> out1.f64
        /// </summary>
        ITOF = 0x71,

        /// <summary>
        /// pc += (int)in1
        /// </summary>
        JMP = 0x90,
        /// <summary>
        /// if(in2.@bool) pc += (int)in1
        /// </summary>
        JMPT = 0x91,
        /// <summary>
        /// if(!in2.@bool) pc += (int)in1
        /// </summary>
        JMPF = 0x92, 


        STOP = 0xF0, //Останавливает виртуальную машину
        /// <summary>
        /// millis() -> out1.f64
        /// </summary>
        MILLS = 0xF1,


        /// <summary>
        /// in1.f64 + 1.0/in2.i64 -> out1.f64
        /// </summary>
        ADDFINVI = 0x1F0
    }
    public unsafe struct VMCommand
    {
        public VMOpcodes opcode;
        public VMValue* in1;
        public VMValue* in2;
        public VMValue* out1;
        public VMCommand(VMOpcodes opcode) {  this.opcode = opcode; }
        public VMCommand(VMOpcodes opcode,  VMValue* in1, VMValue* in2, VMValue* out1) 
        { this.opcode=opcode; this.in1 = in1; this.in2 = in2; this.out1 = out1; }
    }
}
