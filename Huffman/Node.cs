using System;

namespace Huffman
{
    public class Node
    {
        public Node Parent { get; private set; }

        public Node Left { get; private set; }

        public Node Right { get; private set; }

        public int Weight { get; set; }

        public char? Character { get; set; }

        public NodeType Type { get; set; }

        public int GetTreeSize()
        {
            var leftSize = Left?.GetTreeSize() ?? 0;
            var rightSize = Right?.GetTreeSize() ?? 0;

            return 1 + leftSize + rightSize;
        }

        public int GetTreeHeight()
        {
            var leftHeight = Left?.GetTreeHeight() ?? 0;
            var rightHeight = Right?.GetTreeHeight() ?? 0;

            return 1 + Math.Max(leftHeight, rightHeight);
        }

        public int GetDeepnes()
        {
            return 1 + (Parent?.GetDeepnes() ?? 0);
        }

        public void SetChildren(Node left, Node right)
        {
            left.Parent = this;
            right.Parent = this;

            Left = left;
            Right = right;
        }
    }
}
