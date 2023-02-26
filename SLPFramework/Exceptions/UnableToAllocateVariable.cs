using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class UnableToAllocateVariable : Exception
    {
        public UnableToAllocateVariable()
        {
        }

        public UnableToAllocateVariable(string message) : base(message)
        {
        }

        public UnableToAllocateVariable(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
