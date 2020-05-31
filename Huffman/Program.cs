using System;
using System.IO;
using System.Linq;
using Huffman.Drawing;

namespace Huffman
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ClearImagesDirectory();

            var input = File.ReadAllText(args[0]);

            var root = new Node { Type = NodeType.Null, Weight = 0 };
            var nyt = root;

            var treeDrawer = new TreeDrawer();

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];

                var existingCharacterNode = root.FindNode(c);

                if (existingCharacterNode is null)
                {
                    var internalNode = new Node { Type = NodeType.Internal };
                    var newCharacterNode = new Node { Type = NodeType.Character, Character = c, Weight = 1 };
                    var oldNytParent = nyt.Parent;

                    if (oldNytParent != null)
                    {
                        oldNytParent.SetChildren(internalNode, oldNytParent.Right);
                    }
                    else
                    {
                        root = internalNode;
                    }

                    internalNode.SetChildren(nyt, newCharacterNode);
                }
                else
                {
                    existingCharacterNode.Weight++;
                }

                treeDrawer.DrawNodesToFile($"{ImagesDirectory}/tree{i}.png", NodeUtils.PrepareDrawNodes(root).ToDictionary(dn => dn.OrderNumber));
            }

            Console.ReadKey();
        }

        private static void ClearImagesDirectory()
        {
            if (!Directory.Exists(ImagesDirectory))
                Directory.CreateDirectory(ImagesDirectory);

            foreach (var filePath in Directory.GetFiles(ImagesDirectory))
                File.Delete(filePath);
        }

        private const string ImagesDirectory = "images";
    }
}
