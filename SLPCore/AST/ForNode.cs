using SLPCore.Visitors;
using SLPCore;
using SLPCore.Exceptions;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class ForNode : StatementNode
    {
        public ForNode(LexLocation line, IdNode id, ExprNode exprFrom, ExprNode exprTo, StatementNode stat):base(line)
        {
            Id = id;
            ExprFrom = exprFrom;
            ExprTo = exprTo;
            Stat = stat;
        }

        public IdNode Id { get; set; }
        public ExprNode ExprFrom { get; set; }
        public ExprNode ExprTo { get; set; }
        public StatementNode Stat { get; set; }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.ForNodeVisit(this);
        }
    }
}
