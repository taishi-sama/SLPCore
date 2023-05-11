using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class TypecheckException : Exception
    {
        public LexLocation Location { get; set; }
        public TypecheckException(LexLocation location)
        {
            this.Location = location;
        }

        public TypecheckException(LexLocation location, string message) : base(message)
        {
            Location = location;
        }

        public TypecheckException(LexLocation location, string message, Exception innerException) : base(message, innerException)
        {
            Location = location;
        }
    }
}
