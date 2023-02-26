using SLPCore;
using SLPCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Operators
{
    public class FromInitTable
    {
        public Dictionary<TypeSLP, Func<string, RuntimeValue>> initializers = new Dictionary<TypeSLP, Func<string, RuntimeValue>>();
        public RuntimeValue GetRuntimeValue(TypeSLP type, string initToken)
        {
            return initializers[type].Invoke(initToken);
        }
    }
}
