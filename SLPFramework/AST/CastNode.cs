using SLPCore.Visitors;
using SLPCore;
using SLPCore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class CastNode : ExprNode
    {
        public CastNode(LexLocation line, ExprNode exprNode, TypeNode typeNode):base(line)
        {
            ExprNode = exprNode;
            TypeNode = typeNode;
        }

        public ExprNode ExprNode { get; set; }
        public TypeNode TypeNode { get; set; }
        public override R Accept<R>(IASTExprVisitor<R> visitor)
        {
            return visitor.CastNodeVisit(this);
        }
    }
}
