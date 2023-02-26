using SLPCore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Visitors
{
    public interface IASTStatementVisitor<T>
    {
        T AssingNodeVisit(AssignNode node);
        T BlockNodeVisit(BlockNode node);
        T CycleNodeVisit(CycleNode node);
        T DecNodeVisit(DecNode node);
        T ForNodeVisit(ForNode node);
        T IfNodeVisit(IfNode node);
        T RepeatNodeVisit(RepeatNode node);
        T WhileNodeVisit(WhileNode node);
        T WriteNodeVisit(WriteNode node);

    }
}
