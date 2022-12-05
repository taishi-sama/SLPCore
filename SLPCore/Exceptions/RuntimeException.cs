using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class RuntimeException : Exception
    {
        public LexLocation Location { get; }

        public RuntimeException(LexLocation location)
        {
        }

        public RuntimeException(LexLocation location, string message) : base(message)
        {
        }

        public RuntimeException(LexLocation location, string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
