using SLPCore.Visitors;
using SLPCore.Exceptions;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class RepeatNode: StatementNode
    {
        public BlockNode Block { get; set; }
        public ExprNode Expr { get; set; }
        public RepeatNode(LexLocation line, BlockNode block, ExprNode expr):base(line)
        {
            Block = block;
            Expr = expr;
        }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.RepeatNodeVisit(this);
        }
    }
}
