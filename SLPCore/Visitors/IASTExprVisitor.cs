using SLPCore.AST;
using SLPCore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Visitors
{
    public interface IASTExprVisitor<T>
    {
        T BinaryOpNodeVisit(BinaryOpNode node);
        T ConstantNodeVisit(ConstantNode node);
        T IdNodeVisit(IdNode node);
        T UnaryOpNodeVisit(UnaryOpNode node);
        T TypeNodeVisit(TypeNode node);
        T CastNodeVisit(CastNode node);
        T MilliTimeVisit(MilliTimeNode node);

    }
}
