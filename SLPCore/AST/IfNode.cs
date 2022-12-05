using SLPCore.Visitors;
using SLPCore;
using SLPCore.Exceptions;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class IfNode : StatementNode
    {
        public IfNode(LexLocation line, ExprNode condExpr, StatementNode exprMainBranch,  StatementNode exprAltBranch = null):base(line)
        {
            CondExpr = condExpr;
            ExprMainBranch = exprMainBranch;
            ExprAltBranch = exprAltBranch;
        }

        public ExprNode CondExpr { get; set; }
        public StatementNode ExprMainBranch { get; set; }
        public StatementNode ExprAltBranch { get; set; }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.IfNodeVisit(this);
        }

    }
}
