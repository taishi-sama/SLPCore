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
using SLPCore.ClosureVM;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        /*
|      Method | InvocationCount | UnrollFactor |        Mean |     Error |    StdDev | Ratio | RatioSD |
|------------ |---------------- |------------- |------------:|----------:|----------:|------:|--------:|
|    RunSharp |         Default |           16 |    48.26 ms |  0.144 ms |  0.128 ms |  1.00 |    0.00 |
|             |                 |              |             |           |           |       |         |
| RunBytecode |               1 |            1 | 2,219.48 ms | 35.909 ms | 33.589 ms |     ? |       ? |
|  RunClosure |               1 |            1 |   244.15 ms |  2.096 ms |  1.750 ms |     ? |       ? |
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
