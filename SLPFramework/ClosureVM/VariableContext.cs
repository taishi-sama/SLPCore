using SLPCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ClosureVM
{
    public class VariableContext
    {
        public VariableContext(VariableContext variableContext = null)
        {
            this.ParentVariableContext = variableContext;
        }
        public Dictionary<string, ushort> variables = new Dictionary<string, ushort>();
        public VariableContext ParentVariableContext { get; set; }
        public ushort GetLocalVariableNumber(string varName)
        {
            var currCont = this;
            while (currCont != null)
            {
                if (currCont.variables.TryGetValue(varName, out var v))
                    return v;
                currCont = currCont.ParentVariableContext;
            }
            throw new VariableNotExists(null, varName);
        }
        public ushort AllocateNumber(string varName)
        {
            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                var CurrCont = this;
                while (CurrCont != null)
                {
                    if (CurrCont.variables.Values.Contains(i))
                        goto co;
                    CurrCont = CurrCont.ParentVariableContext;
                }
                variables.Add(varName, i);
                return i;
            co:;
            }
            throw new UnableToAllocateVariable();
        }
    }
}
