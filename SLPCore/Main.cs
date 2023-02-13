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
            var summary = BenchmarkRunner.Run<BenchmarkLang>();
            //var b = new BenchmarkLang();
            //b.SetupBytecode();
            //b.RunBytecode();
            //Run(@"..\..\..\bench.txt");
            //ClosureCompileTest(@"..\..\..\bench.txt");
            Console.ReadLine();
        }

        public static void VMTest()
        {
            var ops = new OpcodesEmitter();
            ops.PushPUSH8(15);
            ops.PushPUSH8(5);
            var f = ops.OpcodeList.Count;
            ops.PushPUSH8(6);
            ops.PushPUSH8(-10);
            ops.PushSWAP();
            ops.PushSUBI();
            ops.PushWRTI();
            var d = ops.OpcodeList.Count - f;
            ops.PushJMP2((short)-(d + 3));
            ops.PushSTOP();
            var svm = new StackVM();
            svm.bytecode = ops.AsByteArray();
            svm.VMRun();

        }
        public static void VCTest()
        {
            var vc = new SLPCore.StackVM.VariableContext();
            vc.AllocateNumber("a");
            vc.AllocateNumber("b");
            vc.AllocateNumber("c");
            var vc2 = new SLPCore.StackVM.VariableContext(vc);
            vc2.AllocateNumber("d");
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
