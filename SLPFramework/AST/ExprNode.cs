using SLPCore.Visitors;
using SLPCore;
using QUT.Gppg;

namespace SLPCore.AST
{
    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
        protected ExprNode(LexLocation line) : base(line)
        {
        }

        public abstract R Accept<R>(IASTExprVisitor<R> visitor);
    }
}
