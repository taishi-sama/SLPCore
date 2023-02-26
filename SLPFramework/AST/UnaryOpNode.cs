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
    public enum UnaryOperations
    {
        Negate,
        LogicalNot
    }

    public class UnaryOpNode : ExprNode
    {
        public UnaryOpNode(LexLocation line, ExprNode expr, UnaryOperations operation) : base(line)
        {
            Expr = expr;
            Operation = operation;
        }

        public ExprNode Expr { get; set; }
        public UnaryOperations Operation { get; set; }
        public override R Accept<R>(IASTExprVisitor<R> visitor)
        {
            return visitor.UnaryOpNodeVisit(this);
        }

    }
}
