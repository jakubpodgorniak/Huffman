using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using Huffman.Drawing;

namespace Huffman
{
    public class Node
    {
        public static Node FindLastWithLowerWeight(Node node)
        {
            var root = node.GetSuperRoot();
            var flattenNodes = NodeUtils.FlattenNodes(new[] { root }).Reverse().ToList();
            var checkedNodeIndex = flattenNodes.IndexOf(node);

            Node lastNodeWithLowerWeight = null;
            for (int i = checkedNodeIndex + 1; i < flattenNodes.Count; i++)
            {
                var n = flattenNodes[i];

                if (n.Weight >= node.Weight)
                {
                    break;
                }

                if (n != node.Parent)
                {
                    lastNodeWithLowerWeight = n;
                }
            }

            return lastNodeWithLowerWeight;
        }

        public Node Parent { get; private set; }

        public Node Left { get; private set; }

        public Node Right { get; private set; }

        public int Weight
        {
            get => _weight;
            set
            {
                if (Type != NodeType.Character) return;

                _weight = value;

                var toSwap = FindLastWithLowerWeight(this);
                if (toSwap != null)
                {
                    Swap(this, toSwap);
                }
                else
                {
                    Parent?.UpdateWeight();
                }
            }
        }

        public char? Character { get; set; }

        public NodeType Type { get; set; }

        public Node GetSuperRoot()
        {
            if (Parent != null) return Parent.GetSuperRoot();
            return this;
        }

        public Node GetNeighbor()
        {
            if (Parent != null)
            {
                if (Parent.Left == this) return Parent.Right;
                return Parent.Left;
            }

            return null;
        }

        public Node FindNode(char character)
        {
            if (Type == NodeType.Internal)
            {
                var left = Left?.FindNode(character);
                if (left != null) return left;

                var right = Right?.FindNode(character);
                if (right != null) return right;
            }
            else if (Type == NodeType.Character)
            {
                return Character == character ? this : null;
            }

            return null;
        }

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

        public void SetChildrenConsiderWeights(Node first, Node second)
        {
            if (first.Weight == second.Weight)
            {
                if (first.Type == NodeType.Internal && second.Type != NodeType.Internal)
                    SetChildren(first, second);
                else SetChildren(second, first);

                return;
            }

            if (first.Weight > second.Weight)
                SetChildren(second, first);
            else
                SetChildren(first, second);
        }

        public void SetChildren(Node left, Node right)
        {
            left.Parent = this;
            right.Parent = this;

            Left = left;
            Right = right;

            UpdateWeight();
        }


        public void Draw(string name)
        {
            new TreeDrawer().DrawNodesToFile($"images/{name}.png", NodeUtils.PrepareDrawNodes(GetSuperRoot())
                .ToDictionary(dn => dn.OrderNumber));
        }

        private void UpdateWeight()
        {
            if (Type == NodeType.Internal)
            {
                _weight = (Left?.Weight ?? 0) + (Right?.Weight ?? 0);

                var toSwap = FindLastWithLowerWeight(this);
                if (toSwap != null)
                {
                    Swap(this, toSwap);
                }

                Parent?.UpdateWeight();
            }
        }

        public static void Swap(Node first, Node second)
        {
            if (first == second) throw new ArgumentException();

            var firstParent = first.Parent;
            var secondParent = second.Parent;

            var firstNeighbor = first.GetNeighbor();
            var secondNeighbor = second.GetNeighbor();

            if (firstParent == secondParent)
            {
                var parent = firstParent;

                if (parent.Left == first)
                {
                    parent.Left = second;
                    parent.Right = first;
                }
                else
                {
                    parent.Left = first;
                    parent.Right = second;
                }

                return;
            }

            if (firstParent != null)
            {
                firstParent.SetChildrenConsiderWeights(firstNeighbor, second);
            }

            if (secondParent != null)
            {
                secondParent.SetChildrenConsiderWeights(secondNeighbor, first);
            }
        }

        private int _weight;
    }
}
