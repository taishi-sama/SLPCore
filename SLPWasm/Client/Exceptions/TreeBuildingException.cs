using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class TreeBuildingException : Exception
    {
        public LexLocation Location { get; }

        public TreeBuildingException(LexLocation location)
        {
            Location = location;
        }

        public TreeBuildingException(LexLocation location, string message) : base(message)
        {
            Location = location;
        }

        public TreeBuildingException(LexLocation location, string message, Exception innerException) : base(message, innerException)
        {
            Location = location;
        }
    }
}
