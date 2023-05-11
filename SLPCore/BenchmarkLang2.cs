using BenchmarkDotNet.Attributes;
using SimpleParser;
using SimpleScanner;
using SLPCore.Inits;
using SLPCore.ThreecodedIM;
using SLPCore.ThreecodedVM;
using SLPCore.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore
{
    [InProcess]
    public class BenchmarkLang2
    {
        string Text = File.ReadAllText("../../../bench3.txt");
        [Benchmark]
        [ArgumentsSource(nameof(SetupVMRun))]
        public void RunThreecodedVM(ThreecodedVM.ThreecodedVM vm)
        {
            vm.VMRun();
        }
        public IEnumerable<ThreecodedVM.ThreecodedVM> SetupVMRun() 
        {
            var scanner = new Scanner();
            scanner.SetSource(Text, 0);
            var parser = new Parser(scanner);
            var b = parser.Parse();
            var (t, c, f, i) = InitLang.InitTables();
            var e = new TypeOfExpression();
            var typechecker = new TypecheckVisitor(t, c, e);
            parser.root.Accept(typechecker);
            var compiler = new ThreecodedCompilerVisitor(e, t, c, i);
            var emitter = parser.root.Accept(compiler);
            emitter.EmitSTOP();
            var strpool = compiler.constStringPool;
            yield return IMtoVMTranslator.Process(emitter, compiler.constants, strpool);
        }
        //[Benchmark]
        [ArgumentsSource(nameof(SetupVMRunHandWritten))]
        public void RunThreecodedVMHandWritten(ThreecodedVM.ThreecodedVM vm)
        {
            vm.VMRun();
        }
        public IEnumerable<ThreecodedVM.ThreecodedVM> SetupVMRunHandWritten()
        {
            var consts = new Dictionary<string, VMValue>();
            consts["1i64"] = new VMValue() { i64 = 1 };
            consts["0i64"] = new VMValue() { i64 = 0 };
            consts["itersi64"] = new VMValue() { i64 = 100000000 };
            consts["1.0f64"] = new VMValue() { f64 = 1.0 };
            consts["0.0f64"] = new VMValue() { f64 = 0.0 };


            var emitter = new CommandsEmitter();
            //emitter.EmitMOV8("itersi64", "")
            emitter.EmitSTOP();
            var strpool = new List<string>();
            yield return IMtoVMTranslator.Process(emitter, consts, strpool);
        }
        [Benchmark]
        public void RunCSharp()
        {
            var sum = 0.0;
            var i = 1;
            var n = 100000000;
            do
            {
                if (i - i / 13 * 13 == 0)
                    sum += 1.0 / i;
                i += 1;
            }
            while (i < n);
        }
    }
}
