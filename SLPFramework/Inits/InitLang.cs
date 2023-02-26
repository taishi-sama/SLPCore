using SLPCore.Operators;
using SLPCore.Types;
using SLPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Inits
{
    public static class InitLang
    {
        public static (TypeNameTable, ConvertorTable, FunctionTable, FromInitTable) InitTables()
        {
            
            var tt = new TypeNameTable();
            var convertors = new ConvertorTable();
            var functions = new FunctionTable();
            var frominit = new FromInitTable();
            var i64 = new TypeSLP("i64");
            var f64 = new TypeSLP("f64");
            var conststr = new TypeSLP("conststr");
            var bool_t = new TypeSLP("bool");
            tt.AddLangType(i64);
            tt.AddLangType(f64);
            tt.AddLangType(bool_t);
            tt.AddLangType(conststr);
            convertors.ImplicitConvertors.Add((i64, f64), (RuntimeValue x) => { return new RuntimeValue(f64, (double)(long)x.value); });
            convertors.ExplicitConvertors.Add((f64, i64), (RuntimeValue x) => { return new RuntimeValue(i64, (long)(double)x.value); });
            frominit.initializers.Add(i64, (x) => new RuntimeValue(i64, long.Parse(x)));
            frominit.initializers.Add(f64, (x) => new RuntimeValue(f64, double.Parse(x, System.Globalization.CultureInfo.InvariantCulture)));
            frominit.initializers.Add(bool_t, (x) => new RuntimeValue(bool_t, x.Equals("true")));
            frominit.initializers.Add(conststr, (x) => new RuntimeValue(conststr, x.Substring(1, x.Length - 2)));
            //functions.dict.Add(("operator+", i64, i64, i64), (x, y) => { return new RuntimeValue(i64, (long)x.value + (long)y.value); });
            //functions.dict.Add(("operator-", i64, i64, i64), (x, y) => { return new RuntimeValue(i64, (long)x.value - (long)y.value); });
            //functions.dict.Add(("operator*", i64, i64, i64), (x, y) => { return new RuntimeValue(i64, (long)x.value * (long)y.value); });
            //functions.dict.Add(("operator/", i64, i64, i64), (x, y) => { return new RuntimeValue(i64, (long)x.value / (long)y.value); });
            //functions.dict.Add(("operator+", f64, f64, f64), (x, y) => { return new RuntimeValue(i64, (double)x.value + (double)y.value); });
            //functions.dict.Add(("operator-", f64, f64, f64), (x, y) => { return new RuntimeValue(i64, (double)x.value - (double)y.value); });
            //functions.dict.Add(("operator*", f64, f64, f64), (x, y) => { return new RuntimeValue(i64, (double)x.value * (double)y.value); });
            //functions.dict.Add(("operator/", f64, f64, f64), (x, y) => { return new RuntimeValue(i64, (double)x.value / (double)y.value); });
            //functions.dict.Add(("operator!", bool_t, null, bool_t), (x, y) => { return new RuntimeValue(bool_t, !(bool)x.value); });
            return (tt, convertors, functions, frominit);

        }
        

    }
}
