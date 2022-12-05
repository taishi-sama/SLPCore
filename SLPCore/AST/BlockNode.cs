using SLPCore.Visitors;
using SLPCore;
using System.Collections.Generic;
using QUT.Gppg;

namespace SLPCore.AST
{
    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(LexLocation line, StatementNode stat):base(line)
        {
            Add(stat);
        }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.BlockNodeVisit(this);
        }

        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }
}
