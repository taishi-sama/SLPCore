using SLPCore.Visitors;
using SLPCore;
using SLPCore.Exceptions;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(LexLocation line, ExprNode expr, StatementNode stat) : base(line)
        {
            Expr = expr;
            Stat = stat;
        }


        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.WhileNodeVisit(this);
        }
    }
}
