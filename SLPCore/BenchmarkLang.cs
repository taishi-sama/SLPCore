using BenchmarkDotNet.Attributes;
using SimpleParser;
using SimpleScanner;
using SLPCore.Inits;
using SLPCore.Operators;
using SLPCore.Runtime;
using SLPCore.StackVM;
using SLPCore.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore
{
    [InProcess]
    public class BenchmarkLang
    {
        string Text = File.ReadAllText("../../../bench.txt");
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
            while (vm1.Step()) ;
        }
        [GlobalSetup(Target = nameof(RunSharp))] 
        public void SetupSharp() 
        { 
            sw = new Stopwatch(); 
        }
        [Benchmark(Baseline = true)]
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
            } while (!(a >= 100000));
            var end = sw.Elapsed.TotalMilliseconds;
            //Console.WriteLine(a);
            //Console.WriteLine(b);
            //Console.WriteLine("Delta:");
            //Console.WriteLine(end-start);
        }
        //[Benchmark]

        public void RunSharp2()
        {
            var sum = 0.0;
            var i = 1;
            while (i < 100000000) 
            {
                sum += 1.0 / i;
                i += 1;
            }

        }
    }
}
