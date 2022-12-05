using BenchmarkDotNet.Attributes;
using SimpleParser;
using SimpleScanner;
using SLPCore.Inits;
using SLPCore.Runtime;
using SLPCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore
{
    [InProcess]
    public class BenchmarkLang
    {
        [Benchmark]
        public void Run() 
        {
            string Text = File.ReadAllText("../../../bench.txt");

            Scanner scanner = new Scanner();
            scanner.SetSource(Text, 0);

            Parser parser = new Parser(scanner);
            var b = parser.Parse();
            (var t, var c, var f, var i) = InitLang.InitTables();
            var e = new TypeOfExpression();
            var typechecker = new TypecheckVisitor(t, c, e);
            parser.root.Accept(typechecker);
            var runtime = new RuntimeVisitor(e, t, c, i) { WriteOut = false };
            parser.root.Accept(runtime);
        }
    }
}
