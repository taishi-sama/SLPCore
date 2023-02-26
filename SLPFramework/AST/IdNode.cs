using SLPCore.Visitors;
using SLPCore;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(LexLocation line, string name):base(line) { Name = name; }

        public override R Accept<R>(IASTExprVisitor<R> visitor)
        {
            return visitor.IdNodeVisit(this);
        }
    }
}
