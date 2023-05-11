using BenchmarkDotNet.Attributes;
using SimpleParser;
using SimpleScanner;
using SLPCore.ClosureVM;
using SLPCore.Inits;
using SLPCore.Operators;
using SLPCore.Runtime;
using SLPCore.StackVM;
using SLPCore.ThreecodedIM;
using SLPCore.ThreecodedVM;
using SLPCore.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore
{
    [InProcess]
    public class BenchmarkLang
    {
        string Text = File.ReadAllText("../../../bench2.txt");
        Scanner scanner;
        Parser parser;
        TypeOfExpression e;
        TypeNameTable t;
        ConvertorTable c;
        FunctionTable f;
        FromInitTable i;
        //OpcodesEmitter ops;
        //List<string> strpool;
        StackVM.StackVM vm;
        ClosureVM.ClosureVM closureVM;

        [GlobalSetup(Target = nameof(RunInterpreter))]
        public void SetupInterpreter() 
        {
            scanner = new Scanner();
            scanner.SetSource(Text, 0);
            parser = new Parser(scanner);
            var b = parser.Parse();
            (t, c, f, i) = InitLang.InitTables();
            e = new TypeOfExpression();
            var typechecker = new TypecheckVisitor(t, c, e);
            parser.root.Accept(typechecker);

        }
        //[Benchmark]
        public void RunInterpreter() 
        {
            var runtime = new RuntimeVisitor(e, t, c, i) { WriteOut = false };
            parser.root.Accept(runtime);
        }
        Stopwatch sw;
        [IterationSetup(Target = nameof(RunBytecode))]
        public void SetupBytecode()
        {
            scanner = new Scanner();
            scanner.SetSource(Text, 0);
            parser = new Parser(scanner);
            var b = parser.Parse();
            (t, c, f, i) = InitLang.InitTables();
            e = new TypeOfExpression();
            var typechecker = new TypecheckVisitor(t, c, e);
            parser.root.Accept(typechecker);
            var compiler = new CompilerVisitor(e, t, c, i);

            OpcodesEmitter ops = parser.root.Accept(compiler);
            ops.PushSTOP();

            var strpool = compiler.constStringPool;
            vm = new StackVM.StackVM();
            vm.bytecode = ops.AsByteArray();
            vm.constStringPool = strpool;
        }
        [Benchmark]
        public void RunBytecode()
        {
            var vm1 = vm;
            vm1.VMRun();
        }
        [IterationSetup(Target = nameof(RunClosure))]
        public void SetupClosure()
        {
            scanner = new Scanner();
            scanner.SetSource(Text, 0);
            parser = new Parser(scanner);
            var b = parser.Parse();
            (t, c, f, i) = InitLang.InitTables();
            e = new TypeOfExpression();
            var typechecker = new TypecheckVisitor(t, c, e);
            parser.root.Accept(typechecker);
            var compiler = new ClosureCompileVisitor(e, t, c, i);
            var pred = parser.root.Accept(compiler);
            var strpool = compiler.constStringPool;
            closureVM = new ClosureVM.ClosureVM();
            closureVM.Lambda = pred;
            closureVM.constStringPool = strpool;
        }
        [Benchmark]
        public void RunClosure()
        {
            var vm1 = closureVM;
            vm1.Lambda(vm1);
        }
        [IterationSetup(Target = nameof(RunThreecodedVM_handwritten))]
        public void SetupThreecodedVM_handwritten()
        {
            var t = new ThreecodedVM.ThreecodedVM();
            var c = new List<VMCommand>();
            unsafe
            {
                //Constanst:
                // 0    1   2           3   4   5
                // 0.0  1   100000000   1.0 of1 of2
                (*t.constants) = new VMValue()      { f64 = 0.0 };
                *(t.constants + 1) = new VMValue()  { i64 = 1 };
                *(t.constants + 2) = new VMValue()  { i64 = 100000000 };
                *(t.constants + 3) = new VMValue()  { f64 = 1.0 };
                *(t.constants + 4) = new VMValue()  { i64 = 3 };
                *(t.constants + 5) = new VMValue()  { i64 = -5 };
                //Variables:
                // 0    1    2    3    4    5   
                // ps   pi   pb   tmp
                // f64  i64  bool f64
                
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.MOV8, in1 = t.constants + 0,                                out1 = t.registers + 0  });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.MOV8, in1 = t.constants + 1,                                out1 = t.registers + 1  });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.GEQI, in1 = t.registers + 1,    in2 = t.constants + 2,      out1 = t.registers + 2  });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.JMPT, in1 = (VMValue*)3    ,    in2 = t.registers + 2                               });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.ADDFINVI, in1 = t.registers + 0,in2 = t.registers + 1,      out1 = t.registers + 0  });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.ADDI, in1 = t.registers + 1,    in2 = t.constants + 1,      out1 = t.registers + 1  });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.JMP,  in1 = (VMValue*)-5   ,                                                        });
                c.Add(new VMCommand() { opcode = ThreecodedVM.VMOpcodes.STOP });
                t.commands = c.ToArray();
            }
            //Console.WriteLine(t.DumpCommands());
            ThreecodedVM_handwritten = t;
        }
        ThreecodedVM.ThreecodedVM ThreecodedVM_handwritten;
        [Benchmark]
        public void RunThreecodedVM_handwritten()
        {
            var l = ThreecodedVM_handwritten;
            l.VMRun();
        }
        ThreecodedVM.ThreecodedVM threecodedVM;
        [IterationSetup(Target = nameof(RunThreecodedVM))]
        public void SetupThreecodedVM()
        {
            scanner = new Scanner();
            scanner.SetSource(Text, 0);
            parser = new Parser(scanner);
            var b = parser.Parse();
            (t, c, f, i) = InitLang.InitTables();
            e = new TypeOfExpression();
            var typechecker = new TypecheckVisitor(t, c, e);
            parser.root.Accept(typechecker);
            var compiler = new ThreecodedCompilerVisitor(e, t, c, i);
            var emitter = parser.root.Accept(compiler);
            emitter.EmitSTOP();
            var strpool = compiler.constStringPool;
            threecodedVM = IMtoVMTranslator.Process(emitter, compiler.constants, strpool);
        }
        [Benchmark]
        public void RunThreecodedVM()
        {
            var l = threecodedVM;
            l.VMRun();
        }
        [GlobalSetup(Target = nameof(RunSharp))] 
        public void SetupSharp() 
        { 
            sw = new Stopwatch(); 
        }
        //[Benchmark(Baseline = true)]
        public void RunSharp()
        {
            sw.Start();
            var start = sw.Elapsed.TotalMilliseconds;
            double a, b;
            a = 2;
            b = 1;
            do
            {
                a = a + 1.5;
                for (int i = 0; i < 10; i++)
                {
                    b = b * 2 / 3;
                }
            } while (!(a >= 1000000));
            var end = sw.Elapsed.TotalMilliseconds;
        }
        [Benchmark(Baseline = true)]
        public void RunSharp2()
        {
            var sum = 0.0;
            var i = 1;
            var n = 100000000;
            while (i < n)
            {
                sum += 1.0 / i;
                i += 1;
            }
        }
    }
}
