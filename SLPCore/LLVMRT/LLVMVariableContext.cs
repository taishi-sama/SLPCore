using LLVMSharp;
using SLPCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.LLVMRT
{
    public class LLVMVariableContext
    {
        public LLVMVariableContext(LLVMVariableContext? variableContext = null)
        {
            this.ParentVariableContext = variableContext;
        }
        public Dictionary<string, LLVMValueRef> variables = new Dictionary<string, LLVMValueRef>();
        public LLVMVariableContext? ParentVariableContext { get; set; }
        public LLVMValueRef GetLocalVariableNumber(string varName)
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
        public LLVMValueRef AllocateNumber(string varName, LLVMValueRef val)
        {
            variables.Add(varName, val);
            return val;
        }
    }
}
