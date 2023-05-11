using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedVM
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct VMValue
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public long i64;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public double f64;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public bool @bool;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public long conststr;
    }
}
