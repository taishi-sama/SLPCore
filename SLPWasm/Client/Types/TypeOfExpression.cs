using SLPCore.AST;
using SLPCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Types
{
    public class TypeOfExpression
    {
        public Dictionary<ExprNode, TypeSLP> dict = new Dictionary<ExprNode, TypeSLP>();
        public TypeSLP this[ExprNode e]
        {
            get {return dict[e];}
            set { dict[e] = value;}
        }
    }
}
