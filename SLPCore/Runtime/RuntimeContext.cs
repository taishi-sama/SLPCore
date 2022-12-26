using SLPCore.Exceptions;
using SLPCore.Types;
using System.Collections.Generic;

namespace SLPCore
{
    public class RuntimeContext
    {
        public RuntimeValue GetRuntimeValue(string varName)
        {
            var currCont = this;
            while (currCont != null)
            {
                if (currCont.variables.TryGetValue(varName, out var runtimeValue))
                    return runtimeValue;
                currCont = this.ParentRuntimeContext;
            }
            throw new VariableNotExists(null, varName);
        }
        public void InitializeRuntimeValue(string varName, RuntimeValue val)
        {
            if (!variables.ContainsKey(varName))
            {
                variables.Add(varName, val);
                return;
            }
            throw new VariableAlreadyInitialized(null, varName);
        }
        public void SetVariable(string varName, RuntimeValue runtimeValue)
        {
            var currCont = this;
            while (currCont != null)
            {
                if (currCont.variables.ContainsKey(varName))
                {
                    currCont.variables[varName] = runtimeValue;
                    return;
                }
                currCont = this.ParentRuntimeContext;
            }
            throw new VariableNotExists(null, varName);
        }
        public void SetOrInitVariable(string varName, RuntimeValue runtimeValue)
        {
            variables.Add(varName, runtimeValue);
        }
        public Dictionary<string, RuntimeValue> variables = new Dictionary<string, RuntimeValue>();
        public RuntimeContext? ParentRuntimeContext { get; set; }
        public RuntimeContext(RuntimeContext parentContext = null)
        {
            this.ParentRuntimeContext = parentContext;
        }
    }
}