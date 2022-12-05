using SLPCore.Exceptions;
using System.Collections.Generic;

namespace SLPCore.Types
{
    public class TypeContext
    {
        public TypeContext parentContext = null;

        public TypeContext(TypeContext parentContext = null)
        {
            this.parentContext = parentContext;
        }

        Dictionary<string, TypeSLP> table = new Dictionary<string, TypeSLP>();
        public TypeSLP GetTypeOfID(string ID)
        {
            var t = this;
            while (t != null)
            {
                if (t.table.ContainsKey(ID))
                    return t.table[ID];
                t = t.parentContext;
            }
            throw new VariableNotExists(null, ID);
        }
        public void DeclareVarType(string ID, TypeSLP langType)
        {
            table.Add(ID, langType);
        }
        public void RedefineVarType(string ID, TypeSLP langType)
        {
            if (table.ContainsKey(ID))
            {
                table[ID] = langType;
                return;
            }
            throw new VariableNotExists(null,ID);

        }
    }
}