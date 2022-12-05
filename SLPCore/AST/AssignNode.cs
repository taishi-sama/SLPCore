using SLPCore.Visitors;
using SLPCore;
using System;
using QUT.Gppg;

namespace SLPCore.AST
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };
    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(LexLocation line, IdNode id, ExprNode expr, AssignType assop = AssignType.Assign):base(line)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.AssingNodeVisit(this);
        }
    }
}
