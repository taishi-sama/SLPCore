using SLPCore.Visitors;
using SLPCore;
using QUT.Gppg;

namespace SLPCore.AST
{
    public abstract class StatementNode : Node // базовый класс для всех операторов
    {
        protected StatementNode(LexLocation line) : base(line)
        {
        }

        public abstract R Accept<R>(IASTStatementVisitor<R> visitor);
    }
}
