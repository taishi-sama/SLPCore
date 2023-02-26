using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class ValueIsNotConvertable : RuntimeException
    {
        public ValueIsNotConvertable(LexLocation location, string value, string error_try) : base(location, $"{value} is not converatable to {error_try}")
        {
        }
    }
}
