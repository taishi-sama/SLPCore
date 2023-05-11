using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Exceptions
{
    public class VariableAlreadyInitialized : RuntimeException
    {
        public VariableAlreadyInitialized(LexLocation location,  string variable):base(location, $"{variable} already initialized.") 
        {
        }
    }
}
