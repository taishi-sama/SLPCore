using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Types
{
    public class TypeNameTable
    {
        private Dictionary<string, TypeSLP> _table;
        public TypeNameTable() 
        { 
            _table = new Dictionary<string, TypeSLP>(); 
        }
        public TypeSLP GetLangType(string name)
        {
            return _table[name];
        }
        public void AddLangType(string name, TypeSLP type) 
        {
            _table.Add(name, type);
        }
        public void AddLangType(TypeSLP type)
        {
            _table.Add(type.Name, type);
        }
    }
}
