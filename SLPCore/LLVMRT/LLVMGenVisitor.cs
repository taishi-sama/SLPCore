using BenchmarkDotNet.Attributes;
using LLVMSharp;
using SLPCore.AST;
using SLPCore.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPCore.LLVMRT
{
    public class LLVMGenVisitor : IASTVisitor<Node>
    {
        private readonly LLVMModuleRef module;
        private readonly LLVMBuilderRef builder;
        private readonly Stack<LLVMValueRef> valueStack = new Stack<LLVMValueRef>();
        LLVMVariableContext variableContext = new LLVMVariableContext();

        public LLVMGenVisitor(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            this.module = module;
            this.builder = builder;
        }
        public Node AssingNodeVisit(AssignNode node)
        {
            throw new NotImplementedException();
        }

        public Node BinaryOpNodeVisit(BinaryOpNode node)
        {
            throw new NotImplementedException();
        }

        public Node BlockNodeVisit(BlockNode node)
        {
            var rc = variableContext;
            this.variableContext = new LLVMVariableContext(rc);
            foreach (var s in node.StList)
            {
                s.Accept(this);
            }
            this.variableContext = rc;
            return node;
        }

        public Node CastNodeVisit(CastNode node)
        {
            throw new NotImplementedException();
        }

        public Node ConstantNodeVisit(ConstantNode node)
        {
            throw new NotImplementedException();
        }

        public Node CycleNodeVisit(CycleNode node)
        {
            throw new NotImplementedException();
        }

        public Node DecNodeVisit(DecNode node)
        {
            throw new NotImplementedException();
        }

        public Node ForNodeVisit(ForNode node)
        {
            throw new NotImplementedException();
        }

        public Node IdNodeVisit(IdNode node)
        {
            throw new NotImplementedException();
        }

        public Node IfNodeVisit(IfNode node)
        {
            throw new NotImplementedException();
        }

        public Node MilliTimeVisit(MilliTimeNode node)
        {
            throw new NotImplementedException();
        }

        public Node RepeatNodeVisit(RepeatNode node)
        {
            throw new NotImplementedException();
        }

        public Node TypeNodeVisit(TypeNode node)
        {
            throw new NotImplementedException();
        }

        public Node UnaryOpNodeVisit(UnaryOpNode node)
        {
            throw new NotImplementedException();
        }

        public Node WhileNodeVisit(WhileNode node)
        {
            throw new NotImplementedException();
        }

        public Node WriteNodeVisit(WriteNode node)
        {
            throw new NotImplementedException();
        }
    }
}
