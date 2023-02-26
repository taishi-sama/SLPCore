using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class InvalidBinaryOperation : RuntimeException
    {

        public InvalidBinaryOperation(LexLocation location, string operation) : base(location, $"{operation} is not valid") { }
    }
}
