using SLPCore.Types;
using SLPCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Operators
{
    public class FunctionTable
    {
        public Dictionary<(string name, TypeSLP arg1, TypeSLP arg2, TypeSLP output), Func<RuntimeValue, RuntimeValue, RuntimeValue>> dict 
            = new Dictionary<(string name, TypeSLP arg1, TypeSLP arg2, TypeSLP output), Func<RuntimeValue, RuntimeValue, RuntimeValue>>();

    }
}
