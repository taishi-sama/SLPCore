using SLPCore.StackVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ClosureVM
{
    public class ClosureVM
    {
        public RuntimeValue[] localVariables = new RuntimeValue[1024];
        public List<string> constStringPool = new();
        public Stopwatch sw = Stopwatch.StartNew();
        public StatementLambda? Lambda;
    }
}
