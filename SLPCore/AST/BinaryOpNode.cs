using SLPCore.Visitors;
using SLPCore;
using SLPCore.Exceptions;
using QUT.Gppg;

namespace SLPCore.AST
{
    public enum BinaryOperations
    {
        Add,
        Subtract, 
        Multiply,
        Divide,
        CompEqual,
        CompUnequal,
        CompGreater,
        CompLesser,
        CompGreaterEqual,
        CompLesserEqual,
        LogicOr,
        LogicAnd,
    }
    public class BinaryOpNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public BinaryOperations Operation { get; set; }
        public BinaryOpNode(LexLocation line, ExprNode left, ExprNode right, BinaryOperations operation):base(line)
        {
            Left = left;
            Right = right;
            this.Operation = operation;
        }

        public override R Accept<R>(IASTExprVisitor<R> visitor)
        {
            return visitor.BinaryOpNodeVisit(this);
        }
    }
}
