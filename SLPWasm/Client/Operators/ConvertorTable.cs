using SLPCore.Types;
using SLPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLPCore.Exceptions;

namespace SLPCore.Operators
{
    public class ConvertorTable
    {
        public Dictionary<(TypeSLP from, TypeSLP to), Func<RuntimeValue, RuntimeValue>> ImplicitConvertors 
            = new Dictionary<(TypeSLP from, TypeSLP to), Func<RuntimeValue, RuntimeValue>>();
        public Dictionary<(TypeSLP from, TypeSLP to), Func<RuntimeValue, RuntimeValue>> ExplicitConvertors
            = new Dictionary<(TypeSLP from, TypeSLP to), Func<RuntimeValue, RuntimeValue>>();
        public bool IsImplicitConvertable(TypeSLP from, TypeSLP to)
        {
            return ImplicitConvertors.ContainsKey((from, to)) || from == to;
        }
        public bool IsExplicitConvertable(TypeSLP from, TypeSLP to)
        {
            
            return IsImplicitConvertable(from, to) || ExplicitConvertors.ContainsKey((from, to));
        }
        public RuntimeValue ConvertTo(RuntimeValue val, TypeSLP to)
        {
            if (val.langType == to)
                return val;
            if (ImplicitConvertors.ContainsKey((val.langType, to)))
                return ImplicitConvertors[(val.langType, to)].Invoke(val);
            if (ExplicitConvertors.ContainsKey((val.langType, to)))
                return ExplicitConvertors[(val.langType, to)].Invoke(val);
            throw new RuntimeException(null, $"Probably bug in typechecking system: impossible to cast {val.langType.Name} to {to.Name}");
        }
    }
}
