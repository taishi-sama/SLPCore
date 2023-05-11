using SLPCore.Visitors;
using SLPCore;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class ConstantNode : ExprNode
    {
        public string Value
        {
            get; set;
        }
        public TypeNode TypeNode { get; set; }
        public ConstantNode(LexLocation line, string value, TypeNode typeNode):base(line)
        {
            Value = value;
            TypeNode = typeNode;
        }

        public override R Accept<R>(IASTExprVisitor<R> visitor)
        {
            return visitor.ConstantNodeVisit(this);
        }
    }
}
