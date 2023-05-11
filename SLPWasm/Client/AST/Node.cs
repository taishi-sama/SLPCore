using QUT.Gppg;
using SLPCore.Visitors;

namespace SLPCore.AST
{
    public abstract class Node // базовый класс для всех узлов    
    {
        public LexLocation Line { get; set; }
        public Node(LexLocation line) { Line = line; }
    }
}
