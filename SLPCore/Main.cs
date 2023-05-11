using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SLPCore.Inits;
using SLPCore.Types;
using SLPCore.Exceptions;
using SLPCore.Runtime;
using BenchmarkDotNet.Running;
using SLPCore;
using SLPCore.StackVM;
using LLVMSharp;
using SLPCore.LLVMRT;
using SLPCore.ClosureVM;
using SLPCore.ThreecodedVM;
using SLPCore.ThreecodedIM;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        /*
|      Method | InvocationCount | UnrollFactor |       Mean |    Error |   StdDev | Ratio | RatioSD |
|------------ |---------------- |------------- |-----------:|---------:|---------:|------:|--------:|
|    RunSharp |         Default |           16 |   385.8 ms |  2.54 ms |  2.25 ms |  1.00 |    0.00 |
|             |                 |              |            |          |          |       |         |
| RunBytecode |               1 |            1 | 6,336.1 ms | 36.42 ms | 34.07 ms |     ? |       ? |

|      Method | InvocationCount | UnrollFactor |       Mean |   Error |  StdDev | Ratio | RatioSD |
|------------ |---------------- |------------- |-----------:|--------:|--------:|------:|--------:|
|    RunSharp |         Default |           16 |   384.3 ms | 0.04 ms | 0.03 ms |  1.00 |    0.00 |
|             |                 |              |            |         |         |       |         |
| RunBytecode |               1 |            1 | 4,453.9 ms | 6.53 ms | 5.79 ms |     ? |       ? |

|      Method | InvocationCount | UnrollFactor |      Mean |    Error |    StdDev | Ratio | RatioSD |
|------------ |---------------- |------------- |----------:|---------:|----------:|------:|--------:|
|    RunSharp |         Default |           16 |  44.72 ms | 0.542 ms |  0.507 ms |  1.00 |    0.00 |
|             |                 |              |           |          |           |       |         |
| RunBytecode |               1 |            1 | 499.28 ms | 9.877 ms | 10.143 ms |     ? |       ? |
|  RunClosure |               1 |            1 | 211.48 ms | 4.173 ms |  5.570 ms |     ? |       ? |

|      Method | InvocationCount | UnrollFactor |      Mean |    Error |   StdDev | Ratio | RatioSD |
|------------ |---------------- |------------- |----------:|---------:|---------:|------:|--------:|
|    RunSharp |         Default |           16 |  44.64 ms | 0.159 ms | 0.149 ms |  1.00 |    0.00 |
|             |                 |              |           |          |          |       |         |
| RunBytecode |               1 |            1 | 479.05 ms | 6.921 ms | 6.474 ms |     ? |       ? |
|  RunClosure |               1 |            1 | 196.51 ms | 1.124 ms | 0.997 ms |     ? |       ? |

|                      Method | InvocationCount | UnrollFactor |       Mean |    Error |   StdDev | Ratio | RatioSD |
|---------------------------- |---------------- |------------- |-----------:|---------:|---------:|------:|--------:|
|                   RunSharp2 |         Default |           16 |   133.7 ms |  0.46 ms |  0.39 ms |  1.00 |    0.00 |
|                             |                 |              |            |          |          |       |         |
|                 RunBytecode |               1 |            1 | 6,100.5 ms | 39.25 ms | 34.80 ms |     ? |       ? |
|                  RunClosure |               1 |            1 | 3,660.9 ms | 42.38 ms | 39.65 ms |     ? |       ? |
| RunThreecodedVM_handwritten |               1 |            1 | 1,838.7 ms | 10.88 ms |  9.08 ms |     ? |       ? |

|                      Method | InvocationCount | UnrollFactor |       Mean |    Error |   StdDev | Ratio | RatioSD |
|---------------------------- |---------------- |------------- |-----------:|---------:|---------:|------:|--------:|
|                   RunSharp2 |         Default |           16 |   133.8 ms |  0.23 ms |  0.19 ms |  1.00 |    0.00 |
|                             |                 |              |            |          |          |       |         |
|                  RunClosure |               1 |            1 | 3,638.1 ms | 49.75 ms | 44.10 ms |     ? |       ? |
| RunThreecodedVM_handwritten |               1 |            1 | 1,331.1 ms |  8.35 ms |  7.81 ms |     ? |       ? |

|                      Method | InvocationCount | UnrollFactor |       Mean |    Error |   StdDev | Ratio | RatioSD |
|---------------------------- |---------------- |------------- |-----------:|---------:|---------:|------:|--------:|
|                   RunSharp2 |         Default |           16 |   134.5 ms |  0.95 ms |  0.89 ms |  1.00 |    0.00 |
|                             |                 |              |            |          |          |       |         |
|                  RunClosure |               1 |            1 | 3,602.5 ms | 13.19 ms | 11.01 ms |     ? |       ? |
| RunThreecodedVM_handwritten |               1 |            1 | 1,739.3 ms |  3.02 ms |  2.36 ms |     ? |       ? |
|             RunThreecodedVM |               1 |            1 | 2,669.8 ms |  2.30 ms |  1.80 ms |     ? |       ? |
         */
        public static void Main()
        {

            //foreach (var i in Directory.EnumerateFiles(@"..\..\Tests", "*.slp", SearchOption.AllDirectories))
            //{
            //    Run(i);
            //};
            //VMTest();
            //VCTest();
            //CompileTest(@"bench.txt");
            //Test3VM();
            var summary = BenchmarkRunner.Run<BenchmarkLang>();
            //var b = new BenchmarkLang();
            //b.SetupThreecodedVM();
            //b.RunThreecodedVM();
            //b.SetupBytecode();
            //b.RunBytecode();
            //Run(@"..\..\..\bench.txt");
            //ClosureCompileTest(@"..\..\..\bench.txt");
            //TestTIM();
            //var summary2 = BenchmarkRunner.Run<BenchmarkLang2>();
            Console.ReadLine();
        }
        
        public static void Test3VM()
        {
            var t = new ThreecodedVM();
            var c = new List<VMCommand>();
            unsafe
            {
                //Constanst:
                // 0    1   2           3   4   5
                // 0.0  1   100000000   1.0 of1 of2
                (*t.constants) = new VMValue() { f64 = 0.0 };
                *(t.constants + 1) = new VMValue() { i64 = 1 };
                *(t.constants + 2) = new VMValue() { i64 = 100000000 };
                *(t.constants + 3) = new VMValue() { f64 = 1.0 };
                *(t.constants + 4) = new VMValue() { i64 = 3 };
                *(t.constants + 5) = new VMValue() { i64 = -5 };
                //Variables:
                // 0    1    2    3    4    5   
                // ps   pi   pb   tmp
                // f64  i64  bool f64
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.MOV8, in1 = t.constants + 0, out1 = t.registers + 0 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.MOV8, in1 = t.constants + 1, out1 = t.registers + 1 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.GEQI, in1 = t.registers + 1, in2 = t.constants + 2, out1 = t.registers + 2 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.JMPT, in1 = t.constants + 4, in2 = t.registers + 2 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.ADDFINVI, in1 = t.registers + 0, in2 = t.registers + 1, out1 = t.registers + 0 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.ADDI, in1 = t.registers + 1, in2 = t.constants + 1, out1 = t.registers + 1 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.JMP, in1 = t.constants + 5, });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.WRTF, in1 = t.registers + 0 });
                c.Add(new VMCommand() { opcode = SLPCore.ThreecodedVM.VMOpcodes.STOP });
                t.commands = c.ToArray();
            }
            Console.WriteLine(t.DumpCommands());
            t.VMRun();
        }
        public static void TestTIM()
        {
            string Text = """
                begin
                    var t := 1;
                    if (false) then
                      write(t)
                    else
                      t := 2;
                    if (true) then
                      t:= 3;
                    write(t);
                end
                """;
            Text = File.ReadAllText("../../../bench2.txt");
            Scanner scanner = new Scanner();
            scanner.SetSource(Text, 0);

            Parser parser = new Parser(scanner);
            Console.WriteLine($"Анализ ");
            var b = parser.Parse();
            if (!b)
                Console.WriteLine("Ошибка");
            else
            {
                Console.WriteLine("Синтаксическое дерево построено");
                //foreach (var st in parser.root.StList)
                //    Console.WriteLine(st);
                (var t, var c, var f, var i) = InitLang.InitTables();
                var e = new TypeOfExpression();
                var typechecker = new TypecheckVisitor(t, c, e);
                parser.root.Accept(typechecker);
                Console.WriteLine("Проверка типов завершена");
                var compiler = new ThreecodedCompilerVisitor(e, t, c, i);
                var emitter = parser.root.Accept(compiler);
                emitter.EmitSTOP();
                var strs = compiler.constStringPool;
                Console.WriteLine();
                foreach (var (k, v) in compiler.constants)
                {
                    Console.WriteLine($"{k}: {v.i64}");
                }
                Console.WriteLine();
                foreach (var com in emitter)
                {
                    Console.WriteLine(com);
                }
                var vm = IMtoVMTranslator.Process(emitter, compiler.constants, strs);
                Console.WriteLine(vm.DumpCommands());
                vm.VMRun();
            }
        }
        public static void CompileTest(string FileName)
        {
            string Text = File.ReadAllText(FileName);

            Scanner scanner = new Scanner();
            scanner.SetSource(Text, 0);

            Parser parser = new Parser(scanner);
            Console.WriteLine($"Анализ {FileName}");
            var b = parser.Parse();
            if (!b)
                Console.WriteLine("Ошибка");
            else
            {
                Console.WriteLine("Синтаксическое дерево построено");
                //foreach (var st in parser.root.StList)
                //    Console.WriteLine(st);
                (var t, var c, var f, var i) = InitLang.InitTables();
                var e = new TypeOfExpression();
                var typechecker = new TypecheckVisitor(t, c, e);
                parser.root.Accept(typechecker);
                Console.WriteLine("Проверка типов завершена");
                var compiler = new CompilerVisitor(e, t, c, i);
                var bytecode = parser.root.Accept(compiler);
                var strs = compiler.constStringPool;
                Console.WriteLine();
                var vm = new StackVM();
                bytecode.PushSTOP();
                vm.bytecode = bytecode.AsByteArray();
                vm.constStringPool = strs;
                vm.VMRun();
            }
        }
        public static void ClosureCompileTest(string FileName) 
        {
            string Text = File.ReadAllText(FileName);
            Scanner scanner = new Scanner();
            scanner.SetSource(Text, 0);
            Parser parser = new Parser(scanner);
            Console.WriteLine($"Анализ {FileName}");
            var b = parser.Parse();
            if (!b)
                Console.WriteLine("Ошибка");
            else
            {
                Console.WriteLine("Синтаксическое дерево построено");
                //foreach (var st in parser.root.StList)
                //    Console.WriteLine(st);
                (var t, var c, var f, var i) = InitLang.InitTables();
                var e = new TypeOfExpression();
                var typechecker = new TypecheckVisitor(t, c, e);
                parser.root.Accept(typechecker);
                Console.WriteLine("Проверка типов завершена");
                var compiler = new ClosureCompileVisitor(e, t, c, i);
                var closure = parser.root.Accept(compiler);
                var strs = compiler.constStringPool;
                Console.WriteLine();
                var vm = new ClosureVM();
                vm.constStringPool = strs;
                closure(vm);
            }
        }
        public static void LLVMRun(string FileName)
        {
            LLVMModuleRef module = LLVM.ModuleCreateWithName("my cool jit");
            LLVMBuilderRef builder = LLVM.CreateBuilder();
            var codeGenlistener = new LLVMGenVisitor(module, builder);
        }
        public static void Run(string FileName) 
        {
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);
                Console.WriteLine($"Анализ {FileName}");
                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    //foreach (var st in parser.root.StList)
                    //    Console.WriteLine(st);
                    (var t, var c, var f, var i) = InitLang.InitTables();
                    var e = new TypeOfExpression();
                    var typechecker = new TypecheckVisitor(t, c, e);
                    parser.root.Accept(typechecker);
                    Console.WriteLine("Проверка типов завершена");
                    var runtime = new RuntimeVisitor(e, t, c, i);
                    parser.root.Accept(runtime);

                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine($"Лексическая ошибка. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine($"Синтаксическая ошибка. " + e.Message);
            }
            catch (TypecheckException e)
            {
                Console.WriteLine($"Ошибка проверки типов на строке {e.Location.StartLine} : {e.Location.StartColumn}. " + e.Message);
            }
        }

    }
}
