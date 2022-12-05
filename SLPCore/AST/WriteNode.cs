using SLPCore.Visitors;
using SLPCore;
using System;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class WriteNode : StatementNode
    {
        public WriteNode(LexLocation line, ExprNode expr) : base(line)
        {
            Expr = expr;
        }

        public ExprNode Expr { get; set; }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.WriteNodeVisit(this);
        }

    }
}
