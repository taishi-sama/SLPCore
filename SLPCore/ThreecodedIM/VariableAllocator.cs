using SLPCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.ThreecodedIM
{
    public class NameAllocator
    {
        private List<Dictionary<string, string>> LocalNamespaceStack = new List<Dictionary<string, string>>();
        private HashSet<string> TakenThreecodedVariables = new HashSet<string>();
        ulong counter = 0;
        public void AddNamespaceLayer()
        {
            LocalNamespaceStack.Add(new Dictionary<string, string>());
        }
        public void RemoveNamespaceLayer()
        {
            LocalNamespaceStack.RemoveAt(LocalNamespaceStack.Count - 1);
        }
        public string GetLocalVariable(string varName)
        {
            foreach (var item in ((IEnumerable<Dictionary<string, string>>)LocalNamespaceStack).Reverse())
            {
                if (item.ContainsKey(varName))
                    return item[varName];
            }
            throw new VariableNotExists(null, varName);
        }
        public string AllocateLocalVariable(string varName)
        {
            counter++;
            var t = $"{varName}_{counter}";
            LocalNamespaceStack[LocalNamespaceStack.Count - 1].Add(varName, t);
            TakenThreecodedVariables.Add(varName);
            return t;
        }
        public string AllocateTemporaryVariable()
        {
            counter++;
            return $"tmp_{counter}";
        }
        public string GetWhileLabelName()
        {
            counter++;
            return $"label_while_{counter}";
        }
        public string GetWhileExitLabelName()
        {
            counter++;
            return $"label_while_exit_{counter}";
        }
        public string GetRepeatLabelName()
        {
            counter++;
            return $"label_repeat_{counter}";
        }
        public string GetIfLabelName()
        {
            counter++;
            return $"label_if_{counter}";
        }
        public string GetElseLabelName()
        {
            counter++;
            return $"label_else_{counter}";
        }
        public string GetCycleLabelName()
        {
            counter++;
            return $"label_cycle_{counter}";
        }
        public string GetCycleExitLabelName()
        {
            counter++;
            return $"label_cycle_exit_{counter}";
        }
        public string GetForLabelName()
        {
            counter++;
            return $"label_for_{counter}";
        }
        public string GetConstLabelName(string var_name)
        {
            counter++;
            return $"const_{var_name}_{counter}";
        }
    }
}
