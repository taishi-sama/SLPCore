using SLPCore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.Visitors
{
    public interface IASTTypeVisitor<T>
    {
        T TypeNodeVisit(TypeNode typeNode);
    }
}
