using QUT.Gppg;
using SLPCore.AST;
using SLPCore.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.AST
{
    public class MilliTimeNode : ExprNode
    {
        public MilliTimeNode(LexLocation line) : base(line)
        {
        }

        public override R Accept<R>(IASTExprVisitor<R> visitor)
        {
            return visitor.MilliTimeVisit(this);
        }
    }
}
