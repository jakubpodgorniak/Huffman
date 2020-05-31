using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Huffman.Drawing;
using SkiaSharp;

namespace Huffman
{
    public static class NodeUtils
    {
        public static IList<Node> FlattenNodes(IEnumerable<Node> nodes)
        {
            var childNodes = new List<Node>();

            if (!nodes.Any())
                return childNodes;

            foreach (var node in nodes)
            {
                if (node.Right != null)
                    childNodes.Add(node.Right);

                if (node.Left != null)
                    childNodes.Add(node.Left);
            }

            return nodes.Concat(FlattenNodes(childNodes)).ToList();
        }

        private const float WidthForNode = 60;
        private const float HeightPerLevel = 80;

        public static IList<DrawNode> PrepareDrawNodes(Node node)
        {
            var width = WidthForNode * Math.Pow(2, node.GetTreeHeight() - 1);
            var x = (float)(width / 2.0);
            const float y = HeightPerLevel / 2f;

            return PrepareDrawNodes(new[] { CreateDrawNode(node, node.GetTreeSize(), new SKPoint(x, y)) });
        }

        private static IList<DrawNode> PrepareDrawNodes(IList<DrawNode> drawNodes)
        {
            var childDrawNodes = new List<DrawNode>();
            if (drawNodes.Count == 0) return childDrawNodes;

            var lastNode = drawNodes.Last();
            float y = (lastNode.Node.GetDeepnes() * HeightPerLevel) + (HeightPerLevel / 2);
            int nextOrderNumber = lastNode.OrderNumber - 1;

            foreach (var drawNode in drawNodes) {
                if (drawNode.Node.Right != null) {
                    var x = drawNode.Position.X
                        + (float)(WidthForNode / 2f * Math.Pow(2, drawNode.Node.Right.GetTreeHeight() - 1));
                    var orderNumber = nextOrderNumber--;
                    var rightDrawNode = CreateDrawNode(drawNode.Node.Right, orderNumber, new SKPoint(x, y));
                    drawNode.RightOrderNumber = orderNumber;
                    childDrawNodes.Add(rightDrawNode);
                }
                if (drawNode.Node.Left != null) {
                    var x = drawNode.Position.X
                        - (float)(WidthForNode / 2f * Math.Pow(2, drawNode.Node.Left.GetTreeHeight() - 1));
                    var orderNumber = nextOrderNumber--;
                    var leftDrawNode = CreateDrawNode(drawNode.Node.Left, orderNumber, new SKPoint(x, y));
                    drawNode.LeftOrderNumber = orderNumber;
                    childDrawNodes.Add(leftDrawNode);
                }
            }
            return drawNodes.Concat(PrepareDrawNodes(childDrawNodes)).ToList();
        }

        private static DrawNode CreateDrawNode(Node node, int orderNumber, SKPoint position)
        {
            return new DrawNode(node)
            {
                OrderNumber = orderNumber,
                Weight = node.Weight,
                Title = GetNodeTitle(node),
                Position = position,
                Shape = node.Type == NodeType.Internal ? DrawNodeShape.Circle : DrawNodeShape.Rect,
                Color = node.Type == NodeType.Null ? SKColors.LightBlue : SKColors.LightGreen
            };
        }

        private static string GetNodeTitle(Node node)
        {
            if (node.Type == NodeType.Null)
                return "nyt";
            else if (node.Type == NodeType.Internal)
                return string.Empty;
            else return node.Character.Value switch
            {
                ' ' => "space",
                _ => node.Character.Value.ToString()
            };
        }
    }
}
