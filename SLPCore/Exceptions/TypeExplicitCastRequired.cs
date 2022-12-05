using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class TypeExplicitCastRequired : TypecheckException
    {
        public TypeExplicitCastRequired(LexLocation location, string from, string to):base(location, $"Casting from {from} to {to} requires explicit cast") { }
    }
}
