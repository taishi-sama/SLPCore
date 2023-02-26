using SLPCore.Visitors;
using SLPCore;
using System.Collections.Generic;
using QUT.Gppg;
using SLPCore.Exceptions;

namespace SLPCore.AST
{
    public class DecNode : StatementNode
    {
        public List<IdNode> idNodes = new List<IdNode>();
        public List<ExprNode> exprNodes = new List<ExprNode>();
        public TypeNode typeNode;
        public DecNode(LexLocation line, List<IdNode> ids, TypeNode typeNode = null) : base(line)
        {
            idNodes.AddRange(ids);
            this.typeNode = typeNode;
        }
        public DecNode(LexLocation line, List<IdNode> ids, List<ExprNode> exprs, TypeNode typeNode = null) : base(line)
        {
            idNodes.AddRange(ids);
            this.exprNodes.AddRange(exprs);
            this.typeNode = typeNode;
            if (exprNodes.Count != 0 && exprNodes.Count != idNodes.Count) 
            {
                throw new InitializersNotPaired(this.Line);
            }
        }

        public override R Accept<R>(IASTStatementVisitor<R> visitor)
        {
            return visitor.DecNodeVisit(this);
        }

        public void Add(IdNode id)
        {
            idNodes.Add(id);
        }
    }
}
