using SkiaSharp;

namespace Huffman.Drawing
{
    public class DrawNode
    {
        public DrawNode(Node node)
        {
            Node = node;
        }

        public Node Node { get; }

        public int OrderNumber { get; set; }

        public int Weight { get; set; }

        public string Title { get; set; }

        public SKPoint Position { get; set; }

        public int? LeftOrderNumber { get; set; }

        public int? RightOrderNumber { get; set; }

        public DrawNodeShape Shape { get; set; }

        public SKColor Color { get; set; }
    }
}
