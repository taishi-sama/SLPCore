using SLPCore.Visitors;
using SLPCore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class TypeNode : Node
    {
        public string TypeName { get; set; }

        public TypeNode(LexLocation line, string typeName):base(line)
        {
            TypeName = typeName;
        }

        public virtual R Accept<R>(IASTTypeVisitor<R> visitor)
        {
            return visitor.TypeNodeVisit(this);
        }
    }
}
